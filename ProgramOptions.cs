using System.Text.Json;
using System.Text.Json.Serialization;

namespace Android18;

internal class ProgramOptions
{
    public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}
