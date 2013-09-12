using System;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.AccountManagement
{
    public class LiveAccountManager : IAccountManager
    {
        private OrderPlacementEngine _opEngine;
        public LiveAccountManager(OrderPlacementEngine opEngine)
        {
            _opEngine = opEngine;
        }

        public void PlaceMarketOrder(Symbol sym, int size, Position.PositionSide side, double stopPips, double LimitPips)
        {
            OrderPlacementEngine.OrderObject orderObject = _opEngine.prepareParamsFromLoginRules(sym.SymbolString);
            _opEngine.CreateTrueMarketOrder(orderObject.AccountID, orderObject.OfferID, size,
                (side.Equals(Position.PositionSide.Long)) ? "Buy" : "Sell");
        }


        public void ClosePosition(Symbol s)
        {
            OrderPlacementEngine.OrderObject orderObject = _opEngine.prepareParamsFromLoginRules(s.SymbolString);
            /*
            _opEngine.CreateTrueMarketCloseOrder(orderObject.AccountID, orderObject.OfferID, size,
                (side.Equals(Position.PositionSide.Long)) ? "Buy" : "Sell");
             */
        }

        public void ModifyStopOrder()
        {
            throw new NotImplementedException();
        }

        public void SetTrailingStop()
        {
            throw new NotImplementedException();
        }

        public void PlaceStopOrder()
        {
            throw new NotImplementedException();
        }
        
        public bool ExistsShortPositionForSymbol(Symbol s)
        {
            throw new NotImplementedException();
        }

        public bool ExistsLongPositionForSymbol(Symbol s)
        {
            throw new NotImplementedException();
        }

        public bool ExistsPositionForSymbol(Symbol s)
        {
            throw new NotImplementedException();
        }
    }
}
