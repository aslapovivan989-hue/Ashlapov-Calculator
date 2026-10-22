namespace AshlapovCalculator.Core;

/// <summary>Арифметические операции калькулятора.</summary>
public class Calculator
{
    public double Add(double a, double b)
    {
        return a + b;
    }

    public double Subtract(double a, double b)
    {
        return a - b;
    }

    public double Multiply(double a, double b)
    {
        return a * b;
    }

    public double Divide(double a, double b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Деление на ноль невозможно");
        }

        return a / b;
    }

    public double Power(double a, double b)
    {
        return Math.Pow(a, b);
    }

    public double Calculate(double a, char operation, double b)
    {
        switch (operation)
        {
            case '+':
                return Add(a, b);
            case '-':
                return Subtract(a, b);
            case '*':
                return Multiply(a, b);
            case '/':
                return Divide(a, b);
            case '^':
                return Power(a, b);
            default:
                throw new ArgumentException("Неизвестная операция: " + operation);
        }
    }
}
