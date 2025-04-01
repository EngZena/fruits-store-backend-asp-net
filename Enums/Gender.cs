using System.Text.Json.Serialization;

namespace FruitsStoreBackendASPNET.Enums
{
    /// <summary>
    /// Represents the gender of a user.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        /// <summary>
        /// Male gender (0).
        /// </summary>
        Male = 0,

        /// <summary>
        /// Female gender (1).
        /// </summary>
        Female = 1,
    }
}
