﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Validation;
using System;
using System.Collections.Generic;

namespace Wrappers
{
    public static class WrapOpenXml
    {
        public static Dictionary<uint, String> GetFormatMappings(SpreadsheetDocument document)
        {
            Dictionary<uint, String> formatMappings = new Dictionary<uint, String>();

            var stylePart = document.WorkbookPart.WorkbookStylesPart;

            var numFormatsParentNodes = stylePart.Stylesheet.ChildElements.OfType<NumberingFormats>();

            foreach (var numFormatParentNode in numFormatsParentNodes)
            {
                var formatNodes = numFormatParentNode.ChildElements.OfType<NumberingFormat>();
                foreach (var formatNode in formatNodes)
                {
                    formatMappings.Add(formatNode.NumberFormatId.Value, formatNode.FormatCode);
                }
            }
            
            return formatMappings;
        }
        
        public static string Validate(SpreadsheetDocument document)
        {
            var er = string.Empty;

            try
            {
                var validator = new OpenXmlValidator();
                int count = 0;
                foreach (ValidationErrorInfo error in validator.Validate(document))
                {
                    count++;
                    er += $"Error Count : {count}{Environment.NewLine}";
                    er += $"Description : {error.Description}{Environment.NewLine}";
                    er += $"Path: {error.Path.XPath}{Environment.NewLine}";
                    er += $"Part: {error.Part.Uri}{Environment.NewLine}";
                }                
            }
            catch (Exception ex)
            {
                er += ex.Message;
            }

            return er;
        }
        
        public static void GetFormatId()
        {
            /*
                0 = 'General';
                1 = '0';
                2 = '0.00';
                3 = '#,##0';
                4 = '#,##0.00';
                5 = '$#,##0;\-$#,##0';
                6 = '$#,##0;[Red]\-$#,##0';
                7 = '$#,##0.00;\-$#,##0.00';
                8 = '$#,##0.00;[Red]\-$#,##0.00';
                9 = '0%';
                10 = '0.00%';
                11 = '0.00E+00';
                12 = '# ?/?';
                13 = '# ??/??';
                14 = 'mm-dd-yy';
                15 = 'd-mmm-yy';
                16 = 'd-mmm';
                17 = 'mmm-yy';
                18 = 'h:mm AM/PM';
                19 = 'h:mm:ss AM/PM';
                20 = 'h:mm';
                21 = 'h:mm:ss';
                22 = 'm/d/yy h:mm';
                37 = '#,##0 ;(#,##0)';
                38 = '#,##0 ;[Red](#,##0)';
                39 = '#,##0.00;(#,##0.00)';
                40 = '#,##0.00;[Red](#,##0.00)';
                44 = '_("$"* #,##0.00_);_("$"* \(#,##0.00\);_("$"* "-"??_);_(@_)';
                45 = 'mm:ss';
                46 = '[h]:mm:ss';
                47 = 'mmss.0';
                48 = '##0.0E+0';
                49 = '@';
                27 = '[$-404]e/m/d';
                30 = 'm/d/yy';
                36 = '[$-404]e/m/d';
                50 = '[$-404]e/m/d';
                57 = '[$-404]e/m/d';
                59 = 't0';
                60 = 't0.00';
                61 = 't#,##0';
                62 = 't#,##0.00';
                67 = 't0%';
                68 = 't0.00%';
                69 = 't# ?/?';
                70 = 't# ??/??';
             */
        }
    }
}
