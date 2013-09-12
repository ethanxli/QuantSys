using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes.Constraints;

namespace QuantSys.MachineLearning.GeneticAlgorithm.Genes
{
    public class BinaryGene : Gene
    {
        public bool[] GeneValue { get; set; }
        public BinaryGene(bool[] value, Random r, GeneConstraint c) : base(r)
        {
            GeneValue = value;
            GeneConstraint = c;
        }

        public override bool WithinConstraints()
        {
            throw new NotImplementedException();
        }

        public override void InitializeGene()
        {
            for (int i = 0; i < (GeneValue).Length; i++) GeneValue[i] = (_randomSeed.NextDouble() > 0.5);
        }

        public override bool Mutate(double mRate)
        {
            if (_randomSeed.NextDouble() <= mRate)
            {
                int mutateIndex = _randomSeed.Next(GeneValue.Length);
                GeneValue[mutateIndex] = !GeneValue[mutateIndex];
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < GeneValue.Length; i++)
            {
                if (GeneValue[i]) stringBuilder.Append("1 ");
                else stringBuilder.Append("0 ");
            }
            return "[" + stringBuilder.ToString().Trim() + "]";
        }

        public override void Crossover(Gene gene2, out Gene child1, out Gene child2)
        {
            BinaryCrossover(this, (BinaryGene)gene2, out child1, out child2);
        }

        public override Gene Clone()
        {
            return new BinaryGene((bool[])GeneValue.Clone(), _randomSeed, GeneConstraint);
        }


        private void BinaryCrossover(BinaryGene gene1, BinaryGene gene2, out Gene child1, out Gene child2)
        {
            int crossPoint = _randomSeed.Next(gene1.GeneValue.Length);

            bool[] a = new bool[gene1.GeneValue.Length];
            bool[] b = new bool[gene1.GeneValue.Length];

            do
            {
                for (int i = 0; i < crossPoint; i++)
                {
                    a[i] = gene1.GeneValue[i];
                    b[i] = gene2.GeneValue[i];
                }

                for (int i = crossPoint; i < gene1.GeneValue.Length; i++)
                {
                    a[i] = gene2.GeneValue[i];
                    b[i] = gene1.GeneValue[i];
                }

            } while (!GeneConstraint.ConstraintFunction(a) || !GeneConstraint.ConstraintFunction(b));

            child1 = new BinaryGene(a, _randomSeed, gene1.GeneConstraint);
            child2 = new BinaryGene(b, _randomSeed, gene2.GeneConstraint);

        }
    }
}
