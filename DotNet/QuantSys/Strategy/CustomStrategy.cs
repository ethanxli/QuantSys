using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.Indicators.Abstraction;
using QuantSys.Indicators.Averages;
using QuantSys.Indicators.Oscillators;
using QuantSys.Indicators.Transforms;
using QuantSys.MarketData;
using QuantSys.PortfolioEngine;
using QuantSys.PortfolioEngine.Order;
using QuantSys.Util;

namespace QuantSys.Strategy
{
    public class CustomStrategy : AbstractStrategy
    {
        public AccountManager AccountManager { get; set; }

        private const double STOPLEVEL = 0.005;
        public Dictionary<string, AbstractIndicator> Indicators { get { return indicatorList; } }


        public int MACD1 { get; set; }
        public int MACD2 { get; set; }
        public int MACD3 { get; set; }

        public int WILLIAMSR1 { get; set; }
        public int WILLENTRY1 { get; set; }
        public int WILLENTRY2 { get; set; }

        public int PRANK1 { get; set; }
        public int SMA1 { get; set; }

        public int SWITCH { get; set; }

        public CustomStrategy(int MACD1, int MACD2, int MACD3, int WilliamsR1, int WillEntry1, int WillEntry2, int PRANK1, int SMA1, int SWITCH)
        {
            this.MACD1 = MACD1;
            this.MACD2 = MACD2;
            this.MACD3 = MACD3;
            this.WILLENTRY1 = WillEntry1;
            this.WILLENTRY2 = WillEntry2;
            this.WILLIAMSR1 = WilliamsR1;
            this.PRANK1 = PRANK1;
            this.SMA1 = SMA1;
            this.SWITCH = SWITCH;

            //AttachIndicator("AO", new AO());
            //AttachIndicator("UO", new UltimateOscillator());
            //AttachIndicator("AC", new AC());
            //AttachIndicator("MFI", new MFI());
            AttachIndicator("MACD", new MACD(MACD1, MACD2, MACD3));
            AttachIndicator("William", new WilliamsR(WILLIAMSR1));
            AttachIndicator("PRankSMA", new PercentileRank(PRANK1, new SMA(SMA1)));

       } 

        public override void OnTick(params Tick[] t)
        {
            foreach (var indicator in this.indicatorList)
            {
                indicator.Value.HandleNextTick(t[0]);
            }


            //Check Spread Cost before trading
            if (t[0].AskClose - t[0].BidClose > 0.001) return;

            /////////////////////////////
            /// ENTRY
            /// ////////////////////////
            
            //UPTREND
            if (indicatorList["PRankSMA"][0] > SWITCH && indicatorList["PRankSMA"][0] >= indicatorList["PRankSMA"][1])
            {
                //%R and MFI Oversold
                //if (indicatorList["MFI"][0] < 20)
                //{
                    //AC Cross over 0
                    /*
                    if(indicatorList["AC"][0] > 0 && indicatorList["AC"][1] < 0)
                    {
                        if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                        {
                            AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                        }
                    }
                    */

                    //%R Cross over 20

                if (indicatorList["William"][0] > -WILLENTRY1 && indicatorList["William"][1] < -WILLENTRY1)
                {
                    if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                    }
                }

                    /*
                    if (indicatorList["PRankSMA"][1] <50)
                    {
                        if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                        {
                            AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                        }
                    }*/

                    //MFI Cross over 30
                    /*
                    if (indicatorList["MFI"][0] > 20 && indicatorList["MFI"][1] < 20)
                    {
                        if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                        {
                            AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                        }
                    }*/
                    
                    //MACD Cross Over Signal Line and MACD < 0
                    /*
                    if (((MACD) indicatorList["MACD"])[0] > ((MACD) indicatorList["MACD"]).Signal[0] &&
                        ((MACD)indicatorList["MACD"])[1] < ((MACD)indicatorList["MACD"]).Signal[1] && ((MACD)indicatorList["MACD"])[0] < 0)
                    {
                        if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                        {
                            AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Long, 10000), STOPLEVEL);
                        }
                    }
                 */

                //}


            }

            //DOWNTREND
            else if (indicatorList["PRankSMA"][0] < SWITCH && indicatorList["PRankSMA"][0] <= indicatorList["PRankSMA"][1]) 
            {

                if (indicatorList["William"][0] < -WILLENTRY2 && indicatorList["William"][1] > -WILLENTRY2)
                {
                    if (!AccountManager.ExistsPositionForSymbol(t[0].Symbol))
                    {
                        AccountManager.PlaceMarketOrder(new MarketOrder(t[0], Position.PositionSide.Short, 10000), STOPLEVEL);
                    }
                }            
    
            }

            //////////////////////////////
            /// EXIT
            /// //////////////////////////
            
            //Exit LONG
            if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Long))
            {
                if (((MACD) indicatorList["MACD"])[0] < ((MACD)indicatorList["MACD"]).SubIndicators["Signal"][0] &&
                        ((MACD)indicatorList["MACD"])[1] > ((MACD)indicatorList["MACD"]).SubIndicators["Signal"][1] && ((MACD)indicatorList["MACD"])[0] > 0)
                    {
                        AccountManager.CloseOrder(t[0]);
                    }
                 
                /*
                if (indicatorList["William"][0] > -20)
                {
                    AccountManager.CloseOrder(t[0]);
                }*/
            }

            //Exit LONG
            if (AccountManager.ExistsPositionForSymbol(t[0].Symbol) && AccountManager.Portfolio.Positions[t[0].Symbol].Side.Equals(Position.PositionSide.Short))
            {
                if (((MACD)indicatorList["MACD"])[0] > ((MACD)indicatorList["MACD"]).SubIndicators["Signal"][0] &&
                        ((MACD)indicatorList["MACD"])[1] < ((MACD)indicatorList["MACD"]).SubIndicators["Signal"][1] && ((MACD)indicatorList["MACD"])[0] < 0)
                {
                    AccountManager.CloseOrder(t[0]);
                }

                /*
                if (indicatorList["William"][0] > -20)
                {
                    AccountManager.CloseOrder(t[0]);
                }*/
            }

        }
    }
}
