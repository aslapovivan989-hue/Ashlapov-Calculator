using AshlapovCalculator.Core;

namespace AshlapovCalculator.Tests;

[TestClass]
public class CalculatorTests
{
    private readonly Calculator _calculator = new Calculator();

    [TestMethod]
    public void Add_TwoNumbers_ReturnsSum()
    {
        Assert.AreEqual(5.0, _calculator.Add(2, 3));
    }

    [TestMethod]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        Assert.AreEqual(-2.0, _calculator.Subtract(3, 5));
    }

    [TestMethod]
    public void Multiply_TwoNumbers_ReturnsProduct()
    {
        Assert.AreEqual(10.0, _calculator.Multiply(4, 2.5));
    }

    [TestMethod]
    public void Divide_TwoNumbers_ReturnsQuotient()
    {
        Assert.AreEqual(2.5, _calculator.Divide(10, 4));
    }

    [TestMethod]
    public void Divide_ByZero_ThrowsException()
    {
        Assert.ThrowsExactly<DivideByZeroException>(() => _calculator.Divide(5, 0));
    }

    [TestMethod]
    public void Power_TwoNumbers_ReturnsPower()
    {
        Assert.AreEqual(1024.0, _calculator.Power(2, 10));
    }

    [TestMethod]
    public void Power_NegativeExponent_ReturnsFraction()
    {
        Assert.AreEqual(0.25, _calculator.Power(2, -2));
    }

    [TestMethod]
    public void Calculate_UnknownOperation_ThrowsException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => _calculator.Calculate(1, '%', 2));
    }
}
