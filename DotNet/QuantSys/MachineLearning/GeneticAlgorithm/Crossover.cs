using System;
using System.Collections.Generic;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public static class Crossover
    {
        private static void SimulatedBinaryCrossover(Gene g1, Gene g2, Random r, out Gene child1, out Gene child2)
        {
            //n_c is a parameter that controls the crossover process. A high value of the parameter will create near-parent solution
            const double n_c = .05;

            double c1, c2;

            do
            {
                double random = r.NextDouble();
                double beta = (random < 0.5)
                    ? Math.Pow((2*random), (1/(n_c + 1)))
                    : Math.Pow(1/(2*(1 - random)), (1/(n_c + 1)));

                var x_i1 = (double) g1.GeneValue;
                var x_i2 = (double) g2.GeneValue;

                c1 = 0.5*((1 + beta)*x_i1 + (1 - beta)*x_i2);
                c2 = 0.5*((1 - beta)*x_i1 + (1 + beta)*x_i2);
            } while (!g1.WithinConstraints(c1) || !g2.WithinConstraints(c2));

            child1 = new Gene(c1, g1.GeneConstraint);
            child2 = new Gene(c2, g2.GeneConstraint);
        }

        private static void BooleanBinaryCrossover(Gene g1, Gene g2, Random r, out Gene child1, out Gene child2)
        {
            var g1g = (bool[]) g1.GeneValue;
            var g2g = (bool[]) g2.GeneValue;

            int crossPoint = r.Next(g1g.Length);

            var c1 = new bool[g1g.Length];
            var c2 = new bool[g1g.Length];

            for (int i = 0; i < crossPoint; i++)
            {
                c1[i] = g1g[i];
                c2[i] = g2g[i];
            }

            for (int i = crossPoint; i < g1g.Length; i++)
            {
                c1[i] = g2g[i];
                c2[i] = g1g[i];
            }

            child1 = new Gene(c1, g1.GeneConstraint);
            child2 = new Gene(c2, g2.GeneConstraint);
        }

        private static void BinaryCrossover(Gene g1, Gene g2, Random r, out Gene child1, out Gene child2)
        {
            child1 = null;
            child2 = null;

            switch (g1.Encoding)
            {
                case Gene.GeneEncoding.Binary:
                {
                    BooleanBinaryCrossover(g1, g2, r, out child1, out child2);
                    break;
                }
                case Gene.GeneEncoding.Realcoded:
                {
                    SimulatedBinaryCrossover(g1, g2, r, out child1, out child2);
                    break;
                }
            }
        }

        public static Population BinaryCrossover(Chromosome x1, Chromosome x2, Random r)
        {
            var child1 = new List<Gene>();
            var child2 = new List<Gene>();

            //for each gene in the chromosomes, we crossover
            for (int i = 0; i < x1.GeneList.Count; i++)
            {
                Gene c1, c2;
                BinaryCrossover(x1.GeneList[i], x2.GeneList[i], r, out c1, out c2);
                child1.Add(c1);
                child2.Add(c2);
            }

            var x1_t = new Chromosome(child1);
            var x2_t = new Chromosome(child2);
            var l1 = new List<Chromosome>();
            l1.Add(x1_t);
            l1.Add(x2_t);

            return new Population(l1);
        }
    }
}