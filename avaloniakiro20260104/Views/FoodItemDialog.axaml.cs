using Avalonia.Controls;
using Avalonia.Interactivity;
using avaloniakiro20260104.Models;
using System;

namespace avaloniakiro20260104.Views;

public partial class FoodItemDialog : Window
{
    public FoodItem? Result { get; private set; }
    
    public FoodItemDialog()
    {
        InitializeComponent();
        InitializeDefaults();
    }
    
    public FoodItemDialog(FoodItem foodItem) : this()
    {
        LoadFoodItem(foodItem);
    }
    
    private void InitializeDefaults()
    {
        ToDatePicker.SelectedDate = DateTime.Now.AddDays(7);
        
        SaveButton.Click += SaveButton_Click;
        CancelButton.Click += CancelButton_Click;
    }
    
    private void LoadFoodItem(FoodItem foodItem)
    {
        // API 支援的欄位
        NameTextBox.Text = foodItem.Name;
        AmountNumeric.Value = foodItem.Amount;
        ToDatePicker.SelectedDate = foodItem.ToDate;
        PhotoTextBox.Text = foodItem.Photo;
        PriceNumeric.Value = (decimal)(foodItem.Price ?? 0m);
        ShopTextBox.Text = foodItem.Shop ?? string.Empty;
    }
    
    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            // 可以顯示錯誤訊息
            return;
        }
        
        Result = new FoodItem
        {
            // API 支援的欄位
            Name = NameTextBox.Text,
            Amount = (int)(AmountNumeric.Value ?? 1),
            ToDate = ToDatePicker.SelectedDate?.DateTime ?? DateTime.Now.AddDays(7),
            Photo = PhotoTextBox.Text ?? string.Empty,
            Price = PriceNumeric.Value > 0 ? (decimal?)PriceNumeric.Value : null,
            Shop = string.IsNullOrWhiteSpace(ShopTextBox.Text) ? null : ShopTextBox.Text
        };
        
        Close(Result);
    }
    
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}