using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Tema2 {
    public static class BigIntegerExtensions {

        public static decimal ToDecimal(this BigInteger bi) {
            return (decimal)bi;
        }
    }

    public static class BigRationalExtensions {

        public static decimal ToDecimal(this BigRational br) {
            return br.Numerator.ToDecimal() / br.Denominator.ToDecimal();
        }
    }
}
