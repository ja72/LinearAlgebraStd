using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JA.Numerics
{
    public readonly struct Matrix2 :
        IEquatable<Matrix2>,
        System.Collections.ICollection,
        ICollection<double>,
        IReadOnlyList<double>,
        IFormattable
    {
        readonly (double m_11, double m_12, 
            double m_21, double m_22) data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2(double m_11, double m_12, double m_21, double m_22) : this()
        {
            this.data = (m_11, m_12, m_21, m_22);
        }

        public static implicit operator Matrix2(System.Numerics.Matrix3x2 matrix3X2)
            => new Matrix2(
                matrix3X2.M11, matrix3X2.M12,
                matrix3X2.M21, matrix3X2.M22);

        public static explicit operator System.Numerics.Matrix3x2(Matrix2 matrix)
            => new System.Numerics.Matrix3x2(
                (float)matrix.data.m_11, (float)matrix.data.m_12,
                (float)matrix.data.m_21, (float)matrix.data.m_22,
                0, 0);

        public static readonly Matrix2 Zero = new Matrix2(0, 0, 0, 0);
        public static readonly Matrix2 Identity = new Matrix2(1, 0, 0, 1);
        public static implicit operator Matrix2(double value) => FromScalar(value);

        public static Matrix2 FromScalar(double d) => Diagonal(d, d);
        public static Matrix2 Diagonal(Vector2 v) => Diagonal(v.X, v.Y);
        public static Matrix2 Diagonal(double d1, double d2) => new Matrix2(d1, 0, 0, d2);

        public static Matrix2 FromRows(Vector2 row1, Vector2 row2)
            => new Matrix2(
                row1.X, row1.Y, 
                row2.X, row2.Y);
        public static Matrix2 FromColumns(Vector2 column1, Vector2 column2)
            => new Matrix2(
                column1.X, column2.X, 
                column1.Y, column2.Y);

        #region Properties        
        public double A11 { get => data.m_11; }
        public double A12 { get => data.m_12; }
        public double A21 { get => data.m_21; }
        public double A22 { get => data.m_22; }

        /// <summary>
        /// Access elements of the matrix.
        /// </summary>
        /// <param name="index">The index of the element as if they were layed out by rows.</param>
        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => data.m_11,
                    1 => data.m_12,
                    2 => data.m_21,
                    3 => data.m_22,
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        /// <summary>
        /// Access elements of the matrix.
        /// </summary>
        /// <param name="row">The matrix row index.</param>
        /// <param name="column">The matrix column index.</param>
        public double this[int row, int column]
        {
            get
            {
                return row switch
                {
                    0 => column switch
                    {
                        0 => data.m_11,
                        1 => data.m_12,
                        _ => throw new ArgumentOutOfRangeException(nameof(column)),
                    },
                    1 => column switch
                    {
                        0 => data.m_21,
                        1 => data.m_22,
                        _ => throw new ArgumentOutOfRangeException(nameof(column)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(row)),
                };
            }
        }

        /// <summary>
        /// Get the number of rows
        /// </summary>
        public int RowCount => 2;
        /// <summary>
        /// Get the number of columns
        /// </summary>
        public int ColumnCount => 2;
        /// <summary>
        /// Check of the number of rows equals the number of columns. Always returns <c>true</c>.
        /// </summary>
        public bool IsSquare => true;
        /// <summary>
        /// Check if all elements are zero
        /// </summary>
        public bool IsZero
            => data.m_11 == 0 && data.m_12 == 0
            && data.m_21 == 0 && data.m_22 == 0;
        /// <summary>
        /// Check if the matrix is symmetric
        /// </summary>
        public bool IsSymmetric => data.m_12 == data.m_21;
        /// <summary>
        /// Check if the matrix is skew-symmetric
        /// </summary>
        public bool IsSkewSymmetric
            => data.m_12 == -data.m_21
            && data.m_11 == 0 && data.m_22 == 0;
        /// <summary>
        /// Check if the matrix is diagonal
        /// </summary>
        public bool IsDiagonal
            => data.m_12 == 0 && data.m_21 == 0;
        /// <summary>
        /// Check if the matrix is scalar (diagonal & all elements the same).
        /// </summary>
        public bool IsScalar
            => IsDiagonal && data.m_11 == data.m_22;

        public double Determinant { get => data.m_11 * data.m_22 - data.m_12 * data.m_21; }
        public double Trace { get => data.m_11 + data.m_22; }

        public Vector2 Row1 => new Vector2(data.m_11, data.m_12);
        public Vector2 Row2 => new Vector2(data.m_21, data.m_22);
        public Vector2 Column1 => new Vector2(data.m_11, data.m_21);
        public Vector2 Column2 => new Vector2(data.m_12, data.m_22);

        public Vector2 GetDiagonal() => new Vector2(data.m_11, data.m_22);

        public Matrix2 ToSymmetricMatrix() => (this + Transpose()) / 2;
        public Matrix2 ToSkewSymmetricMatrix() => (this - Transpose()) / 2;
        public Matrix2 ToDiagonalMatrix() => Diagonal(data.m_11, data.m_22);

        public Matrix2 Transpose() 
            => new Matrix2(
                data.m_11, data.m_21, 
                data.m_12, data.m_22);

        #endregion

        #region Algebra
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2 Add(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a.data.m_11 + b.data.m_11,
                a.data.m_12 + b.data.m_12,
                a.data.m_21 + b.data.m_21,
                a.data.m_22 + b.data.m_22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2 Subtract(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a.data.m_11 - b.data.m_11,
                a.data.m_12 - b.data.m_12,
                a.data.m_21 - b.data.m_21,
                a.data.m_22 - b.data.m_22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2 Negate(Matrix2 a)
            => new Matrix2(
                -a.data.m_11,
                -a.data.m_12,
                -a.data.m_21,
                -a.data.m_22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2 Scale(double f, Matrix2 a)
            => new Matrix2(
                f * a.data.m_11,
                f * a.data.m_12,
                f * a.data.m_21,
                f * a.data.m_22);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Product(Matrix2 a, Vector2 b)
            => new Vector2(
                a.data.m_11 * b.X + a.data.m_12 * b.Y,
                a.data.m_21 * b.X + a.data.m_22 * b.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Product(Vector2 a, Matrix2 b)
            => new Vector2(
                b.data.m_11 * a.X + b.data.m_21 * a.Y,
                b.data.m_12 * a.X + b.data.m_22 * a.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix2 Product(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a.data.m_11 * b.data.m_11 + a.data.m_12 * b.data.m_21,
                a.data.m_11 * b.data.m_12 + a.data.m_12 * b.data.m_22,
                a.data.m_21 * b.data.m_11 + a.data.m_22 * b.data.m_21,
                a.data.m_21 * b.data.m_12 + a.data.m_22 * b.data.m_22);

        public static Matrix2 operator -(Matrix2 a) => Negate(a);
        public static Matrix2 operator *(double f, Matrix2 a) => Scale(f, a);
        public static Matrix2 operator *(Matrix2 a, double f) => Scale(f, a);
        public static Matrix2 operator /(Matrix2 a, double d) => Scale(1 / d, a);
        public static Matrix2 operator +(Matrix2 a, Matrix2 b) => Add(a, b);
        public static Matrix2 operator -(Matrix2 a, Matrix2 b) => Subtract(a, b);
        public static Matrix2 operator ~(Matrix2 a) => a.Transpose();

        public static Vector2 operator *(Matrix2 a, Vector2 b) => Product(a, b);
        public static Vector2 operator *(Vector2 a, Matrix2 b) => Product(a, b);
        public static Matrix2 operator *(Matrix2 a, Matrix2 b) => Product(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Solve(Vector2 b) 
            => Inverse()*b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2 Solve(Matrix2 B)
            => Inverse()*B;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix2 Inverse()
            => new Matrix2(
                data.m_22, -data.m_12, 
                -data.m_21, data.m_11) 
            /Determinant;

        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            string a11 = data.m_11.ToString(formatting, provider);
            string a12 = data.m_12.ToString(formatting, provider);
            string a21 = data.m_21.ToString(formatting, provider);
            string a22 = data.m_22.ToString(formatting, provider);
            return $"[{a11} {a12}; {a21} {a22}]";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion

        #region Equality

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(SymMatrix2)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is int i)
            {
                obj = (double)i;
            }
            if (obj is double f)
            {
                return IsScalar && A11 == f;
            }
            return obj is Matrix2 matrix && Equals(matrix);
        }

        /// <summary>
        /// Checks for equality among <see cref="Matrix2"/> classes
        /// </summary>
        /// <returns>True if equal</returns>
        public bool Equals(Matrix2 other)
        {
            return data.Equals(other.data);
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="Matrix2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return -1817952719 + data.GetHashCode();
            }
        }
        public static bool operator ==(Matrix2 target, Matrix2 other) { return target.Equals(other); }
        public static bool operator !=(Matrix2 target, Matrix2 other) { return !target.Equals(other); }

        #endregion

        #region Collection
        /// <summary>
        /// Gets a value indicating whether this array is of fixed size.
        /// </summary>
        public bool IsReadOnly => true;
        /// <summary>
        /// Get the number of elements in the matrix.
        /// </summary>
        public int Count => 4;

        /// <summary>
        /// Get the elements as a span
        /// </summary>
        public unsafe ReadOnlySpan<double> AsSpan()
        {
            fixed (double* ptr = &data.m_11)
            {
                return new ReadOnlySpan<double>(ptr, 4);
            }
        }
        /// <summary>
        /// Get the elemetns as an array
        /// </summary>
        public double[] ToArray() => AsSpan().ToArray();
        /// <summary>
        /// Get the elemetns as a 2D array
        /// </summary>
        public double[,] ToArray2() => new[,] { 
            { data.m_11, data.m_12 }, 
            { data.m_21, data.m_22 } };
        /// <summary>
        /// Get the elemetns as a jagged array
        /// </summary>
        public double[][] ToJaggedArray() => [
            [data.m_11, data.m_12],
            [data.m_21, data.m_22] ];

        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);
        public void CopyTo(double[] array, int index) => ToArray().CopyTo(array, index);
        object System.Collections.ICollection.SyncRoot { get => null; }
        bool System.Collections.ICollection.IsSynchronized { get => false; }
        public System.Collections.IEnumerator GetEnumerator() => AsSpan().ToArray().GetEnumerator();
        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            yield return data.m_11;
            yield return data.m_12;
            yield return data.m_21;
            yield return data.m_22;
        }
        void ICollection<double>.Add(double item) => throw new NotSupportedException();
        void ICollection<double>.Clear() => throw new NotSupportedException();
        bool ICollection<double>.Remove(double item) => throw new NotSupportedException();
        bool ICollection<double>.Contains(double item) => throw new NotSupportedException();
        #endregion

    }
}
