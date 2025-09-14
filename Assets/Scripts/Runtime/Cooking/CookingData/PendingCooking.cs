using System;
using Newtonsoft.Json;

namespace Cooking;

[Serializable]
public record PendingCooking
{
    [JsonProperty("food_id")] public string FoodId { get; init; } = string.Empty;
    [JsonProperty("finish_time")] public DateTime FinishTime { get; init; }
}