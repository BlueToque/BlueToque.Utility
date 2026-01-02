using System;
using System.Reflection;

namespace BlueToque.Utility
{
    /// <summary>
    /// https://csharpindepth.com/articles/singleton#lazy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazySingleton<T> where T : class
    {
        private static readonly Lazy<T> lazy = new(() => CreateInstance());

        private static T CreateInstance()
        {
            ConstructorInfo? constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null) ?? throw new ApplicationException($"{typeof(T)} has a public constructor");
            // alternatively, throw an exception indicating the type parameter
            // should have a private parameterless constructor

            if (constructorInfo == null) 
                throw new ArgumentNullException(nameof(constructorInfo));

            if (constructorInfo.Invoke(null) is not T instance) 
                throw new ArgumentNullException(nameof(instance));
            
            return instance;
        }

        public static T Instance => lazy.Value;

        private LazySingleton()
        {
        }
    }
}
