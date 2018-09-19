using Extensions;
using System;

namespace Wrappers.Consoles.Enums
{
    public static class ConsoleWebColors
    {        
        public const string Black = "#000000";
        public const string DarkBlue = "#003380";
        public const string DarkGray = "#4d4d4d";
        public const string DarkGreen = "#00802b";
        public const string DarkCyan = "#00bfff";       
        public const string DarkMagenta = "#e6005c";
        public const string DarkRed = "#e60000";
        public const string DarkYellow = "#daa520";        
        public const string Blue = "#66a3ff";
        public const string Gray = "#bfbfbf";
        public const string Green = "#1aff1a";
        public const string Cyan = "#66d9ff";
        public const string Red = "#ff3333";
        public const string Magenta = "#ff4d94";
        public const string Yellow = "#ffff00";
        public const string White = "#ffffff";

        public static string Get(string color)
        {
            switch(color)
            {
                case "Black":        return Black;
                case "DarkBlue":     return DarkBlue;
                case "DarkGray":     return DarkGray;
                case "DarkGreen":    return DarkGreen;
                case "DarkCyan":     return DarkCyan;
                case "DarkMagenta":  return DarkMagenta;
                case "DarkRed":      return DarkRed;
                case "DarkYellow":   return DarkYellow;
                case "Blue":         return Blue;
                case "Gray":         return Gray;
                case "Green":        return Green;
                case "Cyan":         return Cyan;
                case "Red":          return Red;
                case "Magenta":      return Magenta;
                case "Yellow":       return Yellow;
                case "White":        return White;
            }

            return color;
        }
        
        public static string Get(ConsoleColor color)
        {
            return Get(color.ToString());
        }

        public static string GetExcel(ConsoleColor color)
        {
            return Get(color).Replace("#", "");
        }

    }
}
