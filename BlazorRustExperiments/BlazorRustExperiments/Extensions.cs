/*
 * Optional strategy for manage strings
 */
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

static class Extensions
{
    public static string TextFromPointer(this IntPtr pointer)
    {
        return Marshal.PtrToStringUTF8(pointer);
    }

    public static byte[] Utf8Text(this string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    public static string TextFromUtf8(this byte[] utf8)
    {
        return Encoding.UTF8.GetString(utf8);
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