using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JA.Numerics;
using static System.Math;

namespace JA.Numerics
{
    /// <summary>Vector implementation. This is thin wrapper over 1D array</summary>
    public readonly struct Vector : 
        IEquatable<Vector>,
        System.Collections.ICollection,
        IReadOnlyList<double>,
        IFormattable
    {
        readonly double[] _data;

        /// <summary>Gets number of components in a vector</summary>
        /// <exception cref="NullReferenceException">Thrown when vector constructed by default constructor and not assigned yet</exception>
        public int Length
        {
            get { return _data == null ? 0 : _data.Length; }
        }

        /// <summary>Constructs vector from array of arguments</summary>
        /// <param name="elements">Elements of array</param>
        /// <example>This creates vector with three elements -1,0,1. New
        /// array is allocated for this vector:
        /// <code>Vector v = new Vector(-1,0,1)</code>
        /// Next example creates vector that wraps specified array. Please note
        /// that no array copying is made thus changing of source array with change vector elements.
        /// <code>
        /// double[] arr = new double[] { -1,0,1 };
        /// Vector v = new Vector(arr);
        /// </code>
        /// </example>
        public Vector(params double[] elements)
        {
            _data = elements??throw new ArgumentNullException(nameof(elements));
        }

        /// <summary>Constructs vector of specified length filled with zeros</summary>
        /// <param name="n">Length of vector</param>
        /// <returns>Constructed vector</returns>
        public static Vector Zeros(int n)
        {
            return new Vector(new double[n]);
        }

        /// <summary>Clones specified vector</summary>
        /// <param name="v">Vector to clone</param>
        /// <returns>Copy of vector passes as parameter</returns>
        public Vector Clone()
        {
            return _data == null ? new Vector() : new Vector((double[])_data.Clone());
        }

        /// <summary>
        /// Copies vector to double[] array 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return (double[])_data.Clone();
        }

        /// <summary>Copies content of one vector to another. Vectors must have same length.</summary>
        /// <param name="src">Source vector</param>
        /// <param name="dst">Vector to copy results</param>
        public static void Copy(Vector src, ref Vector dst)
        {
            if (src._data == null)
                throw new ArgumentNullException(nameof(src));
            if (dst._data == null)
                throw new ArgumentNullException(nameof(dst));
            int n = src._data.Length;
            if (dst._data.Length != n)
                dst = new Vector(n);
            Array.Copy(src._data, dst._data, n);
        }

        public static Vector Append(params Vector[] vectors)
        {
            int n = vectors.Sum((v) => v.Length);
            var result = new double[n];
            var span = result.AsSpan();
            int index = 0;
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i].AsReadOnlySpan().CopyTo(span.Slice(index, vectors[i].Length));
                index += vectors[i].Length;
            }
            return new Vector(result);
        }
        public static Vector[] Split(Vector vector, int divisions)
        {

            int n = Math.DivRem(vector.Length, divisions, out int remainder);
            var result = new Vector[ remainder > 0 ? n + 1 : n ];
            var span = vector.AsReadOnlySpan();
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                span.Slice(index, n).CopyTo(result[i].AsSpan());
                index += n;
            }
            if (remainder > 0)
            {
                span.Slice(index, remainder).CopyTo(result[n].AsSpan());
            }
            return result;
        }

        /// <summary>Gets L-infinity norm of the vector</summary>
        public double LInfinityNorm
        {
            get
            {
                double max = 0;

                for (int i = 0; i < _data.Length; i++)
                {
                    if (Math.Abs(_data[i]) > max)
                        max = _data[i];
                }

                return Math.Abs(max);
            }
        }


        ///<summary>Gets vector's Euclidean norm</summary>
        public double EuclideanNorm
        {
            get
            {
                double lsq = 0;

                for (int i = 0; i < _data.Length; i++)
                {
                    lsq += _data[i] * _data[i];

                }

                return Sqrt(lsq);
            }
        }

        ///<summary>Gets vector's sum</summary>
        public double Sum
        {
            get
            {
                double sum = 0;

                for (int i = 0; i < _data.Length; i++)
                {
                    sum += _data[i];
                }

                return sum;
            }
        }

        /// <summary>Returns Euclidean norm of difference between two vectors. 
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Euclidean norm of vector's difference</returns>
        public static double GetEuclideanNorm(Vector v1, Vector v2)
        {
            double[] av1 = v1._data;
            double[] av2 = v2._data;
            if (av1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (av2 == null)
                throw new ArgumentNullException(nameof(v2));
            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lenghtes do not match");
            double norm = 0;
            for (int i = 0; i < av1.Length; i++)
                norm += (av1[i] - av2[i]) * (av1[i] - av2[i]);
            return Sqrt(norm);
        }

        /// <summary>Returns L-infinity norm of difference between two vectors. 
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>L-infinity norm of vector's difference</returns>
        public static double GetLInfinityNorm(Vector v1, Vector v2)
        {
            double[] av1 = v1._data;
            double[] av2 = v2._data;
            if (av1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (av2 == null)
                throw new ArgumentNullException(nameof(v2));
            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lengths do not match");
            double norm = 0;
            for (int i = 0; i < av1.Length; i++)
                norm = Math.Max(norm, Math.Abs(av1[i] - av2[i]));
            return norm;
        }

        /// <summary>Performs linear interpolation between two vectors at specified point</summary>
        /// <param name="t">Point of interpolation</param>
        /// <param name="t0">First time point</param>
        /// <param name="v0">Vector at first time point</param>
        /// <param name="t1">Second time point</param>
        /// <param name="v1">Vector at second time point</param>
        /// <returns>Interpolated vector value at point <paramref name="t"/></returns>
        public static Vector Lerp(double t, double t0, Vector v0, double t1, Vector v1)
        {
            return (v0 * (t1 - t) + v1 * (t - t0)) / (t1 - t0);
        }

        /// <summary>Gets or sets vector element at specified index</summary>
        /// <exception cref="NullReferenceException">Thrown when vector is not initialized</exception>
        /// <exception cref="IndexOutOfRangeException">Throws when <paramref name="index"/> is out of range</exception>
        /// <param name="index">Index of element</param>
        /// <returns>Value of vector element at specified index</returns>
        public double this[int index]
        {
            get { return _data[index]; }
            set { _data[index] = value; }
        }

        /// <summary>
        /// Gets the a vector slice at the specified index and for the specified number of elements.
        /// </summary>
        /// <param name="index">The index where the slice starts.</param>
        /// <param name="count">The count of elements in the slice.</param>
        /// <returns>A <see cref="Span{Double}"/> with the slice of the vector data.</returns>
        public Span<double> this[int index, int count]
        {
            get => _data.AsSpan().Slice(index, count);
        }

        /// <summary>Performs conversion of vector to array</summary>
        /// <param name="v">Vector to be converted</param>
        /// <returns>Array with contents of vector</returns>
        /// <remarks>This conversions doesn't perform array copy. In fact in returns reference
        /// to the same data</remarks>
        public static implicit operator double[] (Vector v)
        {
            return v._data;
        }

        /// <summary>Performs conversion of 1d vector to</summary>
        /// <param name="v">Vector to be converted</param>
        /// <returns>Scalar value with first component of vector</returns>
        /// <exception cref="InvalidOperationException">Thrown when vector length is not one</exception>
        public static implicit operator double(Vector v)
        {
            double[] av = v;
            if (av == null)
                throw new ArgumentNullException(nameof(v));
            if (av.Length != 1)
                throw new InvalidOperationException("Cannot convert multi-element vector to scalar");
            return av[0];
        }

        /// <summary>Performs conversion of array to vector</summary>
        /// <param name="v">Array to be represented by vector</param>
        /// <returns>Vector that wraps specified array</returns>
        public static implicit operator Vector(double[] v)
        {
            return new Vector(v);
        }

        /// <summary>Performs conversion of scalar to vector with length 1</summary>
        /// <param name="v">Double precision vector</param>
        /// <returns>Vector that wraps array with 1 element</returns>
        public static implicit operator Vector(double v)
        {
            return new Vector(v);
        }

        /// <summary>Adds vector <paramref name="v1"/> multiplied by <paramref name="factor"/> to this object.</summary>
        /// <param name="v1">Vector to add</param>
        /// <param name="factor">Multiplication factor</param>
        public void MulAdd(Vector v1, double factor)
        {
            double[] av1 = v1._data;
            if (av1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (this.Length != av1.Length)
                throw new InvalidOperationException("Cannot add vectors of different length");

            for (int i = 0; i < _data.Length; i++)
                _data[i] = _data[i] + factor * av1[i];
        }

        /// <summary>Sums two vectors. Vectors must have same length.</summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Sum of vectors</returns>
        public static Vector operator +(Vector v1, Vector v2)
        {
            double[] av1 = v1;
            double[] av2 = v2;
            if (av1.Length != av2.Length)
                throw new InvalidOperationException("Cannot add vectors of different length");
            double[] result = new double[av1.Length];
            for (int i = 0; i < av1.Length; i++)
                result[i] = av1[i] + av2[i];
            return new Vector(result);
        }

        /// <summary>Add a scalar to a vector.</summary>
        /// <param name="v">Vector</param>
        /// <param name="c">Scalar to add</param>
        /// <returns>Shifted vector</returns>
        public static Vector operator +(Vector v, double c)
        {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = av[i] + c;
            return new Vector(result);
        }


        /// <summary>Subtracts first vector from second. Vectors must have same length</summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Difference of two vectors</returns>
        public static Vector operator -(Vector v1, Vector v2)
        {
            double[] av1 = v1;
            double[] av2 = v2;
            if (av1.Length != av2.Length)
                throw new InvalidOperationException("Cannot subtract vectors of different length");
            double[] result = new double[av1.Length];
            for (int i = 0; i < av1.Length; i++)
                result[i] = av1[i] - av2[i];
            return new Vector(result);
        }

        /// <summary>Multiplies a vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Vector with all components multiplied by scalar</returns>
        public static Vector operator *(Vector v, double a)
        {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = a * av[i];
            return new Vector(result);
        }

        /// <summary>Multiplies a vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Vector with all components multiplied by scalar</returns>
        public static Vector operator *(double a, Vector v)
        {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = a * av[i];
            return new Vector(result);
        }

        /// <summary>Performs scalar multiplication of two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Result of scalar multiplication</returns>
        public static double operator *(Vector a, Vector b)
        {
            double res = 0;
            if (a.Length != a.Length)
                throw new InvalidOperationException("Cannot multiply vectors of different length");

            for (int i = 0; i < a.Length; i++)
            {
                res = res + a[i] * b[i];
            }
            return res;
        }

        /// <summary>Multiplies vector a[i] by vector b[j] and returns matrix with components a[i]*b[j]</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Matrix with number of rows equals to first vector length and numbers of column equals to second vector length</returns>
        public static Matrix operator &(Vector a, Vector b)
        {
            int m = a.Length, n = b.Length;
            Matrix res = new Matrix(m, n);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    res[i, j] = a[i] * b[j];
                }
            }
            return res;
        }

        /// <summary>Divides vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Result of division</returns>
        public static Vector operator /(Vector v, double a)
        {
            double[] av = v;

            if (a == 0.0d) throw new DivideByZeroException("Cannot divide by zero");

            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = av[i] / a;
            return new Vector(result);
        }

        /// <summary>Performs element-wise division of two vectors</summary>
        /// <param name="a">Numerator vector</param>
        /// <param name="b">Denominator vector</param>
        /// <returns>Result of scalar multiplication</returns>
        public static Vector operator /(Vector a, Vector b)
        {
            if (a.Length != b.Length)
                throw new InvalidOperationException("Cannot element-wise divide vectors of different length");
            double[] res = Zeros(a.Length);

            for (int i = 0; i < a.Length; i++)
            {
                if (b[i] == 0.0d) throw new DivideByZeroException("Cannot divide by zero");
                res[i] = a[i] / b[i];
            }

            return res;
        }

        /// <summary>
        /// Returns a vector each of whose elements is the maximal from the corresponding
        /// ones of argument vectors. Note that dimensions of the arguments must match.
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>vector v3 such that for each i = 0...dim(v1) v3[i] = max( v1[i], v2[i] )</returns>
        public static Vector Max(Vector v1, Vector v2)
        {
            double[] av1 = v1._data;
            double[] av2 = v2._data;

            if (av1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (av2 == null)
                throw new ArgumentNullException(nameof(v2));

            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lengths do not match");
            Vector y = Zeros(av1.Length);
            for (int i = 0; i < av1.Length; i++)
                y[i] = Math.Max(av1[i], av2[i]);

            return y;
        }

        /// <summary>
        /// Returns a vector whose elements are the absolute values of the given vector elements
        /// </summary>
        /// <param name="v">Vector to operate with</param>
        /// <returns>Vector v1 such that for each i = 0...dim(v) v1[i] = |v[i]|</returns>
        public Vector Abs()
        {
            if (_data == null)
                return new Vector();

            int n = _data.Length;
            Vector y = Zeros(n);
            for (int i = 0; i < n; i++)
                y[i] = Math.Abs(_data[i]);
            return y;
        }

        #region Equality
        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }
        public override bool Equals(object obj)
        {
            return obj is Vector vector && Equals(vector);
        }
        public bool Equals(Vector other) 
            => Enumerable.SequenceEqual(_data, other._data);

        public override int GetHashCode()
        {
            unchecked
            {
                return _data.Aggregate(1768953197, (h, x) => (-1521134295)*h + x.GetHashCode());
            }
        }

        #endregion

        #region Collections
        public double[] AsArray() => _data;
        public ReadOnlySpan<double> AsReadOnlySpan()
        {
            return new ReadOnlySpan<double>(_data);
        }
        public Span<double> AsSpan()
        {
            return new Span<double>(_data);
        }
        public void CopyTo(Array array, int index) => _data.CopyTo(array, index);
        public int Count { get => _data.Length; }
        object System.Collections.ICollection.SyncRoot { get => _data; }
        bool System.Collections.ICollection.IsSynchronized { get => true; }
        public System.Collections.IEnumerator GetEnumerator() => _data.GetEnumerator();
        public System.Collections.Generic.IEnumerable<double> AsEnumerable() => this as System.Collections.Generic.IEnumerable<double>;
        System.Collections.Generic.IEnumerator<double> System.Collections.Generic.IEnumerable<double>.GetEnumerator()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                yield return _data[i];
            }
        }
        #endregion

        #region Formatting
        /// <summary>Converts vector to string representation.</summary>
        /// <returns>String consists from vector components separated by comma.</returns>
        public string ToString(string formatting, IFormatProvider provider)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (_data != null)
                for (int i = 0; i < _data.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(_data[i].ToString(formatting, provider));
                }
            sb.Append("]");
            return sb.ToString();
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion


    }
}