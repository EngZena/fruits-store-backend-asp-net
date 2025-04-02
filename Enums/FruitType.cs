using System.Text.Json.Serialization;

namespace fruits_store_backend_asp_net.Enums
{
    /// <summary>
    /// Represents the type of the fruit.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FruitType
    {
        /// <summary>
        /// Winter Fruits (0).
        /// </summary>
        WINTER_FRUITS,

        /// <summary>
        /// Summer Fruits (1).
        /// </summary>

        SUMMER_FRUITS,
    }
}
