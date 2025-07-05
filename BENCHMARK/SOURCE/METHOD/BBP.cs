using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BENCHMARK.SOURCE.METHOD
{
    // Рабочий набор всего несколько double

    internal static class BBP
    {
        private const int Warmups = 1;                 // прогрев JIT/Boost
        public const long IterPerCore = 100_000_000;   // масштаб нагрузки

        public static TimeSpan Run()
        {
            int cores = Environment.ProcessorCount;
            var partial = new PaddingCell[cores];

            for (int i = 0; i < Warmups; i++)
                OnePass(partial);              // холостой прогон

            var sw = Stopwatch.StartNew();
            OnePass(partial);                  // измеряем
            sw.Stop();

            // защищаем результат от dead-code-elimination
            ulong checksum = 0;
            foreach (ref readonly var cell in partial.AsSpan())
                checksum += cell.Value;
            GC.KeepAlive(checksum);

            return sw.Elapsed;
        }

        private static void OnePass(PaddingCell[] partial)
        {
            int cores = partial.Length;

            Parallel.For(0, cores,
                new ParallelOptions { MaxDegreeOfParallelism = cores },
                core =>
                {
                    double sum = 0;
                    for (long k = 0; k < IterPerCore; k++)
                        sum += BbpTerm(k);

                    partial[core].Value = BitConverter.DoubleToUInt64Bits(sum);
                });
        }

        private static double BbpTerm(long k)
        {
            double eightK = 8.0 * k;
            double pow16 = Math.Pow(16.0, -k);
            return pow16 * (
                   4.0 / (eightK + 1.0) -
                   2.0 / (eightK + 4.0) -
                   1.0 / (eightK + 5.0) -
                   1.0 / (eightK + 6.0));
        }

        [StructLayout(LayoutKind.Explicit, Size = 64)]
        private struct PaddingCell
        {
            [FieldOffset(0)]
            public ulong Value;
        }
    }
}