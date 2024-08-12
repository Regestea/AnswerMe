namespace AnswerMe.Client.Core.Extensions;

public static class TextExtension
{
    public static string SummarizeIfMoreThan(this string input, int minLength)
    {
        if (input.Length < minLength)
        {
            return input;
        }

        string firstPart = input.Substring(0, 7);
        string lastPart = input.Substring(input.Length - 4);

        return firstPart + "..." + lastPart;
    }
    public static string FirstLetter(this string content)
    {
        try
        {
            return content.Substring(0, 1).ToUpper();
        }
        catch (Exception)
        {
          return  "";
        }
       
    }
}