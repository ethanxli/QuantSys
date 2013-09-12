using System;
using fxcore2;

namespace QuantSys.TradeEngine.MarketInterface.FXCMInterface.Listener
{
    public class SessionStatusListener : IO2GSessionStatus
    {
        private readonly FXSession _connection;

        public SessionStatusListener(FXSession connection)
        {
            _connection = connection;
        }

        public void onLoginFailed(string s)
        {
            Console.WriteLine("\nLogin Failed. " + s);
            _connection.ChangeStatus(FXSession.LOGIN_STATUS.LOGIN_FAILED);
        }

        public void onSessionStatusChanged(O2GSessionStatusCode ssc)
        {
            Console.WriteLine(ssc);

            switch (ssc)
            {
                case O2GSessionStatusCode.Connected:
                {
                    _connection.ChangeStatus(FXSession.LOGIN_STATUS.LOGGED_IN);
                    break;
                }
                case O2GSessionStatusCode.Connecting:
                case O2GSessionStatusCode.Disconnected:
                case O2GSessionStatusCode.Disconnecting:
                case O2GSessionStatusCode.PriceSessionReconnecting:
                case O2GSessionStatusCode.Reconnecting:
                case O2GSessionStatusCode.SessionLost:
                case O2GSessionStatusCode.TradingSessionRequested:
                case O2GSessionStatusCode.Unknown:
                    break;
            }
        }
    }
}