using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
    public struct DecimalIterator
    {
        public int i, j;
        public decimal value;
    }

    public delegate decimal FoldDelegate(decimal value, decimal accumulator);

    public class DecimalMatrix
    {

        private decimal[,] values;
        private int rowCount, columnCount;

        public DecimalMatrix(int n) : this(n, n) { }
        public DecimalMatrix(int n, decimal val) : this(n, n, val) { }


        public DecimalMatrix(int n, int m) : this(n, m, 0) { }
        public DecimalMatrix(int n, int m, decimal val)
        {
            rowCount = n;
            columnCount = m;
            values = new decimal[n, m];
            if (val != 0)
                Iterate(it => this[it.i, it.j] = val);
        }

        public decimal this[int i]
        {
            get { return values[i, 0]; }
            set { values[i, 0] = value; }
        }
        public decimal this[int i, int j]
        {
            get { return values[i, j]; }
            set { values[i, j] = value; }
        }

        public static DecimalMatrix Eye(int n)
        {
            var ret = new DecimalMatrix(n);
            ret.Iterate(it =>
            {
                if (it.i == it.j)
                    return 1m;
                return 0m;
            });
            return ret;
        }

        public DecimalMatrix Transpose()
        {
            DecimalMatrix ret = new DecimalMatrix(columnCount, rowCount);

            Iterate(it => ret[it.j, it.i] = it.value);

            return ret;
        }

        public DecimalMatrix Multiply(DecimalMatrix other)
        {
            if (columnCount != other.rowCount)
                throw new ArgumentException("Matrix sizes must match");

            DecimalMatrix ret = new DecimalMatrix(rowCount, other.columnCount);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < other.columnCount; j++)
                {
                    ret[i, j] = 0;
                    for (int k = 0; k < columnCount; k++)
                    {
                        ret[i, j] += this[i, k] * other[k, j];
                    }
                }
            }
            return ret;
        }

        public DecimalMatrix Multiply(decimal val)
        {
            var ret = new DecimalMatrix(this.rowCount, this.columnCount);

            ret.Iterate(it =>
            {
                return this[it.i, it.j] * val;
            });

            return ret;
        }

        public DecimalMatrix SubMatrix(int startRow, int rowCount, int startColumn, int columnCount)
        {
            DecimalMatrix ret = new DecimalMatrix(rowCount, columnCount);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    ret[i, j] = this[startRow + i, startColumn + j];
                }
            }

            return ret;
        }

        public void Iterate(Action<DecimalIterator> func)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    func(new DecimalIterator() { i = i, j = j, value = this[i, j] });
                }
            }
        }

        public void Iterate(Func<DecimalIterator, decimal> func)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    this[i, j] = func(new DecimalIterator() { i = i, j = j, value = this[i, j] });
                }
            }
        }

        public DecimalMatrix GetColumn(int column)
        {
            if (column < 0 || column >= columnCount)
                throw new ArgumentException("Column index outside bounds.");

            return SubMatrix(0, rowCount, column, 1);
        }

        public DecimalMatrix GetRow(int row)
        {
            if (row < 0 || row >= rowCount)
                throw new ArgumentException("Row index outside bounds.");

            return SubMatrix(row, 1, 0, columnCount);
        }

        public decimal Fold(FoldDelegate func, decimal initialVal = 0)
        {
            decimal acc = initialVal;
            Iterate(it =>
            {
                acc = func(it.value, acc);
            });


            return acc;
        }

        public decimal Sum()
        {
            decimal sum = 0;
            Iterate(it => sum += it.value);
            return sum;
        }

        public static DecimalMatrix operator +(DecimalMatrix left, DecimalMatrix right)
        {
            if (left.rowCount != right.rowCount || left.columnCount != right.columnCount)
                throw new Exception("Matrix sizes must match");

            var ret = new DecimalMatrix(left.rowCount, left.columnCount);

            ret.Iterate(it =>
            {
                return left[it.i, it.j] + right[it.i, it.j];
            });

            return ret;
        }

        public static DecimalMatrix operator -(DecimalMatrix left, DecimalMatrix right)
        {
            if (left.rowCount != right.rowCount || left.columnCount != right.columnCount)
                throw new Exception("Matrix sizes must match");

            var ret = new DecimalMatrix(left.rowCount, left.columnCount);

            ret.Iterate(it =>
            {
                return left[it.i, it.j] - right[it.i, it.j];
            });

            return ret;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int lasti = 0;
            Iterate(it =>
            {
                if (lasti != it.i)
                {
                    lasti = it.i;
                    sb.Append("\r\n");
                }
                sb.Append($"{it.value,14:N10} ");
            });
            sb.Append('\n');
            return sb.ToString();
        }

        public double[,] ToDoubleMatrix()
        {
            double[,] ret = new double[rowCount, columnCount];
            Iterate(it => ret[it.i, it.j] = (double)it.value);
            return ret;
        }
    }
}
