using System.Runtime.InteropServices;

namespace BENCHMARK.SOURCE.CPU
{
    internal static class ThreadCheck
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetLogicalProcessorInformation(
            IntPtr buffer,
            ref uint returnLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
        {
            public IntPtr ProcessorMask;
            public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
            public PROCESSOR_INFORMATION_UNION ProcessorInformation;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PROCESSOR_INFORMATION_UNION
        {
            [FieldOffset(0)] public byte Flags;
            [FieldOffset(0)] public uint NodeNumber;
            [FieldOffset(0)] public CACHE_DESCRIPTOR Cache;
            [FieldOffset(0)] private ulong Reserved1;
            [FieldOffset(8)] private ulong Reserved2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CACHE_DESCRIPTOR
        {
            public byte Level;
            public byte Associativity;
            public ushort LineSize;
            public uint Size;
            public PROCESSOR_CACHE_TYPE Type;
        }

        public enum LOGICAL_PROCESSOR_RELATIONSHIP
        {
            RelationProcessorCore,
            RelationNumaNode,
            RelationCache,
            RelationProcessorPackage,
            RelationGroup,
            RelationAll = 0xFFFF
        }

        public enum PROCESSOR_CACHE_TYPE
        {
            Unified,
            Instruction,
            Data,
            Trace
        }

        public static int Logic() => Environment.ProcessorCount;
    }
}