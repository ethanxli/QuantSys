using System;
using System.Collections.Generic;
using System.Threading;
using QuantSys.Analytics.Timeseries.Indicators.Abstraction;
using QuantSys.MarketData;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface;
using QuantSys.TradeEngine.MarketInterface.FXCMInterface.Functions;
using QuantSys.TradeEngine.Strategy;

namespace QuantSys.TradeEngine.AccountManagement
{
    public class LiveTradeEngine
    {
        public string TradeTimeframe { get; set; }


        private OrderPlacementEngine _opEngine;
        private HistoricPriceEngine _priceEngine;

        private LiveAccountManager _accountManager;
        private List<AbstractStrategy> _strategies;
        private List<Symbol> _symbolList;
        private FXSession _session;
        
        public LiveTradeEngine()
        {
            _session = new FXSession();
            _opEngine = new OrderPlacementEngine(_session);
            _priceEngine = new HistoricPriceEngine(_session);
            _accountManager = new LiveAccountManager(_opEngine);
        }
        public void LoadStrategy(AbstractStrategy s)
        {
            //s.SetAccountManager(_accountManager);
            s.IsLive = false;
            _strategies.Add(s);
            Console.WriteLine("Loaded" + s.ToString());
        }

        public void LoadSymbols(params Symbol[] symbols)
        {
            foreach (Symbol s in symbols)
            {
                _symbolList.Add(s);
                Console.WriteLine("Loaded" + s);
            }
        }

        private void PrepDataForStrategies()
        {
            int maxN = 0;
            List<Quantum> data = new List<Quantum>();

            foreach (AbstractStrategy s in _strategies)
            {
                foreach (AbstractIndicator i in s.indicatorList.Values)
                {
                    if (i.Period > maxN) maxN = i.Period;
                }
                foreach (AbstractChannel i in s.channelList.Values)
                {
                    if (i.Period > maxN) maxN = i.Period;
                }
            }

            foreach (Symbol s in _symbolList)
            {
                _priceEngine.GetLongHistoricPrices(s.SymbolString, TradeTimeframe, 2*maxN);
                data.Add(_priceEngine.Data);
                _priceEngine.Reset();
            }

            MultiQuantum m = MultiQuantum.OrganizeMultiQuantum(data);

            foreach (List<Tick> ticks in m)
            {
                foreach (AbstractStrategy s in _strategies)
                {
                    s.OnTick(ticks.ToArray());
                }
            }

            foreach (AbstractStrategy s in _strategies)
            {
                s.IsLive = true;
            }

        }

        public void Execute()
        {
            _session.InitializeSession();

            Console.WriteLine("Prepping strategy data...");
            PrepDataForStrategies();
            Console.WriteLine("Done.");

            while (true)
            {
                if (!_session.LoggedIn) _session.InitializeSession();

                try
                {
                    List<Tick> ticks = new List<Tick>();

                    foreach (Symbol s in _symbolList)
                    {
                        OrderPlacementEngine.OrderObject ob = _opEngine.prepareParamsFromLoginRules(s.SymbolString);
                        Tick t = new Tick();
                    }

                    foreach (var strat in _strategies)
                    {
                        strat.OnTick(ticks.ToArray());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //Sleep specified minutes
                Console.WriteLine(DateTime.Now + ":Sleeping...");
                Thread.Sleep(Timeframe.TimeframeToMinutes(TradeTimeframe) * 60 * 1000);
            }

            return;
        }

    }
}
