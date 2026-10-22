using AshlapovCalculator.Core;

namespace AshlapovCalculator.Tests;

[TestClass]
public class ExpressionCalculatorTests
{
    private readonly ExpressionCalculator _calculator = new ExpressionCalculator();

    [TestMethod]
    [DataRow("2+3", 5.0)]
    [DataRow("10-4", 6.0)]
    [DataRow("6*7", 42.0)]
    [DataRow("9/2", 4.5)]
    public void Calculate_OneOperation_ReturnsResult(string expression, double expected)
    {
        Assert.AreEqual(expected, _calculator.Calculate(expression));
    }

    [TestMethod]
    [DataRow("2+3*4", 14.0)]
    [DataRow("2*3^2", 18.0)]
    public void Calculate_OperationPriority_IsRespected(string expression, double expected)
    {
        Assert.AreEqual(expected, _calculator.Calculate(expression));
    }

    [TestMethod]
    public void Calculate_PowerChain_CalculatedFromRightToLeft()
    {
        Assert.AreEqual(512.0, _calculator.Calculate("2^3^2"));
    }

    [TestMethod]
    [DataRow("-5+3", -2.0)]
    [DataRow("2*-3", -6.0)]
    public void Calculate_NegativeNumbers_ReturnsResult(string expression, double expected)
    {
        Assert.AreEqual(expected, _calculator.Calculate(expression));
    }

    [TestMethod]
    public void Calculate_MinusBeforePower_PowerCalculatedFirst()
    {
        Assert.AreEqual(-4.0, _calculator.Calculate("-2^2"));
    }

    [TestMethod]
    public void Calculate_NegativeExponent_ReturnsFraction()
    {
        Assert.AreEqual(0.25, _calculator.Calculate("2^-2"));
    }

    [TestMethod]
    [DataRow("(2+3)*4", 20.0)]
    [DataRow("(-2)^2", 4.0)]
    [DataRow("2*(1+(5-2))", 8.0)]
    public void Calculate_Brackets_CalculatedFirst(string expression, double expected)
    {
        Assert.AreEqual(expected, _calculator.Calculate(expression));
    }

    [TestMethod]
    [DataRow("(2+3")]
    [DataRow("2+3)")]
    public void Calculate_WrongBrackets_ThrowsException(string expression)
    {
        Assert.ThrowsExactly<FormatException>(() => _calculator.Calculate(expression));
    }

    [TestMethod]
    public void Calculate_DecimalNumbers_ReturnsRoundedResult()
    {
        Assert.AreEqual(0.3, _calculator.Calculate("0,1+0,2"));
    }

    [TestMethod]
    public void Calculate_DivisionByZero_ThrowsException()
    {
        Assert.ThrowsExactly<DivideByZeroException>(() => _calculator.Calculate("5/0"));
    }

    [TestMethod]
    public void Calculate_EmptyExpression_ThrowsException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => _calculator.Calculate(""));
    }

    [TestMethod]
    [DataRow("2+")]
    [DataRow("2a+3")]
    public void Calculate_InvalidExpression_ThrowsException(string expression)
    {
        Assert.ThrowsExactly<FormatException>(() => _calculator.Calculate(expression));
    }

    [TestMethod]
    public void Calculate_TooBigResult_ThrowsException()
    {
        Assert.ThrowsExactly<ArithmeticException>(() => _calculator.Calculate("10^400"));
    }
}
