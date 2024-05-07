using System;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Math;

namespace JA.Numerics.Geometry
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public readonly struct Ellipse2 :
        IEquatable<Ellipse2>,
        IFormattable
    {
        public Ellipse2(double majorAxis, double minorAxis)
            : this(Point2.Origin, majorAxis, minorAxis)
        {
        }
        public Ellipse2(Point2 center, double majorAxis, double minorAxis)
        {
            Center = center;
            MajorAxis=majorAxis;
            MinorAxis=minorAxis;
        }

        public double MajorAxis { get; }
        public double MinorAxis { get; }
        public Point2 Center { get; }

        public Point2 GetPoint(double t)
        {
            Vector2 delta = Vector2.Elliptical(MajorAxis, MinorAxis, t);
            return Center + delta;
        }
        public Point2 GetClosestPoint(Circle2 circle, double tol)
            => GetClosestPoint(circle.Center, tol);

        public Point2 GetClosestPoint(Point2 point, double tol)
        {
            Vector2 delta = Center.VectorTo(point);
            var sign = Math.Sign(delta.X);
            double Q = MajorAxis*MajorAxis-MinorAxis*MinorAxis;
            double A = 2*delta.X*MajorAxis/Q;
            double B = 2*delta.Y*MinorAxis/Q;

            if (A!=0)
            {

                double IterFun(double z)
                {
                    return 1/A*(B + (sign)*2*z/Sqrt(1+z*z));
                }

                double z_sol = NumericalMethods.GaussPointIteration(IterFun, 0, tol);
                double t = sign == 1 ? Atan(z_sol) : Atan(z_sol) + PI;

                return GetPoint(t);
            }
            else
            {
                double t = Sign(B)*(PI/2);
                return GetPoint(t);
            }
        }

        public Point2 GetClosestPoint(Line2 line, double tol)
        {
            var (a, b, c)= line.Coords;
            var (cx, cy) = Center.AsVector().Coords;
            // transform line coords to being relative to ellipse center
            c -= -a*cx - b*cy;
            double rx = MajorAxis, ry = MinorAxis;

            double IterFun(double z)
            {
                return (b*ry-a*rx*z)*(a*rx+b*ry*z)/(a*c*rx*Sqrt(1+z*z))+(b*ry)/(a*rx);
            }

            double z_sol = NumericalMethods.GaussPointIteration(IterFun, 0, tol);

            double t = Atan(z_sol);

            return GetPoint(t);

        }

        public double DistanceTo(Point2 point, double tol)
            => GetClosestPoint(point, tol).DistanceTo(point);
        public double DistanceTo(Circle2 circle, double tol)
            => GetClosestPoint(circle.Center, tol).DistanceTo(circle);
        public double DistanceTo(Line2 line, double tol)
            => GetClosestPoint(line, tol).DistanceTo(line);

        #region Equality
        public override bool Equals(object obj) => obj is Ellipse2 ellipse && Equals(ellipse);

        public bool Equals(Ellipse2 other)
        {
            return MajorAxis==other.MajorAxis&&
                   MinorAxis==other.MinorAxis&&
                   Center.Equals(other.Center);
        }

        public override int GetHashCode()
        {
            var hashCode = -1545607722;
            hashCode=hashCode*-1521134295+MajorAxis.GetHashCode();
            hashCode=hashCode*-1521134295+MinorAxis.GetHashCode();
            hashCode=hashCode*-1521134295+EqualityComparer<Point2>.Default.GetHashCode(Center);
            return hashCode;
        }

        public static bool operator ==(Ellipse2 ellipse1, Ellipse2 ellipse2)
        {
            return ellipse1.Equals(ellipse2);
        }

        public static bool operator !=(Ellipse2 ellipse1, Ellipse2 ellipse2)
        {
            return !(ellipse1==ellipse2);
        }
        #endregion

        #region Formatting
        public string ToString(string formatting, IFormatProvider provider)
        {
            string x_str = Center.AsVector().X.ToString(formatting, provider);
            string y_str = Center.AsVector().Y.ToString(formatting, provider);
            string a_str = MajorAxis.ToString(formatting, provider);
            string b_str = MinorAxis.ToString(formatting, provider);
            return $"Ellipse(x={x_str}, y={y_str}, rx={a_str}, ry={b_str})";
        }
        public string ToString(string formatting)
            => ToString(formatting, null);
        public override string ToString()
            => ToString("g4");
        #endregion

    }
}
