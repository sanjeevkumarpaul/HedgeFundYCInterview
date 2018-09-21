using System;
using System.Collections.Generic;
using Wrappers;
using Wrappers.Consoles;
using Wrappers.Consoles.Enums;
using Wrappers.Outputers;
using Extensions;

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
            var _table = new ConsoleTable
            {
                OtherOptions = new ConsoleOtherOptions { Sort = new Wrappers.Consoles.ConsoleSort { SortColumnIndex = 0 }, BorderColor = ConsoleColor.Black }, /*ConsoleOutputType.HTML will give html output*/
                ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = ConsoleAlignment.LEFT , Color = ConsoleColor.Yellow, Wrap = ConsoleWrapType.WORDCHAR , Text = "Name"},
                        new ConsoleColumnOptions {Width = 30, Alignment = ConsoleAlignment.CENTER , Color = ConsoleColor.Green, Text = "Display Name" },
                        new ConsoleColumnOptions {Width = 20, Alignment = ConsoleAlignment.RIGHT , Color = ConsoleColor.Cyan, Aggregate = ConsoleAggregate.MEDIAN, Text = "Some Numbers" },
                    },

                Headers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { Heading = new ConsoleRecord{ Text = "Information" }, Value = new ConsoleRecord {Text = "Test Naming" }, Alignment = ConsoleAlignment.RIGHT  },
                        new ConsoleHeaderFooterRow { Heading = new ConsoleRecord { Text = "Sorting" }, Value = new ConsoleRecord { Text = "Ascending" }, Alignment = ConsoleAlignment.LEFT },
                        new ConsoleHeaderFooterRow { Heading = new ConsoleRecord { Text = "Wrapping" }, Value = new ConsoleRecord { Text = "Word Wrapping" } , Alignment = ConsoleAlignment.CENTER}
                    },
                Footers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { Heading = new ConsoleRecord { Text = "Signature" }, Value = new ConsoleRecord { Text = "Machine", Color = ConsoleColor.Red } }
                    },

                Rows = new List<ConsoleRow>
                    {                         
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
                                 new ConsoleRecord { Text = "-90.00" },
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
            };
            var _option = new WrapOutputerOptions { Output = new ConsoleOutput
                                                        { Style = ConsoleOutputType.XSLX } };            
            var outputs = WrapOutputerRadar.OutputFactory(_table, _option).Add(_table).Add(_table);
            
            outputs.Draw();
        }
    }
}
