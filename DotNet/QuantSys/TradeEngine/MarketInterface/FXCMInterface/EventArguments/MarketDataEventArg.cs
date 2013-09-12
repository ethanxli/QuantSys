using System;
using System.Collections.Generic;
using fxcore2;
using QuantSys.MarketData;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments
{
    public class MarketDataEventArg : EventArgs
    {
        public Quantum data;

        public MarketDataEventArg(Quantum data)
        {
            this.data = data;
        }

        public MarketDataEventArg()
        {
            data = new Quantum();
        }

        public static MarketDataEventArg ProcessMarketData(FXSession connection, O2GResponse response)
        {
            try
            {
                O2GResponseReaderFactory rrfactory = connection.Session.getResponseReaderFactory();
                O2GMarketDataSnapshotResponseReader mReader = rrfactory.createMarketDataSnapshotReader(response);

                var d = new SortedList<DateTime, Tick>(mReader.Count);

                for (int i = 0; i < mReader.Count; i++)
                {
                    // information like reader.getDate(i), reader.getBidOpen(i), reader.getBidHigh(i), reader.getBidLow(i), reader.getBidClose(i), reader.getVolume(i) is now available
                    //Console.WriteLine(i + ":" + mReader.getDate(i).ToString() + ":" + mReader.getBidOpen(i));
                    //create a quantum of ticks for the market data
                    var tick = new Tick(
                        mReader.getBid(i),
                        mReader.getBidOpen(i),
                        mReader.getBidHigh(i),
                        mReader.getBidLow(i),
                        mReader.getBidClose(i),
                        mReader.getAsk(i),
                        mReader.getAskOpen(i),
                        mReader.getAskHigh(i),
                        mReader.getAskLow(i),
                        mReader.getAskClose(i),
                        mReader.getVolume(i),
                        mReader.getDate(i));
                        
                    d.Add(mReader.getDate(i), tick);
                }

                var q = new Quantum(d);
                return new MarketDataEventArg(q);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new MarketDataEventArg(new Quantum(new SortedList<DateTime, Tick>(300)));
            }
        }
    }
}