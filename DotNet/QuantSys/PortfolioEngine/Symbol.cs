namespace QuantSys.PortfolioEngine
{
    public class Symbol
    {
        public Symbol(string symbol)
        {
            SymbolString = symbol;
        }

        public string SymbolString { get; set; }


        public override int GetHashCode()
        {
            return SymbolString.GetHashCode();
        }
    }
}