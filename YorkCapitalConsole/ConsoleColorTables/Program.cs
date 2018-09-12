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
                    OtherOptions = new ConsoleOtherOptions { Sort = new Wrappers.Consoles.ConsoleSort { SortColumnIndex = 0 },
                                                             Output = new ConsoleOutput { Style = ConsoleOutputType.HTML } },
                    ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = ConsoleAlignment.LEFT , Color = ConsoleColor.Yellow, Wrap = ConsoleWrapType.WORDCHAR },
                        new ConsoleColumnOptions {Width = 30, Alignment = ConsoleAlignment.CENTER , Color = ConsoleColor.Green },
                        new ConsoleColumnOptions {Width = 20, Alignment = ConsoleAlignment.RIGHT , Color = ConsoleColor.Cyan, Aggregate = ConsoleAggregate.MEDIAN },
                    },

                    Headers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { Heading = "Information", Text = "Test Naming", Alignment = ConsoleAlignment.RIGHT },
                        new ConsoleHeaderFooterRow { Heading = "Sorting", Text = "Ascending" , Alignment = ConsoleAlignment.RIGHT },
                        new ConsoleHeaderFooterRow { Heading = "Wrapping", Text = "Word Wrapping" }
                    },
                    Footers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { Heading = "Signature", Text = "Machine" }                        
                    },

                    Rows = new List<ConsoleRow>
                    {
                         new ConsoleRow
                         {
                             Column = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Name" },
                                 new ConsoleRecord { Text = "Display Name"},
                                 new ConsoleRecord { Text = "Some Numbers"},
                             }
                         },
                         new ConsoleRow
                         {
                             Column = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Surpal~Singh Raja,      Rani Rajputana Hillay La bunglow. In Paris where the doors are bells and windows are curtained."},
                                 new ConsoleRecord { Text = "Reday To Install" },
                                 new ConsoleRecord { Text = "100.00" },
                             }
                         },
                         new ConsoleRow
                         {
                             Column = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Rajendra, Patil"},
                                 new ConsoleRecord { Text = "65 million History" },
                                 new ConsoleRecord { Text = "90.00" },
                             }
                         },
                         new ConsoleRow
                         {
                             Column = new List<ConsoleRecord>
                             {
                                 new ConsoleRecord { Text = "Nandu Halwai Panighat Ladayi Beer Sang Jai"},
                                 new ConsoleRecord { Text = "Current Century" },
                                 new ConsoleRecord { Text = "23.00" },
                             }
                         }
                    }
                });        
        }
    }
}
