﻿using System;
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
                    OtherOptions = new ConsoleOtherOptions { Sort = new ConsoleSort { SortColumnIndex = 0 }  },
                    ColumnOptions = new List<ConsoleColumnOptions>
                    {
                        new ConsoleColumnOptions {Width = 35, Alignment = WrapAlignment.LEFT , Color = ConsoleColor.DarkYellow, Wrap = WrapConsoleWrapType.WORDWRAP },
                        new ConsoleColumnOptions {Width = 30, Alignment = WrapAlignment.CENTER , Color = ConsoleColor.DarkGreen },
                        new ConsoleColumnOptions {Width = 20, Alignment = WrapAlignment.RIGHT , Color = ConsoleColor.Cyan, Aggregate = WrapAggregate.MEDIAN },
                    },

                    Headers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { StaticText = "Information", StaticValue = "Test Naming" },
                        new ConsoleHeaderFooterRow { StaticText = "Sorting", StaticValue = "Ascending" },
                        new ConsoleHeaderFooterRow { StaticText = "Wrapping", StaticValue = "Word Wrapping" }
                    },
                    Footers = new List<ConsoleHeaderFooterRow>
                    {
                        new ConsoleHeaderFooterRow { StaticText = "Signature", StaticValue = "Machine" }                        
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
                                 new ConsoleRecord { Text = "Surpal, Singh Raja      Rani Rajputana Hillay La bunglow."},
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
                                 new ConsoleRecord { Text = "Nandu Halwai Panighat Ladayi"},
                                 new ConsoleRecord { Text = "Current Century" },
                                 new ConsoleRecord { Text = "23.00" },
                             }
                         }
                }   });
        
        }
    }
}
