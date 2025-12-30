using System;
using System.Reflection;

namespace BlueToque.Utility
{
    public readonly struct TypeName(string type, string assembly = "", string version = "", string culture = "", string key = "")
    {
        public readonly string Type = type.Trim();

        public readonly string Assembly = assembly.Trim();

        public readonly string Version = version.Trim();

        public readonly string Culture = culture.Trim();

        public readonly string PublicKey = key.Trim();

        public readonly bool IsEmpty => Type.IsNullOrEmpty() && Assembly.IsNullOrEmpty() && Version.IsNullOrEmpty();

        public readonly bool TypeOnly => !Type.IsNullOrEmpty() && Assembly.IsNullOrEmpty() && Version.IsNullOrEmpty();

        public readonly bool TypeAssembly => !Type.IsNullOrEmpty() && !Assembly.IsNullOrEmpty();

        public readonly string VersionString => $"{Version}, {Culture}, {PublicKey}";

        public readonly string AssemblyQualifiedName => $"{Type}, {Assembly}, {Version}, {Culture}, {PublicKey}";

        internal static TypeName FromAssemblyQualifiedName(string assemblyQualifiedName)
        {
            if (assemblyQualifiedName.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(assemblyQualifiedName));
            string[] parts = assemblyQualifiedName.Split(',');

            return parts.Length switch
            {
                0 => new TypeName(),
                1 => new TypeName(parts[0]),
                2 => new TypeName(parts[0], parts[1]),
                3 => new TypeName(parts[0], parts[1], parts[2]),
                4 => new TypeName(parts[0], parts[1], parts[2], parts[3]),
                _ => new TypeName(parts[0], parts[1], parts[2], parts[3], parts[4]),
            };
        }
    }

    public static class AssemblyHelper
    {
        /// <summary>
        /// Get the assembly short name.
        /// If the fullname is 
        ///     BlueToque.Utility, Version=1.21.501.0, Culture=neutral, PublicKeyToken=545b5d548687123f 
        /// the short name will be 
        ///     BlueToque.Utility
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static string? ShortName(this Assembly asm) => asm.FullName?.GetName(',');

        /// <summary>
        /// Convert a Assembly Qualified Name to a structure with three components
        ///  * Type
        ///  * Assembly
        ///  * Version
        /// </summary>
        /// <param name="assemblyQualifiedName"></param>
        /// <returns></returns>
        public static TypeName GetTypeName(string assemblyQualifiedName) =>
            TypeName.FromAssemblyQualifiedName(assemblyQualifiedName);

    }
}
