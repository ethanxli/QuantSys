using QuantSys.MarketData;

namespace QuantSys.Indicators.Abstraction
{
    public interface IIndicator
    {
        double this[int index] { get; }
        double HandleNextTick(Tick t);

    }
}