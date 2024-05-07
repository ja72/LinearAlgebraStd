using System;

namespace JA.Numerics
{
    public readonly struct Vector21 :
        IEquatable<Vector21>,
        IFormattable,
        System.Collections.ICollection,
        System.Collections.Generic.IReadOnlyList<double>
    {
        readonly (double x, double y, double w) _data;

        public Vector21(Vector2 vector, double scalar) : this()
        {
            _data = (vector.X, vector.Y, scalar);
        }
        public Vector21(double x, double y, double w)
        {
            _data = (x, y, w);
        }

        public static Vector21 Zero { get; } = new Vector21(Vector2.Zero, 0);
        public static Vector21 ScalarOne { get; } = new Vector21(Vector2.Zero, 1);
        public static Vector21 UnitX { get; } = new Vector21(Vector2.UnitX, 0);
        public static Vector21 UnitY { get; } = new Vector21(Vector2.UnitY, 0);
        public static Vector21 One { get; } = new Vector21(Vector2.One, 1);

        public Vector2 Vector { get => new Vector2(_data.x, _data.y); }
        public double Scalar { get => _data.w; }

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            var x = _data.x.ToString(formatting, provider);
            var y = _data.y.ToString(formatting, provider);
            var w = _data.w.ToString(formatting, provider);
            return $"({x},{y}|{w})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion

        #region Algebra
        public static Vector21 Negate(Vector21 a)
            => new Vector21(
                -a._data.x,
                -a._data.y,
                -a._data.w);
        public static Vector21 Scale(double factor, Vector21 a)
            => new Vector21(
                factor * a._data.x,
                factor * a._data.y,
                factor * a._data.w);
        public static Vector21 Add(Vector21 a, Vector21 b)
            => new Vector21(
                a._data.x + b._data.x,
                a._data.y + b._data.y,
                a._data.w + b._data.w);
        public static Vector21 Subtract(Vector21 a, Vector21 b)
            => new Vector21(
                a._data.x - b._data.x,
                a._data.y - b._data.y,
                a._data.w - b._data.w);

        public static Vector21 operator +(Vector21 a, Vector21 b) => Add(a, b);
        public static Vector21 operator -(Vector21 a) => Negate(a);
        public static Vector21 operator -(Vector21 a, Vector21 b) => Subtract(a, b);
        public static Vector21 operator *(double f, Vector21 a) => Scale(f, a);
        public static Vector21 operator *(Vector21 a, double f) => Scale(f, a);
        public static Vector21 operator /(Vector21 a, double d) => Scale(1 / d, a);
        public static double operator |(Vector21 a, Vector21 b) => Dot(a, b);

        public static bool operator ==(Vector21 vector1, Vector21 vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector21 vector1, Vector21 vector2)
        {
            return !(vector1 == vector2);
        }
        #endregion

        #region Vector Algera
        /// <summary>
        /// Dot product of two vectors.
        /// </summary>
        public static double Dot(Vector21 a, Vector21 b)
            => a._data.x*b._data.x + a._data.y*b._data.y + a._data.w*b._data.w;

        /// <summary>
        /// Outer product of two vectors.
        /// </summary>
        public static Matrix21 Outer(Vector21 a, Vector21 b)
            => new Matrix21(
                a._data.x*b._data.x, a._data.x*b._data.y, a._data.x*b._data.w,
                a._data.y*b._data.x, a._data.y*b._data.y, a._data.y*b._data.w,
                a._data.w*b._data.x, a._data.w*b._data.y, a._data.w*b._data.w);

        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            return obj is Vector21 vec && Equals(vec);
        }
        public bool Equals(Vector21 other) => _data.Equals(other._data);

        public override int GetHashCode()
        {
            return -1945990370 + _data.GetHashCode();
        }


        #endregion

        #region Collections
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _data.x;
                    case 1: return _data.y;
                    case 2: return _data.w;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public double[] ToArray() => new[] { _data.x, _data.y, _data.w };
        public unsafe ReadOnlySpan<double> AsSpan()
        {
            fixed (double* ptr = &_data.x)
            {
                return new ReadOnlySpan<double>(ptr, 3);
            }
        }

        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);
        public int Count { get => 3; }
        object System.Collections.ICollection.SyncRoot { get => null; }
        bool System.Collections.ICollection.IsSynchronized { get => false; }
        public System.Collections.IEnumerator GetEnumerator() => ToArray().GetEnumerator();
        public System.Collections.Generic.IEnumerable<double> AsEnumerable() => this as System.Collections.Generic.IEnumerable<double>;
        System.Collections.Generic.IEnumerator<double> System.Collections.Generic.IEnumerable<double>.GetEnumerator()
        {
            yield return _data.x;
            yield return _data.y;
            yield return _data.w;
        }
        #endregion

    }

}
