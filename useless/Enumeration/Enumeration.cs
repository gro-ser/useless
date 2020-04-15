using System;
using System.Collections.Generic;
using System.Reflection;

namespace useless.Enumeration
{
    public abstract class Enumeration<T>
    {
        protected T value;
        public static T GetRawValue(Enumeration<T> enumeration) => enumeration.value;

        private static readonly Dictionary<Type, Dictionary<T, string>> values = new Dictionary<Type, Dictionary<T, string>>();

        protected Enumeration()
        {
            Type type = GetType();
            if (!values.ContainsKey(type))
            {
                T value = default;
                Dictionary<T, string> dict = new Dictionary<T, string>();
                values[type] = dict;
                foreach (FieldInfo prop in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    if (prop.FieldType != type)
                        continue;
                    StartValueAttribute sv = prop.GetCustomAttribute<StartValueAttribute>();
                    Enumeration<T> some = prop.GetValue(null) as Enumeration<T>;
                    if (some == null)
                    {
                        some = (Enumeration<T>)type.GetConstructors((BindingFlags)(-1))[0].Invoke(Array.Empty<Type>());
                    }
                    if (some.value == null || some.value.Equals(default(T)))
                    {
                        if (sv != null && sv.StartValue is T)
                            value = (T)sv.StartValue;
                        some.value = value;
                        prop.SetValue(null, some);
                    }
                    else
                    {
                        value = some.value;
                    }

                    value = Calculator<T>.Inc(value);
                    dict.Add(value, prop.Name);
                }
            }
        }

        public static string[] GetNames(Type type)
        {
            if (!values.TryGetValue(type, out Dictionary<T, string> dict))
                return null;
            string[] result = new string[dict.Count];
            dict.Values.CopyTo(result, 0);
            return result;
        }

        public override string ToString()
        {
            Dictionary<T, string> dict = values[GetType()];
            if (dict.TryGetValue(value, out string str))
                return str;
            return value.ToString();
        }
    }
    public class TypeCode : Enumeration<int>
    {
        public static TypeCode
            Undefined,
            Object,
            Int,
            Float;
        [StartValue(111)]
        public static TypeCode Boolean,
        DBNull,
        String;

        private TypeCode() { }
        static TypeCode() => _ = new TypeCode();
    }

    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class StartValueAttribute : Attribute
    {
        private readonly object startValue;

        public StartValueAttribute(object positionalString) => startValue = positionalString;

        public object StartValue => startValue;
    }
}
