/// <remarks>
/// To view LaTeX comments, you need to download the Tex Comments 2022 addin.
/// </remarks>
namespace JA.Numerics.Robotics
{
    using JA.Numerics;

    using static SpatialAlgebra;

    public class RigidBody2
    {
        public RigidBody2(double mass, double mMoi, double length, double cgRatio)
        {
            Mass=mass;
            MMoi=mMoi;
            Length=length;
            CgRatio=cgRatio;
        }

        public double Mass { get; }
        public double MMoi { get; }
        public double Length { get; }
        public double CgRatio { get; }

        public static RigidBody2 Bob(double mass, double length)
            => new RigidBody2(mass, mass*length*length, length, 1.0);
        public static RigidBody2 Rod(double mass, double length)
            => new RigidBody2(mass, mass/12*length*length, length, 0.5);

        public Vector21 GetWeigthWrench(Vector2 cg, Vector2 gravity)
            => Wrench(Mass*gravity, cg);

        public Matrix21 GetSpatialInertiaMatrix(Vector2 cg)
            => Spi(Mass, MMoi, cg);
        public Matrix21 GetSpatialMobilityMatrix(Vector2 cg)
            => Spm(Mass, MMoi, cg);
    }

}
