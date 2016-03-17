using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Factorizarea LU");
                Console.WriteLine("2. Cholesky");
                Console.WriteLine("3. QR");

                var sel = int.Parse(Console.ReadLine());

                switch (sel)
                {
                    case 1: ResolveLU(); break;
                    case 2: ResolveCholesky(); break;
                    case 3: ResolveQR(); break;
                    default: continue;
                }
                Console.Write("Press any key to try another method...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }

        static void ResolveCholesky()
        {

            int n, p;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            var A = GetMatrix(n, p);
            var At = A.Transpose();
            A = At.Multiply(A);
            var b = new DecimalMatrix(n, 1, 1);
            b = At.Multiply(b);


            var L = new DecimalMatrix(n);
            var Y = new DecimalMatrix(n, 1);
            var output = new DecimalMatrix(n, 1);


            #region MetodaRadaciniiPatrate
            for (int j = 0; j < n; j++)
            {
                decimal suma = 0;
                for (int k = 0; k < j; k++)
                {
                    suma += Power(L[j, k], 2);
                }
                L[j, j] = Sqrt(A[j, j] - suma);

                for (int i = (j + 1); i < n; i++)
                {
                    decimal suma2 = 0;
                    for (int k = 0; k < j; k++)
                    {
                        suma2 += L[i, k] * L[j, k];
                    }
                    L[i, j] = (A[i, j] - suma2) / L[j, j];
                }
            }
            #endregion

            #region Calculam Y
            for (int i = 0; i < n; i++)
            {
                decimal suma = 0;
                for (int k = 0; k < i; k++)
                {
                    suma += L[i, k] * Y[k, 0];
                }
                Y[i, 0] = (b[i, 0] - suma) / L[i, i];
            }
            #endregion

            #region Calculam X
            for (int i = (int.Parse(n.ToString()) - 1); i >= 0; i--)
            {
                decimal suma = 0;
                for (int k = i + 1; k < n; k++)
                {
                    suma += L[k, i] * output[k, 0];
                }
                output[i, 0] = (Y[i, 0] - suma) / L[i, i];
            }
            #endregion

            Console.WriteLine(output.ToString());
        }

        static decimal Power(decimal x, int power)
        {
            decimal ret = 1;
            for (int i = 0; i < power; i++)
            {
                ret *= x;
            }
            return ret;
        }

        static void ResolveQR()
        {
            int n, p;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            var A = GetMatrix(n, p);

            var b = new DecimalMatrix(n, 1, 1);

            var At = A.Transpose();
            var AtA = At.Multiply(A);

            var Atb = At.Multiply(b);

            var R = new DecimalMatrix(n);
            var Q = new DecimalMatrix(n);

            for (int k = 0; k < n; k++)
            {
                for (int j = 0; j <= k - 1; j++)
                {
                    decimal sum = 0;
                    for (int i = 0; i < n; i++)
                    {
                        sum += AtA[i, k] * Q[i, j];
                    }

                    R[j, k] = sum;
                }

                decimal aSum = 0;
                for (int i = 0; i < n; i++)
                {
                    aSum += AtA[i, k] * AtA[i, k];
                }
                decimal rSum = 0;
                for (int i = 0; i <= k - 1; i++)
                {
                    rSum += R[i, k] * R[i, k];
                }
                R[k, k] = Sqrt(aSum - rSum);

                for (int i = 0; i < n; i++)
                {
                    decimal sum = 0;
                    for (int j = 0; j <= k - 1; j++)
                    {
                        sum += R[j, k] * Q[i, j];
                    }

                    Q[i, k] = (AtA[i, k] - sum) / R[k, k];
                }
            }

            var y = new DecimalMatrix(n, 1);
            var x = new DecimalMatrix(n, 1);
            for (int i = 0; i < n; i++)
            {
                y[i] = Q.GetColumn(i).Transpose().Multiply(Atb.GetColumn(0)).Sum();
            }

            for (int i = n - 1; i >= 0; i--)
            {
                decimal sum = 0;
                for (int j = i + 1; j < n; j++)
                {
                    sum += R[i, j] * x[j];
                }
                x[i] = (y[i] - R.SubMatrix(i, 1, i + 1, n - i - 1).Multiply(x.SubMatrix(i + 1, n - i - 1, 0, 1)).Sum()) / R[i, i];
            }
            Console.WriteLine(x.ToString());
        }

        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);
            return current;
        }


        static decimal QRSum(DecimalMatrix A, DecimalMatrix Q, int from, int to, int k, int j)
        {
            decimal ret = 0;
            for (int i = from; i <= to; i++)
            {
                ret += A[i, k] * Q[i, j];
            }
            return ret;
        }

        static DecimalMatrix GetMatrix(int n, int p)
        {
            DecimalMatrix ret = new DecimalMatrix(n);

            ret.Iterate(it => ret[it.i, it.j] = (decimal)MathNet.Numerics.Combinatorics.Combinations(p + it.j, it.i));
            return ret;
        }

        static void ResolveLU()
        {
            int n, p;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());


            var A = GetMatrix(n, p);
            var b = new DecimalMatrix(n, 1, 1);

            var At = A.Transpose();
            var AtA = At.Multiply(A);

            var Atb = At.Multiply(b);

            DecimalMatrix L, U;
            L = new DecimalMatrix(n);
            U = new DecimalMatrix(n);


            for (int k = 0; k < n; k++)
            {

                for (int j = k; j < n; j++)
                {
                    U[k, j] = AtA[k, j] - LUSum(L, U, 0, k - 1, k, j);
                }

                for (int i = k + 1; i < n; i++)
                {
                    L[i, k] = (AtA[i, k] - LUSum(L, U, 0, k - 1, i, k)) / U[k, k];
                }

            }

            var x = ComputeLUResult(L, U, Atb, n);

            Console.WriteLine(x.ToString());
        }

        static decimal LUSum(DecimalMatrix L, DecimalMatrix U, int from, int to, int Li, int Uj)
        {
            decimal result = 0;
            for (int p = from; p <= to; p++)
            {
                result += L[Li, p] * U[p, Uj];
            }
            return result;
        }

        static decimal ySum(DecimalMatrix L, DecimalMatrix y, int from, int to, int Li)
        {
            decimal result = 0;
            for (int k = from; k <= to; k++)
            {
                result += L[Li, k] * y[k, 0];
            }
            return result;
        }

        static DecimalMatrix ComputeLUResult(DecimalMatrix L, DecimalMatrix U, DecimalMatrix b, int n)
        {
            var y = new DecimalMatrix(n, 1);
            var x = new DecimalMatrix(n, 1);

            for (int i = 0; i < n; i++)
            {
                y[i, 0] = b[i, 0] - ySum(L, y, 0, i - 1, i);
            }

            for (int i = n - 1; i >= 0; i--)
            {
                x[i, 0] = y[i, 0] - ySum(U, x, i + 1, n - 1, i);
                x[i, 0] = x[i, 0] / U[i, i];
            }
            return x;
        }
    }
}
