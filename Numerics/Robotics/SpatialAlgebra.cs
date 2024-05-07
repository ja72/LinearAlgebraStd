/// <remarks>
/// To view LaTeX comments, you need to download the Tex Comments 2022 addin.
/// </remarks>
namespace JA.Numerics.Robotics
{
    public static class SpatialAlgebra
    {
        public static Vector21 Twist(double value, Vector2 position)
            => new Vector21(Vector2.Cross(position, value), value);
        public static Vector21 Twist(Vector2 value)
            => new Vector21(value, 0);
        public static Vector2 TwistCenter(Vector21 v)
            => Vector2.Cross(v.Scalar, v.Vector) / (v.Scalar * v.Scalar);

        public static Vector21 Wrench(Vector2 value, Vector2 position)
            => new Vector21(value, Vector2.Cross(position, value));
        public static Vector21 Wrench(double value)
            => new Vector21(Vector2.Zero, value);
        public static Vector2 WrenchCenter(Vector21 v)
            => Vector2.Cross(v.Vector, v.Scalar) / v.Vector.SumSquares();

        public static Vector21 TwistCross(Vector21 a, Vector21 b)
            => new Vector21(
                Vector2.Cross(a.Scalar, b.Vector) + Vector2.Cross(a.Vector, b.Scalar),
                0);

        public static Vector21 WrenchCross(Vector21 a, Vector21 b)
            => new Vector21(
                Vector2.Cross(a.Scalar, b.Vector),
                Vector2.Cross(a.Vector, b.Vector));

        public static Matrix21 Spi(double mass, double mmoi, Vector2 cg)
            => new Matrix21(
                mass, Vector2.Cross(cg, -mass),
                Vector2.Cross(cg, -mass), mmoi + mass * cg.SumSquares());

        public static Matrix21 Spm(double mass, double mmoi, Vector2 cg)
            => new Matrix21(
                1 / mass + (Vector2.Dot(cg, cg) - Vector2.Outer(cg, cg)) / mmoi,
                Vector2.Cross(cg, 1 / mmoi), Vector2.Cross(cg, 1 / mmoi), 1 / mmoi);
    }

}
