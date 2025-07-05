namespace BENCHMARK.SOURCE.METHOD.HELPERS
{
    internal static class OnePass
    {
        public static void Logic(PaddingCell[] partial)
        {
            int cores = partial.Length;
            long chunk = Billion.Limit / cores;

            Parallel.For(0, cores,
                new ParallelOptions { MaxDegreeOfParallelism = cores },
                core =>
                {
                    long start = core * chunk;
                    long stop = (core == cores - 1) ? Billion.Limit : start + chunk;

                    ulong local = 0;
                    for (long i = start; i < stop; i++)
                        local += (ulong)i;

                    partial[core].Value = local;
                });
        }
    }
}