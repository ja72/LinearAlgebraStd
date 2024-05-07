using System;
using System.Linq;

namespace JA.Numerics
{
    public struct SparseVector : 
        IEquatable<SparseVector>,
        IFormattable
    {
        const int IncrementSize = 16; // Size of chunk for array increments

        readonly int _size;

        /// <summary>The nonzero elements of the sparse vector</summary>
        public readonly double[] _items;
        /// <summary>The indexes of the nonzero elements in the sparse vector</summary>
        public readonly int[] _indexes;
        /// <summary>Number of initialized elements</summary>
        public readonly int _count;

        /// <summary>Constructs a sparse vector with all zeros</summary>
        /// <param name="size">Length of the vector</param>
        public SparseVector(int size)
        {
            this._size = size;
            this._items = new double[IncrementSize];
            this._indexes = new int[IncrementSize];
            this._count = 0;
        }

        /// <summary>Constructs a sparse vector with defined nonzero elements</summary>
        /// <param name="size">Length of the vector</param>
        /// <param name="items">The nonzero entries</param>
        /// <param name="indexes">The locations of the non zeros</param>
        public SparseVector(int size, double[] items, int[] indexes, int count = -1)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (indexes == null)
                throw new ArgumentNullException(nameof(indexes));
            this._items = items;
            this._indexes = indexes;
            this._count = count>=0 ? count : items.Length;
            this._size = size;
        }

        public static SparseVector Empty { get; } = new SparseVector(0);

        public bool IsEmpty { get => _size ==0 || _items.Length==0; }

        /// <summary>Length of the sparse vector</summary>
        public int Length
        {
            get { return _size; }
        }

        public SparseVector Clone()
        {
            return _size == 0 ? Empty : new SparseVector(_size, (double[])_items.Clone(), (int[])_indexes.Clone());
        }

        public Vector AsVector()
        {
            double[] values = new double[_size];
            for (int i = 0; i < _items.Length; i++)
            {
                values[_indexes[i]] = _items[i];
            }
            return new Vector(values);
        }

        /// <summary>Public accessors method</summary>
        /// <param name="index">Index of the request</param>
        /// <returns>The i-th element of a sparse vector</returns>
        public double this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                int idx = Array.BinarySearch(_indexes, 0, _count, index);
                if (idx < 0)
                    return 0;
                else
                    return _items[idx];
            }
            set
            {
                if (index < 0 || index >= _size)
                    throw new IndexOutOfRangeException();
                int idx = Array.BinarySearch(_indexes, 0, _count, index);
                if (idx >= 0)
                {
                    _items[idx] = value;
                }
                else
                {
                    int indexToAdd = ~idx;
                    int[] newIndexes = _indexes;
                    double[] newItems = _items;
                    int count = _count;
                    if (_count >= _items.Length)
                    {
                        int delta = Math.Min(IncrementSize, _size - _items.Length);
                        newIndexes = new int[_indexes.Length + delta];
                        newItems = new double[_items.Length + delta];
                        Array.Copy(_indexes, newIndexes, _indexes.Length);
                        Array.Copy(_items, newItems, _items.Length);

                    }
                    Array.Copy(newIndexes, indexToAdd, newIndexes, indexToAdd + 1, count - indexToAdd);
                    Array.Copy(newItems, indexToAdd, newItems, indexToAdd + 1, count - indexToAdd);
                    count++;
                    newIndexes[indexToAdd] = index;
                    newItems[indexToAdd] = value;
                    this = new SparseVector(_size, newItems, newIndexes, count);
                }
            }
        }

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            return AsVector().ToString(formatting, provider);
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
        /// <returns>False if object is a different type, otherwise it calls <code>Equals(SparseVector)</code></returns>
        public override bool Equals(object obj)
        {
            if (obj is SparseVector item)
            {
                return Equals(item);
            }
            return false;
        }

        /// <summary>
        /// Checks for equality among <see cref="SparseVector"/> classes
        /// </summary>
        /// <returns>True if equal</returns>
        public bool Equals(SparseVector other)
        {
            return _items.SequenceEqual(other._items)
                && _indexes.SequenceEqual(other._indexes);
        }
        /// <summary>
        /// Calculates the hash code for the <see cref="SparseVector"/>
        /// </summary>
        /// <returns>The int hash value</returns>
        public override int GetHashCode()
        {
            unchecked
            {                
                int hc = -1817952719;
                hc = _items.Aggregate(hc, (h, x) => (-1521134295)*hc + x.GetHashCode());
                hc = _indexes.Aggregate(hc, (h, x) => (-1521134295)*hc + x.GetHashCode());
                return hc;
            }
        }
        public static bool operator ==(SparseVector target, SparseVector other) { return target.Equals(other); }
        public static bool operator !=(SparseVector target, SparseVector other) { return !target.Equals(other); }

        #endregion


    }
}
