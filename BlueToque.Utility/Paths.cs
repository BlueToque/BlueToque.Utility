using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BlueToque.Utility
{
    public static class Paths
    {

        /// <summary>
        /// A tag/value tuple
        /// </summary>
        public struct PathTag
        {
            //
            // Summary:
            //     The path tag
            public string Tag;

            //
            // Summary:
            //     The value of the tag
            public string? Value;

            //
            // Summary:
            //     The description
            public string? Description;

            //
            // Summary:
            //     A function to get a path string
            public Func<string, string> GetPathString;

            //
            // Parameters:
            //   tag:
            //
            //   val:
            //
            //   description:
            public PathTag(string tag, string val, string? description = "")
            {
                Tag = tag;
                Value = val;
                GetPathString = GetPathStringInternal;
                Description = description;
            }

            readonly string GetPathStringInternal(string val) => val;

            //
            // Parameters:
            //   tag:
            //
            //   del:
            //
            //   description:
            public PathTag(string tag, Func<string, string> del, string description = "")
            {
                Tag = tag;
                Value = null;
                GetPathString = del;
                Description = description;
            }

            public override readonly string ToString() => $"{Tag}:{Value} ({Description})";
        }

        #region constants

        public static class DirectoryTags
        {
            /// <summary>
            ///     Common application data shared among all users
            /// </summary>
            public const string CommonApplicationData = "{CommonApplicationData}";

            /// <summary>
            /// Local user application data
            /// </summary>
            public const string ApplicationData = "{ApplicationData}";

            /// <summary>
            ///     My Documents
            /// </summary>
            public const string MyDocuments = "{MyDocuments}";

            /// <summary>
            ///     Startup folder: path to the executable
            /// </summary>
            public const string ApplicationDirectory = "{ApplicationDirectory}";

            /// <summary>
            ///     The Personal data folder, or "My Documents
            /// </summary>
            public const string Personal = "{Personal}";

            /// <summary>
            ///     Common documents visible to all users
            /// </summary>
            public const string CommonDocuments = "{CommonDocuments}";

            /// <summary>
            ///     User profile directory
            /// </summary>
            public const string UserProfile = "{UserProfile}";

            /// <summary>
            ///     The windows directory
            /// </summary>
            public const string Windows = "{Windows}";

            /// <summary>
            ///     The system directory
            /// </summary>
            public const string System = "{System}";

            /// <summary>
            ///     The Temp directory
            /// </summary>
            public const string Temp = "{Temp}";

            /// <summary>
            ///     A filesystem safe date directory
            /// </summary>
            public const string Date = "{Date}";

            /// <summary>
            ///     A filesystem safe time directory
            /// </summary>
            public const string Time = "{Time}";

            /// <summary>
            ///     A filesystem safe datetime directory
            /// </summary>
            public const string DateTime = "{DateTime}";
        }

        ///// <summary>
        ///// Local user application data
        ///// </summary>
        //public const string APPLICATION_DATA_TAG = "{ApplicationData}";

        ///// <summary>
        /////     Common application data shared among all users
        ///// </summary>
        //public const string COMMONAPPLICATION_DATA_TAG = "{CommonApplicationData}";

        ///// <summary>
        /////     My Documents
        ///// </summary>
        //public const string MY_DOCUMENTS_TAG = "{MyDocuments}";

        ///// <summary>
        /////     Startup folder: path to the executable
        ///// </summary>
        //public const string APPLICATION_DIRECTORY_TAG = "{ApplicationDirectory}";

        ///// <summary>
        /////     The Personal data folder, or "My Documents
        ///// </summary>
        //public const string PERSONAL_TAG = "{Personal}";

        ///// <summary>
        /////     Common documents visible to all users
        ///// </summary>
        //public const string COMMON_DOCUMENTS_TAG = "{CommonDocuments}";

        ///// <summary>
        /////     User profile directory
        ///// </summary>
        //public const string USER_PROFILE_TAG = "{UserProfile}";

        ///// <summary>
        /////     The windows directory
        ///// </summary>
        //public const string WINDOWS_TAG = "{Windows}";

        ///// <summary>
        /////     The system directory
        ///// </summary>
        //public const string SYSTEM_TAG = "{System}";

        ///// <summary>
        /////     The temporary directory
        ///// </summary>
        //public const string TEMP_TAG = "{Temp}";

        ///// <summary>
        /////     The date tag is used to replace the tag with the current date
        ///// </summary>
        //public const string DATE_TAG = "{Date}";

        ///// <summary>
        /////     The time tag is used to replace the tag with the current time
        ///// </summary>
        //public const string TIME_TAG = "{Time}";

        ///// <summary>
        /////     DateTime tag replaces the path tag with the current date and time the string
        /////     is made safe for paths, files and directory names
        ///// </summary>
        //public const string DATETIME_TAG = "{DateTime}";

        #endregion

        #region properties

        /// <summary>
        ///     List of tags
        /// </summary>
        public static List<PathTag> Tags { get; private set; }

        #endregion

        #region properties

        //
        // Summary:
        //     Gets the path for the executable file that started the application, including
        //     the executable name
        public static string ApplicationDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? AppDomain.CurrentDomain.BaseDirectory;

        //
        // Summary:
        //     Gets the name of the entry application
        public static string? ApplicationName => Assembly.GetEntryAssembly()?.FullName?.Split(',')[0];

        /// <summary>
        /// The company attribute 
        /// </summary>
        public static string ApplicationCompany => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;

        //
        // Summary:
        //     Gets the version of the entry application
        public static string? ApplicationVersion => Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

        //
        // Summary:
        //     Gets the name of the entry application
        public static string? AssemblyName => Assembly.GetCallingAssembly()?.FullName?.Split(',')[0];

        //
        // Summary:
        //     Gets the version of the calling application
        public static string? AssemblyVersion => Assembly.GetCallingAssembly()?.GetName()?.Version?.ToString();

        //
        // Summary:
        //     Gets the path for the application data that is shared among all users
        public static string CommonApplicationData => ApplicationName == null ? string.Empty : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ApplicationCompany, ApplicationName);

        //
        // Summary:
        //     Gets the path for the application data of a local, non-roaming user
        public static string ApplicationData => ApplicationName == null ? string.Empty : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationCompany, ApplicationName);

        //
        // Summary:
        //     Gets the path for the application data of the My Documents folder
        public static string MyDocuments => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        //
        // Summary:
        //     The directory that serves as a common repository for documents C:\Users\User\Documents
        public static string Personal => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        //
        // Summary:
        //     CommonDocuments = C:\Users\Public\Documents
        public static string CommonDocuments => Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        //
        // Summary:
        //     UserProfile = C:\Users\UserName
        public static string UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        //
        // Summary:
        //     C:\Windows
        public static string Windows => Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        //
        // Summary:
        //     C:\Windows\system32
        public static string System => Environment.GetFolderPath(Environment.SpecialFolder.System);

        //
        // Summary:
        //     The temporary directory
        public static string Temp => Path.GetTempPath().TrimEnd('\\');

        #endregion

        /// <summary>
        /// Static constructor 
        /// Set up the tag replacement array
        /// </summary>
        static Paths() =>
            Tags = [
                new PathTag(DirectoryTags.CommonApplicationData, CommonApplicationData, "Common Application Data"),
                new PathTag(DirectoryTags.ApplicationData, ApplicationData, "Application Data"),
                new PathTag(DirectoryTags.MyDocuments, MyDocuments, "My Documents"),
                new PathTag(DirectoryTags.ApplicationDirectory, ApplicationDirectory, "Application Directory"),
                new PathTag(DirectoryTags.Personal, Personal, "User Directory"),
                new PathTag(DirectoryTags.CommonDocuments, CommonDocuments, "Common Documents"),
                new PathTag(DirectoryTags.UserProfile, UserProfile, "User Profile"),
                new PathTag(DirectoryTags.Windows, Windows, "Windows Directory"),
                new PathTag(DirectoryTags.System, System, "System Directory"),
                new PathTag(DirectoryTags.Temp, Temp, "Temporary Directory"),
                new PathTag(DirectoryTags.Date, DateString, "Current Date"),
                new PathTag(DirectoryTags.Time, TimeString, "Current Time"),
                new PathTag(DirectoryTags.DateTime, DateTimeString, "Current Date / Time")
            ];

        //
        // Summary:
        //     Returns a path safe date string
        //
        // Parameters:
        //   tag:
        private static string DateString(string tag)
        {
            string text = DateTime.Now.ToShortDateString().Replace("/", "_");
            char[] invalidPathChars = Path.GetInvalidPathChars();
            foreach (char oldChar in invalidPathChars)
                text = text.Replace(oldChar, '_');

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char oldChar2 in invalidFileNameChars)
                text = text.Replace(oldChar2, '_');

            return text;
        }

        //
        // Summary:
        //     returns a path safe time string
        //
        // Parameters:
        //   tag:
        private static string TimeString(string tag)
        {
            string text = DateTime.Now.ToShortTimeString().Replace(":", "_");
            char[] invalidPathChars = Path.GetInvalidPathChars();
            foreach (char oldChar in invalidPathChars)
                text = text.Replace(oldChar, '_');

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char oldChar2 in invalidFileNameChars)
                text = text.Replace(oldChar2, '_');

            return text;
        }

        //
        // Summary:
        //     returns a path safe datetime string
        //
        // Parameters:
        //   tag:
        private static string DateTimeString(string tag) => $"{DateString(tag)}T{TimeString(tag)}";

        //
        // Summary:
        //     Get the parent directory of the given path
        //
        // Parameters:
        //   path:
        internal static string? GetParent(string path) => Directory.GetParent(path)?.ToString();

        //
        // Summary:
        //     Register a tag in the list of valid tags
        //
        // Parameters:
        //   tag:
        //
        //   value:
        public static void Register(string tag, string value, string description = "")
        {
            ArgumentException.ThrowIfNullOrEmpty(tag);
            ArgumentException.ThrowIfNullOrEmpty(value);

            try
            {
                if (Tags.FindIndex(x => x.Tag == tag) >= 0)
                    throw new ArgumentException("Tag already exists in collection");

                Tags.Add(new PathTag(tag, value, description));
            }
            catch { }
        }

        //
        // Summary:
        //     Register a tag in the Paths replacement structure
        //
        // Parameters:
        //   tag:
        //
        //   del:
        public static void Register(string tag, Func<string, string> del, string description = "")
        {
            ArgumentException.ThrowIfNullOrEmpty(tag);
            ArgumentNullException.ThrowIfNull(del);

            try
            {
                if (Tags.FindIndex(x => x.Tag == tag) >= 0)
                    throw new ArgumentException("Tag already exists in collection");

                Tags.Add(new PathTag(tag, del, description));
            }
            catch { }
        }

        //
        // Summary:
        //     Expand a tag into the correct directory name
        //
        // Parameters:
        //   path:
        public static string Expand(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            foreach (PathTag s_tag in Tags)
                path = s_tag.Value == null ? path.Trim().Replace(s_tag.Tag, s_tag.GetPathString(s_tag.Tag)) : path.Trim().Replace(s_tag.Tag, s_tag.Value);

            return path;
        }

        //
        // Summary:
        //     Copy one directory to another, recursively
        //
        // Parameters:
        //   source:
        //
        //   destination:
        public static void CopyDirectory(string source, string destination)
        {
            EnsureDirectoryExists(destination);

            string[] fileSystemEntries = Directory.GetFileSystemEntries(source);
            string[] array = fileSystemEntries;
            foreach (string text in array)
            {
                if (Directory.Exists(text))
                    CopyDirectory(text, Path.Combine(destination, Path.GetFileName(text)));
                else
                    File.Copy(text, Path.Combine(destination, Path.GetFileName(text)), overwrite: true);
            }
        }

        /// <summary>
        /// Ensure the given path exists
        /// Do not pass file names.
        /// </summary>
        /// <param name="path"></param>
        public static string EnsureDirectoryExists(string path, bool expand = true)
        {
            if (expand)
                path = Expand(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

    }
}