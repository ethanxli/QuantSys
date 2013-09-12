using System;
using fxcore2;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Listener
{
    
    public delegate void ResponseHandler(object sender, EventArgs e);
    
    public class ResponseListener : IO2GResponseListener
    {
        private readonly FXSession _connection;
        public event ResponseHandler ResponseReceived;
        

        public ResponseListener(FXSession connection)
        {
            _connection = connection;
        }

        
        public void onRequestCompleted(string s, O2GResponse response)
        {
            Console.WriteLine(s);

            switch (response.Type)
            {
                case O2GResponseType.CommandResponse:
                case O2GResponseType.CreateOrderResponse:
                case O2GResponseType.GetAccounts:
                case O2GResponseType.GetClosedTrades:
                case O2GResponseType.GetMessages:
                case O2GResponseType.GetOffers:
                case O2GResponseType.GetOrders:
                case O2GResponseType.GetSystemProperties:
                case O2GResponseType.GetTrades:
                    {
                        try
                        {
                            AccountInformationEventArg data = AccountInformationEventArg.ProcessData(_connection,
                                                                                                     response);
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;
                    }
                case O2GResponseType.MarketDataSnapshot:
                    {
                        try
                        {
                            MarketDataEventArg mData = MarketDataEventArg.ProcessMarketData(_connection, response);
                            OnResponseReceived(mData);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    }
                case O2GResponseType.ResponseUnknown:
                case O2GResponseType.TablesUpdates:
                    break;
            }
        }

        public void onRequestFailed(string s1, string s2)
        {
            Console.WriteLine(s1 + " " + s2);
        }

        public void onTablesUpdates(O2GResponse response)
        {
            //Console.WriteLine(response.ToString());

            switch (response.Type)
            {
                case O2GResponseType.CommandResponse:
                case O2GResponseType.CreateOrderResponse:
                case O2GResponseType.GetAccounts:
                case O2GResponseType.GetClosedTrades:
                case O2GResponseType.GetMessages:
                case O2GResponseType.GetOffers:
                case O2GResponseType.GetOrders:
                case O2GResponseType.GetSystemProperties:
                case O2GResponseType.GetTrades:
                case O2GResponseType.MarketDataSnapshot:
                case O2GResponseType.ResponseUnknown:
                case O2GResponseType.TablesUpdates:
                    break;
            }
        }


        protected virtual void OnResponseReceived(EventArgs e)
        {
            if (ResponseReceived != null)
                ResponseReceived(this, e);
        }

    }
}