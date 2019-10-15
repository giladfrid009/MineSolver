using System;

namespace Minesolver
{
    public static class OnlineGraphics
    {
        public static char MineChar = '@';

        public static void Subscibe(MineFieldBase field)
        {
            field.OnFlag += PrintMine;
            field.OnUnflag += PrintHidden;
            field.OnReveal += PrintVal;
        }

        public static void Unsubscibe(MineFieldBase field)
        {
            field.OnFlag -= PrintMine;
            field.OnUnflag -= PrintHidden;
            field.OnReveal -= PrintVal;
        }

        private static void PrintMine(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y);
            Console.Write(MineChar);
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
    }
}
