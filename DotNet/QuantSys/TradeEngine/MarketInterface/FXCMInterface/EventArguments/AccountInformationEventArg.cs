using System;
using fxcore2;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.EventArguments
{
    public class AccountInformationEventArg : EventArgs
    {

        public AccountInformationEventArg()
        {
            
        }

        public static AccountInformationEventArg ProcessData(FXSession connection, O2GResponse response)
        {
            return new AccountInformationEventArg();
        }
    }
}
