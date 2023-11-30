namespace Starkov.Application.Common;
public static class StringExtentions
{
    public static string NormalizePhoneNumber(string phone)
    {
        phone = new string(phone.Where(x => char.IsDigit(x)).ToArray());

        //ожидаю что номер России
        if (phone.Length < 10 || phone.Length > 11) 
        {
            throw new ArgumentNullException();
        }

        if (phone[0] != 7)
        {
            phone = "7" + phone;
        }

        return phone;
    }
}
