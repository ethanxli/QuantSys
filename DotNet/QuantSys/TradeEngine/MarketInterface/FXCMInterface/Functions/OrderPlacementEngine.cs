using fxcore2;
using QuantSys.MarketData;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions
{
    public class OrderPlacementEngine
    {

        private FXSession _session;

        public class OrderObject
        {
            public string AccountID { get; set; }
            public string OfferID { get; set; }
            public Symbol Symbol { get; set; }
            public double Ask { get; set; }
            public double Bid { get; set; }
            public double PointSize { get; set; }
            public double Spread { get { return Bid - Ask; } }
            public double BaseAmount { get; set; }

            public OrderObject()
            {
                
            }
        }

        public OrderPlacementEngine(FXSession session)
        {
            _session = session;
        }

        public OrderObject prepareParamsFromLoginRules(string instrument)
        {
            OrderObject orderObject = new OrderObject();
            O2GLoginRules loginRules = _session.Session.getLoginRules();
            O2GResponseReaderFactory factory = _session.Session.getResponseReaderFactory();
            // Gets first account from login.
            O2GResponse accountsResponse = loginRules.getTableRefeshResponse(O2GTable.Accounts);
            O2GAccountsTableResponseReader accountsReader = factory.createAccountsTableReader(accountsResponse);
            O2GAccountRow account = accountsReader.getRow(0);

            orderObject.AccountID = account.AccountID;
            // Store base iAmount
            orderObject.BaseAmount = account.BaseUnitSize;

            O2GResponse offerResponse = loginRules.getTableRefeshResponse(O2GTable.Offers);
            O2GOffersTableResponseReader offersReader = factory.createOffersTableReader(offerResponse);
            for (int i = 0; i < offersReader.Count; i++)
            {
                O2GOfferRow offer = offersReader.getRow(i);
                if (instrument.Equals(offer.Instrument))
                {
                    orderObject.OfferID = offer.OfferID;
                    orderObject.Ask = offer.Ask;
                    orderObject.Bid = offer.Bid;
                    orderObject.PointSize = offer.PointSize;
                    orderObject.Symbol = new Symbol(instrument);
                    break;
                }
            }

            return orderObject;
        }


        public void CreateTrueMarketOrder(string accountID, string sOfferID, int iAmount, string sBuySell)
        {
            
            O2GRequestFactory factory = _session.Session.getRequestFactory();

            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Order.TrueMarketOpen);
            valuemap.setString(O2GRequestParamsEnum.AccountID, accountID);            // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                // The identifier of the instrument the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.BuySell, (sBuySell.Equals("Buy")?Constants.Buy:Constants.Sell));                // The order direction: Constants.Sell for "Sell", Constants.Buy for "Buy".
            valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                    // The quantity of the instrument to be bought or sold.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "TrueMarketOrder");    // The custom identifier of the order.

            O2GRequest request = factory.createOrderRequest(valuemap);
            _session.Session.sendRequest(request);
        }


        public void CreateTrueMarketCloseOrder(string sOfferID, string sAccountID, string sTradeID, int iAmount, string sBuySell)
        {

            O2GRequestFactory factory = _session.Session.getRequestFactory();

            O2GValueMap valuemap = factory.createValueMap();
            valuemap.setString(O2GRequestParamsEnum.Command, Constants.Commands.CreateOrder);
            valuemap.setString(O2GRequestParamsEnum.OrderType, Constants.Order.TrueMarketClose);
            valuemap.setString(O2GRequestParamsEnum.AccountID, sAccountID);                // The identifier of the account the order should be placed for.
            valuemap.setString(O2GRequestParamsEnum.OfferID, sOfferID);                    // The identifier of the instrument the order should be placed for.
            // valuemap.setString(O2GRequestParamsEnum.TradeID, sTradeID);                    // The identifier of the trade to be closed.
            // valuemap.setInt(O2GRequestParamsEnum.Amount, iAmount);                      // The quantity of the instrument to be bought or sold. Must be <= to the size of the position ("Trades" table, Lot column).
            valuemap.setString(O2GRequestParamsEnum.NetQuantity, "Y");
            valuemap.setString(O2GRequestParamsEnum.BuySell, (sBuySell.Equals("Buy")?Constants.Buy:Constants.Sell));                    // The order direction: Constants.Buy for Buy, Constants.Sell for Sell. Must be opposite to the direction of the trade.
            valuemap.setString(O2GRequestParamsEnum.CustomID, "CloseTrueMarketOrder");  // The custom identifier of the order.

            O2GRequest request = factory.createOrderRequest(valuemap);
            _session.Session.sendRequest(request);
        }

    }
}
