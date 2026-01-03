using System;

namespace avaloniakiro20260104.Models;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime NextPaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "正常";
    public string Url { get; set; } = string.Empty;
}