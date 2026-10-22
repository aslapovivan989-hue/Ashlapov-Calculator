using AshlapovCalculator.Wpf;

namespace AshlapovCalculator.Tests;

[TestClass]
public class MainViewModelTests
{
    [TestMethod]
    public void DigitAfterCalculation_StartsNewExpression()
    {
        var model = Evaluate("2+3");
        model.AddSymbol("7");
        Assert.AreEqual("7", model.Expression);
    }

    [TestMethod]
    public void OperatorAfterCalculation_ContinuesFromResult()
    {
        var model = Evaluate("2+3");
        model.AddSymbol("*");
        model.AddSymbol("4");
        model.Calculate();
        Assert.AreEqual("20", model.Result);
    }

    [TestMethod]
    public void PowerAfterNegativeResult_PreservesTheResultAsBase()
    {
        var model = Evaluate("2-4");
        model.AddSymbol("^");
        model.AddSymbol("2");
        model.Calculate();
        Assert.AreEqual("4", model.Result);
    }

    [TestMethod]
    public void DivisionError_IsDisplayedSeparatelyAndCanBeCorrected()
    {
        var model = Evaluate("5/0");
        Assert.AreEqual("Ошибка", model.Result);
        model.Backspace();
        model.AddSymbol("2");
        model.Calculate();
        Assert.IsTrue(double.TryParse(model.Result, out double value));
        Assert.AreEqual(2.5, value);
    }

    [TestMethod]
    public void DecimalAfterCalculation_StartsNewFraction()
    {
        var model = Evaluate("2+3");
        model.AddSymbol(",");
        model.AddSymbol("5");
        Assert.AreEqual("0,5", model.Expression);
    }

    [TestMethod]
    public void BackspaceAfterCalculation_EditsOriginalExpression()
    {
        var model = Evaluate("2+3");
        model.Backspace();
        model.AddSymbol("4");
        model.Calculate();
        Assert.AreEqual("6", model.Result);
    }

    [TestMethod]
    public void SuccessfulCalculations_AreStoredNewestFirst()
    {
        var model = Evaluate("2+3");
        model.Expression = "6*7";
        model.Calculate();
        Assert.HasCount(2, model.History);
        Assert.AreEqual("6*7", model.History[0].Expression);
        Assert.AreEqual("42", model.History[0].Result);
    }

    [TestMethod]
    public void Error_IsNotStoredInHistory()
    {
        var model = Evaluate("5/0");
        Assert.IsEmpty(model.History);
        StringAssert.Contains(model.ErrorMessage, "ноль");
    }

    [TestMethod]
    public void RepeatedEquals_DoesNotDuplicateTheLatestEntry()
    {
        var model = Evaluate("2+3");
        model.Calculate();
        Assert.HasCount(1, model.History);
    }

    [TestMethod]
    public void History_IsLimitedToTwentyNewestEntries()
    {
        var model = new MainViewModel();
        for (int i = 0; i < 25; i++)
        {
            model.Expression = i + "+1";
            model.Calculate();
        }
        Assert.HasCount(20, model.History);
        Assert.AreEqual("24+1", model.History[0].Expression);
        Assert.AreEqual("5+1", model.History[19].Expression);
    }

    [TestMethod]
    public void HistoryCommand_RestoresAnEditableExpression()
    {
        var model = Evaluate("2+3");
        var entry = model.History[0];
        model.Clear();
        model.UseHistoryCommand.Execute(entry);
        Assert.AreEqual("2+3", model.Expression);
        model.Backspace();
        model.AddSymbol("4");
        model.Calculate();
        Assert.AreEqual("6", model.Result);
    }

    [TestMethod]
    public void ClearExpression_KeepsHistoryAndClearsError()
    {
        var model = Evaluate("2+3");
        model.Expression = "1/0";
        model.Calculate();
        model.Clear();
        Assert.AreEqual("", model.Expression);
        Assert.AreEqual("0", model.Result);
        Assert.AreEqual("", model.ErrorMessage);
        Assert.HasCount(1, model.History);
    }

    [TestMethod]
    public void ClearHistory_DoesNotChangeCurrentExpression()
    {
        var model = Evaluate("2+3");
        model.ClearHistoryCommand.Execute(null);
        Assert.IsEmpty(model.History);
        Assert.AreEqual("2+3", model.Expression);
        Assert.AreEqual("5", model.Result);
    }

    [TestMethod]
    public void OperatorAfterLargeResult_UsesAParsableDecimal()
    {
        var model = Evaluate("10^20");
        model.AddSymbol("/");
        model.AddSymbol("10^20");
        model.Calculate();
        Assert.AreEqual("1", model.Result);
    }

    private static MainViewModel Evaluate(string expression)
    {
        var model = new MainViewModel { Expression = expression };
        model.Calculate();
        return model;
    }

    [TestMethod]
    [DataRow("1234567890123456")]
    [DataRow("1234567.123456789")]
    [DataRow("0.0000000001")]
    public void ContinuingFromResult_PreservesSignificantDigits(string expression)
    {
        var model = Evaluate(expression);
        model.AddSymbol("-");
        model.AddSymbol(expression);
        model.Calculate();
        Assert.AreEqual("0", model.Result);
    }
}
