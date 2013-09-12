using System;
using System.Collections.Generic;
using System.Threading;
using fxcore2;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Listener;
using QuantSys.Util;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions
{
    public class HistoricPriceEngine
    {

        private Quantum _mktData;
        private int _completeCounter;
        private bool complete;

        private FXSession session;
        private ResponseHandler mHandler;
        private object locker;

        public bool Complete { get { return complete; } }
        public Quantum Data { get { return _mktData; } }
        public Symbol Symbol { get; set; }

        public HistoricPriceEngine(FXSession session)
        {
            _mktData = new Quantum();
            _completeCounter = 0;
            complete = false;
            mHandler = HistoricDataReceived;
            this.session = session;
        }

        public void Reset()
        {
            _mktData = new Quantum();
            _completeCounter = 0;
            complete = false;
        }

        public void GetLongHistoricPrices(string symbol, string timeframe, int ticks)
        {
            _mktData = new Quantum();
            Symbol = new Symbol(symbol);
            session.AttachHandler(mHandler);

            DateTime dateNow = DateTime.Now;
            TimeSpan time = Timeframe.StringToTimeSpan(timeframe);

            DateTime startDate = dateNow.AddMinutes(-ticks * Timeframe.TimeframeToMinutes(timeframe));

            O2GRequestFactory factory = session.Session.getRequestFactory();
            O2GTimeframeCollection timeframes = factory.Timeframes;
            O2GTimeframe tfo = timeframes[timeframe];
            
            int counter = ticks;

            lock (locker)
            {
                while (counter > 0)
                {

                    _completeCounter++;
                    int subticks = (counter >= QSConstants.MAX_FXCM_API_TICKS)
                        ? QSConstants.MAX_FXCM_API_TICKS
                        : counter;
                    O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(symbol, tfo, subticks);
                    factory.fillMarketDataSnapshotRequestTime(request, startDate,
                        startDate.AddMinutes(2*subticks*Timeframe.TimeframeToMinutes(timeframe)));
                    session.Session.sendRequest(request);

                    startDate = startDate.AddMinutes(subticks*Timeframe.TimeframeToMinutes(timeframe));
                    counter -= (counter >= QSConstants.MAX_FXCM_API_TICKS) ? QSConstants.MAX_FXCM_API_TICKS : counter;
                }
            }

            int timeCounter = 0;
            while (!Complete || timeCounter++ < 3000) //max timeout 30 seconds
            {
                Thread.Sleep(100);
            }

        }

        public void HistoricDataReceived(object sender, EventArgs e)
        {
            lock (locker)
            {
                MarketDataEventArg marketData = ((MarketDataEventArg) e);
                _mktData.CombineWith(marketData.data);
                _completeCounter--;
            }

            if (_completeCounter == 0)
            {
                complete = true;
                session.DetachHandler(mHandler);

                foreach (KeyValuePair<DateTime, Tick> KVP in _mktData.Data)
                {
                    KVP.Value.Symbol = Symbol;
                }
            }
        }
    }

}
