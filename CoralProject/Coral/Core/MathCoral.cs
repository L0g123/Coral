using System;
using System.Collections.Generic;
using System.Text;

namespace Coral.Core
{
    public static class MathCoral
    {
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static int Lerp(int a, int b, float t) => (int)MathF.Round(a + (b - a) * t);
    }
}
