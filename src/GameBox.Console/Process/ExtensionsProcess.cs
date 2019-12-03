/*
 * This file is part of the GameBox package. Borrow from: https://github.com/Microsoft/msbuild
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SProcess = System.Diagnostics.Process;

namespace GameBox.Console.Process
{
    /// <summary>
    /// The process extensions method.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class ExtensionsProcess
    {
        /// <summary>
        /// Kill the process tree.
        /// </summary>
        public static void KillTree(this SProcess process, int timeout)
        {
            if (NativeMethodsShared.IsWindows)
            {
                try
                {
                    // issue the kill command
                    NativeMethodsShared.KillTree(process.Id);
                }
                catch (InvalidOperationException)
                {
                    // The process already exited, which is fine,
                    // just continue.
                }
            }
            else
            {
                var children = new HashSet<int>();
                GetAllChildIdsUnix(process.Id, children);
                foreach (var childId in children)
                {
                    KillProcessUnix(childId);
                }

                KillProcessUnix(process.Id);
            }

            // wait until the process finishes exiting/getting killed.
            // We don't want to wait forever here because the task is already supposed to be dieing, we just want to give it long enough
            // to try and flush what it can and stop. If it cannot do that in a reasonable time frame then we will just ignore it.
            process.WaitForExit(timeout);
        }

        private static void GetAllChildIdsUnix(int parentId, ISet<int> children)
        {
            var exitCode = RunProcessAndWaitForExit(
                "pgrep",
                $"-P {parentId}",
                out string stdout,
                out _);

            if (exitCode == 0 && !string.IsNullOrEmpty(stdout))
            {
                using (var reader = new StringReader(stdout))
                {
                    while (true)
                    {
                        var text = reader.ReadLine();
                        if (text == null)
                        {
                            return;
                        }

                        if (int.TryParse(text, out int id))
                        {
                            children.Add(id);

                            // Recursively get the children
                            GetAllChildIdsUnix(id, children);
                        }
                    }
                }
            }
        }

        private static void KillProcessUnix(int processId)
        {
            RunProcessAndWaitForExit(
                "kill",
                $"-TERM {processId}",
                out _,
                out _);
        }

        private static int RunProcessAndWaitForExit(string fileName, string arguments, out string stdout, out string stderr)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };

            var process = SProcess.Start(startInfo);

            stdout = null;
            stderr = null;
            if (process.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds))
            {
                stdout = process.StandardOutput.ReadToEnd();
                stderr = process.StandardError.ReadToEnd();
            }
            else
            {
                process.Kill();

                // Kill is asynchronous so we should still wait a little
                process.WaitForExit((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            }

            return process.HasExited ? process.ExitCode : -1;
        }
    }
}
