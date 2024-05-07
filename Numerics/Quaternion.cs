using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vector4 = System.Numerics.Vector4;
using Matrix4x4 = System.Numerics.Matrix4x4;
using System.Runtime.CompilerServices;

namespace JA.Numerics
{
    /// <summary>Represents a vector that is used to encode three-dimensional physical rotations.</summary>
    public readonly struct Quaternion :
        IEquatable<Quaternion>,
        IReadOnlyList<double>,
        ICollection<double>,
        System.Collections.ICollection,
        IFormattable
    {
        readonly (double x, double y, double z, double w) data;

        #region Factory
        /// <summary>Constructs a quaternion from the specified components.</summary>
        /// <param name="x">The value to assign to the X component of the quaternion.</param>
        /// <param name="y">The value to assign to the Y component of the quaternion.</param>
        /// <param name="z">The value to assign to the Z component of the quaternion.</param>
        /// <param name="w">The value to assign to the W component of the quaternion.</param>
        public Quaternion(double x, double y, double z, double w)
        {
            data = (x, y, z, w);
        }

        /// <summary>Creates a quaternion from the specified vector and rotation parts.</summary>
        /// <param name="vectorPart">The vector part of the quaternion.</param>
        /// <param name="scalarPart">The rotation part of the quaternion.</param>
        public Quaternion(Vector3 vectorPart, double scalarPart)
        {
            this.data.x = vectorPart.X;
            this.data.y = vectorPart.Y;
            this.data.z = vectorPart.Z;
            this.data.w = scalarPart;
        }
        /// <summary>Gets a quaternion that represents no rotation.</summary>
        /// <returns>A quaternion whose values are <c>(0, 0, 0, 1)</c>.</returns>
        public static Quaternion Identity { get; } = new Quaternion(0, 0, 0, 1);
        public static Quaternion Empty { get; } = new Quaternion(0, 0, 0, 0);

        public static implicit operator Quaternion(System.Numerics.Quaternion quaternion)
            => new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        public static explicit operator System.Numerics.Quaternion(Quaternion quaternion)
            => new System.Numerics.Quaternion((float)quaternion.X, (float)quaternion.Y, (float)quaternion.Z, (float)quaternion.W);
        #endregion

        #region Properties
        /// <summary>The X value of the vector component of the quaternion.</summary>
        public double X => data.x;
        /// <summary>The Y value of the vector component of the quaternion.</summary>
        public double Y => data.y;
        /// <summary>The Z value of the vector component of the quaternion.</summary>
        public double Z => data.z;
        /// <summary>The rotation component of the quaternion.</summary>
        public double W => data.w;

        public readonly double this[int index] => index switch
        {
            0 => data.x,
            1 => data.y,
            2 => data.z,
            3 => data.w,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        /// <summary>Gets a value that indicates whether the current instance is the identity quaternion.</summary>
        /// <returns>
        ///   <see langword="true" /> if the current instance is the identity quaternion; otherwise, <see langword="false" />.</returns>
        public bool IsIdentity => data.x==0 && data.y ==0 && data.z == 0 && data.w==1;


        /// <summary>Calculates the length of the quaternion.</summary>
        /// <returns>The computed length of the quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Magnitude()
        {
            double magsq = MagnitudeSquared();
            return Math.Sqrt(magsq);
        }

        /// <summary>Calculates the squared length of the quaternion.</summary>
        /// <returns>The length squared of the quaternion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double MagnitudeSquared()
        {
            return data.x * data.x + data.y * data.y + data.z * data.z + data.w * data.w;
        }

        public void Deconstruct(out double x, out double y, out double z, out double w)
        {
            (x, y, z, w) = data;
        }

        #endregion

        #region Algebra
        /// <summary>Divides each component of a specified <see cref="T:System.Numerics.Quaternion" /> by its length.</summary>
        /// <param name="value">The quaternion to normalize.</param>
        /// <returns>The normalized quaternion.</returns>
        public static Quaternion Normalize(Quaternion value)
        {
            double num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            double num2 = 1 / Math.Sqrt(num);
            var rx = value.X * num2;
            var ry = value.Y * num2;
            var rz = value.Z * num2;
            var rw = value.W * num2;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the conjugate of a specified quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>A new quaternion that is the conjugate of <see langword="value" />.</returns>
        public static Quaternion Conjugate(Quaternion value)
        {
            var rx = 0 - value.X;
            var ry = 0 - value.Y;
            var rz = 0 - value.Z;
            var rw = value.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the inverse of a quaternion.</summary>
        /// <param name="value">The quaternion.</param>
        /// <returns>The inverted quaternion.</returns>
        public static Quaternion Inverse(Quaternion value)
        {
            double num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
            double num2 = 1 / num;
            var rx = (0 - value.X) * num2;
            var ry = (0 - value.Y) * num2;
            var rz = (0 - value.Z) * num2;
            var rw = value.W * num2;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Creates a quaternion from a unit vector and an angle to rotate around the vector.</summary>
        /// <param name="axis">The unit vector to rotate around.</param>
        /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion CreateFromAxisAngle(Vector3 axis, double angle)
        {
            double num = angle * 0.5f;
            double num2 = Math.Sin(num);
            double w = Math.Cos(num);

            var rx = axis.X * num2;
            var ry = axis.Y * num2;
            var rz = axis.Z * num2;
            var rw = w;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Creates a new quaternion from the given yaw, pitch, and roll.</summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y axis.</param>
        /// <param name="pitch">The pitch angle, in radians, around the X axis.</param>
        /// <param name="roll">The roll angle, in radians, around the Z axis.</param>
        /// <returns>The resulting quaternion.</returns>
        public static Quaternion CreateFromYawPitchRoll(double yaw, double pitch, double roll)
        {
            double num = roll * 0.5f;
            double num2 = Math.Sin(num);
            double num3 = Math.Cos(num);
            double num4 = pitch * 0.5f;
            double num5 = Math.Sin(num4);
            double num6 = Math.Cos(num4);
            double num7 = yaw * 0.5f;
            double num8 = Math.Sin(num7);
            double num9 = Math.Cos(num7);

            var rx = num9 * num5 * num3 + num8 * num6 * num2;
            var ry = num8 * num6 * num3 - num9 * num5 * num2;
            var rz = num9 * num6 * num2 - num8 * num5 * num3;
            var rw = num9 * num6 * num3 + num8 * num5 * num2;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Creates a quaternion from the specified rotation matrix.</summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix4x4 matrix)
        {
            double num = matrix.M11 + matrix.M22 + matrix.M33;
            double rx, ry, rz, rw;
            if (num > 0)
            {
                double num2 = Math.Sqrt(num + 1);
                rw = num2 * 0.5f;
                num2 = 0.5f / num2;
                rx = (matrix.M23 - matrix.M32) * num2;
                ry = (matrix.M31 - matrix.M13) * num2;
                rz = (matrix.M12 - matrix.M21) * num2;
            }
            else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
            {
                double num3 = Math.Sqrt(1 + matrix.M11 - matrix.M22 - matrix.M33);
                double num4 = 0.5f / num3;
                rx = 0.5f * num3;
                ry = (matrix.M12 + matrix.M21) * num4;
                rz = (matrix.M13 + matrix.M31) * num4;
                rw = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                double num5 = Math.Sqrt(1 + matrix.M22 - matrix.M11 - matrix.M33);
                double num6 = 0.5f / num5;
                rx = (matrix.M21 + matrix.M12) * num6;
                ry = 0.5f * num5;
                rz = (matrix.M32 + matrix.M23) * num6;
                rw = (matrix.M31 - matrix.M13) * num6;
            }
            else
            {
                double num7 = Math.Sqrt(1 + matrix.M33 - matrix.M11 - matrix.M22);
                double num8 = 0.5f / num7;
                rx = (matrix.M31 + matrix.M13) * num8;
                ry = (matrix.M32 + matrix.M23) * num8;
                rz = 0.5f * num7;
                rw = (matrix.M12 - matrix.M21) * num8;
            }
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Calculates the dot product of two quaternions.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        }

        /// <summary>Interpolates between two quaternions, using spherical linear interpolation.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of the second quaternion in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            double num = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            bool flag = false;
            if (num < 0)
            {
                flag = true;
                num = 0 - num;
            }
            double num2;
            double num3;
            if (num > 0.999999f)
            {
                num2 = 1 - amount;
                num3 = (flag ? (0 - amount) : amount);
            }
            else
            {
                double num4 = Math.Acos(num);
                double num5 = 1.0 / Math.Sin(num4);
                num2 = Math.Sin((1 - amount) * num4) * num5;
                num3 = (flag ? ((0.0 - Math.Sin(amount * num4)) * num5) : (Math.Sin(amount * num4) * num5));
            }
            var rx = num2 * quaternion1.X + num3 * quaternion2.X;
            var ry = num2 * quaternion1.Y + num3 * quaternion2.Y;
            var rz = num2 * quaternion1.Z + num3 * quaternion2.Z;
            var rw = num2 * quaternion1.W + num3 * quaternion2.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Performs a linear interpolation between two quaternions based on a value that specifies the weighting of the second quaternion.</summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="amount">The relative weight of <paramref name="quaternion2" /> in the interpolation.</param>
        /// <returns>The interpolated quaternion.</returns>
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            double num = 1 - amount;
            double rx, ry, rz, rw;
            double dotProduct = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            if (dotProduct >= 0)
            {
                rx = num * quaternion1.X + amount * quaternion2.X;
                ry = num * quaternion1.Y + amount * quaternion2.Y;
                rz = num * quaternion1.Z + amount * quaternion2.Z;
                rw = num * quaternion1.W + amount * quaternion2.W;
            }
            else
            {
                rx = num * quaternion1.X - amount * quaternion2.X;
                ry = num * quaternion1.Y - amount * quaternion2.Y;
                rz = num * quaternion1.Z - amount * quaternion2.Z;
                rw = num * quaternion1.W - amount * quaternion2.W;
            }
            double magSqr = rx*rx + ry*ry + rz*rz+ rw*rw;
            double invSqrt = 1 / Math.Sqrt(magSqr);
            rx *= invSqrt;
            ry *= invSqrt;
            rz *= invSqrt;
            rw *= invSqrt;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Concatenates two quaternions.</summary>
        /// <param name="value1">The first quaternion rotation in the series.</param>
        /// <param name="value2">The second quaternion rotation in the series.</param>
        /// <returns>A new quaternion representing the concatenation of the <paramref name="value1" /> rotation followed by the <paramref name="value2" /> rotation.</returns>
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            double x = value2.X;
            double y = value2.Y;
            double z = value2.Z;
            double w = value2.W;
            double x2 = value1.X;
            double y2 = value1.Y;
            double z2 = value1.Z;
            double w2 = value1.W;
            double num = y * z2 - z * y2;
            double num2 = z * x2 - x * z2;
            double num3 = x * y2 - y * x2;
            double num4 = x * x2 + y * y2 + z * z2;
            var rx = x * w2 + x2 * w + num;
            var ry = y * w2 + y2 * w + num2;
            var rz = z * w2 + z2 * w + num3;
            var rw = w * w2 - num4;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        public static Quaternion Negate(Quaternion value)
        {
            var rx = 0 - value.X;
            var ry = 0 - value.Y;
            var rz = 0 - value.Z;
            var rw = 0 - value.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        public static Quaternion Add(Quaternion value1, Quaternion value2)
        {
            var rx = value1.X + value2.X;
            var ry = value1.Y + value2.Y;
            var rz = value1.Z + value2.Z;
            var rw = value1.W + value2.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        public static Quaternion Subtract(Quaternion value1, Quaternion value2)
        {
            var rx = value1.X - value2.X;
            var ry = value1.Y - value2.Y;
            var rz = value1.Z - value2.Z;
            var rw = value1.W - value2.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            double x = value1.X;
            double y = value1.Y;
            double z = value1.Z;
            double w = value1.W;
            double x2 = value2.X;
            double y2 = value2.Y;
            double z2 = value2.Z;
            double w2 = value2.W;
            double num = y * z2 - z * y2;
            double num2 = z * x2 - x * z2;
            double num3 = x * y2 - y * x2;
            double num4 = x * x2 + y * y2 + z * z2;
            var rx = x * w2 + x2 * w + num;
            var ry = y * w2 + y2 * w + num2;
            var rz = z * w2 + z2 * w + num3;
            var rw = w * w2 - num4;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        public static Quaternion Multiply(Quaternion value1, double value2)
        {
            var rx = value1.X * value2;
            var ry = value1.Y * value2;
            var rz = value1.Z * value2;
            var rw = value1.W * value2;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        public static Quaternion Divide(Quaternion value1, Quaternion value2)
        {
            double x = value1.X;
            double y = value1.Y;
            double z = value1.Z;
            double w = value1.W;
            double num = value2.X * value2.X + value2.Y * value2.Y + value2.Z * value2.Z + value2.W * value2.W;
            double num2 = 1 / num;
            double num3 = (0 - value2.X) * num2;
            double num4 = (0 - value2.Y) * num2;
            double num5 = (0 - value2.Z) * num2;
            double num6 = value2.W * num2;
            double num7 = y * num5 - z * num4;
            double num8 = z * num3 - x * num5;
            double num9 = x * num4 - y * num3;
            double num10 = x * num3 + y * num4 + z * num5;
            double rx, ry, rz, rw;
            rx = x * num6 + num3 * w + num7;
            ry = y * num6 + num4 * w + num8;
            rz = z * num6 + num5 * w + num9;
            rw = w * num6 - num10;
            return new Quaternion(rx, ry, rz, rw);
        }
        #endregion

        #region Operators
        /// <summary>Reverses the sign of each component of the quaternion.</summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>The negated quaternion.</returns>
        public static Quaternion operator -(Quaternion value)
        {
            double rx, ry, rz, rw;
            rx = 0 - value.X;
            ry = 0 - value.Y;
            rz = 0 - value.Z;
            rw = 0 - value.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Adds each element in one quaternion with its corresponding element in a second quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion that contains the summed values of <paramref name="value1" /> and <paramref name="value2" />.</returns>
        public static Quaternion operator +(Quaternion value1, Quaternion value2)
        {
            double rx, ry, rz, rw;
            rx = value1.X + value2.X;
            ry = value1.Y + value2.Y;
            rz = value1.Z + value2.Z;
            rw = value1.W + value2.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Subtracts each element in a second quaternion from its corresponding element in a first quaternion.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The quaternion containing the values that result from subtracting each element in <paramref name="value2" /> from its corresponding element in <paramref name="value1" />.</returns>
        public static Quaternion operator -(Quaternion value1, Quaternion value2)
        {
            double rx, ry, rz, rw;
            rx = value1.X - value2.X;
            ry = value1.Y - value2.Y;
            rz = value1.Z - value2.Z;
            rw = value1.W - value2.W;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the quaternion that results from multiplying two quaternions together.</summary>
        /// <param name="value1">The first quaternion.</param>
        /// <param name="value2">The second quaternion.</param>
        /// <returns>The product quaternion.</returns>
        public static Quaternion operator *(Quaternion value1, Quaternion value2)
        {
            double x = value1.X;
            double y = value1.Y;
            double z = value1.Z;
            double w = value1.W;
            double x2 = value2.X;
            double y2 = value2.Y;
            double z2 = value2.Z;
            double w2 = value2.W;
            double num = y * z2 - z * y2;
            double num2 = z * x2 - x * z2;
            double num3 = x * y2 - y * x2;
            double num4 = x * x2 + y * y2 + z * z2;
            double rx, ry, rz, rw;
            rx = x * w2 + x2 * w + num;
            ry = y * w2 + y2 * w + num2;
            rz = z * w2 + z2 * w + num3;
            rw = w * w2 - num4;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Returns the quaternion that results from scaling all the components of a specified quaternion by a scalar factor.</summary>
        /// <param name="value1">The source quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The scaled quaternion.</returns>
        public static Quaternion operator *(Quaternion value1, double value2)
        {
            double rx, ry, rz, rw;
            rx = value1.X * value2;
            ry = value1.Y * value2;
            rz = value1.Z * value2;
            rw = value1.W * value2;
            return new Quaternion(rx, ry, rz, rw);
        }

        /// <summary>Divides one quaternion by a second quaternion.</summary>
        /// <param name="value1">The dividend.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The quaternion that results from dividing <paramref name="value1" /> by <paramref name="value2" />.</returns>
        public static Quaternion operator /(Quaternion value1, Quaternion value2)
        {
            double x = value1.X;
            double y = value1.Y;
            double z = value1.Z;
            double w = value1.W;
            double num = value2.X * value2.X + value2.Y * value2.Y + value2.Z * value2.Z + value2.W * value2.W;
            double num2 = 1 / num;
            double num3 = (0 - value2.X) * num2;
            double num4 = (0 - value2.Y) * num2;
            double num5 = (0 - value2.Z) * num2;
            double num6 = value2.W * num2;
            double num7 = y * num5 - z * num4;
            double num8 = z * num3 - x * num5;
            double num9 = x * num4 - y * num3;
            double num10 = x * num3 + y * num4 + z * num5;
            double rx, ry, rz, rw;
            rx = x * num6 + num3 * w + num7;
            ry = y * num6 + num4 * w + num8;
            rz = z * num6 + num5 * w + num9;
            rw = w * num6 - num10;
            return new Quaternion(rx, ry, rz, rw);
        }
        #endregion

        #region Equality
        /// <summary>Returns a value that indicates whether two quaternions are equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Quaternion value1, Quaternion value2)
        {
            if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
            {
                return value1.W == value2.W;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether two quaternions are not equal.</summary>
        /// <param name="value1">The first quaternion to compare.</param>
        /// <param name="value2">The second quaternion to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Quaternion value1, Quaternion value2)
        {
            if (value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z)
            {
                return value1.W != value2.W;
            }
            return true;
        }

        /// <summary>Returns a value that indicates whether this instance and another quaternion are equal.</summary>
        /// <param name="other">The other quaternion.</param>
        /// <returns>
        ///   <see langword="true" /> if the two quaternions are equal; otherwise, <see langword="false" />.</returns>
        public bool Equals(Quaternion other)
        {
            if (X == other.X && Y == other.Y && Z == other.Z)
            {
                return W == other.W;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
            }
            return false;
        }
        #endregion

        #region Formatting
        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode=hashCode*-1521134295+data.GetHashCode();
            return hashCode;

        }
        #endregion

        #region Collections
        public int Count => 4;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double[] ToArray()
        {
            return new double[] { data.x, data.y, data.z, data.w };
        }
        public bool Contains(double item) => IndexOf(item)>=0;
        public int IndexOf(double item) => Array.IndexOf(ToArray(), item);
        public IEnumerator<double> GetEnumerator()
        {
            yield return data.x;
            yield return data.y;
            yield return data.z;
            yield return data.w;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
        void ICollection<double>.Add(double item) => throw new NotSupportedException();
        void ICollection<double>.Clear() => throw new NotSupportedException();
        bool ICollection<double>.Remove(double item) => throw new NotSupportedException();
        bool ICollection<double>.IsReadOnly { get => true; }        
        bool System.Collections.ICollection.IsSynchronized => false;
        object System.Collections.ICollection.SyncRoot => null;
        void System.Collections.ICollection.CopyTo(Array array, int index)
            => Array.Copy(ToArray(), 0, array, 0, index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(double[] array, int index)
            => Array.Copy(ToArray(), 0, array, 0, index);
        #endregion

        #region Formatting
        /// <summary>Returns a string that represents this quaternion.</summary>
        /// <returns>The string representation of this quaternion.</returns>
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string formatting, IFormatProvider formatProvider)
        {
            string x = data.x.ToString(formatting, formatProvider);
            string y = data.y.ToString(formatting, formatProvider);
            string z = data.z.ToString(formatting, formatProvider);
            string w = data.w.ToString(formatting, formatProvider);
            return $"{{{x},{y},{z}|{w}}}";
        }

        #endregion

    }
}
