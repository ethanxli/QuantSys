using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;

namespace QuantSys.Visualization
{
    class TableGeneration
    {
        HtmlTable table = new HtmlTable();

        public HtmlTable Table
        {
            get { return table; }
            set { table = value; }
        }

    }
}
