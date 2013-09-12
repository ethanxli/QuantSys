using System;
using System.Collections.Generic;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public static class Selection
    {
        public static Population ElitismSelection(this Population p, int number)
        {
            Comparison<Chromosome> comparator = (
                (x, y) =>
                {
                    if (x.Fitness.Equals(y.Fitness)) return 0;
                    return (x.Fitness > y.Fitness) ? -1 : 1;
                }
                );

            p.Chromosomes.Sort(comparator);
            var elite = new List<Chromosome>();

            for (int i = 0; i < number; i++) elite.Add(p.Chromosomes[i]);

            return new Population(elite);
        }

        public static Population SimpleRouletteSelection(this Population pCopy, int number, Random r)
        {
            var result = new List<Chromosome>();
            var p = new Population(new List<Chromosome>(pCopy.Chromosomes));

            for (int n = 0; n < number; n++)
            {
                var fitnessProportion = new double[p.PopulationSize];
                double totalFitness = 0;
                double totalProbability = 0;

                for (int i = 0; i < p.PopulationSize; i++)
                {
                    totalFitness += p.Chromosomes[i].Fitness;
                }

                for (int i = 0; i < p.PopulationSize; i++)
                {
                    fitnessProportion[i] = totalProbability + (p.Chromosomes[i].Fitness/totalFitness);
                    totalProbability = fitnessProportion[i];
                }

                double random = r.NextDouble();
                for (int i = 0; i < p.PopulationSize; i++)
                {
                    if (random <= fitnessProportion[i])
                    {
                        result.Add(p.Chromosomes[i]);
                        p.Chromosomes.Remove(p.Chromosomes[i]);
                        break;
                    }
                }
            }

            return new Population(result);
        }
    }
}