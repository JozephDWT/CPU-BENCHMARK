using System.Numerics;

namespace BENCHMARK.SOURCE.METHOD.HELPERS
{
    /// <summary>Один параллельный проход: регистровая FMA-петля на каждом ядре.</summary>
    internal static class FmaOnePassLogic
    {
        public static void Execute(PaddingCell[] partial)
        {
            int cores = partial.Length;

            Parallel.For(0, cores,
                new ParallelOptions { MaxDegreeOfParallelism = cores },
                core =>
                {
                    // четыре регистра-вектора 256/512 бит
                    Vector<double> v0 = new(core + 1.0);
                    Vector<double> v1 = new(1.732050807569);   // √3
                    Vector<double> v2 = new(2.718281828459);   // e
                    Vector<double> v3 = new(0.577215664901);   // γ
                    Vector<double> add = new(1e-9);

                    Vector<double> acc = Vector<double>.Zero;

                    for (long i = 0; i < CpuFma.IterationsPerCore; i++)
                    {
                        // 4 × FMA за итерацию
                        v0 = Vector.MultiplyAddEstimate(v0, v1, add);
                        v1 = Vector.MultiplyAddEstimate(v1, v2, add);
                        v2 = Vector.MultiplyAddEstimate(v2, v3, add);
                        v3 = Vector.MultiplyAddEstimate(v3, v0, add);

                        acc += v0 + v1;            // ненулевая работа
                    }

                    partial[core].Value =
                        BitConverter.DoubleToUInt64Bits(Vector.Sum(acc));
                });
        }
    }
}