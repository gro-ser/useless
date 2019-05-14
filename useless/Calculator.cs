using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace _
{
    /// <summary>
    /// Class to allow operations (like Add, Multiply, etc.) for generic types. This type should allow these operations themselves.
    /// If a type does not support an operation, an exception is throw when using this operation, not during construction of this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Calculator<T>
    {
        private static readonly Calculator<T> instance = new Calculator<T>();

        /// <summary>
        /// Adds two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> add;

        /// <summary>
        /// Subtracts two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> subtract;

        /// <summary>
        /// Multiplies two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> multiply;

        /// <summary>
        /// Divides two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> divide;

        /// <summary>
        /// Divides two values of the same type and returns the remainder.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> modulo;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values, but will throw an OverflowException on unsigned values which are not 0.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T> negate;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T> plus;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T> increment;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T> decrement;

        /// <summary>
        /// Shifts the number to the left.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, int, T> leftShift;

        /// <summary>
        /// Shifts the number to the right.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, int, T> rightShift;

        /// <summary>
        /// Inverts all bits inside the value.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T> onesComplement;

        /// <summary>
        /// Performs a bitwise OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> or;

        /// <summary>
        /// Performs a bitwise AND
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> and;

        /// <summary>
        /// Performs a bitwise Exclusive OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private readonly Func<T, T, T> xor;

        public Calculator()
        {
            this.add = CreateDelegate<T>(Expression.AddChecked, "Addition", true);
            this.subtract = CreateDelegate<T>(Expression.SubtractChecked, "Substraction", true);
            this.multiply = CreateDelegate<T>(Expression.MultiplyChecked, "Multiply", true);
            this.divide = CreateDelegate<T>(Expression.Divide, "Divide", true);
            this.modulo = CreateDelegate<T>(Expression.Modulo, "Modulus", true);
            this.negate = CreateDelegate(Expression.NegateChecked, "Negate", true);
            this.plus = CreateDelegate(Expression.UnaryPlus, "Plus", true);
            this.increment = CreateDelegate(Expression.Increment, "Increment", true);
            this.decrement = CreateDelegate(Expression.Decrement, "Decrement", true);
            this.leftShift = CreateDelegate<int>(Expression.LeftShift, "LeftShift", false);
            this.rightShift = CreateDelegate<int>(Expression.RightShift, "RightShift", false);
            this.onesComplement = CreateDelegate(Expression.OnesComplement, "OnesComplement", false);
            this.and = CreateDelegate<T>(Expression.And, "BitwiseAnd", false);
            this.or = CreateDelegate<T>(Expression.Or, "BitwiseOr", false);
            this.xor = CreateDelegate<T>(Expression.ExclusiveOr, "ExclusiveOr", false);
        }
        
        /// <summary>
        /// Adds two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Add
        {
            get
            {
                return this.add;
            }
        }

        /// <summary>
        /// Subtracts two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Subtract
        {
            get
            {
                return this.subtract;
            }
        }

        /// <summary>
        /// Multiplies two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Multiply
        {
            get
            {
                return this.multiply;
            }
        }

        /// <summary>
        /// Divides two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Divide
        {
            get
            {
                return this.divide;
            }
        }

        /// <summary>
        /// Divides two values of the same type and returns the remainder.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Modulo
        {
            get
            {
                return this.modulo;
            }
        }

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values, but will throw an OverflowException on unsigned values which are not 0.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Negate
        {
            get
            {
                return this.negate;
            }
        }

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Plus
        {
            get
            {
                return this.plus;
            }
        }

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Increment
        {
            get
            {
                return this.increment;
            }
        }

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Decrement
        {
            get
            {
                return this.decrement;
            }
        }

        /// <summary>
        /// Shifts the number to the left.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, int, T> LeftShift
        {
            get
            {
                return this.leftShift;
            }
        }

        /// <summary>
        /// Shifts the number to the right.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, int, T> RightShift
        {
            get
            {
                return this.rightShift;
            }
        }

        /// <summary>
        /// Inverts all bits inside the value.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> OnesComplement
        {
            get
            {
                return this.onesComplement;
            }
        }

        /// <summary>
        /// Performs a bitwise OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Or
        {
            get
            {
                return this.or;
            }
        }

        /// <summary>
        /// Performs a bitwise AND
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> And
        {
            get
            {
                return this.and;
            }
        }

        /// <summary>
        /// Performs a bitwise Exclusive OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Xor
        {
            get
            {
                return this.xor;
            }
        }

        private static Func<T, T2, T> CreateDelegate<T2>(Func<Expression, Expression, Expression> @operator, string operatorName, bool isChecked)
        {
            try
            {
                var convertToTypeA = ConvertTo(typeof(T));
                var convertToTypeB = ConvertTo(typeof(T2));
                var parameterA = Expression.Parameter(typeof(T), "a");
                var parameterB = Expression.Parameter(typeof(T2), "b");
                var valueA = (convertToTypeA != null) ? Expression.Convert(parameterA, convertToTypeA) : (Expression)parameterA;
                var valueB = (convertToTypeB != null) ? Expression.Convert(parameterB, convertToTypeB) : (Expression)parameterB;
                var body = @operator(valueA, valueB);

                if (convertToTypeA != null)
                {
                    body = isChecked ? Expression.ConvertChecked(body, typeof(T)) : Expression.Convert(body, typeof(T));
                }

                return Expression.Lambda<Func<T, T2, T>>(body, parameterA, parameterB).Compile();
            }
            catch
            {
                return (a, b) =>
                {
                    throw new InvalidOperationException("Operator " + operatorName + " is not supported by type " + typeof(T).FullName + ".");
                };
            }
        }

        private static Func<T, T> CreateDelegate(Func<Expression, Expression> @operator, string operatorName, bool isChecked)
        {
            try
            {
                var convertToType = ConvertTo(typeof(T));
                var parameter = Expression.Parameter(typeof(T), "a");
                var value = (convertToType != null) ? Expression.Convert(parameter, convertToType) : (Expression)parameter;
                var body = @operator(value);

                if (convertToType != null)
                {
                    body = isChecked ? Expression.ConvertChecked(body, typeof(T)) : Expression.Convert(body, typeof(T));
                }

                return Expression.Lambda<Func<T, T>>(body, parameter).Compile();
            }
            catch
            {
                return a =>
                {
                    throw new InvalidOperationException("Operator " + operatorName + " is not supported by type " + typeof(T).FullName + ".");
                };
            }
        }

        private static Type ConvertTo(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return typeof(int);
            }

            return null;
        }
    }
}
