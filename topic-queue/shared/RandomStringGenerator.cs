using System.Text;

namespace shared;

public static class RandomStringGenerator
{
    private static Random _random = new Random();

    public static string GenerateRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < 5; i++)
        {
            int index = _random.Next(chars.Length);
            builder.Append(chars[index]);
        }

        return builder.ToString();
    }
}
