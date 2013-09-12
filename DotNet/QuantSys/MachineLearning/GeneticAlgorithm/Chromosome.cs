using System;
using System.Collections.Generic;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public class Chromosome
    {
        private readonly List<Gene> _geneList;
        private double _fitness;

        public Chromosome()
        {
            _fitness = double.NaN;
            _geneList = new List<Gene>();
        }

        public Chromosome(List<Gene> genes)
        {
            _geneList = genes;
        }

        public double Fitness
        {
            get { return _fitness; }
        }

        public List<Gene> GeneList
        {
            get { return _geneList; }
        }

        public Gene this[int index]
        {
            get { return _geneList[index]; }
        }

        public void CalculateFitness(Func<Chromosome, double> func)
        {
            _fitness = func(this);
        }

        public void Mutate(double mRate, Random r)
        {
            foreach (Gene gene in GeneList)
            {
                if (gene.Mutate(mRate, r))
                {
                    _fitness = double.NaN;
                }
            }
        }
    }
}