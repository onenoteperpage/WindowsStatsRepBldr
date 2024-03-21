using System.Text.Json.Serialization;

namespace WindowsStatsRepBldr.Models
{
    public class WinboxDataObj
    {
        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("Value")]
        public double Value { get; set; }
    }
}
