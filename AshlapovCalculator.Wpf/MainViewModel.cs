using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AshlapovCalculator.Core;

namespace AshlapovCalculator.Wpf;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ExpressionCalculator _calculator = new ExpressionCalculator();
    private string _expression = "";
    private string _result = "0";
    private string _errorMessage = "";
    private bool _completed;
    private double _lastValue;

    public MainViewModel()
    {
        AddSymbolCommand = new RelayCommand(parameter => AddSymbol(parameter?.ToString() ?? ""));
        CalculateCommand = new RelayCommand(_ => Calculate());
        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        UseHistoryCommand = new RelayCommand(item =>
        {
            if (item is CalculationHistoryItem entry)
            {
                Expression = entry.Expression;
                Result = entry.Result;
            }
        });
        ClearHistoryCommand = new RelayCommand(_ => History.Clear());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Expression
    {
        get => _expression;
        set
        {
            _expression = value;
            _completed = false;
            ErrorMessage = "";
            OnPropertyChanged();
        }
    }

    public string Result
    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddSymbolCommand { get; }

    public ICommand CalculateCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand BackspaceCommand { get; }

    public ICommand UseHistoryCommand { get; }

    public ICommand ClearHistoryCommand { get; }

    public ObservableCollection<CalculationHistoryItem> History { get; } = new();

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public void AddSymbol(string symbol)
    {
        if (string.IsNullOrEmpty(symbol)) return;

        if (_completed)
        {
            if (symbol.Length == 1 && "+-*/^".Contains(symbol))
            {
                // Без экспоненциальной записи: парсер принимает обычные десятичные числа.
                string value = _lastValue.ToString("F10", CultureInfo.InvariantCulture)
                    .TrimEnd('0').TrimEnd('.');
                Expression = _lastValue < 0 ? "(" + value + ")" : value;
            }
            else
            {
                Expression = symbol is "," or "." ? "0" : "";
                Result = "0";
            }
        }

        if (ErrorMessage.Length > 0) Result = "0";
        Expression += symbol;
    }

    public void Calculate()
    {
        try
        {
            _lastValue = _calculator.Calculate(Expression);
            Result = _lastValue.ToString(CultureInfo.CurrentCulture);
            ErrorMessage = "";
            _completed = true;
            var entry = new CalculationHistoryItem(Expression, Result);
            if (History.Count == 0 || History[0] != entry)
                History.Insert(0, entry);
            if (History.Count > 20) History.RemoveAt(History.Count - 1);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or ArithmeticException)
        {
            Result = "Ошибка";
            ErrorMessage = ex.Message;
            _completed = false;
        }
    }

    public void Clear()
    {
        Expression = "";
        Result = "0";
    }

    public void Backspace()
    {
        if (ErrorMessage.Length > 0) Result = "0";
        if (Expression.Length > 0)
        {
            Expression = Expression.Substring(0, Expression.Length - 1);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed record CalculationHistoryItem(string Expression, string Result);
