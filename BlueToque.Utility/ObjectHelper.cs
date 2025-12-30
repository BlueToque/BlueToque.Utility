using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlueToque.Utility;

/// <summary>
/// Helper methods for objects
/// </summary>
public static class ObjectHelper
{

    /// <summary>
    /// Copy an object of a given type by cloning
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? Copy<T>(this T obj) => Clone(obj);

    /// <summary>
    /// Deep copy an object by serializing it using a binary formatter, and deserializing
    /// it into another instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? Clone<T>(T obj)
    {
        if (obj == null)
            return default;

        T? newObject = (T?)Activator.CreateInstance(obj.GetType());
        if (newObject == null)
            return default;

        foreach (var originalProp in obj.GetType().GetProperties())
            originalProp.SetValue(newObject, originalProp.GetValue(obj));

        return newObject;

        //if (!typeof(T).IsSerializable)
        //    throw new ArgumentException("The type must be serializable.", "source");

        //if (obj == null)
        //    return default(T);

        //using MemoryStream memoryStream = new MemoryStream();
        //BinaryFormatter binaryFormatter = new BinaryFormatter();
        //binaryFormatter.Serialize(memoryStream, obj);
        //memoryStream.Position = 0L;
        //return (T)binaryFormatter.Deserialize(memoryStream);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static T? DeepClone<T>(this T original, params object[] args) => original.DeepClone([], args);

    private static T? DeepClone<T>(this T? original, Dictionary<object, object> copies, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(original);
        Type type = original.GetType();
        if (copies.TryGetValue(original, out var value))
            return (T)value;

        T? val;
        if (!type.IsArray)
        {
            val = (T?)Activator.CreateInstance(type, args);
            if (val == null)
                return default;
            copies.Add(original, val);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (FieldInfo fieldInfo in fields)
            {
                object? obj = fieldInfo.GetValue(original);
                if (obj == null) continue;

                Type fieldType = fieldInfo.FieldType;
                if (obj != null && !fieldType.IsValueType && fieldType != typeof(string))
                    obj = obj.DeepClone(copies);

                fieldInfo.SetValue(val, obj);
            }
        }
        else
        {
            Array array = (Array)(object)original;
            Array array2 = (Array)array.Clone();
            copies.Add(original, array2);

            var elementType = type.GetElementType();
            if (elementType == null)
                return default;

            if (!elementType.IsValueType)
            {
                int[] array3 = new int[type.GetArrayRank()];
                int[] array4 = new int[array3.Length];
                for (int j = 0; j < array3.Length; j++)
                    array3[j] = array2.GetLength(j);

                int p = array3.Length - 1;
                while (Increment(array4, array3, p))
                {
                    object? value2 = array2.GetValue(array4);
                    if (value2 != null)
                        array2.SetValue(value2.DeepClone(copies), array4);
                }
            }

            val = (T)(object)array2;
        }

        return val;
    }

    private static bool Increment(int[] indicies, int[] lengths, int p)
    {
        if (p > -1)
        {
            indicies[p]++;
            if (indicies[p] < lengths[p])
                return true;

            if (Increment(indicies, lengths, p - 1))
            {
                indicies[p] = 0;
                return true;
            }

            return false;
        }

        return false;
    }

    /// <summary>
    /// http://www.csharpbydesign.com/2008/03/generic-c-object-copy-function.html Copy
    /// an object to destination object, only matching fields will be copied
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceObject">An object with matching fields of the destination object</param>
    /// <param name="destObject">Destination object, must already be created</param>
    public static void CopyObject<T>(object sourceObject, ref T destObject)
    {
        if (sourceObject == null || destObject == null)
            return;

        Type type = sourceObject.GetType();
        Type type2 = destObject.GetType();
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            PropertyInfo? property = type2.GetProperty(propertyInfo.Name);
            if (property == null) continue;
            property?.SetValue(destObject, propertyInfo.GetValue(sourceObject, null), null);
        }
    }

    /// <summary>
    /// Parse a string in the form of name=value,name=value... Where "name" is the same
    ///  as a parameter name, and value is the "ToString" representation of that
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nameValue"></param>
    /// <param name="elementSeperator"></param>
    /// <param name="pairSeperator"></param>
    /// <returns></returns>
    public static T? FromNameValue<T>(string nameValue, char elementSeperator = ',', char pairSeperator = '=') where T : new()
    {
        if (nameValue.IsNullOrEmpty())
            return default;

        List<string> list = nameValue.UnescapeUri().FromDelimited(elementSeperator);
        T val = new();
        Type typeFromHandle = typeof(T);
        try
        {
            foreach (string item in list)
            {
                string[] array = item.Split(pairSeperator);
                if (array == null || array.Length != 2)
                {
                    Trace.TraceError("Name value pair is not correct length for \"{0}\"", item);
                    continue;
                }

                PropertyInfo? property = typeFromHandle.GetProperty(array[0]);
                if (property == null)
                {
                    Trace.TraceError("Property \"{0}\" does not exist in object \"{1}\"", array[0], typeof(T));
                    continue;
                }

                object? obj = ParseValue(property, array[1]);
                if (obj == null)
                    Trace.TraceError("Value \"{0}\" could not be parsed as \"{1}\"", array[1], property.GetType());
                else
                    property.SetValue(val, obj);
            }

            return val;
        }
        catch (Exception ex)
        {
            Trace.TraceError("Error parsing {0}:\r\n{1}", typeof(T), ex);
            return default;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="elementSeperator"></param>
    /// <param name="pairSeperator"></param>
    /// <returns></returns>
    public static string? ToNameValue(this object obj, char elementSeperator = ',', char pairSeperator = '=')
    {
        ArgumentNullException.ThrowIfNull(obj);

        StringBuilder stringBuilder = new();
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();
        PropertyInfo[] array = properties;
        foreach (PropertyInfo propertyInfo in array)
        {
            Type propertyType = propertyInfo.PropertyType;
            if (propertyType == null)
                continue;

            object? value = propertyInfo.GetValue(obj, null);
            if (value != null)
            {
                if (propertyType.InvokeMember("ToString", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, value, null) is string arg)
                {
                    if (stringBuilder.Length != 0)
                        stringBuilder.Append(elementSeperator);
                    stringBuilder.AppendFormat("{0}{1}{2}", propertyInfo.Name, pairSeperator, arg);
                }
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// parse a single string as a value
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static object? ParseValue(PropertyInfo property, string value)
    {
        if (property.PropertyType.IsEnum)
            return Enum.Parse(property.PropertyType, value);

        return property.PropertyType.ToString() switch
        {
            "System.Int16" => short.Parse(value),
            "System.Int32" => int.Parse(value),
            "System.Int64" => long.Parse(value),
            "System.UInt16" => ushort.Parse(value),
            "System.UInt32" => uint.Parse(value),
            "System.UInt64" => ulong.Parse(value),
            "System.Single" => float.Parse(value),
            "System.Double" => double.Parse(value),
            "System.Boolean" => bool.Parse(value),
            "System.String" => value,
            "System.DateTime" => DateTime.Parse(value),
            "System.Byte" => byte.Parse(value),
            "System.SByte" => sbyte.Parse(value),
            "System.Guid" => Guid.Parse(value),
            "System.Enum" => Enum.Parse(property.PropertyType, value),
            _ => null,
        };
    }

    /// <summary>
    /// Get a property value from an object by reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    public static T? GetPropValue<T>(this object src, string propName)
    {
        ArgumentNullException.ThrowIfNull(src);
        try
        {
            PropertyInfo? property = src.GetType().GetProperty(propName);
            return property == null ? default : (T?)property.GetValue(src, null);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Set a property value on an object by reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <param name="propName"></param>
    /// <param name="value"></param>
    public static void SetPropValue<T>(this object src, string propName, T value)
    {
        ArgumentNullException.ThrowIfNull(src);
        try
        {
            var property = src.GetType().GetProperty(propName);
            property?.SetValue(src, value);
        }
        catch
        {
        }
    }

}
