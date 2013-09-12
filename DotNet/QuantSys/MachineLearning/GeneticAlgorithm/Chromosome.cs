using System;
using System.Collections;
using System.Collections.Generic;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public class Chromosome : IEnumerable<Gene>
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
            _fitness = double.NaN;
            _geneList = genes;
        }

        public double Fitness
        {
            get { return _fitness; }
        }

        public Gene this[int index]
        {
            get { return _geneList[index]; }
        }

        public void CalculateFitness(Func<Chromosome, double> func)
        {
            _fitness = func(this);
        }

        public void Mutate(double mRate)
        {
            foreach (Gene gene in _geneList)
            {
                if (gene.Mutate(mRate))
                {
                    _fitness = double.NaN;
                }
            }
        }

        public IEnumerator<Gene> GetEnumerator()
        {
            return _geneList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}