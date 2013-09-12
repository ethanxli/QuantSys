using System;

namespace QuantSys.MachineLearning.GeneticAlgorithm.Genes.Constraints
{
    public class GeneConstraint
    {
        public Func<object, bool> ConstraintFunction { get; set; }

        public object HI { get; set; }

        public object LOW { get; set; }
        public GeneConstraint(Func<object, bool> constraintFunction)
        {
            ConstraintFunction = constraintFunction;
        }

    }
}
