using Elevator.Challenge.Console.Services;

namespace Elevator.Challenge.Console.Helpers
{
    using System;

    public class PrintHelper : IPrintHelper
    {
        public void Print(string text, ConsoleColor? consoleColor = null, bool resetColor = true)
        {
            if (consoleColor != null)
            {
                Console.ForegroundColor = (ConsoleColor)consoleColor;
            }
            Console.WriteLine(text);

            if (resetColor)
            {
                Console.ResetColor();
            }
            
        }
    }
}