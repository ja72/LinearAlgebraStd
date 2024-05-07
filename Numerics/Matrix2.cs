using System;
using System.Collections.Generic;

namespace JA.Numerics
{
    public readonly struct Matrix2 :
        IEquatable<Matrix2>,
        System.Collections.ICollection,
        IReadOnlyList<double>,
        IFormattable
    {
        readonly (double m_11, double m_12, double m_21, double m_22) _data;

        public Matrix2(double m_11, double m_12, double m_21, double m_22) : this()
        {
            this._data = (m_11, m_12, m_21, m_22);
        }
        public static readonly Matrix2 Zero = new Matrix2(0, 0, 0, 0);
        public static readonly Matrix2 Identity = new Matrix2(1, 0, 0, 1);
        public static implicit operator Matrix2(double value) => FromScalar(value);

        public static Matrix2 FromScalar(double d) => Diagonal(d, d);
        public static Matrix2 Diagonal(Vector2 v) => Diagonal(v.X, v.Y);
        public static Matrix2 Diagonal(double d1, double d2) => new Matrix2(d1, 0, 0, d2);

        public static Matrix2 FromRows(Vector2 row1, Vector2 row2)
            => new Matrix2(row1.X, row1.Y, row2.X, row2.Y);
        public static Matrix2 FromColumns(Vector2 column1, Vector2 column2)
            => new Matrix2(column1.X, column2.X, column1.Y, column2.Y);

        #region Properties

        public double A11 { get => _data.m_11; }
        public double A12 { get => _data.m_12; }
        public double A21 { get => _data.m_21; }
        public double A22 { get => _data.m_22; }

        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 1) throw new ArgumentNullException(nameof(row));
                if (column < 0 || column > 1) throw new ArgumentNullException(nameof(column));
                if (column == 0 && row == 0)
                {
                    return _data.m_11;
                }
                else if (column == 1 && row == 0)
                {
                    return _data.m_12;
                }
                else if (column == 0 && row == 1)
                {
                    return _data.m_21;
                }
                else if (column == 1 && row == 1)
                {
                    return _data.m_22;
                }
                throw new InvalidOperationException();
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
            => _data.m_11 == 0 && _data.m_12 == 0
            && _data.m_21 == 0 && _data.m_22 == 0;
        /// <summary>
        /// Check if the matrix is symmetric
        /// </summary>
        public bool IsSymmetric => _data.m_12 == _data.m_21;
        /// <summary>
        /// Check if the matrix is skew-symmetric
        /// </summary>
        public bool IsSkewSymmetric
            => _data.m_12 == -_data.m_21
            && _data.m_11 == 0 && _data.m_22 == 0;
        /// <summary>
        /// Check if the matrix is diagonal
        /// </summary>
        public bool IsDiagonal
            => _data.m_12 == 0 && _data.m_21 == 0;
        /// <summary>
        /// Check if the matrix is scalar (diagonal & all elements the same).
        /// </summary>
        public bool IsScalar
            => IsDiagonal && _data.m_11 == _data.m_22;

        public double Determinant { get => _data.m_11 * _data.m_22 - _data.m_12 * _data.m_21; }
        public double Trace { get => _data.m_11 + _data.m_22; }

        public Vector2 Row1 => new Vector2(_data.m_11, _data.m_12);
        public Vector2 Row2 => new Vector2(_data.m_21, _data.m_22);
        public Vector2 Column1 => new Vector2(_data.m_11, _data.m_21);
        public Vector2 Column2 => new Vector2(_data.m_12, _data.m_22);

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _data.m_11;
                    case 1: return _data.m_12;
                    case 2: return _data.m_21;
                    case 3: return _data.m_22;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Vector2 GetDiagonal() => new Vector2(_data.m_11, _data.m_22);

        public Matrix2 ToSymmetricMatrix() => (this + Transpose()) / 2;
        public Matrix2 ToSkewSymmetricMatrix() => (this - Transpose()) / 2;
        public Matrix2 ToDiagonalMatrix() => Diagonal(_data.m_11, _data.m_22);

        #endregion

        #region Algebra

        public static Matrix2 Add(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a._data.m_11 + b._data.m_11,
                a._data.m_12 + b._data.m_12,
                a._data.m_21 + b._data.m_21,
                a._data.m_22 + b._data.m_22);

        public static Matrix2 Subtract(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a._data.m_11 - b._data.m_11,
                a._data.m_12 - b._data.m_12,
                a._data.m_21 - b._data.m_21,
                a._data.m_22 - b._data.m_22);

        public static Matrix2 Negate(Matrix2 a)
            => new Matrix2(
                -a._data.m_11,
                -a._data.m_12,
                -a._data.m_21,
                -a._data.m_22);

        public static Matrix2 Scale(double f, Matrix2 a)
            => new Matrix2(
                f * a._data.m_11,
                f * a._data.m_12,
                f * a._data.m_21,
                f * a._data.m_22);

        public Matrix2 Transpose() => new Matrix2(_data.m_11, _data.m_21, _data.m_12, _data.m_22);

        public static Vector2 Product(Matrix2 a, Vector2 v)
            => new Vector2(
                a._data.m_11 * v.X + a._data.m_12 * v.Y,
                a._data.m_21 * v.X + a._data.m_22 * v.Y);
        public static Vector2 Product(Vector2 v, Matrix2 a)
            => new Vector2(
                a._data.m_11 * v.X + a._data.m_21 * v.Y,
                a._data.m_12 * v.X + a._data.m_22 * v.Y);

        public static Matrix2 Product(Matrix2 a, Matrix2 b)
            => new Matrix2(
                a._data.m_11 * b._data.m_11 + a._data.m_12 * b._data.m_21,
                a._data.m_11 * b._data.m_12 + a._data.m_12 * b._data.m_22,
                a._data.m_21 * b._data.m_11 + a._data.m_22 * b._data.m_21,
                a._data.m_21 * b._data.m_12 + a._data.m_22 * b._data.m_22);

        public static Matrix2 operator -(Matrix2 a) => Negate(a);
        public static Matrix2 operator *(double f, Matrix2 a) => Scale(f, a);
        public static Matrix2 operator *(Matrix2 a, double f) => Scale(f, a);
        public static Matrix2 operator /(Matrix2 a, double d) => Scale(1 / d, a);
        public static Matrix2 operator +(Matrix2 a, Matrix2 b) => Add(a, b);
        public static Matrix2 operator -(Matrix2 a, Matrix2 b) => Subtract(a, b);
        public static Matrix2 operator ~(Matrix2 a) => a.Transpose();

        public static Vector2 operator *(Matrix2 a, Vector2 v) => Product(a, v);
        public static Vector2 operator *(Vector2 v, Matrix2 a) => Product(v, a);
        public static Matrix2 operator *(Matrix2 a, Matrix2 b) => Product(a, b);

        public Vector2 Solve(Vector2 v)
        {
            var d = Determinant;
            return new Vector2(
                (_data.m_22 * v.X - _data.m_12 * v.Y) / d,
                (_data.m_11 * v.Y - _data.m_21 * v.X) / d);
        }
        public Matrix2 Solve(Matrix2 m)
            => FromColumns(
                Solve(m.Column1),
                Solve(m.Column2));

        public Matrix2 Inverse()
            => Solve(Identity);

        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            string a11 = _data.m_11.ToString(formatting, provider);
            string a12 = _data.m_12.ToString(formatting, provider);
            string a21 = _data.m_21.ToString(formatting, provider);
            string a22 = _data.m_22.ToString(formatting, provider);
            return $"[{a11} {a12}; {a21} {a22}]";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion

        #region IEquatable Members

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
            return _data.Equals(other._data);
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="Matrix2"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return -1817952719 + _data.GetHashCode();
            }
        }
        public static bool operator ==(Matrix2 target, Matrix2 other) { return target.Equals(other); }
        public static bool operator !=(Matrix2 target, Matrix2 other) { return !target.Equals(other); }

        #endregion

        #region Collection
        /// <summary>
        /// Get the number of elements in the matrix.
        /// </summary>
        public int Count => 4;

        /// <summary>
        /// Get the elements as a span
        /// </summary>
        public unsafe ReadOnlySpan<double> AsSpan()
        {
            fixed (double* ptr = &_data.m_11)
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
        public double[,] ToArray2() => new[,] { { _data.m_11, _data.m_12 }, { _data.m_21, _data.m_22 } };
        /// <summary>
        /// Get the elemetns as a jagged array
        /// </summary>
        public double[][] ToJaggedArray() => new[] { new[] { _data.m_11, _data.m_12 }, new[] { _data.m_21, _data.m_22 } };

        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);

        object System.Collections.ICollection.SyncRoot { get => null; }
        bool System.Collections.ICollection.IsSynchronized { get => false; }
        public System.Collections.IEnumerator GetEnumerator() => AsSpan().ToArray().GetEnumerator();
        System.Collections.Generic.IEnumerator<double> System.Collections.Generic.IEnumerable<double>.GetEnumerator()
        {
            yield return _data.m_11;
            yield return _data.m_12;
            yield return _data.m_21;
            yield return _data.m_22;
        }

        #endregion

    }
}
