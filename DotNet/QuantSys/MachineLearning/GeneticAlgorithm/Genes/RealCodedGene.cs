using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes.Constraints;

namespace QuantSys.MachineLearning.GeneticAlgorithm.Genes
{
    public class RealCodedGene : Gene
    {
        public double GeneValue { get; set; }
        public RealCodedGene(double value, Random r, GeneConstraint g) : base(r)
        {
            GeneValue = value;
            GeneConstraint = g;
        }

        public override bool WithinConstraints()
        {
            return GeneConstraint.ConstraintFunction(GeneValue);
        }

        public override void InitializeGene()
        {
            GeneValue =
                ((double) GeneConstraint.HI -
                 _randomSeed.NextDouble()*
                 ((double) GeneConstraint.HI - (double) GeneConstraint.LOW));
        }

        public override bool Mutate(double mRate)
        {
            if (_randomSeed.NextDouble() <= mRate)
            {
                double tempValue = double.NaN;
                while (!GeneConstraint.ConstraintFunction(tempValue))
                {
                    tempValue = GeneValue + (((_randomSeed.Next(10) % 2 == 0) ? -1 : 1) * _randomSeed.NextDouble() / 10.0) * GeneValue;
                }

                GeneValue = tempValue;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return GeneValue.ToString();
        }

        public override Gene Clone()
        {
            return new RealCodedGene(this.GeneValue, this._randomSeed, this.GeneConstraint);
        }
        public override void Crossover(Gene gene2, out Gene child1, out Gene child2)
        {
            SimulatedBinaryCrossover(this, (RealCodedGene)gene2, out child1, out child2);
        }

        private void SimulatedBinaryCrossover(RealCodedGene g1, RealCodedGene g2, out Gene child1, out Gene child2)
        {
            //n_c is a parameter that controls the crossover process. A high value of the parameter will create near-parent solution
            const double n_c = .05;

            double a, b;
            do
            {
                double random = _randomSeed.NextDouble();
                double beta = (random < 0.5)
                    ? Math.Pow((2 * random), (1 / (n_c + 1)))
                    : Math.Pow(1 / (2 * (1 - random)), (1 / (n_c + 1)));

                var x_i1 = g1.GeneValue;
                var x_i2 = g2.GeneValue;

                a = 0.5 * ((1 + beta) * x_i1 + (1 - beta) * x_i2);
                b = 0.5 * ((1 - beta) * x_i1 + (1 + beta) * x_i2);

            } while (!GeneConstraint.ConstraintFunction(a) || !GeneConstraint.ConstraintFunction(b));


            child1 = new RealCodedGene(g1.GeneValue, _randomSeed, g1.GeneConstraint);
            child2 = new RealCodedGene(g1.GeneValue, _randomSeed, g2.GeneConstraint);

        }

    }
}
