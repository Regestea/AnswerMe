namespace AnswerMe.Client.Core.Extensions;

public static class TextExtension
{
    public static string FirstLetter(this string content)
    {
        try
        {
            return content.Substring(0, 1).ToUpper();
        }
        catch (Exception e)
        {
          return  "";
        }
       
    }
}