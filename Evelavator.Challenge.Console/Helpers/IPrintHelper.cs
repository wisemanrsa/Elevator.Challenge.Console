namespace Elevator.Challenge.Console.Helpers
{
    public interface IPrintHelper
    {
        void Print(string text, ConsoleColor? consoleColor = null, bool resetColor = true);
    }
}