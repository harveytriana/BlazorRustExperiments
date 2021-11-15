/*
 * Optional strategy for manage strings
 */
using System.Text;

static class Extensions
{
    public static byte[] Utf8Text(this string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }
}