using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
    }
}
