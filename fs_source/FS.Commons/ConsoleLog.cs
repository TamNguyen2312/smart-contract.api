namespace FS.Commons;

public class ConsoleLog
{
    public static void WriteExceptionToConsoleLog(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Constants.SomeThingWentWrong);
        Console.WriteLine(exception.Message, exception.StackTrace);
        Console.ResetColor();
    }
}