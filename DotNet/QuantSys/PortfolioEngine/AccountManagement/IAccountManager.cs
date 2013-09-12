using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantSys.PortfolioEngine
{
    public interface IAccountManager
    {


        void PlaceMarketOrder();

        void ModifyStopOrder();

        void SetTrailingStop();

        void PlaceStopOrder();

        bool ExistsPositionFor();

        bool ExistsLongPositionFor();

        bool ExistsShortPositionFor();

        void ClosePosition();


    }
}
