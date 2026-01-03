using System;

namespace avaloniakiro20260104.Models;

public class FoodItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Status => ExpiryDate < DateTime.Now ? "已過期" : 
                           ExpiryDate <= DateTime.Now.AddDays(7) ? "即將過期" : "正常";
}