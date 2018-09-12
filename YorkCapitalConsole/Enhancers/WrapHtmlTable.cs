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
        private StringBuilder _html;

        public WrapHtmlTable(ConsoleTable table)
        {
            _table = table;
            _css = new StringBuilder();
            _html = new StringBuilder();
        }

        public void Draw()
        {
            CreateStyles();
            CreateTags();

            _html.Insert(0, _css);
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
            if (_table.OtherOptions.IsFirstRowAsHeader)
            {
                var _name = "consoleWrapColumHeaders";
                CreateGroupStyle(new HtmlStyles
                {
                    Name = _name,
                    Color = _table.OtherOptions.HeadingColor.ToString(),
                    BorderColor = _table.OtherOptions.BorderColor.ToString(),
                    BorderStyle = "solid",
                    ExtraStyles = "padding: 2px;"
                });

                AssignCssStyle(0, _name, colHeader: true);
            }
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

                    AssignCssStyle(_colIndex++, _name, colHeader: false);
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
                    Color = (title ? hf.Heading.Color : hf.Value.Color) .ToString(),
                    BorderColor = _table.OtherOptions.BorderColor.ToString(),
                    Alignment = hf.Alignment.ToString(),
                    ExtraStyles = "padding: 2px;"
                });

                AssignCssStyle(title ? hf.Heading : hf.Value, _name);
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
        
        private void AssignCssStyle(int colIndex, string cssClass, string cssStyles = null, bool colHeader = false)
        {
           var _count = _table.Rows.Count;
           _table.Rows.Take(colHeader ? 1: _count)
                      .Skip(_table.OtherOptions.IsFirstRowAsHeader && !colHeader ? 1 : 0)
                      .Where(r => !r.IsAggregate)
                      .ToList()
                      .ForEach(r =>
                      {
                          if (colHeader)
                              r.Column.ForEach(col => AssignCssStyle(col, cssClass, cssStyles));
                          else
                              AssignCssStyle(r.Column.ElementAt(colIndex), cssClass, cssStyles);
                      });            
        }

        private void AssignCssStyle<T>(T record, string cssClass, string cssStyles = null) where T : _ConsoleItemBase
        {
            record.HTMLCssClass = cssClass;
            record.HTMLInlineStyles = cssStyles;
        }
        #endregion ~END OF Creating CSS Style Classes

        #region ^Creating HTML Tags
        private void CreateTags()
        {
            string _body = "<body><table>{tab}</table></body>";
            StringBuilder _tab = new StringBuilder()
                                  .Append(CreateColumnHeaders())
                                  .Append(CreateColumnData())
                                  .Append(CreateHeaderFooterRow());

            _html.Append( _body.Replace("{tab}", _tab.ToString()));
        }

        private string CreateColumnHeaders()
        {
            var _tag = new StringBuilder();

            if (_table.OtherOptions.IsFirstRowAsHeader)            
                 _tag.Append(CreateRecordRow(_table.Rows.ElementAt(0), true));
            
            return _tag.ToString();
        }

        private string CreateColumnData()
        {
            var _tag = new StringBuilder();

            _table.Rows.Skip(_table.OtherOptions.IsFirstRowAsHeader ? 1 : 0).ToList().ForEach(row => _tag.Append(CreateRecordRow(row)));
            
            return _tag.ToString();
        }

        private string CreateRecordRow(ConsoleRow rows, bool tableHeader = false)
        {
            var _tag = new StringBuilder();
            var _tl = tableHeader ? "th" : "td";
            _tag.Append("<tr>");
            rows.Column.ForEach(col =>
            {
                _tag.Append($"<{_tl} class=\"{col.HTMLCssClass}\" style=\"{col.HTMLInlineStyles.ToEmpty()}\">{GetText(col)}</{_tl}>");
            });
            _tag.Append("</tr>");
            return _tag.ToString();
        }

        private string CreateHeaderFooterRow(bool header = true)
        {
            var rows = header ? _table.Headers : _table.Footers;
            var _tag = new StringBuilder();
            if (!rows.Null() && rows.Any())
            {
                _tag.Append($"<tr colspan=\"{_table.ColumnOptions.Count}\">");
                //rows.ForEach(r => _tag.Append($"<td class=\"{r.HTMLCssClass}\" style=\"{r.HTMLInlineStyles.ToEmpty()}\" >{GetText(r)}</td>") );
                _tag.Append("</tr>");
            }
            return _tag.ToString();
        }

        private string GetText<T>(T record) where T : _ConsoleItemBase
        {
            var _break = "<br/>";
            var _text = record.Text;
            if (_text.Empty() && record.GetType() == typeof(ConsoleRecord) )
            {
                _text = (record as ConsoleRecord).MText.JoinExt(_break).TrimEx(_break);
            }

            return _text;
        }

        #endregion ^END of Creating HTML Tags
    }
}
