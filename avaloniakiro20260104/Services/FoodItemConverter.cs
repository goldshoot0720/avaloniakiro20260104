using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using avaloniakiro20260104.Models;

namespace avaloniakiro20260104.Services;

public class FoodItemConverter : JsonConverter<FoodItem>
{
    public override FoodItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var foodItem = new FoodItem();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return foodItem;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "id":
                        foodItem.Id = reader.GetString() ?? string.Empty;
                        break;
                    case "name":
                        foodItem.Name = reader.GetString() ?? string.Empty;
                        break;
                    case "amount":
                        foodItem.Amount = reader.GetInt32();
                        break;
                    case "to_date":
                        var dateString = reader.GetString();
                        if (!string.IsNullOrEmpty(dateString) && DateTime.TryParse(dateString, out var date))
                        {
                            foodItem.ToDate = date; // Use ToDate as per the model
                        }
                        break;
                    case "photo":
                        foodItem.Photo = reader.GetString() ?? string.Empty;
                        break;
                    case "photohash":
                        foodItem.PhotoHash = reader.GetString(); // Fixed: store photohash
                        break;
                    case "shop":
                        foodItem.Shop = reader.GetString();
                        break;
                    case "price":
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            foodItem.Price = reader.GetDecimal();
                        }
                        else if (reader.TokenType == JsonTokenType.Null)
                        {
                            foodItem.Price = null;
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, FoodItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        if (!string.IsNullOrEmpty(value.Id))
            writer.WriteString("id", value.Id);
        
        writer.WriteString("name", value.Name);
        writer.WriteNumber("amount", value.Amount);
        writer.WriteString("to_date", value.ToDate.ToString("yyyy-MM-ddTHH:mm:ss"));
        
        if (!string.IsNullOrEmpty(value.Photo))
            writer.WriteString("photo", value.Photo);
        
        if (!string.IsNullOrEmpty(value.PhotoHash))
            writer.WriteString("photohash", value.PhotoHash);
        
        if (value.Price.HasValue)
            writer.WriteNumber("price", value.Price.Value);
        else
            writer.WriteNull("price");
        
        if (!string.IsNullOrEmpty(value.Shop))
            writer.WriteString("shop", value.Shop);
        else
            writer.WriteNull("shop");
        
        writer.WriteEndObject();
    }
}