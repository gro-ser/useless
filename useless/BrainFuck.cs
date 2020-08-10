using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace useless
{
    internal class BrainFuck
    {
        private enum OperationType { Undefined, Change, Shift, LoopIn, LoopOut, Output, Input }

        private static OperationType GetOperationType(char sym)
        {
            switch (sym)
            {
                case '+':
                case '-':
                    return OperationType.Change;
                case '<':
                case '>':
                    return OperationType.Shift;
                case '[':
                    return OperationType.LoopIn;
                case ']':
                    return OperationType.LoopOut;
                case '.':
                    return OperationType.Output;
                case ',':
                    return OperationType.Input;
                default:
                    return OperationType.Undefined;
            }
        }

        private List<Expression> _body;
        private Stack<List<Expression>> _blocks;
        private readonly ParameterExpression _array, _index, _bytes;
        private readonly ParameterExpression[] _variables;
        private readonly int _elementCount;
        private bool _lazyPrint;

        public BrainFuck(int ElementCount, bool LazyPrint)
        {
            _lazyPrint = LazyPrint;
            _elementCount = ElementCount;
            _array = Variable(typeof(byte[]), "array");
            _index = Variable(typeof(int), "index");
            _bytes = Variable(typeof(List<byte>), "bytes");
            _variables = new[] { _array, _index, _bytes };
        }

        public BrainFuck() : this(30000, true) { }


        public Action Compile(string source) => CreateLambda(source).Compile();

        public Expression<Action> CreateLambda(string source)
        {
            int acc = 0;
            OperationType type, last = OperationType.Undefined;
            _blocks = new Stack<List<Expression>>();
            _body = new List<Expression>();
            _blocks.Push(_body);
            Startup();
            foreach (char x in source)
            {
                type = GetOperationType(x);
                if (type == OperationType.Undefined)
                    continue;
                if (last != type)
                {
                    if (last >= OperationType.Change && last <= OperationType.Shift)
                        Add(Operation(last, acc));
                    acc = 0;
                }
                last = type;
                switch (type)
                {
                    case OperationType.Change:
                        acc += x == '+' ? 1 : -1;
                        break;
                    case OperationType.Shift:
                        acc += x == '>' ? 1 : -1;
                        break;
                    case OperationType.LoopIn:
                        AddLoopIn();
                        break;
                    case OperationType.LoopOut:
                        AddLoopOut();
                        break;
                    case OperationType.Output:
                        Add(Output());
                        break;
                    case OperationType.Input:
                        Add(Input());
                        break;
                }
            }
            if (_lazyPrint)
            {
                LazyPrint();
            }

            _body.RemoveAll(x => x == null);
            return Lambda<Action>(Block(_variables, _body));
        }

        private void LazyPrint()
        {
            Add(Call(null, ((Action<string>)Console.Write).Method,
                Call(Encoding(), "GetString", null,
                    Call(_bytes, "ToArray", null))));
            Add(Call(_bytes, "Clear", null));
        }

        private static ConstantExpression Encoding() => Constant(System.Text.Encoding.Default);

        private void Startup()
        {
            _body.Add(Assign(_index, Constant(0)));
            _body.Add(Assign(_array, NewArrayBounds(typeof(byte), Constant(_elementCount))));
            if (_lazyPrint)
                _body.Add(Assign(_bytes, New(typeof(List<byte>))));
        }

        private Expression Operation(OperationType type, int acc)
        {
            if (0 == acc)
                return null;
            switch (type)
            {
                case OperationType.Change when (0 < acc):
                    return Assign(Element(), Convert(Expression.Add(Convert(Element(), typeof(int)), Constant(acc)), typeof(byte)));
                case OperationType.Change when (0 > acc):
                    return Assign(Element(), Convert(Expression.Subtract(Convert(Element(), typeof(int)), Constant(-acc)), typeof(byte)));
                case OperationType.Shift when (0 < acc):
                    return AddAssign(_index, Constant(acc));
                case OperationType.Shift when (0 > acc):
                    return SubtractAssign(_index, Constant(-acc));
                default:
                    throw new ArgumentException();
            }
        }

        private Expression Element() => ArrayAccess(_array, _index);

        private void AddLoopIn() => _blocks.Push(new List<Expression>());

        private void AddLoopOut()
        {
            List<Expression> list = _blocks.Pop();
            LabelTarget label = Label();
            Add(Loop(
                IfThenElse(
                    Equal(Element(), Constant((byte)0)),
                    Break(label),
                    Block(list)),
                label));
        }

        private Expression Output() =>
            _lazyPrint ? Call(_bytes, "Add", null, Element()) :
            //Call(((Action<char>)Console.Write).Method, Convert(Element(), typeof(char)));
            Call(((Action<string>)Console.Write).Method, Call(Encoding(), "GetString", null, _array, _index, Constant(1)));

        private Expression Input()
        {
            if (_lazyPrint)
                LazyPrint();
            return Assign(Element(), Convert(MakeMemberAccess(Call(((Func<ConsoleKeyInfo>)Console.ReadKey).Method),
                typeof(ConsoleKeyInfo).GetProperty("KeyChar")), typeof(byte)));
        }

        private void Add(Expression expr) => _blocks.Peek().Add(expr);

        private static readonly BrainFuck instance = new BrainFuck();

        public static Action CompileSource(string source) =>
            instance.Compile(source);

        public static Expression<Action> LambdaSource(string source) =>
            instance.CreateLambda(source);

        public static Expression<Action> LambdaSource(string source, bool lazyPrint)
        {
            bool last = instance._lazyPrint;
            instance._lazyPrint = lazyPrint;
            Expression<Action> result = instance.CreateLambda(source);
            instance._lazyPrint = last;
            return result;
        }
    }
}