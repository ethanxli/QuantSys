using System;
using System.Collections.Generic;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public class Population
    {
        private readonly List<Chromosome> pop;

        public Population()
        {
            pop = new List<Chromosome>();
        }

        public Population(List<Chromosome> l)
        {
            pop = l;
        }

        public int PopulationSize
        {
            get { return pop.Count; }
        }

        public List<Chromosome> Chromosomes
        {
            get { return pop; }
        }

        public double BestFitness { get; set; }

        public Chromosome this[int index]
        {
            get { return pop[index]; }
        }

        public void MutatePopulation(double mRate, Random r)
        {
            foreach (Chromosome c in pop)
            {
                c.Mutate(mRate);
            }
        }

        public void JoinPopulation(Population p)
        {
            Chromosomes.AddRange(p.Chromosomes);
        }

        public void CalculateAllFitness(Func<Chromosome, double> fitness)
        {
            foreach (Chromosome chromosome in Chromosomes)
            {
                chromosome.CalculateFitness(fitness);
            }
        }


        /*
        public static Population SeedPopulation(int popSize, List<Gene.Constraint> constraints, Random r)
        {
            List<Chromosome> pop = new List<Chromosome>(popSize);
            Population p = new Population();
            
            //generate using seed
            for (int i = 0; i < popSize; i++)
            {
                List <Gene> l= new List<Gene>(constraints.Count);
                double 
                pop[i]
            }

            return null;
        }*/
    }
}