using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema3
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Jacobi relaxata");
                Console.WriteLine("2. Gauss-Saidel relaxata");
                Console.WriteLine("3. Gradient Conjugat");
                Console.WriteLine("4. Metoda Rotatiilor");

                var sel = int.Parse(Console.ReadLine());

                switch (sel)
                {
                    case 1: JacobiRelaxata(); break;
                    case 2: GaussSaidel(); break;
                    case 3: GradConj(); break;
                    case 4: MetRot(); break;
                    default: continue;
                }
                Console.Write("Press any key to try another method...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }

        static void MetRot()
        {
            int n;
            double err;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("err=");
            err = double.Parse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture);

            var A = GetMatrix(n).ToDoubleMatrix();
            var b = GetB(n);
            var modul = 0d;
            var pas = 0;

            do
            {
                pas++;
                int p = 0, q = 0;
                double max = double.MinValue;
                double theta = 0;

                for (int i = 0; i < n; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        if (Math.Abs(A[i, j]) > max)
                        {
                            max = Math.Abs(A[i, j]);
                            p = i;
                            q = j;
                        }
                    }
                }

                if (A[p, p] == A[q, q])
                    theta = Math.PI / 4;
                else
                    theta = 0.5 * Math.Atan(2 * A[p, q] / (A[p, p] - A[q, q]));

                var c = Math.Cos(theta);
                var s = Math.Sin(theta);

                var Y = new double[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i != p && i != q && j != p && j != q)
                            Y[i, j] = A[i, j];
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    Y[p, j] = Y[j, p] = c * A[p, j] + s * A[q, j];
                    Y[q, j] = Y[j, q] = -s * A[p, j] + c * A[q, j];
                }

                Y[p, q] = Y[q, p] = 0;
                Y[p, p] = c * c * A[p, p] + 2 * c * s * A[p, q] + s * s * A[q, q];
                Y[q, q] = s * s * A[p, p] - 2 * c * s * A[p, q] + c * c * A[q, q];

                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        A[i, j] = Y[i, j];

                modul = 0;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            modul += A[i, j] * A[i, j];
                        }
                    }
                }

                modul = Math.Sqrt(modul);

            } while (modul > err);

            var result = new List<double>();
            for (int i = 0; i < n; i++)
            {
                result.Add(A[i, i]);
            }
            result.Sort();
            result.ForEach(val => Console.WriteLine(val));
        }

        static void JacobiRelaxata()
        {
            int n, p;
            decimal err;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            Console.Write("err=");
            err = decimal.Parse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture);

            var A = GetMatrix(n);
            var b = new DecimalMatrix(n, 1, 1m / (n * n));
            var x = new DecimalMatrix(n, 1, 0m);

            var norma = Enumerable.Range(0, n).ToList().Select((i) => A.GetRow(i).Fold((v, acc) => acc + Math.Abs(v), 0)).Max();
            var l = 2 / norma;
            var Bsig = new DecimalMatrix(n);
            DecimalMatrix bs;
            DecimalMatrix I = DecimalMatrix.Eye(n);

            int min_pas = 0;
            for (int k = 1; k <= p - 1; k++)
            {
                var sigma = (l / p) * k;
                x = new DecimalMatrix(n, 1, 0m);

                Bsig = I - A.Multiply(sigma);
                bs = b.Multiply(sigma);


                var na = 1m;
                var pas = 0;
                do
                {
                    pas++;
                    var y = new DecimalMatrix(n, 1, 0m);
                    y = Bsig.Multiply(x) + bs;

                    var sum = 0m;
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                            sum += A[i, j] * (y[i] - x[i]) * (y[j] - x[j]);
                    }
                    na = Sqrt(sum);
                    x = y;
                } while (na > err);

                if (k == 1)
                    min_pas = pas;
                else if (pas < min_pas)
                    min_pas = pas;
            }
            Console.WriteLine(x.ToString());
            Console.WriteLine("Iteratii: " + min_pas);
        }

        static void GaussSaidel()
        {
            int n, p;
            decimal err;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            Console.Write("err=");
            err = decimal.Parse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture);

            var A = GetMatrix(n);
            var b = new DecimalMatrix(n, 1, 1m / (n * n));
            var x = new DecimalMatrix(n, 1, 0m);

            int min_pas = 0;
            for (int k = 1; k <= p - 1; k++)
            {
                var sigma = (2m / p) * k;
                x = new DecimalMatrix(n, 1, 0m);

                int pas = 0;
                var na = 1m;
                do
                {
                    pas++;
                    var y = new DecimalMatrix(n, 1, 0m);
                    for (int i = 0; i < n; i++)
                    {
                        var sum1 = A.SubMatrix(i, 1, 0, i).Multiply(y.SubMatrix(0, i, 0, 1)).Sum();
                        var sum2 = A.SubMatrix(i, 1, i + 1, n - i - 1).Multiply(x.SubMatrix(i + 1, n - i - 1, 0, 1)).Sum();
                        y[i] = (1 - sigma) * x[i] + sigma / A[i, i] * (b[i] - sum1 - sum2);
                    }

                    var sum = 0m;
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                            sum += A[i, j] * (y[i] - x[i]) * (y[j] - x[j]);
                    }
                    na = Sqrt(sum);
                    x = y;
                } while (na > err);

                if (k == 1)
                    min_pas = pas;
                else if (pas < min_pas)
                    min_pas = pas;
            }
            Console.WriteLine(x.ToString());
            Console.WriteLine("Iteratii: " + min_pas);
        }

        static void GradConj()
        {
            int n, p;
            double err;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("err=");
            err = double.Parse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture);

            var A = GetMatrix(n).ToDoubleMatrix();
            var b = GetB(n);
            var x = new double[n];

            double[] r = DifVectors(b, MultiplyMatrixVector(A, x));
            double[] v = r;
            int pas = 0;
            double er = 0;

            do
            {
                double alfa = (Norma(r) * Norma(r)) / Norma(MultiplyMatrixVector(A, v), v);
                double[] newX = SumVectors(x, MultiplyVectorScalar(v, alfa));
                double[] newR = DifVectors(b, MultiplyMatrixVector(A, newX));
                double c = (Norma(newR) * Norma(newR)) / (Norma(r) * Norma(r));
                double[] newV = SumVectors(newR, MultiplyVectorScalar(v, c));

                pas++;
                er = Norma(DifVectors(newX, x));
                x = newX;
                r = newR;
                v = newV;
            } while (er > err);

            x.ToList().ForEach(val => Console.WriteLine(val));
            Console.WriteLine("Iteratii: " + pas);
        }
        private static double[] GetB(int n)
        {
            double[] v = new double[n];

            for (int i = 0; i < n; i++)
            {
                v[i] = 1 / (double)(n * n);
            }

            return v;
        }

        private static double Norma(double[] v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v[i];
            }
            return Math.Sqrt(sum);
        }
        private static double Norma(double[] v, double[] v2)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v2[i];
            }
            return sum;
        }
        private static double[] MultiplyMatrixVector(double[,] matrix, double[] vector)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }
            return result;
        }

        private static double[] MultiplyVectorScalar(double[] vector, double scalar)
        {
            double[] result = new double[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                result[i] = vector[i] * scalar;
            }
            return result;
        }

        private static double[] SumVectors(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + b[i];
            }
            return result;
        }

        private static double[] DifVectors(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
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


        private static DecimalMatrix GetMatrix(int n)
        {
            var mat = new DecimalMatrix(n);
            mat.Iterate((it) =>
            {
                if (it.i == it.j)
                    return 2 + 1m / (n * n);

                if (it.j == it.i + 1 || it.i == it.j + 1)
                    return -1;

                return 0;
            });
            return mat;
        }
    }
}
