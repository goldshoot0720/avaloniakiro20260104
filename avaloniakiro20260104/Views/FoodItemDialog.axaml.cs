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
        ExpiryDatePicker.SelectedDate = DateTime.Now.AddDays(7);
        PurchaseDatePicker.SelectedDate = DateTime.Now;
        CategoryComboBox.SelectedIndex = 9; // 其他
        LocationComboBox.SelectedIndex = 0; // 冰箱
        
        SaveButton.Click += SaveButton_Click;
        CancelButton.Click += CancelButton_Click;
    }
    
    private void LoadFoodItem(FoodItem foodItem)
    {
        NameTextBox.Text = foodItem.Name;
        QuantityNumeric.Value = foodItem.Quantity;
        ExpiryDatePicker.SelectedDate = foodItem.ExpiryDate;
        PurchaseDatePicker.SelectedDate = foodItem.PurchaseDate;
        NotesTextBox.Text = foodItem.Notes;
        
        // 設定分類
        for (int i = 0; i < CategoryComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)CategoryComboBox.Items[i]!).Content?.ToString() == foodItem.Category)
            {
                CategoryComboBox.SelectedIndex = i;
                break;
            }
        }
        
        // 設定位置
        for (int i = 0; i < LocationComboBox.ItemCount; i++)
        {
            if (((ComboBoxItem)LocationComboBox.Items[i]!).Content?.ToString() == foodItem.Location)
            {
                LocationComboBox.SelectedIndex = i;
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
        
        Result = new FoodItem
        {
            Name = NameTextBox.Text,
            Quantity = (int)QuantityNumeric.Value,
            ExpiryDate = ExpiryDatePicker.SelectedDate?.DateTime ?? DateTime.Now.AddDays(7),
            PurchaseDate = PurchaseDatePicker.SelectedDate?.DateTime ?? DateTime.Now,
            Category = ((ComboBoxItem)CategoryComboBox.SelectedItem!)?.Content?.ToString() ?? "其他",
            Location = ((ComboBoxItem)LocationComboBox.SelectedItem!)?.Content?.ToString() ?? "冰箱",
            Notes = NotesTextBox.Text ?? string.Empty
        };
        
        Close(Result);
    }
    
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}