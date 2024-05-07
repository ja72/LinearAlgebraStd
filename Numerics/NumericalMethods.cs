using System;
using static System.Math;

namespace JA.Numerics
{
    public static class NumericalMethods
    {
        public static int MaxIterations { get; } = 128;
        public static double LooseTolerance { get; } = 1e-6;
        public static double TightTolerance { get; } = 1e-10;

        public static double GaussPointIteration(this Func<double, double> f, double x_init, double tol)
        {
            double x = x_init, x_old;
            int iter = 0;
            do
            {
                iter++;
                x_old = x;
                x = f(x);
            } while (iter < MaxIterations && Abs(x - x_old) > tol);

            return x;
        }

        public static double BisectionRoot(this Func<double, double> f, double x_low, double x_high, double tol, bool allowExpandRange = true)
        {
            double f_low = f(x_low), f_high = f(x_high);

            if (Abs(f_low) <= tol) return x_low;
            if (Abs(f_high) <= tol) return x_high;

            int iter = 0;

            while ( !allowExpandRange && iter <= MaxIterations && f_low * f_high > 0)
            {
                iter++;
                (x_low, x_high) = ((3 * x_low - x_high) / 2, (3 * x_high - x_low) / 2);
                (f_low, f_high) = (f(x_low), f(x_high));
            }

            if (iter > MaxIterations)
                throw new ArgumentException("Invalid Initial Conditions for Bisection.");

            iter = 0;
            double x_mid, f_mid;
            do
            {
                iter++;
                x_mid = (x_low + x_high) / 2;
                f_mid = f(x_mid);
                if (Abs(f_mid) <= tol) return x_mid;
                if (f_low * f_mid < 0)
                {
                    x_high = x_mid;
                    f_high = f_mid;
                }
                else if (f_high * f_mid < 0)
                {
                    x_low = x_mid;
                    f_low = f_mid;
                }
                else
                    throw new InvalidOperationException();

            } while (iter <= MaxIterations && Abs(x_high - x_low) > 2 * tol);

            if (iter > MaxIterations)
                throw new ArgumentException("Tolerance is too small. Max iterations reached.");

            return x_mid;
        }
    }
}
