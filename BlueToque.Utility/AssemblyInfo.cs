using System;
using System.Linq;
using System.Reflection;

namespace BlueToque.Utility
{
    /// <summary>
    /// Get assembly metadata
    /// </summary>
    public class AssemblyInfo
    {

        /// <summary>
        /// By default get the info for the entry assembly
        /// </summary>
        public AssemblyInfo()
        {
            var assembly = Assembly.GetEntryAssembly() ?? throw new Exception("could not find entry assembly");
            Assembly = assembly;
        }

        /// <summary>
        /// Get the assembly info the assembly the given type is in
        /// </summary>
        /// <param name="type"></param>
        public AssemblyInfo(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            Assembly = type.Assembly;
        }

        /// <summary>
        /// Get the assembly the given object is defined in
        /// </summary>
        /// <param name="obj"></param>
        public AssemblyInfo(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            Assembly = obj.GetType().Assembly;
        }

        /// <summary>
        /// Get the assembly info for the given assembly
        /// </summary>
        /// <param name="assembly"></param>
        public AssemblyInfo(Assembly? assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            Assembly = assembly;
        }

        #region properties

        #region singleton interface

        static AssemblyInfo? s_info;

        public static AssemblyInfo Info => s_info ??= new AssemblyInfo();

        #endregion

        /// <summary>
        /// The assembly we're getting metadata from
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// The version string
        /// </summary>
        public string? Version => Assembly.GetName()?.Version?.ToString();

        /// <summary>
        /// The file version string
        /// </summary>
        public string FileVersion
        {
            get
            {
                AssemblyFileVersionAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Version;
            }
        }

        /// <summary>
        /// The product version string
        /// </summary>
        public string ProductVersion
        {
            get
            {
                AssemblyVersionAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyVersionAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Version;
            }
        }

        /// <summary>
        /// The product string
        /// </summary>
        public string Product
        {
            get
            {
                AssemblyProductAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyProductAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Product;
            }
        }

        /// <summary>
        /// The description
        /// </summary>
        public string Description
        {
            get
            {
                AssemblyDescriptionAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Description;
            }
        }

        /// <summary>
        /// The title
        /// </summary>
        public string Title
        {
            get
            {
                AssemblyTitleAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyTitleAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Title;
            }
        }

        /// <summary>
        /// The company
        /// </summary>
        public string Company
        {
            get
            {
                AssemblyCompanyAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Company;
            }
        }

        /// <summary>
        /// Copyright
        /// </summary>
        public string Copyright
        {
            get
            {
                AssemblyCopyrightAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Copyright;
            }
        }

        /// <summary>
        /// Trademark
        /// </summary>
        public string TradeMark
        {
            get
            {
                AssemblyTrademarkAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyTrademarkAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Trademark;
            }
        }

        /// <summary>
        /// The configuration string
        /// </summary>
        public string Configuration
        {
            get
            {
                AssemblyConfigurationAttribute? customAttribute = Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
                return (customAttribute == null) ? string.Empty : customAttribute.Configuration;
            }
        }

        #endregion

        /// <summary>
        /// Create an assembly info from a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AssemblyInfo Create(Type type) => new(type);

        /// <summary>
        /// Create an assemblyInfo from an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static AssemblyInfo Create(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new AssemblyInfo(obj.GetType());
        }

        /// <summary>
        /// Create an assemblyInfo from an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyInfo Create(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            return new AssemblyInfo(assembly);
        }

        public static string? GetMetaData(Type type, string field) =>
            type.Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Where(x => x.Key == field).FirstOrDefault()?.Value;

    }
}