using System;
using System.Text;
using System.Linq;

namespace JA.Numerics
{    

    /// <summary> Matrix class.</summary>
    public readonly struct Matrix : 
        IEquatable<Matrix>,
        IFormattable
    {
        /// <summary>Array for internal storage of elements. First index is row, second index is column</summary>
        readonly double[][] _data;

        /// <summary>Row and column dimensions.</summary>
        readonly int _rows, _cols;

        #region Constructors

        /// <summary>Construct an m-by-n matrix of zeros. </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        public Matrix(int m, int n)
        {
            this._rows = m;
            this._cols = n;
            _data = new double[m][];
            for (int i = 0; i < m; i++)
            {
                _data[i] = new double[n];
            }
        }

        /// <summary>Construct an m-by-n constant matrix.</summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="s">Fill the matrix with this scalar value.</param>		
        public Matrix(int m, int n, double s)
        {
            this._rows = m;
            this._cols = n;
            _data = new double[m][];
            for (int i = 0; i < m; i++)
            {
                _data[i] = new double[n];
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    _data[i][j] = s;
                }
            }
        }

        /// <summary>Construct a matrix from a 2-D jagged array.</summary>
        /// <param name="A">Two-dimensional array of doubles.</param>
        /// <exception cref="IllegalArgumentException">All rows must have the same length</exception>
        /// <remarks>Array is not copied</remarks>
        public Matrix(double[][] A)
        {
            if (A == null)
                throw new ArgumentNullException(nameof(A));
            _rows = A.Length;
            _cols = A[0].Length;
            for (int i = 0; i < _rows; i++)
            {
                if (A[i].Length != _cols)
                {
                    throw new ArgumentException("All rows must have the same length.");
                }
            }
            _data = A;
        }

        /// <summary>Construct a matrix from a copy of a 2-D array.</summary>
        /// <param name="m">Two-dimensional array of doubles. First index is row, second is column</param>
        /// </seealso>
        public Matrix(double[,] arr)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));
            _rows = arr.GetLength(0);
            _cols = arr.GetLength(1);
            _data = new double[_rows][];
            for (int i = 0; i < _rows; i++)
            {
                _data[i] = new double[_cols];
                for (int j = 0; j < _cols; j++)
                {
                    _data[i][j] = arr[i, j];
                }
            }
        }

        /// <summary>Construct a matrix quickly without checking arguments.</summary>
        /// <param name="A">Two-dimensional array of doubles.</param>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>		
        public Matrix(double[][] A, int m, int n)
        {
            if (A == null)
                throw new ArgumentNullException(nameof(A));
            this._data = A;
            this._rows = m;
            this._cols = n;
        }

        public static Matrix Empty { get; } = new Matrix(new double[0][], 0, 0);
        #endregion

        #region Getters, setters and accessors

        public bool IsEmpty { get => _data.Length==0 || _rows==0 || _cols==0; }

        public double[][] AsArray() => _data;

        /// <summary>Get row dimension.</summary>
        /// <returns>The number of rows.</returns>
        public int RowDimension
        {
            get
            {
                return _rows;
            }
        }

        /// <summary>Get column dimension.</summary>
        /// <returns>The number of columns.</returns>
        public int ColumnDimension
        {
            get
            {
                return _cols;
            }
        }

        /// <summary>Explicit conversion to CLR 2-D array with copy</summary>
        /// <param name="a">Matrix to convert</param>
        /// <returns>2D array</returns>
        public static explicit operator double[,](Matrix a)
        {
            if (a._data == null)
                throw new ArgumentNullException(nameof(a));
            double[,] X = new double[a._rows, a._cols];
            for (int i = 0; i < a._rows; i++)
            {
                for (int j = 0; j < a._cols; j++)
                {
                    X[i, j] = a._data[i][j];
                }
            }
            return X;
        }


        /// <summary>Explicit conversion to CLR jagged array without making copy</summary>
        /// <param name="a">Matrix to convert</param>
        /// <returns>Jagged array</returns>
        public static explicit operator double[][](Matrix a)
        {
            if (a._data == null)
                throw new ArgumentNullException(nameof(a));
            return a._data;
        }

        /// <summary>Make a deep copy of a matrix</summary>		
        public Matrix Clone()
        {
            Matrix X = new Matrix(_rows, _cols);
            double[][] C = X._data;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    C[i][j] = _data[i][j];
                }
            }
            return X;
        }

        /// <summary>
        /// Gets column with number <paramref name="columnNum"/> from matrix res
        /// </summary>
        /// <param name="columnNum">Column number (zero based)</param>
        /// <returns>Vector containing copy of column's elements</returns>
        public Vector CloneColumn(int columnNum)
        {
            if (0 > columnNum || columnNum > ColumnDimension - 1)
                throw new IndexOutOfRangeException("Column index is out of range");
            Vector v = Vector.Zeros(RowDimension);
            for (int i = 0; i < RowDimension; i++)
            {
                v[i] = _data[i][columnNum];
            }
            return v;
        }

        /// <summary>Matrix elements accessors.</summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        /// <returns>A(i,j)</returns>
        public double this[int i, int j]
        {
            get { return _data[i][j]; }
            set { _data[i][j] = value; }
        }

        /// <summary>Access the Column</summary>
        /// <param name="i">Row index</param>
        /// <returns>A(i)</returns>
        public double[] this[int i]
        {
            get { return _data[i]; }
            set { _data[i] = value; }
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="i0">Initial row index </param>
        /// <param name="i1">Final row index </param>
        /// <param name="j0">Initial column index </param>
        /// <param name="j1">Final column index </param>
        /// <returns>A(i0:i1,j0:j1) </returns>
        /// <exception cref="ArrayIndexOutOfBoundsException">Submatrix indices </exception>	
        public Matrix Submatrix(int i0, int i1, int j0, int j1)
        {
            Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
            double[][] B = X._data;
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i - i0][j - j0] = _data[i][j];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="r">Array of row indexes.</param>
        /// <param name="c">Array of column indexes.</param>
        /// <returns>A(r(:),c(:)) </returns>
        /// <exception cref="ArrayIndexOutOfBoundsException">Submatrix indexes</exception>	
        public Matrix Submatrix(int[] r, int[] c)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            if (c == null)
                throw new ArgumentNullException("c");
            Matrix X = new Matrix(r.Length, c.Length);
            double[][] B = X._data;
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i][j] = _data[r[i]][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indexes", e);
            }
            return X;
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="i0">Initial row index</param>
        /// <param name="i1">Final row index</param>
        /// <param name="c">Array of column indexes.</param>
        /// <returns>A(i0:i1,c(:))</returns>
        /// <exception cref="ArrayIndexOutOfBoundsException">Submatrix indexes</exception>	
        public Matrix Submatrix(int i0, int i1, int[] c)
        {
            if (c == null)
                throw new ArgumentNullException("c");
            Matrix X = new Matrix(i1 - i0 + 1, c.Length);
            double[][] B = X._data;
            try
            {
                for (int i = i0; i <= i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        B[i - i0][j] = _data[i][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indexes", e);
            }
            return X;
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="r">   Array of row indexes.</param>
        /// <param name="i0">  Initial column index.</param>
        /// <param name="i1">  Final column index.</param>
        /// <returns>     A(r(:),j0:j1)</returns>
        /// <exception cref="ArrayIndexOutOfBoundsException">Submatrix indexes</exception>		
        public Matrix Submatrix(int[] r, int j0, int j1)
        {
            if (r == null)
                throw new ArgumentNullException("r");
            Matrix X = new Matrix(r.Length, j1 - j0 + 1);
            double[][] B = X._data;
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        B[i][j - j0] = _data[r[i]][j];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }


        #endregion

        #region Arithmetic operators

        public static Matrix operator +(Matrix A, Matrix B)
        {
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            if (B._data == null)
                throw new ArgumentNullException(nameof(B));
            A.CheckMatrixDimensions(B);
            Matrix X = new Matrix(A._rows, A._cols);
            double[][] C = X._data;
            for (int i = 0; i < A._rows; i++)
            {
                for (int j = 0; j < A._cols; j++)
                {
                    C[i][j] = A._data[i][j] + B._data[i][j];
                }
            }
            return X;
        }

        public static Matrix operator -(Matrix A, Matrix B)
        {
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            if (B._data == null)
                throw new ArgumentNullException(nameof(B));
            A.CheckMatrixDimensions(B);
            Matrix X = new Matrix(A._rows, A._cols);
            double[][] C = X._data;
            for (int i = 0; i < A._rows; i++)
            {
                for (int j = 0; j < A._cols; j++)
                {
                    C[i][j] = A._data[i][j] - B._data[i][j];
                }
            }
            return X;
        }

        /// <summary>Multiply a matrix by a scalar, C = s*A</summary>
        /// <param name="s">   scalar</param>
        /// <returns>     s*A</returns>
        public Matrix times(double s)
        {
            Matrix X = new Matrix(_rows, _cols);
            double[][] C = X._data;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    C[i][j] = s * _data[i][j];
                }
            }
            return X;
        }

        public static Matrix operator *(Matrix A, double s)
        {
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            Matrix X = new Matrix(A._rows, A._cols);
            double[][] C = X._data;
            for (int i = 0; i < A._rows; i++)
            {
                for (int j = 0; j < A._cols; j++)
                {
                    C[i][j] = s * A._data[i][j];
                }
            }
            return X;
        }

        public static Matrix operator *(double s, Matrix A)
        {
            return A * s;
        }

        /// <summary>Multiply a matrix by a scalar in place, A = s*A</summary>
        /// <param name="s">   scalar</param>
        /// <returns>     replace A by s*A</returns>
        public Matrix Mul(double s)
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                {
                    _data[i][j] = s * _data[i][j];
                }
            }
            return this;
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            if (B._data == null)
                throw new ArgumentNullException(nameof(B));
            if (B._rows != A._cols)
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            Matrix X = new Matrix(A._rows, B._cols);
            double[][] C = X._data;
            double[] Bcolj = new double[A._cols];
            for (int j = 0; j < B._cols; j++)
            {
                for (int k = 0; k < A._cols; k++)
                {
                    Bcolj[k] = B._data[k][j];
                }
                for (int i = 0; i < A._rows; i++)
                {
                    double[] Arowi = A._data[i];
                    double s = 0;
                    for (int k = 0; k < A._cols; k++)
                    {
                        s += Arowi[k] * Bcolj[k];
                    }
                    C[i][j] = s;
                }
            }
            return X;
        }
        
        /// <summary>Multiplies vector <see cref="Vector"/> by matrix</summary>
        /// <param name="v">Vector</param>
        /// <param name="A">Matrix</param>
        /// <returns>Product of <see cref="Vector"/> and matrix</returns>
        public static Vector operator *(Matrix A, Vector v)
        {
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            if (v.Length != A.ColumnDimension)
                throw new ArgumentException("Dimensions of vector and matrix do not match");

            double[] av = v;
            int rowDimension = A.RowDimension;
            int columnDimension = A.ColumnDimension;

            double[] result = new double[rowDimension];

            for (int i = 0; i < rowDimension; i++)
            {
                var acc = 0.0;
                var column = A[i];

                for (int j = 0; j < columnDimension; j++)
                {
                    acc += column[j] * av[j];
                }
                result[i] = acc;
            }
            return new Vector(result);
        }

        /// <summary>Implements multiplication of matrix by vector</summary>
        /// <param name="A">Matrix</param>
        /// <param name="v">Vector</param>
        /// <returns>Result of multiplication</returns>
        public static Vector operator *(Vector v, Matrix A)
        {
            double[] av = v;
            if (A._data == null)
                throw new ArgumentNullException(nameof(A));
            if (v.Length != A.RowDimension)
                throw new ArgumentException("Dimensions of matrix and vector do not match");
            double[] result = new double[A.ColumnDimension];
            for (int i = 0; i < A.RowDimension; i++)
            {

                for (int j = 0; j < A.ColumnDimension; j++)
                {
                    result[j] = result[j] + av[i] * A[i, j];
                }
            }
            return new Vector(result);
        }

        #endregion

        #region Matrix operations

        /// <summary>Solve A*x = b using Gaussian elimination with partial pivoting</summary>
        /// <param name="b">    right hand side Vector</param>
        /// <returns>    The solution x = A^(-1) * b as a Vector</returns>
        public Vector SolveGE(Vector b)
        {
            return Gauss.Solve(this, b);
        }


        /// <summary>Generate identity matrix</summary>
        /// <param name="m">   Number of rows.</param>
        /// <param name="n">   Number of columns.</param>
        /// <returns>     An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
        public static Matrix Identity(int m, int n)
        {
            Matrix A = new Matrix(m, n);
            double[][] X = A._data;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    X[i][j] = i == j ? 1.0 : 0.0;
                }
            }
            return A;
        }


        /// <summary>Transpose of a dense matrix</summary>
        /// <returns>  An n-by-m matrix where the input matrix has m rows and n columns.</returns>
        public Matrix Transpose()
        {
            Matrix result = new Matrix(_cols, _rows);
            double[][] T = result._data;
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _cols; j++)
                    T[j][i] = _data[i][j];
            return result;
        }

        /// <summary>Cholesky factorization</summary>
        /// <returns>Lower-triangular Cholesky factor for a symmetric positive-definite matrix</returns>
        public Matrix Cholesky()
        {
            Matrix result = new Matrix(_rows, _rows);
            var Li = result._data;

            // Main loop
            for (int i = 0; i < _cols; i++)
            {
                var Lrowi = Li[i];
                for (int j = 0; j < i + 1; j++)
                {
                    var Lrowj = Li[j];
                    double s = 0;
                    for (int k = 0; k < j; k++)
                        s += Lrowi[k] * Lrowj[k];
                    if (i == j)
                        Lrowi[j] = Math.Sqrt(_data[i][i] - s);
                    else
                        Lrowi[j] = (_data[i][j] - s) / Lrowj[j];
                }
            }

            return result;
        }

        /// <summary>Matrix inverse for a lower triangular matrix</summary>
        /// <param name="L"></param>
        /// <returns></returns>
        public Matrix InverseLower()
        {
            int n = ColumnDimension;
            var I = Identity(n, n);
            var invLtr = new double[n][];
            for (int col = 0; col < n; col++)
            {
                Vector x = Vector.Zeros(n);
                x[col] = 1;
                invLtr[col] = SolveLower(x);
            }
            var invL = new Matrix(invLtr).Transpose();

            return invL;
        }

        public Vector SolveLower(Vector b)
        {
            double[] x = new double[_rows];

            for (int i = 0; i < _rows; i++)
            {
                x[i] = b[i];
                for (int j = 0; j < i; j++)
                    x[i] -= _data[i][j] * x[j];
                x[i] /= _data[i][i];
            }

            return new Vector(x);
        }

        public Vector SolveUpper(Vector b)
        {
            double[] x = new double[_rows];

            for (int i = _rows - 1; i >= 0; i--)
            {
                x[i] = b[i];
                for (int j = i + 1; j < _cols; j++)
                    x[i] -= _data[i][j] * x[j];
                x[i] /= _data[i][i];
            }

            return new Vector(x);
        }

        /// <summary>Check if size(A) == size(B) and throws exception if not</summary>	
        private void CheckMatrixDimensions(Matrix B)
        {
            if (B._rows != _rows || B._cols != _cols)
            {
                throw new ArgumentException("Matrix dimensions must agree.");
            }
        }

        #endregion


        #region IEquatable Members

        /// <summary>
        /// Equality overrides from <see cref="System.Object"/>
        /// </summary>
        /// <param name="obj">The object to compare this with</param>
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(Matrix)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix item)
            {
                return Equals(item);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="Matrix"/> classes
        /// </summary>
        /// <returns>True if equal</returns>
        public bool Equals(Matrix other)
        {
            return _rows==other._rows 
                && _cols==other._cols
                && _data.Zip(other._data, (a, b) => a.SequenceEqual(b)).All(x => x);
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="Matrix"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = -1817952719;
                for (int i = 0; i < _data.Length; i++)
                {
                    double[] row = _data[i];
                    for (int j = 0; j < row.Length; j++)
                    {
                        hc = (-1521134295)*hc + row[j].GetHashCode();

                    }
                }
                return hc;
            }
        }
        public static bool operator ==(Matrix target, Matrix other) { return target.Equals(other); }
        public static bool operator !=(Matrix target, Matrix other) { return !target.Equals(other); }

        #endregion


        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _data.Length; i++)
            {
                sb.Append("| ");
                double[] row = _data[i];
                for (int j = 0; j < row.Length; j++)
                {
                    if (j>0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat(provider, formatting, row[j]);
                }
                sb.AppendLine(" |");
            }
            return sb.ToString();
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g"); 
        #endregion

    }
}