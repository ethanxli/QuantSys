using System;
using System.IO;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using QuantSys.Util;
using QuantSys.Visualization.Highcharts.Enums;
using QuantSys.Visualization.Highcharts.Helpers;
using QuantSys.Visualization.Highcharts.Options;
using QuantSys.Visualization.Highstocks;

namespace QuantSys.Visualization
{
    public static class Visualize
    {

        public static string[] HighChartsIncludes =
        {
            "<script src='http://ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js'></script>",
            "<script src='http://code.highcharts.com/highcharts.js'></script>",
            "<script src='http://code.highcharts.com/modules/exporting.js'></script>"
        };
        

        public static string[] HighStocksIncludes =
        {
           @"<script src='http://ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js'></script>",
           @"<script src='http://code.highcharts.com/stock/highstock.js'></script>",
           @"<script src='http://code.highcharts.com/stock/modules/exporting.js'></script>"
        };

        public static Point[] VectorToPointArray(this DenseVector d)
        {
            var p = new Point[d.Count];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = new Point();
                p[i].X = i;
                p[i].Y = d[i];
            }
            return p;
        }

        public static object[] VectorToObjectArray(this DenseVector d)
        {
            var object_array = new object[d.Count];
            for (int i = 0; i < d.Count; i++) object_array[i] = d[i];
            return object_array;
        }

        public static string RGBtoHEX(int r, int g, int b)
        {
            string h1 = (r/16).ToString("X");
            string h2 = (r%16).ToString("X");
            string h3 = (g/16).ToString("X");
            string h4 = (g%16).ToString("X");
            string h5 = (b/16).ToString("X");
            string h6 = (b%16).ToString("X");

            return (h1 + h2 + h3 + h4 + h5 + h6);
        }

        public static string ScaleToColor(this double s, double low, double high)
        {
            s = (s - low)/(high - low);
            return (s < 0)
                ? RGBtoHEX(255, (int) (255 + 255*s), (int) (255 + 255*s))
                : RGBtoHEX((int) (255 + (-255*s)), 255, (int) (255 + (-255*s)));
        }

        public static void WriteHighChartToFile(this Highcharts.Highcharts chart, string filename)
        {
            using (var file = new StreamWriter(@filename))
            {
                file.WriteLine("<html><head>");
                foreach (string s in HighChartsIncludes) file.WriteLine(s);
                file.WriteLine("</head><body>");
                file.WriteLine(chart.ToHtmlString().Replace("NaN", "null"));
                file.WriteLine("</body></html>");
            }
        }


        public static void WriteDataToJson(string[] symbols, DateTime[] dateTimes, DenseMatrix data, string filename)
        {
            StringBuilder jsonStringBuilder = new StringBuilder();

            jsonStringBuilder.Append("[");
            for (int i = 0; i < data.ColumnCount; i++)
            {
                jsonStringBuilder.Append("[");
                jsonStringBuilder.Append(dateTimes[i].Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString());
                foreach (double d in data.Column(i))
                {
                    jsonStringBuilder.Append(",");
                    jsonStringBuilder.Append((d.Equals(double.NaN) ? "null" : Math.Round(d, 8).ToString()));
                }
                jsonStringBuilder.Append("]");
                if (i != data.ColumnCount - 1) jsonStringBuilder.Append(",\n");
            }
            jsonStringBuilder.Append("]");

            using (var file = new StreamWriter(QSConstants.DEFAULT_DATA_FILEPATH + @filename))
            {
                file.WriteLine(jsonStringBuilder.ToString());
            }

        }

        public static void GenerateHeatMatrix(string[] symbols, DenseMatrix data, string filename)
        {
            string lines =
                "<style type='text/css'>" +
                "#data tr td{ width:30px; height:30px; font:15px Trebuchet Ms; color:#333; padding:15px; text-align:center; border:1px solid #ccc}" +
                "</style>" +
                "<table id='data'>";

            lines += "<tr><td></td>";
            for (int j = 0; j < data.ColumnCount; j++)
            {
                lines += "<td>" + symbols[j] + "</td>";
            }
            lines += "</tr>";

            for (int i = 0; i < data.RowCount; i++)
            {
                lines += "<tr><td>" + symbols[i] + "</td>";

                for (int j = 0; j < data.ColumnCount; j++)
                {
                    string color = ScaleToColor(data[i, j], 0, 1);

                    lines += "<td style='background-color:#" + color + "'>" + data[i, j] + "</td>";
                }

                lines += "</tr>";
            }

            lines += "</tr></table>";

            using (var file = new StreamWriter(@filename))
            {
                file.WriteLine(lines);
            }
        }

        public static void GenerateBarGraph(this DenseVector d, string[] categories, string filename)
        {
            Highcharts.Highcharts chart = new Highcharts.Highcharts("chart3535").SetSeries(new Series
            {
                Data = new Data(d.VectorToPointArray())
            });

            chart.WriteHighChartToFile(filename);
        }

        public static void GenerateSimpleGraph(this DenseVector d, string filename)
        {
            Highcharts.Highcharts chart = new Highcharts.Highcharts("chart3535")
                .SetSeries(new Series
                {
                    Data = new Data(d.VectorToPointArray())
                })
                .SetPlotOptions(new PlotOptions
                {
                    Series = new PlotOptionsSeries {Marker = new PlotOptionsSeriesMarker {Enabled = false}, TurboThreshold = 0}
                })
                .InitChart(new Chart {ZoomType = ZoomTypes.X, Type = ChartTypes.Spline})
                ;

            chart.WriteHighChartToFile(filename);
        }

        public static void GenerateSimpleComparisonGraph(string[] dataNames, DenseMatrix data, string filename)
        {
            Highcharts.Highcharts chart = new Highcharts.Highcharts("chart3535");
            chart.WriteHighChartToFile(filename);
        }

        public static void GenerateScatterPlot(string xAxis, string yAxis, DenseMatrix data, string filename)
        {
            
        }

        public static void GenerateStrategyComparisonGraph(string[] strategies, DateTime[] dateTimes, DenseMatrix data,
                                                      string filename, string title)
        {
            
        }

        public static void GenerateMultiPaneGraph(string[] symbols, DateTime[] dateTimes, DenseMatrix data, string filename, ChartOption[] options = null, HighstockFlag[] flags = null, string JSONFILENAME = "temp.json")
        {
            //Set default options
            if (ReferenceEquals(null,options) || options.Length != symbols.Length)
            {
                options = new ChartOption[symbols.Length];
                for (int i = 0; i < symbols.Length; i++)
                {
                    options[i] = new ChartOption() {ChartType = "spline", Height = 200, YPosition = i};
                }
            }

            WriteDataToJson(symbols, dateTimes, data, JSONFILENAME);

            StringBuilder GraphJSBuilder = new StringBuilder();
            GraphJSBuilder.Append(@"<script type='text/javascript'>                             " + "\n");
            GraphJSBuilder.Append(@"$(function() {                                              " + "\n");
            GraphJSBuilder.Append(@"	$.getJSON('" + JSONFILENAME + "', function(data) {                 " + "\n");

            //add ohlc data (first 4 values)
            GraphJSBuilder.Append("var ohlc =[];" + "\n");

            for (int i = 1; i < symbols.Length; i++)
            {
                GraphJSBuilder.Append("var " + "symbol" + i + "=[];" + "\n");
            }

            GraphJSBuilder.Append(@"dataLength = data.length;"           + "\n");
            GraphJSBuilder.Append(@"for (i = 0; i < dataLength; i++) { " + "\n");
            for (int i = 1; i < symbols.Length; i++)
            {
                
                GraphJSBuilder.Append(@"ohlc.push([ data[i][0], data[i][1], data[i][2], data[i][3], data[i][4] ]);" + "\n"); //ohlc
                GraphJSBuilder.Append(@"symbol" + i + ".push([ data[i][0],data[i][" + (i + 4) + "]]);" + "\n"); //push indicator data
            }
            GraphJSBuilder.Append(@"}" + "\n");
            GraphJSBuilder.Append(@"      var groupingUnits = [                           " + "\n");
            GraphJSBuilder.Append(@"            ['hour', [1,2,3] ],                       " + "\n");
            GraphJSBuilder.Append(@"            ['day',  [1,2,3] ],                       " + "\n");
            GraphJSBuilder.Append(@"            ['week', [1,2]   ],                       " + "\n");
            GraphJSBuilder.Append(@"            ['month',[1]     ]];                      " + "\n");
            GraphJSBuilder.Append(@"		$('#container').highcharts('StockChart', {    " + "\n");
            GraphJSBuilder.Append(@"		    rangeSelector: {selected: 1  },           " + "\n");
            GraphJSBuilder.Append(@"		    title: {  text: '" + symbols[0] + "'},    " + "\n");
            GraphJSBuilder.Append(@"            legend: { enabled: true },                " + "\n");
            GraphJSBuilder.Append(@"            chart:{zoomType: 'x'},                    " + "\n");
            GraphJSBuilder.Append(@"            scrollbar: { enabled:false},              " + "\n");

            GraphJSBuilder.Append(" yAxis: [" + "\n");
            for (int i = 0; i < symbols.Length; i++)
            {
                double top = 70;
                //get height of element
                for (int j = 0; j < i; j++)
                {
                    top += options[j].Height + 10;
                }
                
                GraphJSBuilder.Append(
                    "{" +
                        (!options[i].Layover?"title: {text: '" + symbols[i] + "'}, ":"") +
                        "height:" + options[i].Height + ", " +
                        ((i == 0) ? "" : "top:" + top + ",") + 
                        "lineWidth:2, " +
                        "offset:0" +
                    "}" +
                    ((i == symbols.Length - 1) ? "" : ",") +  "\n");
            }
            GraphJSBuilder.Append(@"]," + "\n");

            //Add Series Data----------------------
            GraphJSBuilder.Append(@"series: [" + "\n");

            //ohlc
            GraphJSBuilder.Append(
                "{" +
                    "type: 'candlestick'," +
                    "name: '" + symbols[0] + "', " +
                    "id: '" + symbols[0] + "', " +
                    "data: ohlc," +
                    "dataGroup:{units:groupingUnits}" +
                "}," + "\n");

            //indicator
            for (int i = 1; i < symbols.Length; i++)
            {
                GraphJSBuilder.Append(
                    "{" +
                        "type: '"+ options[i].ChartType +"'," + 
                        "name: '" + symbols[i] + "', " +
                        "id: '" + symbols[i] + "', " +
                        "data:symbol" + i +"," +
                        "yAxis:" + options[i].YPosition + "," +
                        "dataGroup:{units:groupingUnits}" +
                    "}" +  ((i == symbols.Length - 1) ? "" : ",") +  "\n");
            }

            if (flags != null)
            {
                //add flags------------
                GraphJSBuilder.Append(
                    ",{" +
                    "type : 'flags'," +
                    "data : [");

                if (!ReferenceEquals(null, flags))
                {
                    for (int i = 0; i < flags.Length; i++)
                    {
                        GraphJSBuilder.Append(
                            "{" +
                            "x : Date.UTC(" + flags[i].Date.Year + "," + (flags[i].Date.Month - 1) + "," +
                            flags[i].Date.Day + "," +
                            flags[i].Date.Hour + "," + flags[i].Date.Minute + ")," +
                            "title : '" + flags[i].Title + "'," +
                            "text : '" + flags[i].Text + "'" +
                            "}" + ((i == flags.Length - 1) ? "" : ",") + "\n"
                            );
                    }
                }

                GraphJSBuilder.Append(
                    "]," +
                    "onSeries : '" + symbols[0] + "'," +
                    "shape : 'squarepin'," +
                    "width : 9," +
                    "height: 10," +
                    "stickyTracking: false," +
                    "style: {fontSize:'9px',fontWeight: 'normal'}" +
                    "}"
                    );

                //endflags---------------------
            }

            GraphJSBuilder.Append(@"]" + "\n");
            //endseries-------------------------------------


            //calculate height
            double height = 170;
            for (int i = 0; i < options.Length; i++)
            {
                height += options[i].Height + ((options[i].Height > 0)? 20:0);
            }
            
            GraphJSBuilder.Append(@"});});});   " + "\n");
            GraphJSBuilder.Append(@"</script>     " + "\n");
            GraphJSBuilder.Append(@"<div id='container' style='height:"+height+"; width:90%; margin: 0 auto'></div>" );
            


            using (var file = new StreamWriter(@filename))
            {
                file.WriteLine("<html><head>");
                foreach(string s in HighStocksIncludes)
                    file.WriteLine(s);
                file.WriteLine("</head><body>");
                file.WriteLine(GraphJSBuilder.ToString());
                file.WriteLine("<script src=\"style.js\"></script>");
                file.WriteLine("</body></html>");
            }

        }

        public static void GenerateMultiSymbolGraph(string[] symbols, DenseMatrix data, DateTime startDate,
            TimeSpan timeSpan,
            string filename)
        {
            string startDateStr = "" + startDate.Year + ", " + (startDate.Month - 1) + ", " + startDate.Day + ", " +
                                  startDate.Hour + ", " + startDate.Minute;

            string lines = "<HTML><head>" +
                           "<script src='http://ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js'></script>" +
                           "<script src='http://code.highcharts.com/highcharts.js'></script> " +
                           " <script src='http://code.highcharts.com/modules/exporting.js'></script>" +
                           "</head><body>" +
                           "<script type='text/javascript'>" +
                           "$(function () {$('#container').highcharts(" +
                           "{ chart:{zoomType: 'x', spacingRight: 20}," +
                           "plotOptions: { series:{marker: {enabled:false}}}," +
                           "title: {text: ''}, " +
                           "xAxis: {type: 'datetime', title: { text: '' }  }," +
                           " yAxis: { title: { text: 'Exchange rate' } }, " +
                           "tooltip: { shared: true}, " +
                           "legend: { enabled: true },  " +
                           "series: [";

            for (int i = 0; i < data.RowCount; i++) //for each symbol
            {
                lines += "{type: 'spline', name: '" + symbols[i] + "', pointInterval: " +
                         (int) timeSpan.TotalMilliseconds + ", pointStart: Date.UTC(" + startDateStr + "), data: [";

                for (int j = 0; j < data.ColumnCount; j++)
                {
                    lines += ((data[i, j].Equals(double.NaN)) ? "null" : "" + data[i, j]);
                    if (j < data.ColumnCount - 1) lines += ",";
                }
                lines += "]}";
                if (i < data.RowCount - 1) lines += ",\n";
            }

            lines += "] });});</script><div id='container' style='height:700px;margin: 0 auto'></div></body></html>";


            using (var file = new StreamWriter(@filename))
            {
                file.WriteLine(lines);
            }
        }


        public static void GeneratePredictionGraph(string[] symbols, DenseMatrix data, DateTime startDate,
            TimeSpan timeSpan,
            string filename)
        {
            string startDateStr = "" + startDate.Year + ", " + (startDate.Month - 1) + ", " + startDate.Day + ", " +
                                  startDate.Hour + ", " + startDate.Minute;

            string lines = "<HTML><head>" +
                           "<script src='http://ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js'></script>" +
                           "<script src='http://code.highcharts.com/highcharts.js'></script> " +
                           " <script src='http://code.highcharts.com/modules/exporting.js'></script>" +
                           "</head><body>" +
                           "<script type='text/javascript'>" +
                           "$(function () {$('#container').highcharts(" +
                           "{ chart:{zoomType: 'x', spacingRight: 20}," +
                           "title: {text: ''}, " +
                           "xAxis: {type: 'datetime', title: { text: '' }  }," +
                           " yAxis: { title: { text: 'Exchange rate' } }, " +
                           "tooltip: { shared: true}, " +
                           "legend: { enabled: true },  " +
                           "series: [";

            for (int i = 0; i < data.RowCount; i++) //for each symbol
            {
                string type = (i == 0) ? "spline" : "scatter";
                lines += "{type: '" + type + "', name: '" + symbols[i] + "', pointInterval: " +
                         (int) timeSpan.TotalMilliseconds + ", pointStart: Date.UTC(" + startDateStr + "), data: [";

                for (int j = 0; j < data.ColumnCount; j++)
                {
                    lines += ((data[i, j].Equals(double.NaN)) ? "null" : "" + data[i, j]);
                    if (j < data.ColumnCount - 1) lines += ",";
                }
                lines += "]}";
                if (i < data.RowCount - 1) lines += ",\n";
            }

            lines += "] });});</script><div id='container' style='height:700px;margin: 0 auto'></div></body></html>";


            using (var file = new StreamWriter(@filename))
            {
                file.WriteLine(lines);
            }
        }
    }
}