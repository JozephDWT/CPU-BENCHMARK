using BENCHMARK.SOURCE.METHOD.HELPERS;
using System.Diagnostics;

namespace BENCHMARK.SOURCE.METHOD
{
    // операций FMA на поток

    internal static class CpuFma
    {
        internal const long IterationsPerCore = 1000_000_000; // масштаб
        private const int Warmups = 1;

        public static TimeSpan Run()
        {
            int cores = Environment.ProcessorCount;
            var partial = new PaddingCell[cores];

            for (int i = 0; i < Warmups; i++)      // прогрев JIT и TurboBoost
                FmaOnePassLogic.Execute(partial);

            var sw = Stopwatch.StartNew();
            FmaOnePassLogic.Execute(partial);      // измерение
            sw.Stop();

            // не даём JIT выбросить цикл
            ulong checksum = 0;
            foreach (ref readonly var cell in partial.AsSpan())
                checksum += cell.Value;
            GC.KeepAlive(checksum);

            return sw.Elapsed;
        }
    }
}