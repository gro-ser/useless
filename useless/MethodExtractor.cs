﻿using System;
//using System.Linq;
using System.Reflection;
using System.Text;

namespace useless
{
    public class MethodExtractor
    {
        /* str:    "[[[<assembly>|]<namespace>|]<class>|]<method>[|<parameters>[|<cast_type>]]"
         * priority:[     5      |     4      |    3   |    0     |      1      |     2      ]
         * parameters: "" | parameters, <type>
         * assembly     -- название сборки              /def:"mscorlib"
         * namespace    -- название пространства имен   /def:"System"
         * class        -- название класса              /def:"Math"
         * method       -- название метода              /def: no exist
         * parameters   -- входные параметры метода     /def:"Double"
         * cast_type    -- преобразование к типу        /def:"Double"
         */
        private const string
            ass = "mscorlib",
            nam = "System",
            cls = "Math",
            prm = "R",
            cst = "R"; // null|"" => no cast needed

        private readonly string
            assembly = ass,
            @namespace = nam,
            @class = cls,
            method,
            parameters = prm,
            cast = cst;
        private static readonly string[] localNames = { "I", "R", "S" };
        private static readonly Type[] localTypes = { typeof(long), typeof(double), typeof(string) };

        public MethodExtractor(string parameters)
        : this(parameters.Split('|')) { }
        public MethodExtractor(MethodInfo info)
        {
            method = info.Name;
            Type cls = info.DeclaringType;
            @class = cls.Name;
            @namespace = cls.Namespace;
            Assembly asm = cls.Assembly;
            if (asm == mscorlib || asm == current)
                assembly = asm.GetName().Name;
            else
                assembly = asm.FullName;
            parameters = ParametersToString(info.GetParameters());
            cast = info.ReturnType.Name;
        }
        public MethodExtractor(string[] names)
        {
            switch (names.Length)
            {
                case 1:
                    (method) = (names[0]);
                    break;
                case 2:
                    (method, parameters) = (names[0], names[1]);
                    break;
                case 3:
                    (method, parameters, cast) = (names[0], names[1], names[2]);
                    break;
                case 4:
                    (@class, method, parameters, cast) = (names[0], names[1], names[2], names[3]);
                    break;
                case 5:
                    (@namespace, @class, method, parameters, cast) = (names[0], names[1], names[2], names[3], names[4]);
                    break;
                case 6:
                    (assembly, @namespace, @class, method, parameters, cast) =
                 (names[0], names[1], names[2], names[3], names[4], names[5]);
                    break;
                default:
                    throw new Exception("invalid parameters count");
            }
        }

        private static string ParametersToString(ParameterInfo[] @params)
        {
            StringBuilder sb = new StringBuilder();
            int length = @params.Length;
            if (length != 0)
                sb.Append(TypeToString(@params[0].ParameterType));
            for (int i = 1; i < length; i++)
            {
                sb.Append(';')
                    .Append(TypeToString(@params[i].ParameterType));
            }

            return sb.ToString();
        }
        private static string TypeToString(Type type)
        {
            int ind;
            if ((ind = Array.IndexOf(localTypes, type)) != -1)
                return localNames[ind];
            Assembly ass = type.Assembly;
            if (ass == mscorlib || ass == current)
                return type.Namespace == "System" ? type.Name : type.FullName;
            return type.AssemblyQualifiedName;
        }
        private static Type StringToType(string name)
        {
            int ind;
            if ((ind = Array.IndexOf(localNames, name)) != -1)
                return localTypes[ind];
            if (name.Contains("."))
                return GetType(name);
            if (name[0] == '@')
                return GetType(name.Substring(1));
            return GetType("System." + name);
        }

        public MethodInfo GetMethodInfo()
        {
            Assembly ass = Assembly.LoadWithPartialName(assembly);
            Type typ = GetType(ass, @namespace + "." + @class);
            MethodInfo met;
            try
            { met = typ?.GetMethod(method, GetParametersType()); }
            catch { met = null; }
            return met;
        }

        private Type[] GetParametersType()
        {
            string[] names = parameters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int length = names.Length;
            Type[] types = new Type[length];
            for (int i = 0; i < length; i++)
                types[i] = StringToType(names[i]);
            return types;
        }

        private static Type GetType(string name) => Type.GetType(name, false, true);

        public override string ToString() => $"{{{assembly}|{@namespace}|{@class}|{method}|{parameters}|{cast}}}";

        internal static void Test()
        {
            Type t;
            AssemblyName name;
            Assembly asm;
            MethodInfo met;
            asm = Assembly.Load("mscorlib");
            name = asm.GetName();
            //foreach (var type in asm.GetTypes()) Console.WriteLine(type);
            t = GetType(asm, "system.random");
            ConstructorInfo ctr = t.GetConstructor(Type.EmptyTypes);
            met = t?.GetMethod("Next", new Type[] { });

            foreach (MethodInfo method in t.GetMethods())
                Console.WriteLine(method);
            Console.WriteLine(met?.ToString() ?? "null");
        }

        private static Type GetType(Assembly assembly, string str)
            => assembly?.GetType(str, false, true);

        private static readonly Assembly current = Assembly.GetExecutingAssembly(), mscorlib = Assembly.GetAssembly(typeof(void));

        public static MethodInfo GetMethodFast(string desc) => new MethodExtractor(desc).GetMethodInfo();
    }
}