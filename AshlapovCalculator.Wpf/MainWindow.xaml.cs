using System.Windows;
using System.Windows.Input;

namespace AshlapovCalculator.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    // Ввод цифр и знаков с клавиатуры.
    private void Window_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if ("0123456789+-*/^(),.".Contains(e.Text))
        {
            ViewModel.AddSymbol(e.Text == "." ? "," : e.Text);
            e.Handled = true;
        }
        else if (e.Text == "=")
        {
            ViewModel.Calculate();
            e.Handled = true;
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ViewModel.Calculate();
            e.Handled = true;
        }
        else if (e.Key == Key.Back)
        {
            ViewModel.Backspace();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            ViewModel.Clear();
            e.Handled = true;
        }
    }
}
