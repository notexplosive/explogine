using System;

namespace ExTween
{
    public static class Ease
    {
        public delegate float Delegate(float x);

        public static float Linear(float x)
        {
            return x;
        }

        public static float SineSlowFast(float x)
        {
            return 1 - MathF.Cos((x * MathF.PI) / 2);
        }

        public static float SineFastSlow(float x)
        {
            return MathF.Sin((x * MathF.PI) / 2);
        }

        public static float SineSlowFastSlow(float x)
        {
            return -(MathF.Cos(MathF.PI * x) - 1) / 2;
        }
        
        public static float QuadSlowFast(float x)
        {
            return x * x;
        }

        public static float QuadFastSlow(float x) {
            return 1 - Ease.QuadSlowFast(1 - x);
        }

        public static float QuadSlowFastSlow(float x) {
            return x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
        }
    }
}
