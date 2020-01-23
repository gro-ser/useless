using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;

namespace useless.DataClasses
{
    public class DataClassesCreator
    {
        string typeName;
        Dictionary<string, (Type, object)> properties;

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
