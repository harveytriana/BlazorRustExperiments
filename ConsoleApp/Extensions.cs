/*
 * Optional strategy for manage strings
 */
using System.Text;
using System.Text.Json;

static class Extensions
{
    public static string? TextFromPointer(this IntPtr pointer)
    {
        return Marshal.PtrToStringUTF8(pointer);
    }

    public static byte[] TextToBytes(this string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    public static string PrettyJson(this string plainJson)
    {
        var options = new JsonSerializerOptions() {
            WriteIndented = true
        };
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(plainJson);

        return JsonSerializer.Serialize(jsonElement, options);
    }
}