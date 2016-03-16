using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema2 {
    class Program {
        static void Main(string[] args) {

            while (true) {
                Console.WriteLine("1. Factorizarea LU");
                //Console.WriteLine("2. Metoda Falsei Pozitii");
                Console.WriteLine("3. QR");

                var sel = int.Parse(Console.ReadLine());

                switch (sel) {
                    case 1: ResolveLU(); break;
                    //case 2: falseiPozitii(); break;
                    case 3: ResolveQR(); break;
                    default: continue;
                }
                Console.Write("Press any key to try another method...");
                Console.ReadKey();
                Console.WriteLine();
            }
        }


        static void ResolveQR() {
            int n, p;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());

            Matrix<double> A = GetMatrix(n, p);

            Vector<double> b = CreateVector.Dense(n, 1.0);

            var At = A.Transpose();
            var AtA = At.Multiply(A);

            var Atb = At.Multiply(b);

            Matrix<double> R = new DenseMatrix(n);

            for (int k = 0; k < n; k++) {
                for (int j = 0; j < k - 1; j++) {
                    //R[j,k] = QRSum()
                }
            }

        }

        static double QRSum(Matrix<double> A, Matrix<double> Q) {



            return 0.0;
        }


        static void ResolveLU() {
            int n, p;

            Console.Write("n=");
            n = int.Parse(Console.ReadLine());

            Console.Write("p=");
            p = int.Parse(Console.ReadLine());


            Matrix<double> A = GetMatrix(n, p);


            //Console.WriteLine(A.ToString());
            Vector<double> b = CreateVector.Dense(n, 1.0);

            var At = A.Transpose();
            var AtA = At.Multiply(A);

            var Atb = At.Multiply(b);




            var lu = AtA.LU();
            var luSolve = lu.Solve(Atb);
            var luRes = ComputeLUx(lu.L, lu.U, Atb, n);
            Console.WriteLine("LU Solve: " + luSolve.ToString() + "\nLU Res:" + luRes.ToString());





            for (int k = 1; k <= n; k++) {
                if (AtA.SubMatrix(0, k, 0, k).Determinant() == 0) {
                    Console.WriteLine($"Matricea pentru (n, p) = ({n}, {p}) nu se descompune LU.");
                }
            }

            Matrix<double> L, U;
            L = new DenseMatrix(n);
            U = new DenseMatrix(n);


            for (int k = 0; k < n; k++) {

                for (int j = k; j < n; j++) {

                    double sum = 0.0;
                    for (int _p = 0; _p <= k - 1; _p++) {
                        sum += L[k, _p] * U[_p, j];
                    }

                    U[k, j] = AtA[k, j] - sum;
                }

                for (int i = k + 1; i < n; i++) {
                    L[i, k] = (AtA[i, k] - LUSum(L, U, 0, k - 1, i, k)) / U[k, k];
                }

            }

            Vector<double> y = CreateVector.Dense(n, 0.0);
            Vector<double> x = CreateVector.Dense(n, 0.0);

            for (int i = 0; i < n; i++) {
                y[i] = Atb[i] - ySum(L, y, 0, i - 1, i);
            }

            for (int i = n - 1; i >= 0; i--) {
                x[i] = y[i] - ySum(U, x, i + 1, n-1, i);
                x[i] = x[i] / U[i, i];
            }

            Console.WriteLine(x.ToString());
        }

        static Vector<double> ComputeLUx(Matrix<double> L, Matrix<double> U, Vector<double> b, int n) {
            Vector<double> y = CreateVector.Dense(n, 0.0);
            Vector<double> x = CreateVector.Dense(n, 0.0);

            for (int i = 0; i < n; i++) {
                y[i] = b[i] - ySum(L, y, 0, i - 1, i);
            }

            for (int i = n - 1; i >= 0; i--) {
                x[i] = y[i] - ySum(U, x, i + 1, n - 1, i);
                x[i] = x[i] / U[i, i];
            }
            return x;
        }

        static double ySum(Matrix<double> L, Vector<double> y, int from, int to, int Li) {
            double result = 0;
            for (int k = from; k <= to; k++) {
                result += L[Li, k] * y[k];
            }
            return result;
        }

        static double LUSum(Matrix<double> L, Matrix<double> U, int from, int to, int Li, int Uj) {
            double result = 0;
            for (int p = from; p <= to; p++) {
                result += L[Li, p] * U[p, Uj];
            }
            return result;
        }


        static Matrix<double> GetMatrix(int n, int p) {

            Matrix<double> ret = new DenseMatrix(n);

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    ret[i, j] = MathNet.Numerics.Combinatorics.Combinations(p + j, i);
                }
            }


            return ret;
        }
    }
}
