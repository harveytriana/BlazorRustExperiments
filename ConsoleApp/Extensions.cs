/*
 * Optional strategy for manage strings
 */
using System.Runtime.InteropServices;
using System.Text;

static class Extensions
{
    public static string? TextFromPointer(this IntPtr pointer)
    {
        return Marshal.PtrToStringUTF8(pointer);
    }

    public static byte[] Utf8Text(this string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }
}