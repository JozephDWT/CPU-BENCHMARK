using System.Runtime.InteropServices;

namespace BENCHMARK.SOURCE.METHOD.HELPERS
{
    // 64-байтовый паддинг, чтобы каждый поток писал в отдельную cache-line

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal struct PaddingCell
    {
        [FieldOffset(0)]
        public ulong Value;
    }
}