using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Wrappers.Consoles;
using Extensions;


namespace Wrappers
{
    public partial class WrapHtmlTable
    {
        private ConsoleTable _table;
        private StringBuilder _css;

        public WrapHtmlTable(ConsoleTable table)
        {
            _table = table;
            _css = new StringBuilder();
        }

        public void Draw()
        {
            CreateStyles();

        }
    }

    partial class WrapHtmlTable
    {
        private class HtmlStyles
        {
            internal bool IsElement { get; set; } = false;
            internal string Width { get; set; }
            internal string WidthUnit { get; set; } = "%";
            internal string Name { get; set; }
            internal string Color { get; set; }
            internal string BackgroundColor { get; set; }
            internal string BorderColor { get; set; }
            internal string BorderStyle { get; set; }
            internal string BorderWidth { get; set; } = "1";
            internal string Alignment { get; set; }
            internal string ExtraStyles { get; set; } = "";
        };

        #region ^Creating CSS Style Classes
        private void CreateStyles()
        {
            _css.Append( $"<style>" );            
            CreateGroupStyle(new HtmlStyles
            {
                IsElement = true,
                Name ="body",
                Color = Console.ForegroundColor.ToString(),
                BackgroundColor = Console.BackgroundColor.ToString()
            });
            CreateGroupStyle(new HtmlStyles
            {
                IsElement = true,
                Name = "table",
                Width = "100",
                ExtraStyles = "border-collapse: collapse; "
            });
            CreateGroupStyle(new HtmlStyles
            {
                Name = "consoleWrapColumHeaders",
                Color = _table.OtherOptions.HeadingColor.ToString(),
                BorderColor = _table.OtherOptions.BorderColor.ToString(),
                BorderStyle = "solid",
                ExtraStyles = "padding: 2px;"
            });
            var totalWidth = _table.ColumnOptions.Sum(c => c.Width);
            int _colIndex = 0;
            _table.ColumnOptions.ForEach(col => {
                    var _name = $"consoleWrapColumHeader{col.GetHashCode()}";
                    CreateGroupStyle(new HtmlStyles
                    {
                        Name = _name,
                        Color = col.Color.ToString(),
                        Alignment = col.Alignment.ToString(),
                        Width = ( (col.Width * 100)/totalWidth ).ToString(),
                        BorderColor = _table.OtherOptions.BorderColor.ToString(),
                        BorderStyle = "solid",
                        ExtraStyles = "padding: 2px;",                        
                    });

                    AssignCssStyle(_colIndex++, _name);
                });
            if (!_table.Headers.Null())
                _table.Headers.ForEach(hd => CreateHeaderFooterStyle(hd));
            if (!_table.Footers.Null())
                _table.Footers.ForEach(ft => CreateHeaderFooterStyle(ft, false));
            
            _css.Append($"</style>");
        }

        private void CreateHeaderFooterStyle(ConsoleHeaderFooterRow hf, bool header = true)
        {
            var _text = header ? "Header" : "Footer";
            Create(); Create(false);           

            void Create(bool title = true)
            {
                var _title = title ? "Title" : "Value";
                var _name = $"consoleWrap{_text}{_title}{hf.GetHashCode()}";
                CreateGroupStyle(new HtmlStyles
                {
                    Name = _name,
                    Color = (title ? hf.HeadingColor : hf.ValueColor) .ToString(),
                    BorderColor = _table.OtherOptions.BorderColor.ToString(),
                    Alignment = hf.Alignment.ToString(),
                    ExtraStyles = "padding: 2px;"
                });

                AssignCssStyle(hf, _name);
            }
        }

        private void CreateGroupStyle(HtmlStyles styles)
        {
            _css.Append($"{ (styles.IsElement ? "": ".") }{styles.Name} {{ ");
            if (!styles.Color.Empty()) _css.Append($"color: {styles.Color}; ");
            if (!styles.BackgroundColor.Empty()) _css.Append($"background-color: {styles.BackgroundColor}; ");
            if (!styles.BorderColor.Empty()) _css.Append($"border-color: {styles.BorderColor}; ");
            if (!styles.BorderStyle.Empty()) _css.Append($"border-style: {styles.BorderStyle}; ");
            if (!styles.Alignment.Empty()) _css.Append($"text-align: {styles.Alignment.ToLower()}; ");
            _css.Append($"border-width: {styles.BorderWidth}px; ");
            if (!styles.Width.Empty()) _css.Append($"Width: {styles.Width}{styles.WidthUnit}; ");
            _css.Append(styles.ExtraStyles);
            _css.Append(" } ");            
        }

        private void AssignCssStyle(int colIndex, string cssClass, string cssStyles = null)
        {
            if (!(colIndex == 0 && _table.OtherOptions.IsFirstRowAsHeader) || colIndex > 0)
            {
                _table.Rows.Where(r => !r.IsAggregate).ToList().ForEach(r =>
                {
                    var col = r.Column.ElementAt(colIndex);
                    AssignCssStyle(col, cssClass, cssStyles);
                });
            }
        }

        private void AssignCssStyle<T>(T record, string cssClass, string cssStyles = null) where T : _ConsoleItemBase
        {
            record.HTMLCssClass = cssClass;
            record.HTMLInlineStyles = cssStyles;
        }
        #endregion ~END OF Creating CSS Style Classes
    }
}
