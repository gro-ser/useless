using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace useless.DataClasses
{
    public class DataClassesCreator
    {
        private readonly string typeName;
        private readonly Dictionary<string, (Type, object)> properties;

        public DataClassesCreator(string typeName)
        {
            this.typeName = typeName;
            properties = new Dictionary<string, (Type, object)>();
        }

        public DataClassesCreator AddProperty<T>(string name)
        {
            properties.Add(name, (typeof(T), null));
            return this;
        }

        public DataClassesCreator AddProperty<T>(string name, T defaultValue)
        {
            properties.Add(name, (typeof(T), defaultValue));
            return this;
        }

        public void GenerateClass()
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            CodeExpression expression = new CodeExpression();
        }
    }
}
