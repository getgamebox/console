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

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SProcess = System.Diagnostics.Process;

namespace GameBox.Console.Process
{
    /// <summary>
    /// Interop methods.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class NativeMethodsShared
    {
#pragma warning disable SA1310
        internal const uint ERROR_ACCESS_DENIED = 0x5;
#pragma warning restore SA1300

        private enum PROCESSINFOCLASS
        {
            ProcessBasicInformation = 0,
            ProcessQuotaLimits,
            ProcessIoCounters,
            ProcessVmCounters,
            ProcessTimes,
            ProcessBasePriority,
            ProcessRaisePriority,
            ProcessDebugPort,
            ProcessExceptionPort,
            ProcessAccessToken,
            ProcessLdtInformation,
            ProcessLdtSize,
            ProcessDefaultHardErrorMode,
            ProcessIoPortHandlers, // Note: this is kernel mode only
            ProcessPooledUsageAndLimits,
            ProcessWorkingSetWatch,
            ProcessUserModeIOPL,
            ProcessEnableAlignmentFaultFixup,
            ProcessPriorityClass,
            ProcessWx86Information,
            ProcessHandleCount,
            ProcessAffinityMask,
            ProcessPriorityBoost,
            MaxProcessInfoClass,
        }

#pragma warning disable SA1300
#pragma warning disable IDE1006
        private enum eDesiredAccess
#pragma warning restore IDE1006
#pragma warning restore SA1300
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,
            STANDARD_RIGHTS_ALL = 0x001F0000,

            PROCESS_TERMINATE = 0x0001,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_SET_SESSIONID = 0x0004,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_ALL_ACCESS = SYNCHRONIZE | 0xFFF,
        }

        /// <summary>
        /// Gets a value indicating whether if we are running under Linux.
        /// </summary>
        internal static bool IsLinux
        {
            get { return RuntimeInformation.IsOSPlatform(OSPlatform.Linux); }
        }

        /// <summary>
        /// Gets a value indicating whether if we are running under flavor of BSD (NetBSD, OpenBSD, FreeBSD).
        /// </summary>
        internal static bool IsBSD
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD")) ||
                       RuntimeInformation.IsOSPlatform(OSPlatform.Create("NETBSD")) ||
                       RuntimeInformation.IsOSPlatform(OSPlatform.Create("OPENBSD"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether if we are running under Mac OSX.
        /// </summary>
        internal static bool IsOSX
        {
            get { return RuntimeInformation.IsOSPlatform(OSPlatform.OSX); }
        }

        /// <summary>
        /// Gets a value indicating whether if we are running under a Unix-like system (Mac, Linux, etc.).
        /// </summary>
        internal static bool IsUnixLike { get; } = IsLinux || IsOSX || IsBSD;

        /// <summary>
        /// Gets a value indicating whether if we are running under some version of Windows.
        /// </summary>
        internal static bool IsWindows
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
        }

        /// <summary>
        /// Kills the specified process by id and all of its children recursively.
        /// </summary>
        internal static void KillTree(int processIdToKill)
        {
            // Note that GetProcessById does *NOT* internally hold on to the process handle.
            // Only when you create the process using the Process object
            // does the Process object retain the original handle.
            SProcess thisProcess;
            try
            {
                thisProcess = SProcess.GetProcessById(processIdToKill);
            }
            catch (ArgumentException)
            {
                // The process has already died for some reason.  So shrug and assume that any child processes
                // have all also either died or are in the process of doing so.
                return;
            }

            try
            {
                var myStartTime = thisProcess.StartTime;

                // Grab the process handle.  We want to keep this open for the duration of the function so that
                // it cannot be reused while we are running.
                var hProcess = OpenProcess(eDesiredAccess.PROCESS_QUERY_INFORMATION, false, processIdToKill);
                if (hProcess.IsInvalid)
                {
                    return;
                }

                try
                {
                    try
                    {
                        // Kill this process, so that no further children can be created.
                        thisProcess.Kill();
                    }
                    catch (Win32Exception e)
                    {
                        // Access denied is potentially expected -- it happens when the process that
                        // we're attempting to kill is already dead.  So just ignore in that case.
                        if (e.NativeErrorCode != ERROR_ACCESS_DENIED)
                        {
                            throw;
                        }
                    }

                    // Now enumerate our children.  Children of this process are any process which has this process id as its parent
                    // and which also started after this process did.
                    var children = GetChildProcessIds(processIdToKill, myStartTime);

                    try
                    {
                        foreach (KeyValuePair<int, SafeProcessHandle> childProcessInfo in children)
                        {
                            KillTree(childProcessInfo.Key);
                        }
                    }
                    finally
                    {
                        foreach (KeyValuePair<int, SafeProcessHandle> childProcessInfo in children)
                        {
                            childProcessInfo.Value.Dispose();
                        }
                    }
                }
                finally
                {
                    // Release the handle.  After this point no more children of this process exist and this process has also exited.
                    hProcess.Dispose();
                }
            }
            finally
            {
                thisProcess.Dispose();
            }
        }

        /// <summary>
        /// Returns an array of all the immediate child processes by id.
        /// NOTE: The IntPtr in the tuple is the handle of the child process.  CloseHandle MUST be called on this.
        /// </summary>
        internal static List<KeyValuePair<int, SafeProcessHandle>> GetChildProcessIds(int parentProcessId, DateTime parentStartTime)
        {
            var myChildren = new List<KeyValuePair<int, SafeProcessHandle>>();

            foreach (var possibleChildProcess in SProcess.GetProcesses())
            {
                using (possibleChildProcess)
                {
                    // Hold the child process handle open so that children cannot die and restart with a different parent after we've started looking at it.
                    // This way, any handle we pass back is guaranteed to be one of our actual children.
                    var childHandle = OpenProcess(eDesiredAccess.PROCESS_QUERY_INFORMATION, false, possibleChildProcess.Id);
                    if (childHandle.IsInvalid)
                    {
                        continue;
                    }

                    bool keepHandle = false;
                    try
                    {
                        if (possibleChildProcess.StartTime > parentStartTime)
                        {
                            int childParentProcessId = GetParentProcessId(possibleChildProcess.Id);
                            if (childParentProcessId != 0 && parentProcessId == childParentProcessId)
                            {
                                // Add this one
                                myChildren.Add(new KeyValuePair<int, SafeProcessHandle>(possibleChildProcess.Id, childHandle));
                                keepHandle = true;
                            }
                        }
                    }
                    finally
                    {
                        if (!keepHandle)
                        {
                            childHandle.Dispose();
                        }
                    }
                }
            }

            return myChildren;
        }

        /// <summary>
        /// Returns the parent process id for the specified process.
        /// Returns zero if it cannot be gotten for some reason.
        /// </summary>
        internal static int GetParentProcessId(int processId)
        {
            int parentId = 0;
#if !CLR2COMPATIBILITY
            if (IsUnixLike)
            {
                string line = null;

                try
                {
                    // /proc/<processID>/stat returns a bunch of space separated fields. Get that string
                    using (var r = OpenRead("/proc/" + processId + "/stat"))
                    {
                        line = r.ReadLine();
                    }
                }
#pragma warning disable CA1031
                catch
#pragma warning restore CA1031
                {
                    // Ignore errors since the process may have terminated
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    // One of the fields is the process name. It may contain any characters, but since it's
                    // in parenthesis, we can finds its end by looking for the last parenthesis. After that,
                    // there comes a space, then the second fields separated by a space is the parent id.
                    string[] statFields = line.Substring(line.LastIndexOf(')')).Split(new[] { ' ' }, 4);
                    if (statFields.Length >= 3)
                    {
                        parentId = int.Parse(statFields[2]);
                    }
                }
            }
            else
#endif
            {
                var hProcess = OpenProcess(eDesiredAccess.PROCESS_QUERY_INFORMATION, false, processId);

                if (!hProcess.IsInvalid)
                {
                    try
                    {
                        // UNDONE: NtQueryInformationProcess will fail if we are not elevated and other process is. Advice is to change to use ToolHelp32 API's
                        // For now just return zero and worst case we will not kill some children.
                        var pbi = default(PROCESS_BASIC_INFORMATION);
                        int pSize = 0;

                        if (NtQueryInformationProcess(hProcess, PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, pbi.Size, ref pSize) == 0)
                        {
                            parentId = (int)pbi.InheritedFromUniqueProcessId;
                        }
                    }
                    finally
                    {
                        hProcess.Dispose();
                    }
                }
            }

            return parentId;
        }

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Class name is NativeMethodsShared for increased clarity")]
        [DllImport("NTDLL.DLL")]
        private static extern int NtQueryInformationProcess(SafeProcessHandle hProcess, PROCESSINFOCLASS pic, ref PROCESS_BASIC_INFORMATION pbi, uint cb, ref int pSize);

        [SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Class name is NativeMethodsShared for increased clarity")]
        [DllImport("KERNEL32.DLL")]
        private static extern SafeProcessHandle OpenProcess(eDesiredAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        private static StreamReader OpenRead(string path, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = true)
        {
            const int DefaultFileStreamBufferSize = 4096;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultFileStreamBufferSize, FileOptions.SequentialScan);
            if (encoding == null)
            {
                return new StreamReader(fileStream);
            }
            else
            {
                return new StreamReader(fileStream, encoding, detectEncodingFromByteOrderMarks);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
#pragma warning disable S101
        private struct PROCESS_BASIC_INFORMATION
#pragma warning restore S101
        {
            public uint ExitStatus;
            public IntPtr PebBaseAddress;
            public UIntPtr AffinityMask;
            public int BasePriority;
            public UIntPtr UniqueProcessId;
            public UIntPtr InheritedFromUniqueProcessId;

#pragma warning disable CA1822
            public uint Size
#pragma warning restore CA1822
            {
                get
                {
                    unsafe
                    {
                        return (uint)sizeof(PROCESS_BASIC_INFORMATION);
                    }
                }
            }
        }
    }
}
