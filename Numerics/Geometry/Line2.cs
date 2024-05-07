using System;
using System.ComponentModel;
using static System.Math;

namespace JA.Numerics.Geometry
{
    public readonly struct Line2 :
        IEquatable<Line2>,
        IFormattable
    {
        readonly (double a, double b, double c) data;

        public Line2(double a, double b, double c)
        {
            data = (a, b, c);
        }

        public static Line2 Join(Point2 point1, Point2 point2)
            => new Line2(
                point1.Y*point2.W - point1.W*point2.Y,
                point1.W*point2.X - point1.X*point2.W,
                point1.X*point2.Y - point1.Y*point2.X);

        public static Line2 Empty { get; } = new Line2(0, 0, 0);
        public static Line2 XAxis { get; } = new Line2(0, 1, 0);
        public static Line2 YAxis { get; } = new Line2(-1, 0, 0);
        public static Line2 Horizon { get; } = new Line2(0, 0, 1);

        public double A => data.a;
        public double B => data.b;
        public double C => data.c;

        [Browsable(false)]
        public (double a, double b, double c) Coords => data;

        [Browsable(false)]
        public bool IsFinite { get => WeightSqr > 0; }
        [Browsable(false)]
        public bool IsEmpty { get => data.a != 0 && data.b != 0 && data.c != 0; }
        [Browsable(false)]
        public double WeightSqr { get => data.a*data.a + data.b*data.b; }
        [Browsable(false)]
        public double Weight { get => Sqrt(WeightSqr); }

        public bool IsCoincident(Line2 line)
        {
            double w1 = Weight, w2 = line.Weight;

            return data.a*w2 == line.data.a*w1
                && data.b*w2 == line.data.b*w1
                && data.c*w2 == line.data.c*w1;
        }
        public bool IsCoincident(Point2 point)
            => Dot(this, point) == 0;

        public Point2 GetCenter() => new Point2(-data.c*data.a, -data.c*data.b, WeightSqr);
        public Vector2 GetDirection() => new Vector2(data.b, -data.a)/Weight;

        public Line2 Normalized()
        {
            double w = Weight;
            return new Line2(data.a/w, data.b/w, data.c/w);
        }

        public Line2 ParallelThrough(Point2 point)
            => new Line2(data.a*point.W, data.b*point.W, -data.a*point.X-data.b*point.Y);

        public Point2 GetClosestPoint(Point2 point)
            => new Point2(
                data.b*data.b*point.X - data.a* (data.b*point.Y + data.c*point.W),
                data.a*data.a*point.Y - data.b* (data.a*point.X + data.c*point.W),
                (data.a*data.a + data.b*data.b)*point.W);

        public Point2 GetClosestPoint(Circle2 circle) => GetClosestPoint(circle.Center);

        public double DistanceTo(Point2 target) => GetClosestPoint(target).DistanceTo(target);
        public double DistanceTo(Circle2 target) => DistanceTo(target.Center) - target.Radius;
        public double SignedDistanceTo(Point2 target) => Dot(this, target)/(Weight*target.Weight);
        public double SignedDistanceTo(Circle2 target)
        {
            double d = SignedDistanceTo(target.Center);
            return d - Math.Sign(d)*target.Radius;
        }

        public Point2 GetPoint(double t)
        {
            double w = Weight;
            return new Point2(data.b*w*t - data.a*data.c, -data.a*w*t-data.b*data.c, w*w);
        }

        public Vector2 VectorTo(Point2 target)
            => GetClosestPoint(target).VectorTo(target);

        #region Algebra
        public static Line2 Negate(Line2 line)
            => new Line2(
                -line.A,
                -line.B,
                -line.C);
        public static Line2 Scale(double factor, Line2 line)
            => new Line2(
                factor*line.A,
                factor*line.B,
                factor*line.C);
        public static Line2 Add(Line2 line1, Line2 line2)
            => new Line2(
                line1.A+line2.A,
                line1.B+line2.B,
                line1.C+line2.C);
        public static Line2 Subtract(Line2 line1, Line2 line2)
            => new Line2(
                line1.A-line2.A,
                line1.B-line2.B,
                line1.C-line2.C);

        public static double Dot(Line2 line, Point2 point)
            => line.data.a * point.X + line.data.b * point.Y + line.data.c * point.W;

        public static Line2 operator +(Line2 a, Line2 b) => Add(a, b);
        public static Line2 operator -(Line2 a) => Negate(a);
        public static Line2 operator -(Line2 a, Line2 b) => Subtract(a, b);
        public static Line2 operator *(double f, Line2 a) => Scale(f, a);
        public static Line2 operator *(Line2 a, double f) => Scale(f, a);
        public static Line2 operator /(Line2 a, double d) => Scale(1/d, a);
        public static double operator *(Line2 a, Point2 b) => Dot(a, b);

        public static bool operator ==(Line2 left, Line2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line2 left, Line2 right)
        {
            return !(left==right);
        }
        #endregion

        #region Formatting        
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string formatting, IFormatProvider formatProvider)
        {
            string a_str = data.a.ToString(formatting, formatProvider);
            string b_str = data.b.ToString(formatting, formatProvider);
            string c_str = data.c.ToString(formatting, formatProvider);

            string text = string.Empty;
            if (data.a != 0)
            {
                text += $"{a_str}*x";
            }
            if (data.b != 0)
            {
                text += $" + {b_str}*y";
            }
            if (data.c != 0)
            {
                text += $" + {c_str}";
            }
            return text + " = 0";
        }
        #endregion

        #region Equality

        public override bool Equals(object obj)
        {
            return obj is Line2 line&& Equals(line);
        }

        public bool Equals(Line2 line)
            => data.Equals(line.data);

        public override int GetHashCode()
        {
            int hashCode = 1768953197;
            hashCode=hashCode*-1521134295+data.GetHashCode();
            return hashCode;
        } 
        #endregion


    }
}
