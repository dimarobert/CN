using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Tema2 {
    public struct BigRatIterator {
        public int i, j;
        public BigRational value;
    }

    public class BigRatMatrix {

        private BigRational[,] values;
        private int rowCount, columnCount;

        public BigRatMatrix(int n, bool safe = true) : this(n, n, safe) { }

        public BigRatMatrix(int n, int m, bool safe = true) {
            rowCount = n;
            columnCount = m;
            values = new BigRational[n, m];
            if (safe)
                Iterate(it => {
                    values[it.i, it.j] = new BigRational(0.0);
                });
        }


        public BigRational this[int i, int j]
        {
            get { return values[i, j]; }
            set
            {
                values[i, j] = value;
            }
        }

        public BigRatMatrix Transpose() {
            BigRatMatrix ret = new BigRatMatrix(rowCount, columnCount);

            Iterate(it => ret[it.j, it.i] = it.value);

            return ret;
        }

        public void Iterate(Action<BigRatIterator> func) {
            for (int i = 0; i < rowCount; i++) {
                for (int j = 0; j < columnCount; j++) {
                    func(new BigRatIterator() { i = i, j = j, value = this[i, j] });
                }
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            int lasti = 0;
            Iterate(it => {
                if (lasti != it.i) {
                    lasti = it.i;
                    sb.Append("\r\n");
                }
                sb.Append($"\t{it.value.ToDecimal()}");
            });
            sb.Append('\n');
            return sb.ToString();
        }
    }
}
