using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace JA.Numerics.Simulation
{

    public interface IOdeModel
    {
        Vector Derivative(double t, Vector x);
        SolPoint InitialState { get; set; }
    }

    public enum OdeMethod
    {
        Euler,
        RK45,
        RK547M,
        GearPDF,
    }

    public static partial class Ode
    {

        public static IEnumerable<SolPoint> Integrate(this IOdeModel model, OdeMethod method, double initialStep)
        {
            var options = new Options()
            {
                InitialStep = initialStep,
            };
            return Integrate(model, method, options);
        }
        public static IEnumerable<SolPoint> Integrate(this IOdeModel model, OdeMethod method, Options options)
        {
            var f = model.Derivative;
            var t0 = model.InitialState.T;
            var x0 = model.InitialState.X;
            switch (method)
            {
                case OdeMethod.Euler:
                    return Ode.Euler(t0, x0, f, options);
                case OdeMethod.RK45:
                    return Ode.RK45(t0, x0, f, options);
                case OdeMethod.RK547M:
                    return Ode.RK547M(t0, x0, f, options);
                case OdeMethod.GearPDF:
                    return Ode.GearBDF(t0, x0, f, options);
                default:
                    throw new NotSupportedException(method.ToString());
            }
        }

    }

    // Options name should match if possible: http://www.mathworks.com/help/techdoc/ref/odeset.html

    /// <summary>ODE solver options</summary>
    public class Options
    {
        /// <summary>Gets or sets initial step for solution. Default value 0 means that initial step is computed automatically</summary>
        public double InitialStep { get; set; }
        /// <summary>Gets or sets absolute error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
        public double AbsoluteTolerance { get; set; }
        /// <summary>Gets or sets relative error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
        public double RelativeTolerance { get; set; }
        /// <summary>Gets or sets step value in output solution. Default value 0 means than all points are included to output</summary>
        public double OutputStep { get; set; }
        /// <summary>Gets or sets maximal step value.</summary>
        public double MaxStep { get; set; }
        /// <summary>Gets or sets minimal step value.</summary>
        public double MinStep { get; set; }
        /// <summary>Gets or sets maximal step scale factor.</summary>
        public double MaxScale { get; set; }
        /// <summary>Gets or sets minimal step scale factor.</summary>
        public double MinScale { get; set; }
        /// <summary>Gets or sets number of iterations in GearBDF method - isn't used in RK547M/// </summary>
        public int NumberOfIterations { get; set; }
        /// <summary>Gets or sets a dense Jacobian matrix</summary>
        public Matrix Jacobian { get; set; }
        /// <summary>Gets or sets a sparse Jacobian matrix</summary>
        public SparseMatrix SparseJacobian { get; set; }
        /// <summary>Default construction of an Options instance.</summary>
        public Options()
        {
            InitialStep = 0.0d;
            AbsoluteTolerance = 1e-6;
            RelativeTolerance = 1e-3;
            MaxStep = double.MaxValue;
            MinStep = 0.0d;
            MaxScale = 1.1d;
            MinScale = 0.9d;
            OutputStep = 0.0d;
            NumberOfIterations = 5;
        }

        private static readonly Options defaultOpts = new Options();

        /// <summary>Gets default option set</summary>
        public static Options Default
        {
            get { return defaultOpts; }
        }
    }
}