using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
//using static System.Linq.Expressions.Expression;

namespace useless.Enumeration
{
    public class Calculator<T>
    {
        static Func<T, T, T>
            add, sub, and, or, xor;
        static Func<T, T>
            not, inc, dec;

        static Func<T, T, T> CreateFunc(Func<Expression, Expression, Expression> Operator)
        {
            var left = Expression.Parameter(typeof(T), "left");
            var right = Expression.Parameter(typeof(T), "right");
            try
            {
                return Expression.Lambda<Func<T, T, T>>(Operator(left, right), left, right).Compile();
            }
            catch
            {
                return null;
            }
        }
        static Func<T, T> CreateFunc(Func<Expression, Expression> Operator)
        {
            var val = Expression.Parameter(typeof(T), "left");
            try
            {
                return Expression.Lambda<Func<T, T>>(Operator(val), val).Compile();
            }
            catch
            {
                return null;
            }
        }

        static Calculator()
        {
            add = CreateFunc(Expression.Add);
            sub = CreateFunc(Expression.Subtract);
            and = CreateFunc(Expression.And);
            or = CreateFunc(Expression.Or);
            xor = CreateFunc(Expression.ExclusiveOr);
            not = CreateFunc(Expression.Not);
            inc = CreateFunc(Expression.Increment);
            dec = CreateFunc(Expression.Decrement);
        }

        public static T Add(T left, T right) => add != null ? add(left, right) :
            throw new InvalidOperationException($"Operation {nameof(Add)} not supported.");
        public static T Sub(T left, T right) => sub != null ? sub(left, right) :
            throw new InvalidOperationException($"Operation {nameof(Sub)} not supported.");
        public static T And(T left, T right) => and != null ? and(left, right) :
            throw new InvalidOperationException($"Operation {nameof(And)} not supported.");
        public static T Or(T left, T right) => or != null ? or(left, right) :
            throw new InvalidOperationException($"Operation {nameof(Or)} not supported.");
        public static T Xor(T left, T right) => xor != null ? xor(left, right) :
            throw new InvalidOperationException($"Operation {nameof(Xor)} not supported.");

        static public T Not(T val) => not != null ? not(val) :
            throw new InvalidOperationException($"Operation {nameof(Not)} not supported.");
        static public T Inc(T val) => inc != null ? inc(val) :
            throw new InvalidOperationException($"Operation {nameof(Inc)} not supported.");
        static public T Dec(T val) => dec != null ? dec(val) :
            throw new InvalidOperationException($"Operation {nameof(Dec)} not supported.");
    }
}
