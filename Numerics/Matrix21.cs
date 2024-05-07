using System;
using System.Collections.Generic;

namespace JA.Numerics
{
    public readonly struct Matrix21 : 
        IEquatable<Matrix21>,
        System.Collections.ICollection,
        IReadOnlyList<double>,
        IFormattable
    {
        readonly (Matrix2 matrix, Vector2 upper, Vector2 lower, double scalar) _data;

        public Matrix21(Matrix2 matrix, Vector2 upper, Vector2 lower, double scalar) 
            : this()
        {
            _data = (matrix, upper, lower, scalar);
        }
        public Matrix21(double m11, double m12, double ux, double m21, double m22, double uy, double lx, double ly, double s)
            : this()
        {
            _data = (new Matrix2(m11, m12, m21, m22), new Vector2(ux, uy), new Vector2(lx, ly), s);
        }
        public static implicit operator Matrix21(double scalar) => FromScalar(scalar);

        public static Matrix21 FromScalar(double scalar) => Diagonal(scalar, scalar, scalar);
        public static Matrix21 Diagonal(Vector21 vector)
            => Diagonal(vector.Vector, vector.Scalar);
        public static Matrix21 Diagonal(Vector2 vector, double scalar)
            => Diagonal(vector.X, vector.Y, scalar);
        public static Matrix21 Diagonal(double d1, double d2, double d3)
            => new Matrix21(
                Matrix2.Diagonal(d1, d2),
                Vector2.Zero,
                Vector2.Zero,
                d3);

        public static Matrix21 FromRows(Vector21 row1, Vector21 row2, Vector21 row3)
            => new Matrix21(
                row1.Vector.X, row1.Vector.Y, row1.Scalar,
                row2.Vector.X, row2.Vector.Y, row2.Scalar,
                row3.Vector.X, row3.Vector.Y, row3.Scalar);
        public static Matrix21 FromColumns(Vector21 column1, Vector21 column2, Vector21 column3)
            => new Matrix21(
                column1.Vector.X, column2.Vector.Y, column3.Scalar,
                column1.Vector.X, column2.Vector.Y, column3.Scalar,
                column1.Vector.X, column2.Vector.Y, column3.Scalar);

        public static Matrix21 Zero { get; } = new Matrix21(Matrix2.Zero, Vector2.Zero, Vector2.Zero, 0);
        public static Matrix21 Identity { get; } = new Matrix21(Matrix2.Identity, Vector2.Zero, Vector2.Zero, 1);

        public Matrix2 Matrix { get => _data.matrix; }
        public Vector2 Upper { get => _data.upper; }
        public Vector2 Lower { get => _data.lower; }
        public double Scalar { get => _data.scalar; }

        /// <summary>
        /// Get the number of rows
        /// </summary>
        public int RowCount => 3;
        /// <summary>
        /// Get the number of columns
        /// </summary>
        public int ColumnCount => 3;

        public double Trace { get => _data.matrix.Trace + _data.scalar; }
        public double Determinant { get => (_data.matrix * _data.scalar - Vector2.Outer(_data.upper, _data.lower)).Determinant; }
        public bool IsSquare { get => true; }
        public bool IsDiagonal { get => _data.matrix.IsDiagonal && _data.upper.Equals(Vector2.Zero) && _data.lower.Equals(Vector2.Zero); }
        public bool IsScalar { get => _data.matrix.IsScalar && _data.matrix[0, 0] == _data.scalar && _data.upper.Equals(Vector2.Zero) && _data.lower.Equals(Vector2.Zero); }
        public bool IsSymmetric { get => _data.matrix.IsSymmetric && _data.upper.Equals(_data.lower); }
        public bool IsSkewSymmetric { get => _data.matrix.IsSkewSymmetric && (_data.upper + _data.lower).Equals(Vector2.Zero) && _data.scalar == 0f; }

        public Vector21 Row1 => new Vector21(_data.matrix.Row1, _data.upper.X);
        public Vector21 Row2 => new Vector21(_data.matrix.Row2, _data.upper.Y);
        public Vector21 Row3 => new Vector21(_data.lower, _data.scalar);
        public Vector21 Column1 => new Vector21(_data.matrix.Column1, _data.lower.X);
        public Vector21 Column2 => new Vector21(_data.matrix.Column2, _data.lower.Y);
        public Vector21 Column3 => new Vector21(_data.lower, _data.scalar);

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _data.matrix.A11;
                    case 1: return _data.matrix.A12;
                    case 2: return _data.upper.X;
                    case 3: return _data.matrix.A21;
                    case 4: return _data.matrix.A22;
                    case 5: return _data.upper.Y;
                    case 6: return _data.lower.X;
                    case 7: return _data.lower.Y;
                    case 8: return _data.scalar;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 2) throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 0 || column > 2) throw new ArgumentOutOfRangeException(nameof(column));
                if (row < 2 && column < 2)
                {
                    return _data.matrix[row, column];
                }
                else if (row == 0 && column == 2)
                {
                    return _data.upper.X;
                }
                else if (row == 1 && column == 2)
                {
                    return _data.upper.Y;
                }
                else if (row == 2 && column == 0)
                {
                    return _data.lower.X;
                }
                else if (row == 2 && column == 1)
                {
                    return _data.lower.Y;
                }
                else if (row == 2 && column == 2)
                {
                    return _data.scalar;
                }
                throw new InvalidOperationException();
            }
        }

        public Vector21 GetDiagonal() => new Vector21(_data.matrix.GetDiagonal(), _data.scalar);

        public Matrix21 ToSymmetric() => (this + Transpose()) / 2;
        public Matrix21 ToSkewSymmetric() => (this - Transpose()) / 2;
        public Matrix21 ToDiagonal() => Diagonal(_data.matrix.GetDiagonal(), _data.scalar);

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            var m11 = _data.matrix.A11.ToString(formatting, provider);
            var m12 = _data.matrix.A12.ToString(formatting, provider);
            var m21 = _data.matrix.A21.ToString(formatting, provider);
            var m22 = _data.matrix.A22.ToString(formatting, provider);
            var u1 = _data.upper.X.ToString(formatting, provider);
            var u2 = _data.upper.Y.ToString(formatting, provider);
            var l1 = _data.lower.X.ToString(formatting, provider);
            var l2 = _data.lower.Y.ToString(formatting, provider);
            var s = _data.scalar.ToString(formatting, provider);

            return $"[{m11} {m12} | {u1}; {m21} {m22} | {u2}; {l1} {l2} | {s}";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion

        #region Algebra
        public static Matrix21 Add(Matrix21 a, Matrix21 b)
            => new Matrix21(
                a.Matrix + b.Matrix,
                a.Upper + b.Upper,
                a.Lower + b.Lower,
                a.Scalar + b.Scalar);

        public static Matrix21 Subtract(Matrix21 a, Matrix21 b)
            => new Matrix21(
                a.Matrix - b.Matrix,
                a.Upper - b.Upper,
                a.Lower - b.Lower,
                a.Scalar - b.Scalar);

        public static Matrix21 Scale(double f, Matrix21 a)
            => new Matrix21(
                f * a.Matrix,
                f * a.Upper,
                f * a.Lower,
                f * a.Scalar);

        public static Vector21 Product(Matrix21 matrix, Vector21 vector)
            => new Vector21(
                matrix.Matrix * vector.Vector + matrix.Upper * vector.Scalar,
                Vector2.Dot(matrix.Lower, vector.Vector) + matrix.Scalar * vector.Scalar);

        public static Vector21 Product(Vector21 vector, Matrix21 matrix)
            => new Vector21(
                vector.Vector * matrix.Matrix + vector.Scalar * matrix.Lower,
                Vector2.Dot(vector.Vector, matrix.Upper) + vector.Scalar * matrix.Scalar);

        public static Matrix21 Product(Matrix21 a, Matrix21 b)
            => new Matrix21(
                a.Matrix * b.Matrix + Vector2.Outer(a.Upper, b.Lower), a.Matrix * b.Upper + a.Upper * b.Scalar,
                a.Lower * b.Matrix + a.Scalar * b.Lower, Vector2.Dot(a.Lower, b.Upper) + a.Scalar * b.Scalar);

        public Matrix21 Transpose() => new Matrix21(Matrix.Transpose(), Lower, Upper, Scalar);

        public Vector21 Solve(Vector21 rhs)
        {
            Matrix2 A = _data.matrix - Vector2.Outer(_data.upper, _data.lower) / _data.scalar;
            Vector2 b = rhs.Vector - (rhs.Scalar / _data.scalar) * _data.upper;
            Vector2 v = A.Solve(b);
            double w = (rhs.Scalar - Vector2.Dot(_data.lower, v)) / _data.scalar;
            return new Vector21(v, w);
        }
        public Matrix21 Solve(Matrix21 c)
        {
            Matrix2 Au = Matrix - Vector2.Outer(Upper, Lower) / Scalar;
            Vector2 bu = c.Upper - Upper * (c.Scalar / Scalar);
            Vector2 u = Au.Solve(bu);
            double s = (c.Scalar - Vector2.Dot(Lower, u)) / Scalar;
            Matrix2 Al = Matrix.Solve(c.Matrix);
            Vector2 bl = Matrix.Solve(Upper);
            Vector2 l = (c.Lower - Lower * Al) / (Scalar - Vector2.Dot(Lower, bl));
            Matrix2 M = Matrix.Solve(c.Matrix - Vector2.Outer(Upper, l));
            return new Matrix21(M, u, l, s);
        }
        public Matrix21 Inverse() => Solve(Identity);
        #endregion

        #region Operators
        public static Matrix21 operator +(Matrix21 lhs, Matrix21 rhs) { return Add(lhs, rhs); }
        public static Matrix21 operator -(Matrix21 lhs, Matrix21 rhs) { return Subtract(lhs, rhs); }
        public static Matrix21 operator -(Matrix21 rhs) { return Scale(-1f, rhs); }
        public static Matrix21 operator *(double f, Matrix21 rhs) { return Scale(f, rhs); }
        public static Matrix21 operator *(Matrix21 lhs, double d) { return Scale(d, lhs); }
        public static Vector21 operator *(Matrix21 lhs, Vector21 rhs) { return Product(lhs, rhs); }
        public static Matrix21 operator *(Matrix21 lhs, Matrix21 rhs) { return Product(lhs, rhs); }
        public static Vector21 operator *(Vector21 lhs, Matrix21 rhs) { return Product(rhs, lhs); }
        public static Matrix21 operator /(Matrix21 lhs, double d) { return Scale(1 / d, lhs); }
        public static Matrix21 operator ~(Matrix21 rhs) { return rhs.Transpose(); }
        public static Matrix21 operator !(Matrix21 rhs) { return rhs.Inverse(); }
        public static Vector21 operator /(Vector21 lhs, Matrix21 rhs)
        {
            return rhs.Solve(lhs);
        }
        public static Matrix21 operator /(Matrix21 lhs, Matrix21 rhs)
        {
            return lhs.Solve(rhs);
        }

        public static bool operator ==(Matrix21 matrix1, Matrix21 matrix2)
        {
            return matrix1.Equals(matrix2);
        }

        public static bool operator !=(Matrix21 matrix1, Matrix21 matrix2)
        {
            return !(matrix1 == matrix2);
        }

        #endregion

        #region Equality
        public override bool Equals(object obj)
        {
            if (obj is int i)
            {
                obj = (double)i;
            }
            if (obj is double f)
            {
                return IsScalar && _data.matrix.A11 == f;
            }
            return obj is Matrix21 matrix && Equals(matrix);
        }
        public bool Equals(Matrix21 other) => _data.Equals(other._data);

        public override int GetHashCode()
        {
            unchecked
            {
                return -1945990370 + _data.GetHashCode();
            }
        }


        #endregion

        #region Collections


        public void CopyTo(Array array, int index) => ToArray().CopyTo(array, index);

        public int Count { get => 9; }
        object System.Collections.ICollection.SyncRoot { get => null; }
        bool System.Collections.ICollection.IsSynchronized { get => false; }

        public double[] ToArray()
        {
            return new double[] {
                _data.matrix.A11,
                _data.matrix.A12,
                _data.upper.X,
                _data.matrix.A21,
                _data.matrix.A22,
                _data.upper.Y,
                _data.lower.X,
                _data.lower.Y,
                _data.scalar,
            };
        }

        public ReadOnlySpan<double> AsSpan()
        {
            return new ReadOnlySpan<double>(ToArray());
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }
        public System.Collections.Generic.IEnumerable<double> AsEnumerable() => this as System.Collections.Generic.IEnumerable<double>;
        System.Collections.Generic.IEnumerator<double> System.Collections.Generic.IEnumerable<double>.GetEnumerator()
        {
            return ((System.Collections.Generic.IEnumerable<double>)ToArray()).GetEnumerator();
        }

        #endregion

    }
}
