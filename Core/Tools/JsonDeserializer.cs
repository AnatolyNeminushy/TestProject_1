using System.Text.Json;

namespace TestProjectIntern_n1.Core.Tools;

/// <summary>
/// Инструмент для десериализации данных.
/// </summary>
public static class JsonDeserializer
{
    /// <summary>
    /// Метод возвращающий десериализированный объект данных.
    /// </summary>
    /// <typeparam name="T">общий тип данных.</typeparam>
    /// <param name="content">объект,который нужно десериализировать.</param>
    /// <returns>Десериализованный объект данных.</returns>
    public static T? DeserializeData<T>(string content)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var result = JsonSerializer.Deserialize<T>(content, options);

        return result;
    }
}
