using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Wrappers.Consoles;
using Extensions;
using Wrappers.Consoles.Enums;
using System.IO;

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
            SaveToDisk();
        }
    }

    partial class WrapHtmlTable
    {
        private class HtmlStyles
        {
            private string _color;
            private string _backgroundColor;
            private string _borderColor;

            internal bool IsElement { get; set; } = false;
            internal string Width { get; set; }
            internal string WidthUnit { get; set; } = "%";
            internal string Name { get; set; }            
            internal string BorderStyle { get; set; }
            internal string BorderWidth { get; set; } = "1";
            internal string Alignment { get; set; }

            internal string Color { get { return _color; } set { _color = ConsoleWebColors.Get(value); } }
            internal string BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = ConsoleWebColors.Get(value); } }
            internal string BorderColor { get { return _borderColor; } set { _borderColor = ConsoleWebColors.Get(value); } }

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
                Name = "table#master",
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
            if (_table.Rows.Any(r => r.IsAggregate))
            {
                _table.ColumnOptions.Where(c => c.Aggregate != ConsoleAggregate.NONE).ToList().ForEach(col => {
                    var _name = $"consoleWrapAggregate{col.GetHashCode()}";
                    CreateGroupStyle(new HtmlStyles
                    {
                        Name = _name,
                        Color = _table.OtherOptions.AggregateColor.ToString(),
                        Alignment = col.Alignment.ToString(),                        
                        BorderColor = _table.OtherOptions.AggregateBorderColor.ToString(),
                        BorderStyle = "solid",
                        ExtraStyles = "padding: 2px;",
                    });

                    AssignCssStyleAggregate(col, _name);
                });
            }


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

        private void AssignCssStyleAggregate(ConsoleColumnOptions opt, string cssClass, string cssStyles = null)
        {
            _table.Rows.Where(r => r.IsAggregate).ToList().ForEach(r => r.Column.ForEach(col => AssignCssStyle(col, cssClass, cssStyles)));
        }
        #endregion ~END OF Creating CSS Style Classes

        #region ^Creating HTML Tags
        private void CreateTags()
        {
            string _body = "<body><table id='master'>{tab}</table></body>";
            StringBuilder _tab = new StringBuilder()
                                  .Append(CreateHeaderFooterRow())
                                  .Append(CreateColumnHeaders())
                                  .Append(CreateColumnData())
                                  .Append(CreateHeaderFooterRow(false));

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
            var rows = (header ? _table.Headers : _table.Footers);
            var _tag = new StringBuilder();
            if (!rows.Null() && rows.Any())
            {
                var _outerstyles = $"border-style:solid;border-width:1px;border-color:{_table.OtherOptions.HeadingColor}";
                _tag.Append($"<tr><td colspan='{_table.ColumnOptions.Count}' style='{_outerstyles}' >");
                rows.GroupBy(r => r.Alignment).ToList().ForEach(g =>
                {                   
                    _tag.Append($"<table id='{(header ? "headers" : "footers")}' style='width:100%;text-align:{g.Key.ToString()}'>");
                    foreach (var align in g)
                    {                        
                        _tag.Append("<tr>");
                        CreateColumn(g.Key);

                        CreateRow(align.Heading);
                        _tag.Append("<td>:</td>");
                        CreateRow(align.Value);
                        
                        CreateColumn(g.Key, true);
                        _tag.Append("</tr>");                        
                    }
                    _tag.Append("</table>");                    
                });
                _tag.Append("</td></tr>");
            }

            void CreateColumn(ConsoleAlignment align, bool bottom = false)
            {
               switch(align)
                {
                    case ConsoleAlignment.CENTER: _tag.Append($"<td style='width:50%'></td>"); break;
                    case ConsoleAlignment.LEFT: if (bottom) _tag.Append($"<td style='width:100%'></td>"); break;
                    case ConsoleAlignment.RIGHT: if (!bottom) _tag.Append($"<td style='width:100%'></td>"); break;
                }
            }

            void CreateRow(ConsoleRecord rec)
            {
                _tag.Append($"<td class='{rec.HTMLCssClass}' style='{rec.HTMLInlineStyles.ToEmpty()}'>{GetText(rec, true)}</td>");
            }

            return _tag.ToString();
        }

        private string GetText<T>(T record, bool htmlspaces = false) where T : _ConsoleItemBase
        {
            var _text = "";
            if (record.Text.Empty() && record.GetType() == typeof(ConsoleRecord))
            {
                (record as ConsoleRecord).MText.ForEach(t => _text += $"<xmp>{t}</xmp>");
            }
            else _text = $"<xmp>{record.Text}</xmp>";
            
            return _text;
        }

        #endregion ^END of Creating HTML Tags

        #region ^Save the HTML output to a file
        private void SaveToDisk()
        {
            try
            {
                _html = _html.Insert(0, _css).Insert(0, Environment.NewLine).Insert(0, "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");

                var _path = WrapIOs.CreateAndCheckPath(_table.OtherOptions.Output.Path, "html");
                if (!_path.Empty()) WrapIOs.AppendRecords(new string[] { _html.ToString() }, _path);
                                
            }catch(Exception e)
            {
                throw e;
            }
        }
        #endregion ~END OF Save the HTML output to a file
    }
}
