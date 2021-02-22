using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Recipe.Api.Util {
    public static class BashCommandHelper {
        public static string Bash(this string cmd) {
            bool isWindows = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows);
            string terminal = "/bin/bash";
            if (isWindows)
                terminal = "cmd.exe";
            Console.WriteLine(terminal);
            var escapedArgs = cmd.Replace("\"", "\\\"");
            var process = new Process() {
                StartInfo = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            return "Started Data Processor";
        }
    }
}