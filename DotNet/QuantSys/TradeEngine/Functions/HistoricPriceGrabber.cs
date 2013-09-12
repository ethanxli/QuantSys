using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.TradeEngine.EventArguments;
using QuantSys.TradeEngine.Listener;
using QuantSys.Util;
using fxcore2;

namespace QuantSys.TradeEngine.Functions
{
    public class HistoricPriceGrabber
    {

        private Quantum _mktData;
        private int _completeCounter;
        private bool complete;

        private FXSession session;
        private ResponseHandler mHandler;

        public bool Complete { get { return complete; } }
        public Quantum Data { get { return _mktData; } }
        public Symbol Symbol { get; set; }

        public HistoricPriceGrabber(FXSession session)
        {
            _mktData = new Quantum();
            _completeCounter = 0;
            complete = false;
            mHandler = HistoricDataReceived;
            this.session = session;
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
            while (counter > 0)
            {
                _completeCounter++;
                int subticks = (counter >= QSConstants.MAX_FXCM_API_TICKS) ? QSConstants.MAX_FXCM_API_TICKS : counter;
                O2GRequest request = factory.createMarketDataSnapshotRequestInstrument(symbol, tfo, subticks);
                factory.fillMarketDataSnapshotRequestTime(request, startDate, startDate.AddMinutes(2* subticks * Timeframe.TimeframeToMinutes(timeframe)));
                session.Session.sendRequest(request);

                startDate = startDate.AddMinutes(subticks * Timeframe.TimeframeToMinutes(timeframe));
                counter -= (counter >= QSConstants.MAX_FXCM_API_TICKS) ? QSConstants.MAX_FXCM_API_TICKS : counter;
            }

        }


        public void HistoricDataReceived(object sender, EventArgs e)
        {
            MarketDataEventArg marketData = ((MarketDataEventArg) e);
            _mktData.CombineWith(marketData.data);
            _completeCounter--;

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
