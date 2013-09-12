using System.Text;
using WaveletStudio;
using WaveletStudio.Blocks;

namespace QuantSys.Analytics.SignalProcessing
{
    public class SignalTransform
    {
        public double Run(double[] data, int detailLevel)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i]);
                if (i < data.Length - 1) sb.Append(",");
            }

            string datastring = sb.ToString();

            var textBlock = new ImportFromTextBlock
            {
                Text = datastring,
                ColumnSeparator = ",",
                SignalStart = 0,
                SignalNameInFirstColumn = false
            };

            var dwtBlock = new DWTBlock
            {
                WaveletName = "db10",
                Level = detailLevel,
                ExtensionMode = SignalExtension.ExtensionMode.ZeroPadding
            };


            var b = new BlockList();
            b.Add(textBlock);
            b.Add(dwtBlock);

            textBlock.ConnectTo(dwtBlock);

            b.ExecuteAll();

            int length = dwtBlock.OutputNodes[dwtBlock.OutputNodes.Count-1].Object[detailLevel - 1].Samples.Length;

            double val = dwtBlock.OutputNodes[dwtBlock.OutputNodes.Count-1].Object[detailLevel - 1].Samples[length - 1];

            return val;
        }
    }
}