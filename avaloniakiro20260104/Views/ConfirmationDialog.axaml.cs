using Avalonia.Controls;
using Avalonia.Interactivity;

namespace avaloniakiro20260104.Views;

public partial class ConfirmationDialog : Window
{
    public bool Result { get; private set; } = false;

    public ConfirmationDialog()
    {
        InitializeComponent();
        SetupEventHandlers();
    }

    public ConfirmationDialog(string message) : this()
    {
        MessageTextBlock.Text = message;
    }

    private void SetupEventHandlers()
    {
        ConfirmButton.Click += OnConfirmClick;
        CancelButton.Click += OnCancelClick;
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }
}