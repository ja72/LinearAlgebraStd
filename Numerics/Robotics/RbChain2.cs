using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using static System.Math;

/// <remarks>
/// To view LaTeX comments, you need to download the Tex Comments 2022 addin.
/// </remarks>
namespace JA.Numerics.Robotics
{
    using JA.Numerics.Simulation;

    using static SpatialAlgebra;

    public delegate double JointTorque(double t, double q, double qp);
    public delegate double JointAccel(double t, double q, double qp);

    public class RbChain2 : IOdeModel
    {
        public static Vector2 Gravity { get; set; } = new Vector2(0, -10);

        public RbChain2(int count, params RigidBody2[] bodies)
        {
            Count = count;
            Bodies = bodies;
            InitialState = new SolPoint(0.0, new Vector(2*count));
        }

        public RigidBody2[] Bodies { get; }

        public static RbChain2 Bobs(int count, double mass, double length)
            => new RbChain2(count, Enumerable.Repeat(RigidBody2.Bob(mass, length), count).ToArray());
        public static RbChain2 Rods(int count, double mass, double length)
            => new RbChain2(count, Enumerable.Repeat(RigidBody2.Rod(mass, length), count).ToArray());

        public int Count { get; }

        #region Dynamics
        /// <summary>
        /// Calculate joint accelerations for a serial chain of rigid bodies.
        /// </summary>
        /// <param name="state">Current state of positions and velocities.</param>
        /// <param name="torque">Vector of torque values</param>
        /// <returns>The vector of joint accelerations</returns>
        public Vector CalcInverseDynamics(double time, Vector angle, Vector speed, JointTorque torque)
        {
            int n = Count;                  // Number of links/joints/dof

            double[] q = angle.ToArray();
            double[] qp = speed.ToArray();
            double[] Q = new double[n];
            double[] accel = new double[n];

            // Position & Velocity Kinematics

            var r = new Vector2[n];         // Vector of joint positions
            var θ = new double[n];          // Vector of link orientations
            var cg = new Vector2[n];        // Vector of cg positions
            var s = new Vector21[n];        // Vector of joint axis twists
            var v = new Vector21[n];        // Vector of link velocity twists
            var κ = new Vector21[n];        // Vector of joint bias twists
            var I = new Matrix21[n];        // Vector of spatial mass matrix
            var w = new Vector21[n];        // Vector of weight wrenches
            var p = new Vector21[n];        // Vector of link bias wrenches

            for (int i = 0; i < n; i++)
            {
                //tex: Position Kinematics 
                //$$\begin{aligned}\vec{r}_{i} & =\vec{r}_{i-1}+{\rm rot}\left(\theta_{i-1}\right)\vec{d}_{i}\\
                //\theta_{i} & =\theta_{i-1}+q_{i}\\
                //\vec{c}_{i} & =\vec{r}_{i}+{\rm rot}\left(\theta_{i}\right)\vec{{\rm cg}}_{i}
                //\end{aligned}$$
                if (i > 0)
                {
                    r[i] = r[i-1] + Vector2.Polar(Bodies[i].Length, θ[i-1]);
                    θ[i] = θ[i-1] + q[i];
                }
                else
                {
                    r[i] = Vector2.Zero;
                    θ[i] = q[i];
                }
                cg[i] = r[i] + Vector2.Polar(Bodies[i].CgRatio * Bodies[i].Length, θ[i]);
                //tex: Velocity Kinematics and Body/Joint Properties
                //$$\begin{aligned}\boldsymbol{s}_{i} & =\begin{bmatrix}\vec{r}_{i}\times1\\
                //1\end{bmatrix}\\
                //{\bf I}_{i} & =\begin{bmatrix}m_{i} & -m_{i}\vec{c}_{i}\times\\
                //m_{i}\vec{c}_{i}\times & {\rm I}_{i}+m_{i}\|\vec{c}_{i}\|^{2}
                //\end{bmatrix}
                //\end{aligned}$$
                //$$\begin{aligned}\boldsymbol{v}_{i} & =\boldsymbol{v}_{i-1}+\boldsymbol{s}_{i}\dot{q}_{i}\\
                //\boldsymbol{\kappa}_{i} & =\boldsymbol{v}_{i}\times\boldsymbol{s}_{i}\dot{q}_{i}\\
                //\boldsymbol{w}_{i} & =\begin{bmatrix}m_{i}\vec{g}\\
                //\vec{c}_{i}\times m_{i}\vec{g}
                //\end{bmatrix}\\
                //\boldsymbol{p}_{i} & =\boldsymbol{v}_{i}\times{\bf I}_{i}\boldsymbol{v}_{i}-\boldsymbol{w}_{i}
                //\end{aligned}$$
                s[i] = Twist(1, r[i]);
                if (i>0)
                {
                    v[i] = v[i-1] + s[i]*qp[i];
                }
                else
                {
                    v[i] = s[i]*qp[i];
                }
                κ[i] = TwistCross(v[i], s[i]*qp[i]);
                I[i] = Bodies[i].GetSpatialInertiaMatrix(cg[i]);
                w[i] = Bodies[i].GetWeigthWrench(cg[i], Gravity);
                p[i] = WrenchCross(v[i], I[i]*v[i]) - w[i];
                Q[i] = torque(time, q[i], qp[i]);
            }

            // Articulated Inertia Calculations

            var IA = new Matrix21[n];       // Vector of artculated mass matrix
            var dA = new Vector21[n];       // Vector of articulated bias wrenches
            var TA = new Vector21[n];       // Vector of percussion wrenches

            for (int i = n - 1; i >= 0; i--)
            {
                //tex: Articulated Inertia, recursive definition
                //$$\begin{aligned}{\bf I}_{i}^{A} & ={\bf I}_{i}+\boldsymbol{\Phi}_{i+1}{\bf I}_{i+1}^{A}\\
                //\boldsymbol{p}_{i}^{A} & =\boldsymbol{p}_{i}+\boldsymbol{T}_{i+1}Q_{i+1}+\boldsymbol{\Phi}_{i+1}\left({\bf I}_{i+1}^{A}\boldsymbol{\kappa}_{i+1}+\boldsymbol{p}_{i+1}^{A}\right)\\
                //\boldsymbol{T}_{i} & =\frac{{\bf I}_{i}^{A}\boldsymbol{s}_{i}}{\boldsymbol{s}_{i}^{\intercal}{\bf I}_{i}^{A}\boldsymbol{s}_{i}}\\
                //\boldsymbol{\Phi}_{i} & =1-\boldsymbol{T}_{i}\boldsymbol{s}_{i}^{\intercal}
                //\end{aligned}$$
                if (i < n-1)
                {
                    // Joint reaction space projection
                    Vector21 TA_next = TA[i+1];
                    Matrix21 RU_next = 1 - Vector21.Outer(TA_next, s[i+1]);
                    IA[i] = I[i] + RU_next*IA[i+1];
                    dA[i] = p[i] + TA_next*Q[i+1] + RU_next*(IA[i+1]*κ[i+1] + dA[i+1]);
                }
                else
                {
                    IA[i] = I[i];
                    dA[i] = p[i];
                }
                TA[i] = IA[i]*s[i]/Vector21.Dot(s[i], IA[i]*s[i]);
            }

            // Dynamic solution
            var a = new Vector21[n];
            var f = new Vector21[n];

            for (int i = 0; i < n; i++)
            {
                //tex: Recursive dynamic solution
                //$$\begin{aligned}\ddot{q}_{i} & =\frac{Q_{i}-\boldsymbol{s}_{i}^{\intercal}\left({\bf I}_{i}^{A}\left(\boldsymbol{\kappa}_{i}+\boldsymbol{a}_{i-1}\right)+\boldsymbol{p}_{i}^{A}\right)}{\boldsymbol{s}_{i}^{\intercal}{\bf I}_{i}^{A}\boldsymbol{s}_{i}}\\
                //\boldsymbol{a}_{i} & =\boldsymbol{a}_{i-1}+\boldsymbol{s}_{i}\ddot{q}_{i}+\boldsymbol{\kappa}_{i}\\
                //\boldsymbol{f}_{i} & ={\bf I}_{i}^{A}\boldsymbol{a}_{i}+\boldsymbol{p}_{i}^{A}
                //\end{aligned}$$
                var a_prev = Vector21.Zero;
                if (i > 0)
                {
                    a_prev = a[i-1];
                }
                accel[i] = (Q[i]-Vector21.Dot(s[i], IA[i]*(a_prev+κ[i])+dA[i]))/Vector21.Dot(s[i], IA[i]*s[i]);
                a[i] = a_prev + s[i]*accel[i] + κ[i];
                f[i] = IA[i]*a[i] + dA[i];
            }
            return new Vector(accel);
        }

        /// <summary>
        /// Calculate joint torques for a serial chain of rigid bodies.
        /// </summary>
        /// <param name="state">Current state of positions and velocities.</param>
        /// <param name="accel">The vector of accelerations</param>
        /// <returns>The vector of joint torques</returns>
        public Vector CalcForwardDynamics(double time, Vector angle, Vector speed, JointAccel accel)
        {
            int n = Count;                  // Number of links/joints/dof

            double[] q = angle.ToArray();
            double[] qp = speed.ToArray();
            double[] qpp = new double[n];
            double[] Q = new double[n];

            // Position & Velocity Kinematics

            var r = new Vector2[n];         // Vector of joint positions
            var θ = new double[n];          // Vector of link orientations
            var cg = new Vector2[n];        // Vector of cg positions
            var s = new Vector21[n];        // Vector of joint axis twists
            var v = new Vector21[n];        // Vector of link velocity twists
            var κ = new Vector21[n];        // Vector of joint bias twists
            var I = new Matrix21[n];        // Vector of spatial mass matrix
            var w = new Vector21[n];        // Vector of weight wrenches
            var p = new Vector21[n];        // Vector of link bias wrenches
            var a = new Vector21[n];

            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    r[i] = r[i-1] + Vector2.Polar(Bodies[i].Length, θ[i-1]);
                    θ[i] = θ[i-1] + q[i];
                }
                else
                {
                    r[i] = Vector2.Zero;
                    θ[i] = q[i];
                }
                qpp[i] = accel(time, q[i], qp[i]);
                cg[i] = r[i] + Vector2.Polar(Bodies[i].CgRatio * Bodies[i].Length, θ[i]);
                s[i] = Twist(1, r[i]);
                if (i>0)
                {
                    v[i] = v[i-1] + s[i]*qp[i];
                }
                else
                {
                    v[i] = s[i]*qp[i];
                }
                κ[i] = TwistCross(v[i], s[i]*qp[i]);
                if (i>0)
                {
                    a[i] = a[i-1] + s[i]*qpp[i] + κ[i];
                }
                else
                {
                    a[i] = s[i]*qpp[i];
                }
                I[i] = Bodies[i].GetSpatialInertiaMatrix(cg[i]);
                w[i] = Bodies[i].GetWeigthWrench(cg[i], Gravity);
                p[i] = WrenchCross(v[i], I[i]*v[i]) - w[i];
            }

            var f = new Vector21[n];

            for (int i = n - 1; i >= 0; i--)
            {
                if (i < n-1)
                {
                    f[i] = I[i]*a[i]+p[i]+f[i+1];
                }
                else
                {
                    f[i] = I[i]*a[i]+p[i];
                }
                Q[i] = Vector21.Dot(s[i], f[i]);
            }

            return new Vector(Q);
        }

        #endregion

        #region Simulation

        public Vector Derivative(double t, Vector x)
        {
            Vector q = x[0, Count].ToArray();
            Vector qp = x[Count, Count].ToArray();
            Vector qpp = CalcInverseDynamics(t, q, qp, (t,q,qp)=> 0.0);

            return Vector.Append(qp, qpp);
        }

        public SolPoint InitialState
        {
            get => new SolPoint(InitialTime, Vector.Append(InitialAngles, InitialSpeeds));
            set
            {
                InitialTime = value.T;
                InitialAngles = value.X[0,Count].ToArray();
                InitialSpeeds = value.X[Count,Count].ToArray();
            }
        }
        public double InitialTime { get; set; }
        public Vector InitialAngles { get; set; }
        public Vector InitialSpeeds { get; set; }

        #endregion

    }

}
