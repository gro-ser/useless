using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static System.Linq.Expressions.Expression;

namespace useless
{
    internal class BrainFuck
    {
        private enum OperationType { Undefined, Change, Shift, Loop, Output, Input }

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
                case ']':
                    return OperationType.Loop;
                case '.':
                    return OperationType.Output;
                case ',':
                    return OperationType.Input;
                default:
                    return OperationType.Undefined;
            }
        }

        private List<Expression> body;
        private Stack<List<Expression>> blocks;
        private readonly ParameterExpression array, index, bytes;
        private readonly ParameterExpression[] variables;
        private readonly int elementCount;
        private readonly bool printAtEnd;

        public BrainFuck(int ElementCount, bool PrintAtEnd)
        {
            printAtEnd = PrintAtEnd;
            elementCount = ElementCount;
            array = Variable(typeof(byte[]), "array");
            index = Variable(typeof(int), "index");
            bytes = Variable(typeof(List<byte>), "bytes");
            variables = new[] { array, index, bytes };
        }

        public BrainFuck() : this(30000, true) { }

        public Action Compile(string source) =>
            CreateLambda(source).Compile();

        public Expression<Action> CreateLambda(string source)
        {
            int acc = 0;
            OperationType type, last = OperationType.Undefined;
            blocks = new Stack<List<Expression>>();
            body = new List<Expression>();
            blocks.Push(body);
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
                    case OperationType.Loop:
                        AddLoop(x);
                        break;
                    case OperationType.Output:
                        Add(Output());
                        break;
                    case OperationType.Input:
                        Add(Input());
                        break;
                }
            }
            if (printAtEnd)
            {
                Add(Call(null, ((Action<string>)Console.WriteLine).Method,
                    Call(Constant(Encoding.UTF8), "GetString", null,
                        Call(bytes, "ToArray", null))));
            }

            return Lambda<Action>(Block(variables, body));
        }

        private void Startup()
        {
            body.Add(Assign(index, Constant(0)));
            body.Add(Assign(array, NewArrayBounds(typeof(byte), Constant(elementCount))));
            if (printAtEnd)
                body.Add(Assign(bytes, New(typeof(List<byte>))));
        }

        private Expression Operation(OperationType type, int acc)
        {
            switch (type)
            {
                case OperationType.Change:
                    return Assign(Element(), Convert(Expression.Add(
                        Convert(Element(), typeof(int)), Constant(acc)), typeof(byte)));
                case OperationType.Shift:
                    return AddAssign(index, Constant(acc));
                default:
                    throw new ArgumentException();
            }
        }

        private Expression Element() =>
            ArrayAccess(array, index);

        private void AddLoop(char x)
        {
            if (x == '[')
            {
                blocks.Push(new List<Expression>());
                return;
            }
            List<Expression> list = blocks.Pop();
            LabelTarget label = Label();
            Add(Loop(
                IfThenElse(
                    Equal(Element(), Constant((byte)0)),
                    Break(label),
                    Block(list)),
                label));
        }

        private Expression Output() =>
            printAtEnd ? Call(bytes, "Add", null, Element()) :
            Call(((Action<char>)Console.Write).Method, Convert(Element(), typeof(char)));

        private Expression Input() =>
            Convert(MakeMemberAccess(Call(((Func<ConsoleKeyInfo>)Console.ReadKey).Method),
                typeof(ConsoleKeyInfo).GetProperty("Key")), typeof(byte));

        private void Add(Expression expr) => blocks.Peek().Add(expr);

        private static readonly BrainFuck instance = new BrainFuck();

        public static Action CompileSource(string source) =>
            instance.Compile(source);

        public static Expression<Action> LambdaSource(string source) =>
            instance.CreateLambda(source);

    }
}