using System;
using System.Security.Permissions;


/// <remarks>
/// To view LaTeX comments, you need to download the Tex Comments 2022 addin.
/// </remarks>
namespace JA.Numerics.Robotics
{
    using Vector = Vector;

    public readonly struct RbChain2State : ICloneable
    {
        public readonly double time;
        public readonly double[] angle;
        public readonly double[] speed;
        public readonly double[] accel;
        public readonly double[] torque;

        public RbChain2State(int count)
        {
            time = 0;
            angle = new double[count];
            speed = new double[count];
            accel = new double[count];
            torque = new double[count];
        }

        public RbChain2State(double time, double[] angle, double[] speed, double[] accel, double[] torque) : this()
        {
            this.time=time;
            this.angle=angle??throw new ArgumentNullException(nameof(angle));
            this.speed=speed??throw new ArgumentNullException(nameof(speed));
            this.accel=accel??throw new ArgumentNullException(nameof(accel));
            this.torque=torque??throw new ArgumentNullException(nameof(torque));
        }

        public RbChain2State(RbChain2State other)
        {
            time = other.time;
            angle = other.angle.Clone() as double[];
            speed = other.speed.Clone() as double[];
            accel = other.accel.Clone() as double[];
            torque = other.torque.Clone() as double[];
        }

        public RbChain2State(double t, Vector x)
        {
            // x : { q, qp }
            double[] xv = x;
            int count = xv.Length/2;

            time = t;
            angle = new double[count];
            speed = new double[count];
            accel = new double[count];
            torque = new double[count];

            Array.Copy(xv, 0, angle, 0, count);
            Array.Copy(xv, count, speed, 0, count);

        }

        public Vector AsVector()
        {
            double[] xv = new double[angle.Length + speed.Length];
            Array.Copy(angle, 0, xv, 0, angle.Length);
            Array.Copy(speed, 0, xv, angle.Length, speed.Length);

            return new Vector(xv);
        }
        public Vector AsRateVector()
        {
            double[] xv = new double[angle.Length + speed.Length];
            Array.Copy(speed, 0, xv, 0, speed.Length);
            Array.Copy(accel, 0, xv, speed.Length, accel.Length);

            return new Vector(xv);
        }

        #region ICloneable Members
        public RbChain2State Clone() => new RbChain2State(this);
        object ICloneable.Clone() => Clone();
        #endregion

    }

}
