using System;

namespace QuantSys.Visualization.Highstocks
{
    public class HighstockFlag
    {
        public string Shape { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        
        public HighstockFlag(string title, string text, DateTime date)
        {
            Title = title;
            Text = text;
            Date = date;
        }
    }
}
