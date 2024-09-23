using System.Security.Cryptography;
using System.Text;

namespace Notes.Utils;

public static class StringExtensions
{
    private const string SALT = "2`)Z%X(m23n*df";

    public static string HashWithDate(this string content, DateTime dateNonce)
    {
        string dateString = dateNonce.ToString("yyyy-MM-dd HH:mm:ss"); // Format the date as a string
        var sb = new StringBuilder();
        sb.Append(dateString);
        sb.Append(SALT);
        sb.Append(content);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
