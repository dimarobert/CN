using System;
using System.Collections.Generic;
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
                Console.WriteLine("1. Factorizarea LU");
                Console.WriteLine("2. Cholesky");
                Console.WriteLine("3. QR");

                var sel = int.Parse(Console.ReadLine());

                switch (sel)
                {
                    case 1: JacobiRelaxata(); break;
                    //case 2: ResolveCholesky(); break;
                    //case 3: ResolveQR(); break;
                    default: continue;
                }
                Console.Write("Press any key to try another method...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }

        static void JacobiRelaxata()
        {
            int n, p, err;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            Console.Write("err=");
            err = int.Parse(Console.ReadLine());

            var A = GetMatrix(n);
            var b = new DecimalMatrix(n, 1, 1m / (n * n));
            var x = new DecimalMatrix(n, 0m);

            var norma = Enumerable.Range(0, n).ToList().Select((i) => A.GetRow(i).Fold((v, acc) => acc + Math.Abs(v), 0)).Max();
            var l = 2 / norma;
            var B = new DecimalMatrix(n);
            var bs = new DecimalMatrix(n, 1);

            int pas = 0;
            for (int k = 0; k < p - 1; k++)
            {
                var sigma = (l / p) * k;

                for (int i = 0; i < n; i++)
                {
                    B[i, i] = 1 - sigma * A[i, i];
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                            B[i, j] = -sigma * A[i, j];
                    }
                }

                for (int i = 0; i < n; i++)
                    bs[i] = sigma * i;

                var na = 1m;
                do
                {
                    pas++;
                    var y = new DecimalMatrix(n, 0m);
                    for (int i = 0; i < n; i++)
                    {
                        y[i] = B.GetRow(i).Multiply(x).Sum() + bs[i];
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
            }
            Console.WriteLine(x.ToString());
            Console.WriteLine("Iteratii: " + pas);
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
                    return 2 + 1 / (n * n);

                if (it.j == it.i + 1 || it.i == it.j + 1)
                    return -1;

                return 0;
            });
            return mat;
        }
    }
}
