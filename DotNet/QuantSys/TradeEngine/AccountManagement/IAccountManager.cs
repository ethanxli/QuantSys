using QuantSys.MarketData;
using QuantSys.TradeEngine.Simulation.Account;

namespace QuantSys.TradeEngine.AccountManagement
{
    public interface IAccountManager
    {


        void PlaceMarketOrder(Symbol sym, int size, Position.PositionSide side, double stopPips = double.NaN, double LimitPips = double.NaN);

        void ModifyStopOrder();

        void SetTrailingStop();

        void PlaceStopOrder();

        bool ExistsPositionForSymbol(Symbol s);

        bool ExistsLongPositionForSymbol(Symbol s);

        bool ExistsShortPositionForSymbol(Symbol s);

        void ClosePosition(Symbol s);

        //void increasePosition;

    }
}
