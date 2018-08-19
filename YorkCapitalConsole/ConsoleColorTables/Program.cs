using System;
using System.Collections.Generic;
using Wrappers;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;

namespace ConsoleColorTables
{
    public class Program
    {
        static void Main(string[] args)
        {
            DrawTable();
            Console.ReadKey();
        }

        private static void DrawTable()
        {   
                WrapConsole.WriteTable(new ConsoleTable
                {
                    ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = WrapAlignment.LEFT , Color = ConsoleColor.Yellow  },
                        new ConsoleColumnOptions {Width = 30, Alignment = WrapAlignment.CENTER , Color = ConsoleColor.White },
                        new ConsoleColumnOptions {Width = 20, Alignment = WrapAlignment.RIGHT , Color = ConsoleColor.Cyan },
                    },

                    Rows = new List<ConsoleRow>
                    {
                         new ConsoleRow
                         {
                             Row = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Name", Color = ConsoleColor.DarkGray },
                                 new ConsoleRecord { Text = "Display Name", Color = ConsoleColor.DarkGray},
                                 new ConsoleRecord { Text = "Some Numbers", Color = ConsoleColor.DarkGray},
                             }
                         },
                         new ConsoleRow
                         {
                             Row = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Surpal, Singh"},
                                 new ConsoleRecord { Text = "Reday To Install" },
                                 new ConsoleRecord { Text = "100.00" },
                             }
                         }
                }   });
        
        }
    }
}
