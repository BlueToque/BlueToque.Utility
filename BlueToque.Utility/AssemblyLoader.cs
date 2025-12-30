using System;
using System.Runtime.Remoting;

namespace BlueToque.Utility
{
    //
    // Summary:
    //     Loads assemblies
    public static class AssemblyLoader
    {
        //
        // Summary:
        //     Get a type from the given assembly with the given name, return it cast as the
        //     given type This is used to get an type by name that implements in interface which
        //     is passed in the generic parameter.
        //
        // Parameters:
        //   assembly:
        //
        //   type:
        //
        // Type parameters:
        //   T:
        public static T? CreateInstance<T>(string assembly, string type)
        {
            try
            {
                ObjectHandle? objectHandle = Activator.CreateInstance(assembly, type);
                if (objectHandle == null)
                {
                    Trace.TraceError("AssemblyLoader.GetType<T>: Failed to load type {0} from assembly {1}", type, assembly);
                    return default;
                }

                object? obj = objectHandle.Unwrap();
                if (obj == null)
                {
                    Trace.TraceError("AssemblyLoader.GetType<T>: Failed to resolve type {0} from assembly {1}", type, assembly);
                    return default;
                }

                T val = (T)obj;
                if (val == null)
                {
                    Trace.TraceError("AssemblyLoader.GetType<T>: Loaded object is not of type {0}", typeof(T).ToString());
                    return default;
                }

                return val;
            }
            catch (Exception ex)
            {
                ExceptionManager.CriticalException(ex);
                return default;
            }
        }
    }

}
