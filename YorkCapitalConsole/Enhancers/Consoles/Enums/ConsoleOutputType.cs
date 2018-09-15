using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrappers.Consoles.Enums
{
    public enum ConsoleOutputType
    {
        [Description("")]
        NONE,
        [Description("")]
        CONSOLE,
        [Description("html")]
        HTML,
        [Description("html")]
        HTM,
        [Description("txt")]
        TEXT,
        [Description("txt")]
        TXT,
        [Description("xlsx")]
        EXCEL,
        [Description("xlsx")]
        XLS,
        [Description("xlsx")]
        XSLX,
        [Description("xlsx")]
        XL,
        [Description("csv")]
        CSV,
        [Description("json")]
        JSON,
        [Description("xml")]
        XML
    }
}
