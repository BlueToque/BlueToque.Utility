using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueToque.Utility.Windows
{
    //
    // Summary:
    //     The operating system type
    public enum OSType
    {
        Unknown,
        Workstation,
        DomainController,
        Server,
        Mono
    }

    //
    // Summary:
    //     Some information about the operating system
    public static class OS
    {
        private static string s_currentCirectory = string.Empty;

        private static readonly bool MONO = System.Type.GetType("Mono.Runtime") != null;

        public static string CurrentDirectory
        {
            get
            {
                if (s_currentCirectory.IsNullOrEmpty())
                {
                    s_currentCirectory = Paths.Expand("{TrueNorthMaps}");
                }

                if (s_currentCirectory.IsNullOrEmpty())
                {
                    s_currentCirectory = Environment.CurrentDirectory;
                }

                return s_currentCirectory;
            }
            set
            {
                s_currentCirectory = value;
            }
        }

        //
        // Summary:
        //     True if we're running on Mono
        public static bool IsRunningOnMono => System.Type.GetType("Mono.Runtime") != null;

        //
        // Summary:
        //     http://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con
        public static bool IsLinux
        {
            get
            {
                int platform = (int)Environment.OSVersion.Platform;
                return platform == 4 || platform == 6 || platform == 128;
            }
        }

        //
        // Summary:
        //     Get the revision of the operating system
        public static string Revision
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return "Unknown";
                }

                NativeMethods.OSVERSIONINFOEX lpVersionInfo = new()
                {
                    dwOSVersionInfoSize = (uint)NativeMethods.OSVERSIONINFOEX.GetSizeOf()
                };

                if (NativeMethods.GetVersionEx(ref lpVersionInfo))
                {
                    return lpVersionInfo.szCSDVersion;
                }

                return "Unknown";
            }
        }

        //
        // Summary:
        //     Get the operating system type
        public static OSType Type
        {
            get
            {
                if (IsRunningOnMono)
                    return OSType.Mono;

                NativeMethods.OSVERSIONINFOEX lpVersionInfo = new()
                {
                    dwOSVersionInfoSize = (uint)NativeMethods.OSVERSIONINFOEX.GetSizeOf()
                };

                if (NativeMethods.GetVersionEx(ref lpVersionInfo))
                {
                    if (Enum.IsDefined(typeof(OSType), (OSType)lpVersionInfo.wProductType))
                        return (OSType)lpVersionInfo.wProductType;
                    return OSType.Unknown;
                }

                return OSType.Unknown;
            }
        }

        //
        // Summary:
        //     Is Windows 2000
        public static bool Is2K
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 5 && oSVersion.Version.Minor == 0;
            }
        }

        //
        // Summary:
        //     Are we running on Windows Vista or greater.
        public static bool IsVistaOrGreater
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                if (oSVersion.Platform != PlatformID.Win32NT)
                {
                    return false;
                }

                if (oSVersion.Version.Major >= 6 && oSVersion.Version.Minor > 0)
                {
                    return true;
                }

                return false;
            }
        }

        //
        // Summary:
        //     Are we running on Windows Vista or greater.
        public static bool IsVistaOrLess
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                if (oSVersion.Platform != PlatformID.Win32NT)
                {
                    return false;
                }

                if (oSVersion.Version.Major <= 6 && oSVersion.Version.Minor <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        //
        // Summary:
        //     Is Windows XP
        public static bool IsXP
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 5 && oSVersion.Version.Minor == 1;
            }
        }

        //
        // Summary:
        //     Is Windows Vista
        public static bool IsVista
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 6 && oSVersion.Version.Minor == 0;
            }
        }

        //
        // Summary:
        //     Is Windows 7
        public static bool IsWin7
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 6 && oSVersion.Version.Minor == 1;
            }
        }

        //
        // Summary:
        //     Is Windows 8
        public static bool IsWin8
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 6 && (oSVersion.Version.Minor == 2 || oSVersion.Version.Minor == 3);
            }
        }

        //
        // Summary:
        //     Is Windows 10
        public static bool IsWin10
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                OperatingSystem oSVersion = Environment.OSVersion;
                return oSVersion.Platform == PlatformID.Win32NT && oSVersion.Version.Major == 10;
            }
        }

        //
        // Summary:
        //     Try to figure out if this is a tablet PC
        public static bool IsTablet
        {
            get
            {
                if (IsRunningOnMono)
                {
                    return false;
                }

                int systemMetrics = NativeMethods.GetSystemMetrics(86);
                return systemMetrics != 0;
            }
        }

        //
        // Summary:
        //     The number of logical processors on this system
        public static int LogicalProcessors
        {
            get
            {
                try
                {
                    int num = 0;
                    foreach (ManagementBaseObject item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
                    {
                        if (int.TryParse(item["NumberOfLogicalProcessors"].ToString(), out int count))
                            num += count;
                    }

                    return num;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("OS.Cores: Error retrieving logical processors:\r\n{0}", ex);
                    return -1;
                }
            }
        }

        //
        // Summary:
        //     The number of processors cores on the system
        public static int Cores
        {
            get
            {
                try
                {
                    int num = 0;
                    foreach (ManagementBaseObject item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
                    {
                        if(int.TryParse(item["NumberOfCores"].ToString(), out int count))
                            num += count;
                    }

                    return num;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("OS.Cores: Error retrieving physical processor cores:\r\n{0}", ex);
                    return -1;
                }
            }
        }

        //
        // Summary:
        //     Number of physical processors in this sytem
        public static int PhysicalProcessors => Environment.ProcessorCount;

        //
        // Summary:
        //     Returns true if the current assembly/program is loaded into the 64 bit version
        //     fo the CLR
        public static bool Is64BitMode()
        {
            return Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        //
        // Summary:
        //     Is this system on mono
        public static bool IsMono()
        {
            return System.Type.GetType("Mono.Runtime") != null;
        }

        //
        // Summary:
        //     Function to determine which platform we're on
        public static string GetPlatform(string x32 = "x32")
        {
            if (MONO)
            {
                int platform = (int)Environment.OSVersion.Platform;
                if (platform == 4 || platform == 6 || platform == 128)
                {
                    return (IntPtr.Size == 4) ? "u32" : "u64";
                }

                return (IntPtr.Size == 4) ? x32 : "x64";
            }

            return (IntPtr.Size == 4) ? x32 : "x64";
        }

        private static bool CheckForRegValueEquals(string regKeyName, string regValueName)
        {
            using RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey(regKeyName, writable: false);
            object? obj = null; 
            if (registryKey != null)
                obj = registryKey.GetValue(regValueName);
            
            return obj != null && obj is int v && v == 1;
        }

        //
        // Summary:
        //     .NET 2.0
        //
        // Parameters:
        //   major:
        //
        //   minor:
        //
        //   build:
        private static bool IsDotNet2VersionInstalled(int major, int minor, int build)
        {
            string regValueName = $"Software\\Microsoft\\NET Framework Setup\\NDP\\v{major.ToString(CultureInfo.InvariantCulture)}.{minor.ToString(CultureInfo.InvariantCulture)}.{build.ToString(CultureInfo.InvariantCulture)}";
            return CheckForRegValueEquals("Install", regValueName);
        }

        //
        // Summary:
        //     Check if .NET 3 is installed
        //
        // Parameters:
        //   major:
        //
        //   minor:
        //
        //   build:
        private static bool IsDotNet3VersionInstalled(int major, int minor, int build)
        {
            bool flag = false;
            if (!flag)
                flag |= CheckForRegValueEquals($"Software\\Microsoft\\NET Framework Setup\\NDP\\v{major}.{minor}\\Setup", "InstallSuccess");

            if (!flag)
                flag |= CheckForRegValueEquals($"Software\\Wow6432Node\\Microsoft\\NET Framework Setup\\NDP\\v{major}.{minor}\\Setup", "InstallSuccess");

            return flag;
        }

        //
        // Summary:
        //     Return true if the given version of .NET is installed
        //
        // Parameters:
        //   major:
        //
        //   minor:
        //
        //   build:
        public static bool IsDotNetVersionInstalled(int major, int minor, int build)
        {
            bool flag = false;
            if (!flag)
                flag |= IsDotNet2VersionInstalled(major, minor, build);

            if (!flag)
                flag |= IsDotNet3VersionInstalled(major, minor, build);

            return flag;
        }

        //
        // Summary:
        //     Look for an assembly that starts with the filter, and replace it with a path
        //     that depends on whether we're running 32 or 64 bit OS
        //
        // Parameters:
        //   loading:
        //
        //   filter:
        //
        //   toLoad:
        public static Assembly? LoadAssemblyRedirect(string loading, string filter, string toLoad)
        {
            try
            {
                if (!loading.Contains(filter))
                    return null;

                if (loading.Contains("resources"))
                    return null;

                Trace.TraceVerbose("OS.AssemblyResolve: Resolving Assembly: {0}", loading);
                string text = Path.Combine(Paths.ApplicationDirectory, "Plugins", "Assemblies", GetPlatform(), toLoad);
                if (!File.Exists(text))
                {
                    text = Path.Combine(Paths.ApplicationDirectory, "Plugins", GetPlatform(), toLoad);
                    if (!File.Exists(text))
                    {
                        Trace.TraceError("OS.AssemblyResolve: assembly \"{0}\" does not exist", text);
                        return null;
                    }
                }

                return Assembly.LoadFile(text);
            }
            catch (Exception ex)
            {
                Trace.TraceError("OS.LoadAssembly: Error loading {0} assemblie:\r\n{1}", filter, ex);
                return null;
            }
        }

        //
        // Summary:
        //     This value can be computed separately and hard-coded into the application. The
        //     method is included to illustrate the technique.
        public static int EstimateMemoryUsageInMB()
        {
            long totalMemory = GC.GetTotalMemory(forceFullCollection: true);
            _ = GC.CollectionCount(0);
            long totalMemory2 = GC.GetTotalMemory(forceFullCollection: false);
            //Console.WriteLine("Did a GC occur while measuring?  {0}", num2 == GC.CollectionCount(0));
            long num3 = totalMemory2 - totalMemory;
            if (num3 < 0)
            {
                //Console.WriteLine("GC's occurred while measuring memory usage.  Try measuring again.");
                num3 = 1048576L;
            }

            int num = (int)(1 + (num3 >> 20));
            Console.WriteLine("Memory usage estimate: {0} bytes, rounded to {1} MB", num3, num);
            return num;
        }

        //
        // Summary:
        //     Send a command line to all the other running instances of TrueNorth
        //
        // Parameters:
        //   args:
        public static void SendCommandLine(string[] args)
        {
            try
            {
                Trace.TraceInformation("OS.SendCommandLine");
                string text = string.Join("|", args);
                if (string.IsNullOrEmpty(text))
                {
                    Trace.TraceWarning("OS.SendCommandLine: command line is null: returning");
                    return;
                }

                IEnumerable<IntPtr> enumerable = FindWindowsWithText("TrueNorth Geospatial");
                if (!enumerable.Any())
                {
                    Trace.TraceWarning("SingleInstance.SendCommandLine: could not find other instance of TrueNorth");
                    return;
                }

                foreach (IntPtr item in enumerable)
                {
                    IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
                    if (mainWindowHandle == item)
                    {
                        Trace.TraceInformation("SingleInstance.SendCommandLine: Not sending command line to this process");
                        continue;
                    }

                    Trace.TraceInformation("SingleInstance.SendCommandLine: Sending \"{0}\" to hwnd {1}", text, item);
                    MessagePump.SendWindowsStringMessage(item, 0, text);
                    NativeMethods.PostMessage(item, MessagePump.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("SingleInstance.SendCommandLine: Error passing command line to instance\r\n{0}", ex);
            }
        }

        //
        // Summary:
        //     Get the text for the window pointed to by hWnd
        public static string GetWindowText(IntPtr hWnd)
        {
            int windowTextLength = NativeMethods.GetWindowTextLength(hWnd);
            if (windowTextLength > 0)
            {
                StringBuilder stringBuilder = new(windowTextLength + 1);
                NativeMethods.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);
                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        //
        // Summary:
        //     Find all windows that contain the given title text
        //
        // Parameters:
        //   titleText:
        //     The text that the window title must contain.
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows((wnd, param) => GetWindowText(wnd).Contains(titleText));
        }

        //
        // Summary:
        //     Find all windows that match the given filter
        //
        // Parameters:
        //   filter:
        //     A delegate that returns true for windows that should be returned and false for
        //     windows that should not be returned
        private static IEnumerable<IntPtr> FindWindows(NativeMethods.EnumWindowsProc filter)
        {
            IntPtr zero = IntPtr.Zero;
            List<IntPtr> windows = [];
            NativeMethods.EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                    windows.Add(wnd);

                return true;
            }, IntPtr.Zero);
            return windows;
        }
    }

}
