using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimpleUwpCalculator.Calculate
{
    /// <summary>
    /// Some functions for calculating an expression
    /// </summary>
    static class Calculator
    {
        /// <summary>
        /// Calculate an expression string
        /// </summary>
        /// <param name="str">an expression string</param>
        /// <returns>result of the expression</returns>
        /// <exception cref="SimpleUwpCalculator.Calculate.InvalidExpresionException"></exception>
        /// <exception cref="System.DivideByZeroException"></exception>
        /// <exception cref="System.OverflowException"></exception>
        public static decimal CalculateFromString(string str)
        {
            var tokens = Tokenize(str);
            return CalculateRecursive(tokens, 0, tokens.Count);
        }

        private class NumberBuilder
        {
            public decimal Number { get => decimal.Parse(sb.ToString()); }

            public bool IsEmpty { get => sb.Length == 0; }

            public bool NotEmpty { get => !IsEmpty; }

            public void Append(char ch)
            {
                Debug.Assert(char.IsDigit(ch) || ch == '.');
                if (char.IsDigit(ch))
                    sb.Append(ch);
                else // ch == '.'
                {
                    if (hasDot)
                        throw new InvalidExpresionException(
                            "Too many dots.");
                    hasDot = true;
                    sb.Append(ch);
                }
            }

            public void Clear()
            {
                sb.Clear();
                hasDot = false;
            }

            private readonly StringBuilder sb = new StringBuilder();

            /// <summary>
            /// 是否已有小数点
            /// </summary>
            private bool hasDot = false;
        }

        /// <summary>
        /// string to tokens
        /// </summary>
        private static List<Token> Tokenize(string str)
        {
            var tokens = new List<Token>();
            var nb = new NumberBuilder();
            foreach (char ch in str)
            {
                if (char.IsDigit(ch) || ch == '.')
                    nb.Append(ch);
                else if (ch == ' ')
                    continue;
                else
                {
                    if (nb.NotEmpty)
                    {
                        tokens.Add(new Token(nb.Number));
                        nb.Clear();
                    }
                    tokens.Add(new Token(ch));
                }
            }
            if (nb.NotEmpty)
                tokens.Add(new Token(nb.Number));
            FixUnaryOperator(tokens);
            return tokens;
        }

        /// <summary>
        /// 将某些 + 和 - 修正为一元运算符
        /// </summary>
        private static void FixUnaryOperator(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.OperatorTest(Operator.Plus)
                    || token.OperatorTest(Operator.Minus))
                {
                    bool isUnary = true;
                    if (i != 0
                        && (tokens[i - 1].IsNumber || tokens[i - 1].OperatorTest(Operator.RightBrace)))
                        isUnary = false; // binary operator has left operand
                    if (isUnary)
                        token.ChangeToUnary();
                }
            }
        }

        private class Token
        {
            /// <summary>
            /// Build a number token
            /// </summary>
            public Token(decimal number)
            {
                IsNumber = true;
                this.number = number;
            }

            /// <summary>
            /// Build an operator token
            /// </summary>
            public Token(char op)
            {
                IsNumber = false;
                if (op == '+')
                    Operator = Operator.Plus;
                else if (op == '-')
                    Operator = Operator.Minus;
                else if (op == '×' || op == '*')
                    Operator = Operator.Multiply;
                else if (op == '÷' || op == '/')
                    Operator = Operator.Divide;
                else if (op == '(')
                    Operator = Operator.LeftBrace;
                else if (op == ')')
                    Operator = Operator.RightBrace;
                else
                    throw new ApplicationException($"Unexpect operator: '{op}'.");
            }

            public bool IsNumber { get; private set; }

            public bool IsOperator { get => !IsNumber; }

            private decimal number;
            public decimal Number
            {
                get => IsNumber ? number : throw new ApplicationException(
                    "Try to get a number from a non-number token.");
                private set => number = value;
            }

            private Operator op;
            public Operator Operator
            {
                get => IsOperator ? op : throw new ApplicationException(
                    "Try to get an operator from a non-operator token.");
                private set => op = value;
            }

            /// <summary>
            /// 该 Token 是否为运算符 op
            /// </summary>
            public bool OperatorTest(Operator op) => IsOperator && Operator == op;

            /// <summary>
            /// Change + or - to unary operator
            /// </summary>
            public void ChangeToUnary()
            {
                if (!IsOperator)
                    throw new ApplicationException("Not an operator.");
                if (Operator == Operator.Plus)
                    Operator = Operator.Positive;
                else if (Operator == Operator.Minus)
                    Operator = Operator.Negative;
                else
                    throw new ApplicationException("Not a + or -.");
            }

            public override string ToString()
            {
                if (IsNumber)
                    return Number.ToString();
                else
                {
                    if (Operator == Operator.Plus)
                        return "+";
                    else if (Operator == Operator.Minus)
                        return "-";
                    else if (Operator == Operator.Multiply)
                        return "*";
                    else if (Operator == Operator.Divide)
                        return "/";
                    else if (Operator == Operator.LeftBrace)
                        return "(";
                    else if (Operator == Operator.RightBrace)
                        return ")";
                    else if (Operator == Operator.Positive)
                        return "u+";
                    else
                        return "u-";
                }
            }
        }

        private enum Operator
        {
            Plus,       // binary +
            Minus,      // binary -
            Multiply,   // *
            Divide,     // /
            LeftBrace,  // (
            RightBrace, // (
            Positive,   // unary +
            Negative    // unary -
        }

        private static decimal CalculateRecursive(List<Token> tokens, int start, int stop)
        {
            if (start >= stop) // empty interval
                throw new InvalidExpresionException();
            if (stop - start == 1) // only one token
            {
                if (!tokens[start].IsNumber)
                    throw new InvalidExpresionException();
                return tokens[start].Number;
            }

            int p = 0;   // depth of brace
            int c1 = -1; // right most + or -
            int c2 = -1; // right most * or /
            int c3 = -1; // left most unary op
            for (int i = start; i < stop; i++)
            {
                var token = tokens[i];
                if (token.OperatorTest(Operator.LeftBrace))
                    p++;
                else if (token.OperatorTest(Operator.RightBrace))
                {
                    p--;
                    if (p < 0)
                        throw new InvalidExpresionException(
                            "Braces aren't balanced.");
                }
                else if (p == 0)
                {
                    if (token.OperatorTest(Operator.Plus)
                        || token.OperatorTest(Operator.Minus))
                        c1 = i;
                    else if (token.OperatorTest(Operator.Multiply)
                             || token.OperatorTest(Operator.Divide))
                        c2 = i;
                    else if ((token.OperatorTest(Operator.Positive)
                             || token.OperatorTest(Operator.Negative))
                             && c3 == -1)
                        c3 = i;
                }
            }

            if (p != 0)
                throw new InvalidExpresionException("Braces aren't balanced.");

            if (c1 == -1 && c2 == -1 && c3 == -1)
            {
                if (!(tokens[start].OperatorTest(Operator.LeftBrace)
                      && tokens[stop - 1].OperatorTest(Operator.RightBrace)))
                    throw new InvalidExpresionException();
                // whole expr wrapped by brace
                return CalculateRecursive(tokens, start + 1, stop - 1);
            }
            if (c1 != -1)
            {
                decimal a1 = CalculateRecursive(tokens, start, c1);
                decimal a2 = CalculateRecursive(tokens, c1 + 1, stop);
                return BinaryCalculate(tokens[c1].Operator, a1, a2);
            }
            else if (c2 != -1)
            {
                decimal a1 = CalculateRecursive(tokens, start, c2);
                decimal a2 = CalculateRecursive(tokens, c2 + 1, stop);
                return BinaryCalculate(tokens[c2].Operator, a1, a2);
            }
            else if (c3 != -1)
            {
                if (c3 != start)
                    throw new InvalidExpresionException();
                decimal a = CalculateRecursive(tokens, c3 + 1, stop);
                return UnaryCalculate(tokens[c3].Operator, a);
            }
            else
                throw new InvalidExpresionException();
        }

        /// <summary>
        /// a1 op a2
        /// </summary>
        private static decimal BinaryCalculate(Operator op, decimal a1, decimal a2)
        {
            if (op == Operator.Plus)
                return a1 + a2;
            else if (op == Operator.Minus)
                return a1 - a2;
            else if (op == Operator.Multiply)
                return a1 * a2;
            else if (op == Operator.Divide)
                return a1 / a2;
            else
                throw new ApplicationException(
                    $"Invalid operator {op} in binary calculation.");
        }

        /// <summary>
        /// op a
        /// </summary>
        private static decimal UnaryCalculate(Operator op, decimal a)
        {
            if (op == Operator.Positive)
                return a;
            else if (op == Operator.Negative)
                return -a;
            else
                throw new ApplicationException(
                    $"Invalid operator {op} in unary calculation.");
        }
    }

    public class InvalidExpresionException : ApplicationException
    {
        public InvalidExpresionException() { }
        public InvalidExpresionException(string message)
            : base(message) { }
    }
}
