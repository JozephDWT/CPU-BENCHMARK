using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BENCHMARK.SOURCE.METHOD
{
    internal static class HeavyMath
    {
        private const int BufferLen = 128;                  // 128 double = 1024 байт
        private const long Iteration = 100_000_000;        // регулируйте нагрузку
        private const int Warmups = 2;

        public static TimeSpan Run()
        {
            int cores = Environment.ProcessorCount;
            var partial = new PaddingCell[cores];

            // прогрев
            for (int i = 0; i < Warmups; i++)
                OnePass(partial);

            // измерение
            var sw = Stopwatch.StartNew();
            OnePass(partial);
            sw.Stop();

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
                    Span<double> buf = stackalloc double[BufferLen];
                    for (int i = 0; i < buf.Length; i++)
                        buf[i] = 0.1 + i; // разные исходные числа

                    double localSum = 0.0;
                    int vecSize = Vector<double>.Count;
                    long blocks = Iteration / BufferLen;

                    for (long b = 0; b < blocks; b++)
                    {
                        // SIMD-корень
                        for (int o = 0; o < BufferLen; o += vecSize)
                        {
                            var v = new Vector<double>(buf.Slice(o, vecSize));
                            v = Vector.SquareRoot(v);
                            v.CopyTo(buf.Slice(o));
                        }

                        // скалярно
                        for (int j = 0; j < BufferLen; j++)
                        {
                            double x = buf[j];
                            x = Math.Sin(x);
                            x = Math.Log(x + 1.0);
                            x *= x * x;  // x³ без Math.Pow
                            buf[j] = x;
                            localSum += x;
                        }
                    }

                    partial[core].Value =
                        BitConverter.DoubleToUInt64Bits(localSum);
                });
        }

        [StructLayout(LayoutKind.Explicit, Size = 64)]
        private struct PaddingCell
        {
            [FieldOffset(0)]
            public ulong Value;
        }
    }
}