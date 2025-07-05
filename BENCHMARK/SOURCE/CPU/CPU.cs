using Microsoft.Win32;

namespace BENCHMARK.SOURCE.CPU
{
    // детект модели процессора без WMI

    internal static class CPU
    {
        public static readonly string Name = Detect();

        private static string Detect()
        {
            const string regPath = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(regPath, writable: false);
                if (key?.GetValue("ProcessorNameString") is string s && !string.IsNullOrWhiteSpace(s))
                    return s.Trim();
            }
            catch
            { //
            }

            var env = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            return string.IsNullOrWhiteSpace(env) ? "Unknown CPU" : env.Trim();
        }
    }
}