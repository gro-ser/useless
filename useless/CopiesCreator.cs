using System;
using System.Reflection;

namespace _
{
    using props = System.Collections.Generic.Dictionary<string, object>;
    public class CopiesCreator<T> where T : new()
    {
        public delegate SetValue OnProperty(string name);
        public delegate OnProperty SetValue(object item);

        props properties = new props();
        static PropertyInfo[] infos = typeof(T).GetProperties();
        string prop;

        PropertyInfo GetProp(string name)
        {
            for (int i = 0, length = infos.Length; i < length; i++)
            {
                if (name == infos[i].Name) return infos[i];
            }
            throw new Exception("not found!");
        }

        SetValue onProperty(string name)
        {
            if (prop != null) throw new Exception("last property dosen't set!");
            prop = name;
            return setValue;
        }
        OnProperty setValue(object item)
        {
            properties[prop] = item;
            prop = null;
            return onProperty;
        }

        public OnProperty SetProperties => onProperty;

        public T CreateNew()
        {
            T t = new T();
            foreach (var prop in properties)
                GetProp(prop.Key)
                    .SetValue(t, prop.Value);
            return t;
        }
    }
}