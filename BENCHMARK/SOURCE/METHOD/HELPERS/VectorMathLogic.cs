namespace BENCHMARK.SOURCE.METHOD.HELPERS
{
    internal static class VectorMathLogic
    {
        public static double[] MakeMatrix(int n, int seed = 123)
        {
            var rnd = new Random(seed);
            var m = new double[n * n];
            for (int i = 0; i < m.Length; i++)
                m[i] = rnd.NextDouble() - 0.5;
            return m;
        }
    }
}