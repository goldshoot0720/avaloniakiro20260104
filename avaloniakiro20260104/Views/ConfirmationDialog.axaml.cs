using Avalonia.Controls;
using Avalonia.Interactivity;

namespace avaloniakiro20260104.Views;

public partial class ConfirmationDialog : Window
{
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
        Close(true); // 返回 true 表示確認
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false); // 返回 false 表示取消
    }
}