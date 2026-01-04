using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace avaloniakiro20260104.Models;

public partial class FoodItem : ObservableObject
{
    /// <summary>
    /// 帳戶id uuid自動產生
    /// </summary>
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    /// <summary>
    /// 食品名稱
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    [ObservableProperty]
    private int _amount = 1;

    /// <summary>
    /// 圖片網址
    /// </summary>
    [ObservableProperty]
    private string _photo = string.Empty;

    /// <summary>
    /// 圖片雜湊值
    /// </summary>
    [ObservableProperty]
    private string? _photoHash;

    /// <summary>
    /// 價錢
    /// </summary>
    [ObservableProperty]
    private decimal? _price;

    /// <summary>
    /// 商店
    /// </summary>
    [ObservableProperty]
    private string? _shop;

    /// <summary>
    /// 有效期限
    /// </summary>
    [ObservableProperty]
    private DateTime _toDate = DateTime.Now.AddDays(7);
}