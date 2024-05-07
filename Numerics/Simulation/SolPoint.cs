namespace JA.Numerics.Simulation
{
    /// <summary>Structure to represent solution point. Current point has form (t,x1,x2,...,xn), where
    /// n is problem's dimension</summary>
    public struct SolPoint
    {
        private Vector x; //Problem's phase variables
        private double t; //Current time

        /// <summary>Gets phase variables values in current time point</summary>
        public Vector X
        {
            get { return x; }
        }

        /// <summary>Gets current time</summary>
        public double T
        {
            get { return t; }
        }

        /// <summary>Create solution point from time value and phase vector</summary>
        /// <param name="t">Current time value</param>
        /// <param name="x">Current phase vector</param>
        internal SolPoint(double t, Vector x)
        {
            this.x = x;
            this.t = t;
        }
    }
}