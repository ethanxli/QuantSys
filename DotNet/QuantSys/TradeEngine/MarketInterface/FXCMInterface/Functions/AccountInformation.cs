using System;
using fxcore2;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Listener;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions
{
    public class AccountInformation
    {
        private FXSession session;
        private ResponseHandler mHandler;

        public AccountInformation(FXSession session)
        {
            this.session = session;
            mHandler = AccountInformationReceived;
            session.AttachHandler(mHandler);
        }

        public bool ExistsPositionFor(string symbol)
        {
            O2GResponseReaderFactory factory = session.Session.getResponseReaderFactory();
            // Gets first account from login.
            O2GLoginRules loginRules = session.Session.getLoginRules();
            O2GResponse response = loginRules.getTableRefeshResponse(O2GTable.ClosedTrades);

            O2GTradesTableResponseReader tradesReader = factory.createTradesTableReader(response);
            
            for (int i = 0; i < tradesReader.Count; i++)
            {
                O2GTradeRow tradeRow = tradesReader.getRow(i);
                Console.WriteLine("Trades---");
                Console.WriteLine(tradeRow.OpenQuoteID);
                Console.WriteLine(tradeRow.OfferID);
            }

            return false;
        }



        public void AccountInformationReceived(object sender, EventArgs e)
        {
            AccountInformationEventArg accData = ((AccountInformationEventArg)e);

        }



    }
}
