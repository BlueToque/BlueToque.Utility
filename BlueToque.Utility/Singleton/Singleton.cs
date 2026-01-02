using System;
using System.Reflection;

namespace BlueToque.Utility
{
    /// <summary>
    /// Derive from this class to create a singleton
    /// The requirement is that the contructor is not public so you cannot create an instance 
    /// except for via the CreateInstance method.
    /// This means you must declare a private or protected constructor for any derived classes.
    /// </summary>
    /// <typeparam name="T">The singleton type</typeparam>
    public class Singleton<T> where T : class
    {
        class SingletonCreator
        {
            static SingletonCreator() { }

            private static T CreateInstance()
            {
                ConstructorInfo? constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, Type.EmptyTypes, null) ?? throw new ApplicationException($"{typeof(T)} has a public constructor");
                // alternatively, throw an exception indicating the type parameter
                // should have a private parameterless constructor

                if (constructorInfo == null)
                    throw new ArgumentNullException(nameof(constructorInfo));

                if (constructorInfo.Invoke(null) is not T instance)
                    throw new ApplicationException("instance");
                return instance;
            }

            internal static readonly T s_instance = CreateInstance();
        }

        public static T Instance => SingletonCreator.s_instance;

    }
}
