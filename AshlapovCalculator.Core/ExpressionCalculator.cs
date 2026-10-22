using System.Globalization;

namespace AshlapovCalculator.Core;

/// <summary>Вычисляет выражение вида "2+3*4" или "(2+3)*4" с учётом приоритета операций и скобок.</summary>
public class ExpressionCalculator
{
    private const string Operations = "+-*/^";

    private readonly Calculator _calculator = new Calculator();

    public double Calculate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("Выражение не введено");
        }

        var numbers = new List<double>();
        var signs = new List<int>();
        var operations = new List<char>();
        SplitExpression(expression.Replace(" ", ""), numbers, signs, operations);

        // Сначала степень, потом минус перед числом, потом умножение и деление, потом сложение и вычитание.
        CalculatePowers(numbers, signs, operations);
        ApplySigns(numbers, signs);
        CalculateOperations(numbers, operations, "*/");
        CalculateOperations(numbers, operations, "+-");

        double result = numbers[0];
        if (double.IsNaN(result) || double.IsInfinity(result))
        {
            throw new ArithmeticException("Результат невозможно вычислить");
        }

        // Округление убирает погрешность вида 0,1 + 0,2 = 0,30000000000000004.
        return Math.Round(result, 10);
    }

    // Делит выражение на числа, их знаки и операции: "-2+3*4" -> [2, 3, 4], [-1, 1, 1] и ['+', '*'].
    private void SplitExpression(string expression, List<double> numbers, List<int> signs, List<char> operations)
    {
        string number = "";
        int sign = 1;

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsDigit(c) || c == ',' || c == '.')
            {
                number += c;
            }
            else if (c == '(')
            {
                if (number != "")
                {
                    throw new FormatException("Перед скобкой нет знака операции");
                }

                int end = FindClosingBracket(expression, i);
                string inside = expression.Substring(i + 1, end - i - 1);
                if (inside == "")
                {
                    throw new FormatException("Пустые скобки");
                }

                // Выражение в скобках вычисляем отдельно и дальше работаем с ним как с обычным числом.
                number = Calculate(inside).ToString(CultureInfo.InvariantCulture);
                i = end;

                if (i + 1 < expression.Length && !Operations.Contains(expression[i + 1]))
                {
                    throw new FormatException("После скобки нет знака операции");
                }
            }
            else if (c == ')')
            {
                throw new FormatException("Лишняя закрывающая скобка");
            }
            else if (c == '-' && number == "" && (i == 0 || Operations.Contains(expression[i - 1])))
            {
                // Минус в начале или сразу после знака операции — это минус перед числом: -5, 2*-3.
                sign = -sign;
            }
            else if (Operations.Contains(c))
            {
                numbers.Add(ParseNumber(number));
                signs.Add(sign);
                operations.Add(c);
                number = "";
                sign = 1;
            }
            else
            {
                throw new FormatException("Недопустимый символ: " + c);
            }
        }

        numbers.Add(ParseNumber(number));
        signs.Add(sign);
    }

    private static int FindClosingBracket(string expression, int openIndex)
    {
        int depth = 0;

        for (int i = openIndex; i < expression.Length; i++)
        {
            if (expression[i] == '(')
            {
                depth++;
            }
            else if (expression[i] == ')')
            {
                depth--;
                if (depth == 0)
                {
                    return i;
                }
            }
        }

        throw new FormatException("Не хватает закрывающей скобки");
    }

    private static double ParseNumber(string text)
    {
        text = text.Replace(',', '.');

        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
        {
            throw new FormatException("Некорректное выражение");
        }

        return value;
    }

    // Степень считается справа налево: 2^3^2 = 2^9.
    // Минус перед основанием применяется после степени (-2^2 = -4),
    // а минус перед показателем — сразу (2^-2 = 0,25).
    private void CalculatePowers(List<double> numbers, List<int> signs, List<char> operations)
    {
        for (int i = operations.Count - 1; i >= 0; i--)
        {
            if (operations[i] == '^')
            {
                double exponent = signs[i + 1] * numbers[i + 1];
                numbers[i] = _calculator.Power(numbers[i], exponent);
                numbers.RemoveAt(i + 1);
                signs.RemoveAt(i + 1);
                operations.RemoveAt(i);
            }
        }
    }

    private static void ApplySigns(List<double> numbers, List<int> signs)
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            numbers[i] = signs[i] * numbers[i];
        }
    }

    private void CalculateOperations(List<double> numbers, List<char> operations, string symbols)
    {
        int i = 0;
        while (i < operations.Count)
        {
            if (symbols.Contains(operations[i]))
            {
                numbers[i] = _calculator.Calculate(numbers[i], operations[i], numbers[i + 1]);
                numbers.RemoveAt(i + 1);
                operations.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
