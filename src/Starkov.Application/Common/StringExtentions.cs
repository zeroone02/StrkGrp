using System.Security.Cryptography;
using System.Text;

namespace Starkov.Application.Common;
public static class StringExtentions
{
    public static string NormalizePhoneNumber(this string phone)
    {
        phone = new string(phone.Where(x => char.IsDigit(x)).ToArray());

        //ожидаю что номер России
        if (phone.Length < 10 || phone.Length > 11)
        {
            return string.Empty;
        }

        if (phone[0] != 7)
        {
            phone = "7" + phone;
        }

        return phone;
    }

    public static string TrimEmptyEntries(this string str)
    {
        var arr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', arr);
    }

    public static string ToMd5(this string str)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(str);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
