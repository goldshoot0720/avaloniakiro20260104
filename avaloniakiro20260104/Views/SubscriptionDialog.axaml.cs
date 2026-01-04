using Avalonia.Controls;
using Avalonia.Interactivity;
using avaloniakiro20260104.Models;
using System;

namespace avaloniakiro20260104.Views;

public partial class SubscriptionDialog : Window
{
    public Subscription? Result { get; private set; }
    
    public SubscriptionDialog()
    {
        InitializeComponent();
        InitializeDefaults();
    }
    
    public SubscriptionDialog(Subscription subscription) : this()
    {
        LoadSubscription(subscription);
    }
    
    private void InitializeDefaults()
    {
        NextPaymentDatePicker.SelectedDate = DateTime.Now.AddMonths(1);
        StartDatePicker.SelectedDate = DateTime.Now;
        BillingCycleComboBox.SelectedIndex = 0; // 每月
        CategoryComboBox.SelectedIndex = 0; // 娛樂
        PaymentMethodComboBox.SelectedIndex = 0; // 信用卡
        
        SaveButton.Click += SaveButton_Click;
        CancelButton.Click += CancelButton_Click;
    }
    
    private void LoadSubscription(Subscription subscription)
    {
        NameTextBox.Text = subscription.Name;
        AmountNumeric.Value = (decimal)subscription.Amount;
        NextPaymentDatePicker.SelectedDate = subscription.NextPaymentDate;
        StartDatePicker.SelectedDate = subscription.StartDate;
        UrlTextBox.Text = subscription.Url;
        DescriptionTextBox.Text = subscription.Description;
        IsActiveCheckBox.IsChecked = subscription.IsActive;
        
        // 設定計費週期
        for (int i = 0; i < BillingCycleComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)BillingCycleComboBox.Items[i]!).Tag?.ToString() == subscription.BillingCycle.ToString())
            {
                BillingCycleComboBox.SelectedIndex = i;
                break;
            }
        }
        
        // 設定分類
        for (int i = 0; i < CategoryComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)CategoryComboBox.Items[i]!).Content?.ToString() == subscription.Category)
            {
                CategoryComboBox.SelectedIndex = i;
                break;
            }
        }
        
        // 設定付款方式
        for (int i = 0; i < PaymentMethodComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)PaymentMethodComboBox.Items[i]!).Content?.ToString() == subscription.PaymentMethod)
            {
                PaymentMethodComboBox.SelectedIndex = i;
                break;
            }
        }
    }
    
    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            // 可以顯示錯誤訊息
            return;
        }
        
        Result = new Subscription
        {
            Name = NameTextBox.Text,
            Amount = (decimal)AmountNumeric.Value,
            NextPaymentDate = NextPaymentDatePicker.SelectedDate?.DateTime ?? DateTime.Now.AddMonths(1),
            StartDate = StartDatePicker.SelectedDate?.DateTime ?? DateTime.Now,
            BillingCycle = int.Parse(((ComboBoxItem)BillingCycleComboBox.SelectedItem!)?.Tag?.ToString() ?? "1"),
            Category = ((ComboBoxItem)CategoryComboBox.SelectedItem!)?.Content?.ToString() ?? "娛樂",
            PaymentMethod = ((ComboBoxItem)PaymentMethodComboBox.SelectedItem!)?.Content?.ToString() ?? "信用卡",
            Url = UrlTextBox.Text ?? string.Empty,
            Description = DescriptionTextBox.Text ?? string.Empty,
            IsActive = IsActiveCheckBox.IsChecked ?? true
        };
        
        Close(Result);
    }
    
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}