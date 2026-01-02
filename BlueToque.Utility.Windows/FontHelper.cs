using System;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace BlueToque.Utility.Windows
{
    public static class FontHelper
    {
        /// <summary>
        /// Use this way
        /// myLabel.Font = new Font(Helpers.m_pfc.Families[0], 14, FontStyle.Regular);
        /// </summary>
        public static PrivateFontCollection Fonts { get; } = new();

        public static bool InstallFont(string path, bool system = false)
        {
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException($"Path {path} does not exist");

                if (system)
                {
                    // Try install the font.
                    int result = NativeMethods.AddFontResource(path);
                    int error = Marshal.GetLastWin32Error();
                    if (error != 0)
                        throw new Win32Exception(error);

                    Trace.TraceInformation((result == 0) ? "Font is already installed." : "Font installed successfully.");
                    return true;
                }
                else
                {
                    Fonts.AddFontFile(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Helper.InstallFont: Error installing font:\r\n{0}", ex);
                throw;
            }
        }
    }
}
