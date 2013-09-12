using System;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes.Constraints;

namespace QuantSys.MachineLearning.GeneticAlgorithm.Genes
{
    /// <summary>
    /// Represents an abstraction of a Gene. Must be implemented by
    /// a Gene type such as RealCodedGene.
    /// </summary>
    public abstract class Gene
    {
        //public object GeneValue { get; set; }

        protected Random _randomSeed;
        public GeneConstraint GeneConstraint { get; set; }
        public Gene(Random r)
        {
            _randomSeed = r;
        }

        public abstract bool WithinConstraints();
        public abstract void InitializeGene();
        public abstract bool Mutate(double mRate);
        public abstract override string ToString();
        public abstract void Crossover(Gene gene2, out Gene child1, out Gene child2);
        public abstract Gene Clone();

    }
}