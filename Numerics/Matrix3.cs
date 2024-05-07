using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace JA.Numerics
{
    public readonly struct Matrix3 :
        IEquatable<Matrix3>,
        System.Collections.ICollection,
        ICollection<double>,
        IReadOnlyList<double>,
        IFormattable
    {
        readonly (double m_11, double m_12, double m_13, 
            double m_21, double m_22, double m_23, 
            double m_31, double m_32, double m_33) data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3(double m_11, double m_12, double m_13, double m_21, double m_22, double m_23, double m_31, double m_32, double m_33)
        {
            data=(m_11, m_12, m_13, m_21, m_22, m_23, m_31, m_32, m_33);
        }

        public static implicit operator Matrix3(System.Numerics.Matrix4x4 matrix4X4)
            => new Matrix3(
                matrix4X4.M11, matrix4X4.M12, matrix4X4.M13,
                matrix4X4.M21, matrix4X4.M22, matrix4X4.M23,
                matrix4X4.M31, matrix4X4.M32, matrix4X4.M33);

        public static explicit operator System.Numerics.Matrix4x4(Matrix3 matrix)
            => new System.Numerics.Matrix4x4(
                (float)matrix.data.m_11, (float)matrix.data.m_12, (float)matrix.data.m_13, 0,
                (float)matrix.data.m_21, (float)matrix.data.m_22, (float)matrix.data.m_23, 0,
                (float)matrix.data.m_31, (float)matrix.data.m_32, (float)matrix.data.m_33, 0,
                0, 0, 0, 1);


        public static readonly Matrix3 Zero = new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Matrix3 Identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
        public static implicit operator Matrix3(double value) => FromScalar(value);

        public static Matrix3 FromScalar(double d) => Diagonal(d, d, d);
        public static Matrix3 Diagonal(Vector3 v) => Diagonal(v.X, v.Y, v.Z);
        public static Matrix3 Diagonal(double d1, double d2, double d3) => new Matrix3(d1, 0, 0, 0, d2, 0, 0, 0, d3);

        public static Matrix3 FromRows(Vector3 row1, Vector3 row2, Vector3 row3)
            => new Matrix3(
                row1.X, row1.Y, row1.Z, 
                row2.X, row2.Y, row2.Z,
                row3.Z, row3.Y, row3.Z);
        public static Matrix3 FromColumns(Vector3 column1, Vector3 column2, Vector3 column3)
            => new Matrix3(
                column1.X, column2.X, column3.X,
                column1.Y, column2.Y, column3.Y,
                column1.Z, column2.Z, column3.Z);


        #region Properties
        public double A11 { get => data.m_11; }
        public double A12 { get => data.m_12; }
        public double A13 { get => data.m_13; }
        public double A21 { get => data.m_21; }
        public double A22 { get => data.m_22; }
        public double A23 { get => data.m_23; }
        public double A31 { get => data.m_21; }
        public double A32 { get => data.m_22; }
        public double A33 { get => data.m_23; }

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
                    2 => data.m_13,
                    3 => data.m_21,
                    4 => data.m_22,
                    5 => data.m_23,
                    6 => data.m_31,
                    7 => data.m_32,
                    8 => data.m_33,
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
                        2 => data.m_13,
                        _ => throw new ArgumentOutOfRangeException(nameof(column)),
                    },
                    1 => column switch
                    {
                        0 => data.m_21,
                        1 => data.m_22,
                        2 => data.m_23,
                        _ => throw new ArgumentOutOfRangeException(nameof(column)),
                    },
                    3 => column switch
                    {
                        0 => data.m_31,
                        1 => data.m_32,
                        2 => data.m_33,
                        _ => throw new ArgumentOutOfRangeException(nameof(column)),
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(row)),
                };
            }
        }

        /// <summary>
        /// Get the number of rows
        /// </summary>
        public int RowCount => 3;
        /// <summary>
        /// Get the number of columns
        /// </summary>
        public int ColumnCount => 3;
        /// <summary>
        /// Check of the number of rows equals the number of columns. Always returns <c>true</c>.
        /// </summary>
        public bool IsSquare => true;
        /// <summary>
        /// Check if all elements are zero
        /// </summary>
        public bool IsZero
            => data.m_11 == 0 && data.m_12 == 0 && data.m_13 == 0
            && data.m_21 == 0 && data.m_22 == 0 && data.m_23 == 0
            && data.m_31 == 0 && data.m_32 == 0 && data.m_33 == 0;
        /// <summary>
        /// Check if the matrix is symmetric
        /// </summary>
        public bool IsSymmetric 
            => data.m_12 == data.m_21
            && data.m_23 == data.m_32
            && data.m_31 == data.m_13;

        /// <summary>
        /// Check if the matrix is skew-symmetric
        /// </summary>
        public bool IsSkewSymmetric
            => data.m_11 ==0 && data.m_22 == 0 && data.m_33 == 0
            && data.m_12 == -data.m_21
            && data.m_23 == -data.m_32
            && data.m_31 == -data.m_13;
        /// <summary>
        /// Check if the matrix is diagonal
        /// </summary>
        public bool IsDiagonal
            => data.m_12 == 0 && data.m_13 == 0 
            && data.m_21 == 0 && data.m_23 == 0
            && data.m_31 == 0 && data.m_32 == 0;
        /// <summary>
        /// Check if the matrix is scalar (diagonal & all elements the same).
        /// </summary>
        public bool IsScalar
            => IsDiagonal 
            && data.m_11 == data.m_22 && data.m_11 == data.m_33;

        public double Determinant
        {
            get
            {
                var t2 = data.m_11*data.m_22*data.m_33;
                var t3 = data.m_12*data.m_23*data.m_31;
                var t4 = data.m_13*data.m_21*data.m_32;
                var t7 = data.m_11*data.m_23*data.m_32;
                var t8 = data.m_12*data.m_21*data.m_33;
                var t9 = data.m_13*data.m_22*data.m_31;
                return t2+t3+t4-t7-t8-t9;
            }
        }
        public double Trace => data.m_11 + data.m_22 + data.m_33;

        public Vector3 Row1 => new Vector3(data.m_11, data.m_12, data.m_13);
        public Vector3 Row2 => new Vector3(data.m_21, data.m_22, data.m_23);
        public Vector3 Row3 => new Vector3(data.m_31, data.m_32, data.m_33);
        public Vector3 Column1 => new Vector3(data.m_11, data.m_21, data.m_31);
        public Vector3 Column2 => new Vector3(data.m_12, data.m_22, data.m_32);
        public Vector3 Column3 => new Vector3(data.m_13, data.m_23, data.m_33);

        public Vector3 GetDiagonal() => new Vector3(data.m_11, data.m_22, data.m_33);

        public Matrix3 ToSymmetricMatrix() => (this + Transpose()) / 2;
        public Matrix3 ToSkewSymmetricMatrix() => (this - Transpose()) / 2;
        public Matrix3 ToDiagonalMatrix() => Diagonal(data.m_11, data.m_22, data.m_33);

        public Matrix3 Transpose()
            => new Matrix3(
                data.m_11, data.m_21, data.m_31,
                data.m_12, data.m_22, data.m_32,
                data.m_13, data.m_23, data.m_33);
                
        #endregion

        #region Algebra

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3 Add(Matrix3 a, Matrix3 b)
            => new Matrix3(
                a.data.m_11 + b.data.m_11,
                a.data.m_12 + b.data.m_12,
                a.data.m_13 + b.data.m_13,
                a.data.m_21 + b.data.m_21,
                a.data.m_22 + b.data.m_22,
                a.data.m_23 + b.data.m_23,
                a.data.m_31 + b.data.m_31,
                a.data.m_32 + b.data.m_32,
                a.data.m_33 + b.data.m_33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3 Subtract(Matrix3 a, Matrix3 b)
            => new Matrix3(
                a.data.m_11 - b.data.m_11,
                a.data.m_12 - b.data.m_12,
                a.data.m_13 - b.data.m_13,
                a.data.m_21 - b.data.m_21,
                a.data.m_22 - b.data.m_22,
                a.data.m_23 - b.data.m_23,
                a.data.m_31 - b.data.m_31,
                a.data.m_32 - b.data.m_32,
                a.data.m_33 - b.data.m_33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3 Negate(Matrix3 a)
            => new Matrix3(
                -a.data.m_11,
                -a.data.m_12,
                -a.data.m_13,
                -a.data.m_21,
                -a.data.m_22,
                -a.data.m_23,
                -a.data.m_31,
                -a.data.m_32,
                -a.data.m_33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3 Scale(double f, Matrix3 a)
            => new Matrix3(
                f * a.data.m_11,
                f * a.data.m_12,
                f * a.data.m_13,
                f * a.data.m_21,
                f * a.data.m_22,
                f * a.data.m_23,
                f * a.data.m_31,
                f * a.data.m_32,
                f * a.data.m_33);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Product(Matrix3 a, Vector3 b)
            => new Vector3(
                a.data.m_11 * b.X + a.data.m_12 * b.Y + a.data.m_13 * b.Z,
                a.data.m_21 * b.X + a.data.m_22 * b.Y + a.data.m_23 * b.Z,
                a.data.m_31 * b.X + a.data.m_32 * b.Y + a.data.m_33 * b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Product(Vector3 a, Matrix3 b)
            => new Vector3(
                b.data.m_11 * a.X + b.data.m_21 * a.Y + b.data.m_31 * a.Z,
                b.data.m_12 * a.X + b.data.m_22 * a.Y + b.data.m_32 * a.Z,
                b.data.m_13 * a.X + b.data.m_23 * a.Y + b.data.m_33 * a.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3 Product(Matrix3 a, Matrix3 b)
            => new Matrix3(
                a.data.m_11 * b.data.m_11 + a.data.m_12 * b.data.m_21 + a.data.m_13 * b.data.m_31,
                a.data.m_11 * b.data.m_12 + a.data.m_12 * b.data.m_22 + a.data.m_13 * b.data.m_32,
                a.data.m_11 * b.data.m_13 + a.data.m_12 * b.data.m_23 + a.data.m_13 * b.data.m_33,
                a.data.m_21 * b.data.m_11 + a.data.m_22 * b.data.m_21 + a.data.m_23 * b.data.m_31,
                a.data.m_21 * b.data.m_12 + a.data.m_22 * b.data.m_22 + a.data.m_23 * b.data.m_32,
                a.data.m_21 * b.data.m_13 + a.data.m_22 * b.data.m_23 + a.data.m_23 * b.data.m_33,
                a.data.m_31 * b.data.m_11 + a.data.m_32 * b.data.m_21 + a.data.m_33 * b.data.m_31,
                a.data.m_31 * b.data.m_12 + a.data.m_32 * b.data.m_22 + a.data.m_33 * b.data.m_32,
                a.data.m_31 * b.data.m_13 + a.data.m_32 * b.data.m_23 + a.data.m_33 * b.data.m_33);

        public static Matrix3 operator -(Matrix3 a) => Negate(a);
        public static Matrix3 operator *(double f, Matrix3 a) => Scale(f, a);
        public static Matrix3 operator *(Matrix3 a, double f) => Scale(f, a);
        public static Matrix3 operator /(Matrix3 a, double d) => Scale(1 / d, a);
        public static Matrix3 operator +(Matrix3 a, Matrix3 b) => Add(a, b);
        public static Matrix3 operator -(Matrix3 a, Matrix3 b) => Subtract(a, b);
        public static Matrix3 operator ~(Matrix3 a) => a.Transpose();

        public static Vector3 operator *(Matrix3 a, Vector3 b) => Product(a, b);
        public static Vector3 operator *(Vector3 a, Matrix3 b) => Product(a, b);
        public static Matrix3 operator *(Matrix3 a, Matrix3 b) => Product(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Solve(Vector3 b) 
            => Inverse()*b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3 Solve(Matrix3 B)
            => Inverse()*B;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3 Inverse()
            => new Matrix3(
                data.m_22*data.m_33 - data.m_23*data.m_32, data.m_13*data.m_32 - data.m_12*data.m_33, data.m_12*data.m_23 - data.m_13*data.m_22,
                data.m_23*data.m_31 - data.m_21*data.m_33, data.m_11*data.m_33 - data.m_13*data.m_31, data.m_13*data.m_21 - data.m_11*data.m_23,
                data.m_21*data.m_32 - data.m_22*data.m_31, data.m_12*data.m_31 - data.m_11*data.m_32, data.m_11*data.m_22 - data.m_12*data.m_21)
            /Determinant;

        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            string a11 = data.m_11.ToString(formatting, provider);
            string a12 = data.m_12.ToString(formatting, provider);
            string a13 = data.m_13.ToString(formatting, provider);
            string a21 = data.m_21.ToString(formatting, provider);
            string a22 = data.m_22.ToString(formatting, provider);
            string a23 = data.m_23.ToString(formatting, provider);
            string a31 = data.m_31.ToString(formatting, provider);
            string a32 = data.m_32.ToString(formatting, provider);
            string a33 = data.m_33.ToString(formatting, provider);
            return $"[{a11} {a12} {a13}; {a21} {a22} {a23}; {a13} {a32} {a33}]";
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
            return obj is Matrix3 matrix && Equals(matrix);
        }

        /// <summary>
        /// Checks for equality among <see cref="Matrix2"/> classes
        /// </summary>
        /// <returns>True if equal</returns>
        public bool Equals(Matrix3 other)
        {
            return data.Equals(other.data);
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="Matrix3"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return -1945990370 + data.GetHashCode();
            }
        }
        public static bool operator ==(Matrix3 target, Matrix2 other) { return target.Equals(other); }
        public static bool operator !=(Matrix3 target, Matrix2 other) { return !target.Equals(other); }
        #endregion

        #region Collection        
        /// <summary>
        /// Gets a value indicating whether this array is of fixed size.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Get the number of elements in the matrix.
        /// </summary>
        public int Count => 9;

        /// <summary>
        /// Get the elements as a span
        /// </summary>
        public unsafe ReadOnlySpan<double> AsSpan()
        {
            fixed (double* ptr = &data.m_11)
            {
                return new ReadOnlySpan<double>(ptr, 9);
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
            { data.m_11, data.m_12, data.m_13 },
            { data.m_21, data.m_22, data.m_23 },
            { data.m_31, data.m_32, data.m_33 } };
        /// <summary>
        /// Get the elemetns as a jagged array
        /// </summary>
        public double[][] ToJaggedArray() => [
            [data.m_11, data.m_12, data.m_13],
            [data.m_21, data.m_22, data.m_23],
            [data.m_31, data.m_32, data.m_33] ];

        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);
        public void CopyTo(double[] array, int index) => ToArray().CopyTo(array, index);
        object System.Collections.ICollection.SyncRoot { get => null; }
        bool System.Collections.ICollection.IsSynchronized { get => false; }
        public System.Collections.IEnumerator GetEnumerator() => AsSpan().ToArray().GetEnumerator();
        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            yield return data.m_11;
            yield return data.m_12;
            yield return data.m_13;
            yield return data.m_21;
            yield return data.m_22;
            yield return data.m_23;
            yield return data.m_31;
            yield return data.m_32;
            yield return data.m_33;
        }
        void ICollection<double>.Add(double item) => throw new NotSupportedException();
        void ICollection<double>.Clear() => throw new NotSupportedException();
        bool ICollection<double>.Remove(double item) => throw new NotSupportedException();
        bool ICollection<double>.Contains(double item) => throw new NotSupportedException();
        #endregion

    }
}
