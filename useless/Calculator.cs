using System;
using System.Linq.Expressions;

namespace useless
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
            add = CreateDelegate<T>(Expression.AddChecked, "Addition", true);
            subtract = CreateDelegate<T>(Expression.SubtractChecked, "Substraction", true);
            multiply = CreateDelegate<T>(Expression.MultiplyChecked, "Multiply", true);
            divide = CreateDelegate<T>(Expression.Divide, "Divide", true);
            modulo = CreateDelegate<T>(Expression.Modulo, "Modulus", true);
            negate = CreateDelegate(Expression.NegateChecked, "Negate", true);
            plus = CreateDelegate(Expression.UnaryPlus, "Plus", true);
            increment = CreateDelegate(Expression.Increment, "Increment", true);
            decrement = CreateDelegate(Expression.Decrement, "Decrement", true);
            leftShift = CreateDelegate<int>(Expression.LeftShift, "LeftShift", false);
            rightShift = CreateDelegate<int>(Expression.RightShift, "RightShift", false);
            onesComplement = CreateDelegate(Expression.OnesComplement, "OnesComplement", false);
            and = CreateDelegate<T>(Expression.And, "BitwiseAnd", false);
            or = CreateDelegate<T>(Expression.Or, "BitwiseOr", false);
            xor = CreateDelegate<T>(Expression.ExclusiveOr, "ExclusiveOr", false);
        }

        /// <summary>
        /// Adds two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Add => add;

        /// <summary>
        /// Subtracts two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Subtract => subtract;

        /// <summary>
        /// Multiplies two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Multiply => multiply;

        /// <summary>
        /// Divides two values of the same type.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Divide => divide;

        /// <summary>
        /// Divides two values of the same type and returns the remainder.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Modulo => modulo;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values, but will throw an OverflowException on unsigned values which are not 0.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Negate => negate;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Plus => plus;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Increment => increment;

        /// <summary>
        /// Gets the negative value of T.
        /// Supported by: All numeric values.
        /// </summary>
        /// <exception cref="OverflowException"/>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> Decrement => decrement;

        /// <summary>
        /// Shifts the number to the left.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, int, T> LeftShift => leftShift;

        /// <summary>
        /// Shifts the number to the right.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, int, T> RightShift => rightShift;

        /// <summary>
        /// Inverts all bits inside the value.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T> OnesComplement => onesComplement;

        /// <summary>
        /// Performs a bitwise OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Or => or;

        /// <summary>
        /// Performs a bitwise AND
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> And => and;

        /// <summary>
        /// Performs a bitwise Exclusive OR.
        /// Supported by: All integral types.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public Func<T, T, T> Xor => xor;

        private static Func<T, T2, T> CreateDelegate<T2>(Func<Expression, Expression, Expression> @operator, string operatorName, bool isChecked)
        {
            try
            {
                Type convertToTypeA = ConvertTo(typeof(T));
                Type convertToTypeB = ConvertTo(typeof(T2));
                ParameterExpression parameterA = Expression.Parameter(typeof(T), "a");
                ParameterExpression parameterB = Expression.Parameter(typeof(T2), "b");
                Expression valueA = (convertToTypeA != null) ? Expression.Convert(parameterA, convertToTypeA) : (Expression)parameterA;
                Expression valueB = (convertToTypeB != null) ? Expression.Convert(parameterB, convertToTypeB) : (Expression)parameterB;
                Expression body = @operator(valueA, valueB);

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
                Type convertToType = ConvertTo(typeof(T));
                ParameterExpression parameter = Expression.Parameter(typeof(T), "a");
                Expression value = (convertToType != null) ? Expression.Convert(parameter, convertToType) : (Expression)parameter;
                Expression body = @operator(value);

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
