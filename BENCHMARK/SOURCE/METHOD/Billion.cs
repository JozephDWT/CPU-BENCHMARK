using BENCHMARK.SOURCE.METHOD.HELPERS;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BENCHMARK.SOURCE.METHOD
{
    internal static class Billion
    {
        public const long Limit = 100_000_000_000;   // 100 млрд
        private const int Warmups = 1;                // прогрев JIT

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static TimeSpan Run()
        {
            int cores = Environment.ProcessorCount;
            var partial = new PaddingCell[cores];

            // разгон JIT
            for (int i = 0; i < Warmups; i++)
                OnePass.Logic(partial);

            var sw = Stopwatch.StartNew();
            OnePass.Logic(partial);
            sw.Stop();

            // защищаемся от dead-code-elimination
            ulong checksum = 0;
            foreach (ref readonly var cell in partial.AsSpan())
                checksum += cell.Value;
            GC.KeepAlive(checksum);

            return sw.Elapsed;
        }
    }
}