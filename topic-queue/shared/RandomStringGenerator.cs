using System.Text;

namespace shared;

public static class RandomStringGenerator
{
    private static Random _random = new Random();

    public static string GenerateRandomString(int len = 5)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < len; i++)
        {
            int index = _random.Next(chars.Length);
            builder.Append(chars[index]);
        }

        return builder.ToString();
    }
}
