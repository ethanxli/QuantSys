namespace QuantSys.Visualization.Highstocks
{
    public class ChartOption
    {
        public int YPosition { get; set; }
        public int Height { get; set; }
        public string ChartType { get; set; }
        public bool Layover { get; set; }
        public ChartOption()
        {
            YPosition = 0;
            Height = 200;
            ChartType = "spline";
            Layover = false;
        }

    }
}
