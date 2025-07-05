using System.Runtime.InteropServices;

namespace BENCHMARK.SOURCE.CPU
{
    internal static class PhysicalCheck
    {
        public static int Logic()
        {
            uint returnLength = 0;

            ThreadCheck.GetLogicalProcessorInformation(IntPtr.Zero, ref returnLength);

            IntPtr buffer = Marshal.AllocHGlobal((int)returnLength);
            int coreCount = 0;

            try
            {
                if (ThreadCheck.GetLogicalProcessorInformation(buffer, ref returnLength))
                {
                    int structSize = Marshal.SizeOf<ThreadCheck.SYSTEM_LOGICAL_PROCESSOR_INFORMATION>();
                    int count = (int)returnLength / structSize;

                    for (int i = 0; i < count; i++)
                    {
                        IntPtr currentPtr = IntPtr.Add(buffer, i * structSize);
                        var info = Marshal.PtrToStructure<ThreadCheck.SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(currentPtr);

                        if (info.Relationship == ThreadCheck.LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore)
                            coreCount++;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            return coreCount;
        }
    }
}