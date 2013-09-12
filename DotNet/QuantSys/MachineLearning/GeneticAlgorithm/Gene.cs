using System;
using System.Text;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    //A Gene is 
    public class Gene
    {
        public enum GeneEncoding
        {
            Binary = 1,
            Realcoded = 2
        }

        public Constraint GeneConstraint;
        public object GeneValue;

        public Gene(object geneValue, Constraint c)
        {
            GeneValue = geneValue;
            GeneConstraint = c;

            if (ReferenceEquals(geneValue.GetType(), typeof (double)) ||
                ReferenceEquals(geneValue.GetType(), typeof (int)))
                Encoding = GeneEncoding.Realcoded;
            else if (ReferenceEquals(geneValue.GetType(), typeof (bool[])))
                Encoding = GeneEncoding.Binary;
        }

        public GeneEncoding Encoding { get; set; }

        public bool WithinConstraints(double d)
        {
            return (d <= (double) GeneConstraint.HIGH) && (d >= (double) GeneConstraint.LOW);
        }

        public void InitializeGene(Random r)
        {
            switch (Encoding)
            {
                case GeneEncoding.Binary:
                {
                    for (int i = 0; i < ((bool[]) GeneValue).Length; i++)
                        ((bool[]) GeneValue)[i] = (r.NextDouble() > 0.5);
                    break;
                }
                case GeneEncoding.Realcoded:
                {
                    GeneValue = r.NextDouble()*(double) GeneConstraint.HIGH + (double) GeneConstraint.LOW;
                    break;
                }
            }
        }

        public bool Mutate(double mRate, Random r)
        {
            if (r.NextDouble() <= mRate)
            {
                switch (Encoding)
                {
                    case GeneEncoding.Realcoded:
                    {
                        double newGeneValue = double.NegativeInfinity;
                        while (!WithinConstraints(newGeneValue))
                            newGeneValue = (double) (GeneValue) +
                                           (((r.Next(10)%2 == 0) ? -1 : 1)*r.NextDouble()/10.0)*(double) GeneValue;

                        GeneValue = newGeneValue;
                        return true;
                    }
                    case GeneEncoding.Binary:
                    {
                        var newValue = (bool[]) GeneValue;
                        newValue[r.Next(newValue.Length)] = !newValue[r.Next(newValue.Length)];
                        GeneValue = newValue;
                    }
                        break;
                }
            }
            return false;
        }

        public override string ToString()
        {
            switch (Encoding)
            {
                case GeneEncoding.Realcoded:
                {
                    return ((double) GeneValue).ToString();
                }
                case GeneEncoding.Binary:
                {
                    var stringBuilder = new StringBuilder();
                    var gene = (bool[]) GeneValue;
                    for (int i = 0; i < gene.Length; i++)
                    {
                        if (gene[i]) stringBuilder.Append("1 ");
                        else stringBuilder.Append("0 ");
                    }
                    return "[" + stringBuilder.ToString().Trim() + "]";
                    break;
                }
            }

            return null;
        }

        public class Constraint
        {
            public Constraint(double low, double high)
            {
                HIGH = high;
                LOW = low;
            }

            public Constraint(bool[] high, bool[] low)
            {
                HIGH = high;
                LOW = low;
            }

            public object HIGH { get; set; }
            public object LOW { get; set; }
        }
    }
}