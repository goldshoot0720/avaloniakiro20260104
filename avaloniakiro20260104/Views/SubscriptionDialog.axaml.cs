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
        CategoryComboBox.SelectedIndex = 0; // 娛樂（僅本地）
        
        SaveButton.Click += SaveButton_Click;
        CancelButton.Click += CancelButton_Click;
    }
    
    private void LoadSubscription(Subscription subscription)
    {
        NameTextBox.Text = subscription.Name;
        AccountTextBox.Text = subscription.Account;
        AmountNumeric.Value = subscription.Amount;
        NextPaymentDatePicker.SelectedDate = subscription.NextPaymentDate;
        UrlTextBox.Text = subscription.Url;
        DescriptionTextBox.Text = subscription.Description;
        
        // 本地屬性（不同步到後端）
        IsActiveCheckBox.IsChecked = subscription.IsActive;
        
        // 設定分類（僅本地）
        for (int i = 0; i < CategoryComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)CategoryComboBox.Items[i]!).Content?.ToString() == subscription.Category)
            {
                CategoryComboBox.SelectedIndex = i;
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
            Account = AccountTextBox.Text,
            Amount = AmountNumeric.Value ?? 0,
            NextPaymentDate = NextPaymentDatePicker.SelectedDate?.DateTime ?? DateTime.Now.AddMonths(1),
            Url = UrlTextBox.Text ?? string.Empty,
            Description = DescriptionTextBox.Text ?? string.Empty,
            
            // 本地屬性（不同步到後端）
            Category = ((ComboBoxItem)CategoryComboBox.SelectedItem!)?.Content?.ToString() ?? "娛樂",
            IsActive = IsActiveCheckBox.IsChecked ?? true,
            PaymentMethod = "信用卡", // 預設值
            BillingCycle = 1, // 預設每月
            StartDate = DateTime.Now // 預設今天
        };
        
        Close(Result);
    }
    
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}