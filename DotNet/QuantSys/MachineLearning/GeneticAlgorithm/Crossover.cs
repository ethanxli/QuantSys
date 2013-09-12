using System;
using System.Collections.Generic;
using System.Linq;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public static class Crossover
    {

        public static Population BinaryCrossover(Chromosome x1, Chromosome x2, Random r)
        {
            var genelist1 = new List<Gene>();
            var genelist2 = new List<Gene>();

            //for each gene in the chromosomes, we crossover
            for (int i = 0; i < x1.Count(); i++)
            {
                Gene c1, c2;
                x1[i].Crossover(x2[i], out c1, out c2);
                genelist1.Add(c1);
                genelist2.Add(c2);
            }

            var x1_t = new Chromosome(genelist1);
            var x2_t = new Chromosome(genelist2);

            var l1 = new List<Chromosome>();
            l1.Add(x1_t);
            l1.Add(x2_t);

            return new Population(l1);
        }
    }
}