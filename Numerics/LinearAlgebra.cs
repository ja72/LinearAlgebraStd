using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using static System.Math;

namespace JA.Numerics
{
    public enum Axis2
    {
        X, Y
    }
    public enum Axis3
    {
        X, Y, Z
    }

    public static class LinearAlgebra
    {
        public const double Epsilon = 1f/1048576;

        public static Random RandomNumberGenerator { get; } = new Random();

        #region Arrays
        public static float[] RandomArray(int size, float minValue = 0, float maxValue = 1)
        {
            var data = new float[size];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = minValue + (float)RandomNumberGenerator.NextDouble() * (maxValue - minValue);
            }
            return data;
        }
        public static double[] RandomArray(int size, double minValue = 0, double maxValue = 1)
        {
            var data = new double[size];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = minValue + RandomNumberGenerator.NextDouble() * (maxValue - minValue);
            }
            return data;
        }

        public static bool Equals(double[] a, double[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
        public static bool Near(double[] a, double[] b, double tol = Epsilon)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (Math.Abs(b[i] - a[i]) > Epsilon) return false;
            }
            return true;
        }

        public static int GetHashCode<TData>(params TData[] arguments)
        {
            unchecked
            {
                int hc = -1817952719;
                for (int i = 0; i < arguments.Length; i++)
                {
                    hc = (-1521134295) * hc + arguments[i].GetHashCode();
                }
                return hc;
            }
        }

        #endregion

        #region Values
        public static double Random(double minValue = 0, double maxValue = 1)
            => minValue + (maxValue - minValue) * (double)RandomNumberGenerator.NextDouble();

        public static double Cap(this double value, double minValue = 0, double maxValue = 1)
        {
            if (value < minValue) return minValue;
            if (value > maxValue) return maxValue;
            return value;
        }
        public static double CapAbs(this double value, double minValue = 0, double maxValue = 1)
        {
            if (Abs(value) < minValue) return minValue * Sign(value);
            if (Abs(value) > maxValue) return maxValue * Sign(value);
            return value;
        }

        public static AbsFloatComparer AbsFloat(double delta)
            => new AbsFloatComparer(delta);
        public static AbsFloatComparer AbsFloat(int factor)
            => new AbsFloatComparer(factor * Epsilon);

        #endregion

        #region Planar
        public static Vector2 NaN { get; } = new Vector2(double.NaN, double.NaN);
        public static Vector2 Inf { get; } = new Vector2(double.PositiveInfinity, double.PositiveInfinity);
        public static bool IsNaN(this Vector2 vector) => double.IsNaN(vector.X) || double.IsNaN(vector.Y);
        public static bool IsInf(this Vector2 vector) => double.IsInfinity(vector.X) || double.IsInfinity(vector.Y);
        public static bool IsZero(this Vector2 vector) => vector.Equals(Vector2.Zero);
        public static bool IsZero(this Vector2 vector, double epsilon) => Math.Abs(vector.X)<=epsilon && Math.Abs(vector.Y) <= epsilon;
        public static double Element(this Vector2 vector, int index)
        {
            switch (index)
            {
                case 0: return vector.X;
                case 1: return vector.Y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        public static Vector2 RandomVector2(double minValue = 0, double maxValue = 1)
            => new Vector2(Random(minValue, maxValue), Random(minValue, maxValue));
        public static Matrix2 RandomMatrix2(double minValue = 0, double maxValue = 1)
            => new Matrix2(
                Random(minValue, maxValue), Random(minValue, maxValue),
                Random(minValue, maxValue), Random(minValue, maxValue));
        public static Vector21 RandomVector21(double minValue = 0, double maxValue = 1)
            => new Vector21(RandomVector2(minValue, maxValue), Random(minValue, maxValue));
        public static Matrix21 RandomMatrix21(double minValue = 0, double maxValue = 1)
            => new Matrix21(
                RandomMatrix2(minValue, maxValue), RandomVector2(minValue, maxValue),
                RandomVector2(minValue, maxValue), Random(minValue, maxValue));

        public static double MmoiScalar(this Vector2 position)
            => position.Magnitude();

        public static Matrix2 MmoiMatrix(this Vector2 position)
            => Dot(position, position) - Outer(position, position);

        public static Vector2 Rotate(this Vector2 vector, double angle)
            => angle != 0
                ? Vector2.Transform(vector, Rotor2.CreateRotation(angle))
                : vector;
        public static Vector2 Rotate(this Vector2 vector, Rotor2 rotation)
            => Vector2.Transform(vector, rotation);
        public static double Dot(this Vector2 vector, Vector2 other)
            => Vector2.Dot(vector, other);
        public static Matrix2 Outer(this Vector2 vector, Vector2 other)
            => new Matrix2(
                vector.X * other.X, vector.X * other.Y,
                vector.Y * other.X, vector.Y * other.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double[] ToArray(this Vector2 vector) => new[] { vector.X, vector.Y };

        public static Vector2 ToUnit(this Vector2 vector) => Vector2.Normalize(vector);

        /// <summary>
        /// Vector/Vector cross product.
        /// <code>[rx,ry] × [fx,fy] = rx*fy-ry*fx</code>
        /// </summary>
        /// <param name="vector">The planar vector.</param>
        /// <param name="other">The other planar vector.</param>
        /// <returns>A scalar</returns>
        public static double Cross(this Vector2 vector, Vector2 other)
            => vector.X * other.Y - vector.Y * other.X;

        /// <summary>
        /// Vector/scalar cross product.
        /// <code>[x,y] × ω = [y*ω, -x*ω]</code>
        /// </summary>
        /// <param name="planar">The planar vector.</param>
        /// <param name="normal">The scalar normal.</param>
        /// <returns>A vector</returns>
        public static Vector2 Cross(this Vector2 planar, double normal=1)
            => new Vector2(planar.Y * normal, -planar.X * normal);

        /// <summary>
        /// Vector/scalar cross product.
        /// <code>ω × [x,y] = [-y*ω, x*ω]</code>
        /// </summary>
        /// <param name="normal">The scalar normal</param>
        /// <param name="planar">The planar vector</param>
        /// <returns>A vector</returns>
        public static Vector2 Cross(this double normal, Vector2 planar)
            => Cross(planar, -normal);

        public static double TriangleSignedArea(Vector2 A, Vector2 B, Vector2 C)
        {
            return (Cross(A, B) + Cross(B, C) + Cross(C, A)) / 2;
        }

        #endregion


    }

    #region Comparers
    public class AbsFloatComparer :
        IComparer<double>,
        IEqualityComparer<double>,
        System.Collections.IComparer,
        System.Collections.IEqualityComparer
    {
        public AbsFloatComparer(double delta)
        {
            Delta = delta;
        }

        public double Delta { get; }

        public int Compare(double x, double y)
        {
            if (Math.Abs(x - y) <= Delta) return 0;
            if (x > y) return 1;
            return -1;
        }

        public bool Equals(double x, double y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }

        int System.Collections.IComparer.Compare(object x_obj, object y_obj)
        {
            if (x_obj is double x && y_obj is double y)
            {
                return Compare(x, y);
            }
            return -1;
        }

        bool System.Collections.IEqualityComparer.Equals(object x_obj, object y_obj)
        {
            if (x_obj is double x && y_obj is double y)
            {
                return Equals(x, y);
            }
            return false;
        }

        int System.Collections.IEqualityComparer.GetHashCode(object obj)
        {
            if (obj is double x)
            {
                return x.GetHashCode();
            }
            return obj.GetHashCode();
        }
    }

    #endregion
}
