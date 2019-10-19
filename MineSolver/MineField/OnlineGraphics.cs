using System;

namespace Minesolver
{
    public static class OnlineGraphics
    {
        public static void Subscibe(MineFieldBase field)
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.CursorVisible = false;

            field.OnFlag += PrintMine;
            field.OnUnflag += PrintHidden;
            field.OnReveal += PrintVal;
            field.OnLoss += PrintLoss;
        }

        public static void Unsubscibe(MineFieldBase field)
        {
            field.OnFlag -= PrintMine;
            field.OnUnflag -= PrintHidden;
            field.OnReveal -= PrintVal;
            field.OnLoss -= PrintLoss;
        }

        private static void PrintMine(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y);
            Console.Write('@');
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void PrintHidden(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(' ');
        }

        private static void PrintVal(int x, int y, int val)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(val);
        }

        private static void PrintLoss(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(x, y);
            Console.Write('@');
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
