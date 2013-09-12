using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace QuantSys.Analytics
{
    public static class DenseVectorExtension
    {
        public static DenseVector SubVector(this DenseVector d, int startIndex, int endIndex)
        {
            return (DenseVector)d.SubVector(startIndex, d.Count - endIndex);
        }

    }
}
