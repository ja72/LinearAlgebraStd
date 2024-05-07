using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

using Vector4 = System.Numerics.Vector4;

namespace JA.Numerics.Geometry
{
    /// <summary>Represents a plane in three-dimensional space.</summary>
    public struct Plane3 : IEquatable<Plane3>
    {
        private readonly (Vector3 normal, double d) data;

        /// <summary>Creates a <see cref="T:System.Numerics.Plane" /> object from the X, Y, and Z components of its normal, and its distance from the origin on that normal.</summary>
        /// <param name="x">The X component of the normal.</param>
        /// <param name="y">The Y component of the normal.</param>
        /// <param name="z">The Z component of the normal.</param>
        /// <param name="d">The distance of the plane along its normal from the origin.</param>		
        public Plane3(double x, double y, double z, double d)
        {
            data =(new Vector3(x, y, z), d);
        }

        /// <summary>Creates a <see cref="T:System.Numerics.Plane" /> object from a specified normal and the distance along the normal from the origin.</summary>
        /// <param name="normal">The plane's normal vector.</param>
        /// <param name="d">The plane's distance from the origin along its normal vector.</param>
        public Plane3(Vector3 normal, double d)
        {
            data =(normal, d);
        }

        /// <summary>Creates a <see cref="T:System.Numerics.Plane" /> object from a specified four-dimensional vector.</summary>
        /// <param name="value">A vector whose first three elements describe the normal vector, and whose <see cref="F:System.Numerics.Vector4.W" /> defines the distance along that normal from the origin.</param>
        public Plane3(Vector4 value)
        {
            data =(new Vector3(value.X, value.Y, value.Z), value.W);
        }

        public static Plane3 Empty { get; } = new Plane3(Vector3.Zero, 0);
        public static Plane3 Infinity { get; } = new Plane3(Vector3.Zero, 1);
        public static Plane3 AlongYZ { get; } = new Plane3(Vector3.UnitX, 0);
        public static Plane3 AlongZX { get; } = new Plane3(Vector3.UnitY, 0);
        public static Plane3 AlongXY { get; } = new Plane3(Vector3.UnitZ, 0);

        /// <summary>The normal vector of the plane.</summary>
        public Vector3 Vector => data.normal;
        /// <summary>The distance of the plane along its normal from the origin.</summary>
        public double D => data.d;

        public bool IsFinite => data.normal.LengthSquared()>0;

        /// <summary>The position vector of the point on the plane closest to the origin.</summary>
        public Vector3 Position => data.d * data.normal/data.normal.LengthSquared();

        /// <summary>Creates a <see cref="T:System.Numerics.Plane" /> object that contains three specified points.</summary>
        /// <param name="point1">The first point defining the plane.</param>
        /// <param name="point2">The second point defining the plane.</param>
        /// <param name="point3">The third point defining the plane.</param>
        /// <returns>The plane containing the three points.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plane3 CreateFromVertices(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            Vector3 vector = point2 - point1;
            Vector3 vector2 = point3 - point1;
            Vector3 value = Vector3.Cross(vector, vector2);
            Vector3 vector3 = Vector3.Normalize(value);
            double d = 0 - Vector3.Dot(vector3, point1);
            return new Plane3(vector3, d);
        }

        /// <summary>Creates a new <see cref="T:JA.Sng.Plane3" /> object whose normal vector is the source plane's normal vector normalized.</summary>
        /// <param name="value">The source plane.</param>
        /// <returns>The normalized plane.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plane3 Normalize(Plane3 value)
        {
            double num = value.Vector.LengthSquared();
            const double tolsq = 1.1920929E-07f;
            if (Math.Abs(num - 1f) < tolsq)
            {
                return value;
            }
            double num2 = (double)Math.Sqrt(num);
            return new Plane3(value.Vector / num2, value.D / num2);
        }

        /// <summary>Transforms a normalized plane by a Quaternion rotation.</summary>
        /// <param name="plane">The normalized plane to transform.</param>
        /// <param name="rotation">The Quaternion rotation to apply to the plane.</param>
        /// <returns>A new plane that results from applying the Quaternion rotation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plane3 Transform(Plane3 plane, Quaternion rotation)
        {
            var num = rotation.X + rotation.X;
            var num2 = rotation.Y + rotation.Y;
            var num3 = rotation.Z + rotation.Z;
            var num4 = rotation.W * num;
            var num5 = rotation.W * num2;
            var num6 = rotation.W * num3;
            var num7 = rotation.X * num;
            var num8 = rotation.X * num2;
            var num9 = rotation.X * num3;
            var num10 = rotation.Y * num2;
            var num11 = rotation.Y * num3;
            var num12 = rotation.Z * num3;
            var num13 = 1f - num10 - num12;
            var num14 = num8 - num6;
            var num15 = num9 + num5;
            var num16 = num8 + num6;
            var num17 = 1f - num7 - num12;
            var num18 = num11 - num4;
            var num19 = num9 - num5;
            var num20 = num11 + num4;
            var num21 = 1f - num7 - num10;
            var x = plane.Vector.X;
            var y = plane.Vector.Y;
            var z = plane.Vector.Z;
            return new Plane3(x * num13 + y * num14 + z * num15, x * num16 + y * num17 + z * num18, x * num19 + y * num20 + z * num21, plane.D);
        }

        /// <summary>Calculates the dot product of a plane and a 4-dimensional vector.</summary>
        /// <param name="plane">The plane.</param>
        /// <param name="value">The four-dimensional vector.</param>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Plane3 plane, Vector4 value)
        {
            return plane.Vector.X * value.X + plane.Vector.Y * value.Y + plane.Vector.Z * value.Z + plane.D * value.W;
        }

        /// <summary>Returns the dot product of a specified three-dimensional vector and the normal vector of this plane plus the distance (<see cref="F:System.Numerics.Plane.D" />) value of the plane.</summary>
        /// <param name="plane">The plane.</param>
        /// <param name="value">The 3-dimensional vector.</param>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DotCoordinate(Plane3 plane, Vector3 value)
        {
            return Vector3.Dot(plane.Vector, value) + plane.D;
        }

        /// <summary>Returns the dot product of a specified three-dimensional vector and the <see cref="F:System.Numerics.Plane.Normal" /> vector of this plane.</summary>
        /// <param name="plane">The plane.</param>
        /// <param name="value">The three-dimensional vector.</param>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DotNormal(Plane3 plane, Vector3 value)
        {
            return Vector3.Dot(plane.Vector, value);
        }

        #region Formatting
        /// <summary>Returns the string representation of this plane object.</summary>
        /// <returns>A string that represents this <see cref="T:System.Numerics.Plane" /> object.</returns>
        public string ToString(string formatting, IFormatProvider provider)
        {
            return string.Format(provider, "{{Position:{0} W:{1}}}", new object[2]
            {
                Vector.ToString(formatting, provider),
                D.ToString(formatting, provider)
            });
        }
        public string ToString(string formatting)
            => ToString(formatting, CultureInfo.CurrentCulture.NumberFormat);
        public override string ToString()
            => ToString("g");
        #endregion

        #region Comparisons

        /// <summary>Returns a value that indicates whether two planes are equal.</summary>
        /// <param name="value1">The first plane to compare.</param>
        /// <param name="value2">The second plane to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Plane3 value1, Plane3 value2)
        {
            if (value1.Vector.X == value2.Vector.X && value1.Vector.Y == value2.Vector.Y && value1.Vector.Z == value2.Vector.Z)
            {
                return value1.D == value2.D;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether two planes are not equal.</summary>
        /// <param name="value1">The first plane to compare.</param>
        /// <param name="value2">The second plane to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Plane3 value1, Plane3 value2)
        {
            if (value1.Vector.X == value2.Vector.X && value1.Vector.Y == value2.Vector.Y && value1.Vector.Z == value2.Vector.Z)
            {
                return value1.D != value2.D;
            }
            return true;
        }

        /// <summary>Returns a value that indicates whether this instance and another plane object are equal.</summary>
        /// <param name="other">The other plane.</param>
        /// <returns>
        ///   <see langword="true" /> if the two planes are equal; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Plane3 other)
        {
            return data.Equals(other.data);
        }

        /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Plane3 item)
            {
                return Equals(item);
            }
            return false;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = -1817952719;
                hc = (-1521134295)*hc + data.GetHashCode();
                return hc;
            }
        }

        #endregion

        #region Algebra
        public static Plane3 Negate(Plane3 a)
            => new Plane3(
                -a.Vector,
                -a.D);
        public static Plane3 Scale(double factor, Plane3 a)
            => new Plane3(
                factor*a.Vector,
                factor*a.D);
        public static Plane3 Add(Plane3 a, Plane3 b)
            => new Plane3(
                a.Vector+b.Vector,
                a.D+b.D);
        public static Plane3 Subtract(Plane3 a, Plane3 b)
            => new Plane3(
                a.Vector-b.Vector,
                a.D-b.D);

        public static Plane3 operator +(Plane3 a, Plane3 b) => Add(a, b);
        public static Plane3 operator -(Plane3 a) => Negate(a);
        public static Plane3 operator -(Plane3 a, Plane3 b) => Subtract(a, b);
        public static Plane3 operator *(double f, Plane3 a) => Scale(f, a);
        public static Plane3 operator *(Plane3 a, double f) => Scale(f, a);
        public static Plane3 operator /(Plane3 a, double d) => Scale(1/d, a);
        #endregion

        #region Geometry
        public Point3 Center 
            => new Point3(
                -data.d*data.normal, 
                data.normal.LengthSquared());

        public double Distance
            => Abs(D)/Vector.Length();

        public static Plane3 FromThreePoints(Point3 A, Point3 B, Point3 C)
        {
            return A & B & C;
        }

        public static Plane3 FromLineAndDirection(Line3 line, Vector3 direction)
            => new Plane3(
                Vector3.Cross(line.Vector, direction),
                -Vector3.Dot(line.Moment, direction));

        public static Plane3 FromLineAndPoint(Line3 line, Point3 point)
            => new Plane3(
                Vector3.Cross(point.Vector, line.Vector) - point.W * line.Moment,
                Vector3.Dot(point.Vector, line.Moment));

        public static Plane3 FromLineThroughOrigin(Line3 line)
            => new Plane3(line.Moment, 0);

        public static Plane3 FromLineAwayFromOrigin(Line3 line)
            => new Plane3(
                Vector3.Cross(line.Moment, line.Vector), 
                line.Moment.LengthSquared());

        public static Plane3 FromPointAwayFromOrigin(Point3 point)
            => new Plane3(
                -point.W*point.Vector, point.W*point.W);

        public double DistanceTo(Point3 point)
            => Abs(Vector3.Dot(Vector, point.Vector) + D*point.W)
            /(point.W*Vector).Length();

        public double DistanceTo(Line3 line) => line.DistanceTo(this);

        #endregion

        #region Geometric Algebra        
        /// <summary>
        /// Implements the meet operator.
        /// </summary>
        /// <param name="plane">The plane.</param>
        /// <param name="line">The line.</param>
        /// <returns>The point where the line meets the plane.</returns>
        public static Point3 operator ^(Plane3 plane, Line3 line)
            => Point3.FromLineAndPlane(line, plane);

        /// <summary>
        /// Implements the meet operator.
        /// </summary>
        public static Line3 operator ^(Plane3 plane1, Plane3 plane2)
            => Line3.FromTwoPlanes(plane1, plane2);

        public static double operator *(Plane3 plane, Point3 point)
            => Vector3.Dot(point.Vector, plane.Vector) + point.W*plane.D;

        public static double operator *(Plane3 plane, Line3 line)
            => line * plane;


        #endregion

    }
}
