using System.Text.Json.Serialization;

namespace CountryMaps.TerminalsLoader.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OfficeType
    {
        Terminal = 1,
        Branch = 2
    }
}
