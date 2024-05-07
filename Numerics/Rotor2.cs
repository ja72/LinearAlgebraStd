using System;
using System.Drawing;
using System.Numerics;

namespace JA.Numerics
{
    public readonly struct Rotor2 : IEquatable<Rotor2>
    {
        readonly (double z, double w) _data;

        public Rotor2(double z, double w) : this()
        {
            _data = (z, w);
        }

        public static Rotor2 Zero { get; } = new Rotor2(0, 0);
        public static Rotor2 Identity { get; } = new Rotor2(0, 1);
        public static Rotor2 UnitZ { get; } = new Rotor2(1, 0);

        public static Rotor2 CreateRotation(double angle)
            => new Rotor2((double)Math.Sin(angle / 2), (double)Math.Cos(angle / 2));

        public double Z { get => _data.z; }
        public double W { get => _data.w; }

        public double Magnitude => SumSquares;
        public double SumSquares => _data.z * _data.z + _data.w * _data.w;

        public bool IsVector => _data.w == 0;

        //public double Angle => Math.PI - 2 * Math.Asin((float)_data.w);
        public double Angle => (float)_data.w != 1 ? 2*Math.Atan(_data.z/_data.w) : Math.Sin(_data.w)*Math.PI/2;
        public double Axis => IsVector ? _data.z : 1;

        public Matrix2 ToRotation()
        {
            double cos = 1-2*_data.z*_data.z;
            double sin = 2*_data.w*_data.z;
            return new Matrix2(cos, -sin, sin, cos);
        }

        public static Rotor2 Normalize(Rotor2 rotor)
        {
            if (!rotor.IsVector)
            {
                double m = rotor.Magnitude;
                return rotor / m;
            }
            return UnitZ;
        }

        public Rotor2 Conjugate() => new Rotor2(-_data.z, _data.w);
        public Rotor2 Invert() => Conjugate() / SumSquares;

        public Vector2 Rotate(Vector2 vector)
        {
            double m2 = SumSquares;
            double v2 = _data.z * _data.z, s2 = _data.w * _data.z;
            double cos = 1 - 2 * v2 / m2;
            double sin = 2 * s2 / m2;
            return new Vector2(
                cos * vector.X - sin * vector.Y,
                sin * vector.X + cos * vector.Y);
        }

        public Rotor2 Rate(double ω) => new Rotor2(_data.w * ω / 2, _data.z * ω / 2);
        public double Omega(Rotor2 rate) => 2 * (_data.w * rate._data.z - _data.z * rate._data.w);

        public static Rotor2 Slerp(Rotor2 rotor_1, Rotor2 rotor_2, double amount)
        {
			double dot = rotor_1.Z * rotor_2.Z + rotor_1.W * rotor_2.W;
			bool flag = false;
			if (dot < 0f)
			{
				flag = true;
				dot = 0f - dot;
			}
			double α;
			double β;
			if (dot > 1-LinearAlgebra.Epsilon)
			{
				α = 1f - amount;
				β = (flag ? (0f - amount) : amount);
			}
			else
			{
				double ang = (double)Math.Acos(dot);
				double fct = (double)(1.0 / Math.Sin(ang));
				α = (double)Math.Sin((1f - amount) * ang) * fct;
				β = (flag ? ((double)(0.0 - Math.Sin(amount * ang)) * fct) : ((double)Math.Sin(amount * ang) * fct));
			}
            return new Rotor2(
                α * rotor_1.Z + β * rotor_2.Z,
                α * rotor_1.W + β * rotor_2.W);
        }

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            return $"({_data.z} k_|{_data.w})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g");

        #endregion

        #region Algebra
        public static Rotor2 Negate(Rotor2 a)
            => new Rotor2(
                -a._data.z,
                -a._data.w);
        public static Rotor2 Scale(double factor, Rotor2 a)
            => new Rotor2(
                factor * a._data.z,
                factor * a._data.w);
        public static Rotor2 Add(Rotor2 a, Rotor2 b)
            => new Rotor2(
                a._data.z + b._data.z,
                a._data.w + b._data.w);
        public static Rotor2 Subtract(Rotor2 a, Rotor2 b)
            => new Rotor2(
                a._data.z - b._data.z,
                a._data.w - b._data.w);
        public static Rotor2 Product(Rotor2 a, Rotor2 b)
            => new Rotor2(
                a._data.w * b._data.z + a._data.z * b._data.w,
                a._data.w * b._data.w - a._data.z * b._data.z);
        public static Rotor2 Dot(Rotor2 a, Rotor2 b)
            => new Rotor2(
                a._data.w * b._data.z + a._data.z * b._data.w,
                a._data.w * b._data.w - a._data.z * b._data.z);
        public static Rotor2 Cross(Rotor2 a, Rotor2 b) => Zero;

        public static Rotor2 operator +(Rotor2 a, Rotor2 b) => Add(a, b);
        public static Rotor2 operator -(Rotor2 a) => Negate(a);
        public static Rotor2 operator -(Rotor2 a, Rotor2 b) => Subtract(a, b);
        public static Rotor2 operator *(double f, Rotor2 a) => Scale(f, a);
        public static Rotor2 operator *(Rotor2 a, double f) => Scale(f, a);
        public static Rotor2 operator /(Rotor2 a, double d) => Scale(1 / d, a);
        public static Rotor2 operator *(Rotor2 a, Rotor2 b) => Product(a, b);
        public static Rotor2 operator |(Rotor2 a, Rotor2 b) => Dot(a, b);
        public static Rotor2 operator ^(Rotor2 a, Rotor2 b) => Cross(a, b);
        public static Vector2 operator *(Rotor2 a, Vector2 v) => a.Rotate(v);

        public static bool operator ==(Rotor2 rotor1, Rotor2 rotor2)
        {
            return rotor1.Equals(rotor2);
        }

        public static bool operator !=(Rotor2 rotor1, Rotor2 rotor2)
        {
            return !(rotor1 == rotor2);
        }

        #endregion

        #region Equality
        public bool Equals(Rotor2 other)
        {
            return _data.Equals(other._data);
        }

        public override int GetHashCode()
        {
            return -1945990370 + _data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Rotor2 rotor && Equals(rotor);
        }

        #endregion

    }

}