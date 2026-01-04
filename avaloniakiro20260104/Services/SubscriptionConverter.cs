using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

/// <summary>
/// 自定義 Subscription JSON 轉換器，處理 API 欄位映射
/// </summary>
public class SubscriptionConverter : JsonConverter<Subscription>
{
    public override Subscription Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var subscription = new Subscription();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return subscription;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "id":
                        subscription.Id = reader.GetString() ?? string.Empty;
                        break;
                    case "name":
                        subscription.Name = reader.GetString() ?? string.Empty;
                        break;
                    case "nextdate":
                        var dateString = reader.GetString();
                        if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out var date))
                        {
                            subscription.NextPaymentDate = date;
                        }
                        else
                        {
                            Console.WriteLine($"Failed to parse nextdate: {dateString}");
                            subscription.NextPaymentDate = DateTime.Now.AddMonths(1);
                        }
                        break;
                    case "price":
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            subscription.Amount = reader.GetDecimal();
                        }
                        else if (reader.TokenType == JsonTokenType.Null)
                        {
                            subscription.Amount = 0;
                        }
                        else
                        {
                            Console.WriteLine($"Unexpected price token type: {reader.TokenType}");
                            subscription.Amount = 0;
                        }
                        break;
                    case "site":
                        subscription.Url = reader.GetString() ?? string.Empty;
                        break;
                    case "note":
                        subscription.Description = reader.GetString();
                        break;
                    case "account":
                        subscription.Account = reader.GetString();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Subscription value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        if (!string.IsNullOrEmpty(value.Id))
            writer.WriteString("id", value.Id);
        
        writer.WriteString("name", value.Name);
        writer.WriteString("nextdate", value.NextPaymentDate.ToString("yyyy-MM-ddTHH:mm:ss"));
        writer.WriteNumber("price", value.Amount);
        writer.WriteString("site", value.Url);
        
        if (!string.IsNullOrEmpty(value.Description))
            writer.WriteString("note", value.Description);
        else
            writer.WriteNull("note");
        
        if (!string.IsNullOrEmpty(value.Account))
            writer.WriteString("account", value.Account);
        else
            writer.WriteNull("account");
        
        writer.WriteEndObject();
    }
}