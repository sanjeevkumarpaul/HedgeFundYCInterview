# HedgeFundYCInterview
Just Practice.

<asp:GridView ID="GridView1" HeaderStyle-BackColor="#666666" HeaderStyle-ForeColor="White"
    RowStyle-BackColor="#E4E4E4" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="Id" HeaderText="Customer Id" ItemStyle-Width="100px" />
        <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="120px" />
        <asp:BoundField DataField="Country" HeaderText="Country" ItemStyle-Width="120px" />
    </Columns>
</asp:GridView>
<br />
<asp:Button ID="btnExport" runat="server" Text="Export To PDF" OnClick="ExportToPDF" />

using System.IO;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

protected void Page_Load(object sender, EventArgs e)
{
    if (!this.IsPostBack)
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id"), new DataColumn("Name"), new DataColumn("Country") });
        dt.Rows.Add(1, "John Hammond", "United States");
        dt.Rows.Add(2, "Mudassar Khan", "India");
        dt.Rows.Add(3, "Suzanne Mathews", "France");
        dt.Rows.Add(4, "Robert Schidner", "Russia");
        GridView1.DataSource = dt;
        GridView1.DataBind();
    }
}

protected void ExportToPDF(object sender, EventArgs e)
{
    using (StringWriter sw = new StringWriter())
    {
        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
        {
            GridView1.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }
    }
}
 
public override void VerifyRenderingInServerForm(Control control)
{
    /* Verifies that the control is rendered */
}





    class StartPDFGeneration
    {
              
        public static void ViaiTextSharp(string path)
        {

            //Help: http://csharp.net-informations.com/file/txttopdf.htm

            var filepath = string.Format(@"{0}\iTextSharpExample1.pdf", path);

            TextDoc pdf = new TextDoc(PageSize.A4, 25, 10, 25, 10);
            //PdfWriter pdfWriter = PdfWriter.GetInstance(pdf, Response.OutputStream); //For ASP.NET
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdf, new FileStream(filepath, FileMode.CreateNew)); //For ASP.NET use Response.OutputStream
            pdf.Open();
            Paragraph Text = new Paragraph("This is test file");
            pdf.Add(Text);
            pdfWriter.CloseStream = false;           
            pdf.Close();

            //Below is for ASP.NET
            //Response.Buffer = true;
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=Example.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Write(pdfDoc);
            //Response.End();
        }		    
    }    
   
   
   Bread Crumb
   =============================================================================================
   using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadCrumbs
{
    public class BreadCrumbItem
    {
        /// <summary>
        /// URL of the Item
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Index number of the item clicked by user.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Description to be displayed on top
        /// </summary>
        public string Crumb { get; set; }
        /// <summary>
        /// to be set true or false as per user action.
        /// </summary>
        public bool Disabled { get; set; }
    }
    
    
    public class BreadCrumbList
    {
        public const string Session = "_Breadcrumb_";
        public static BreadCrumbList Get(string name, string currentUrl)
        {
            if (SessionManager.Get<BreadCrumbList>(BreadCrumbList.Session) == null) SessionManager.Set<BreadCrumbList>(BreadCrumbList.Session, new BreadCrumbList { Name = name });
            var bread = SessionManager.Get<BreadCrumbList>(BreadCrumbList.Session); //should not be null at all.

            //return if same url is called via @Html.RenderAction() or @Html.Action().
            //We need to perform this before .Reset().
            if (bread.Count > 0 && bread.Items[bread.Count - 1].Url.Equals(currentUrl, StringComparison.CurrentCultureIgnoreCase)) return null;

            return bread;
        }
        
        public static string GetHtml()
        {
            var breads = SessionManager.Get<BreadCrumbList>(BreadCrumbList.Session);
            string breadCrumb = "";

            if (breads != null && breads.Items.Count > 0)
            {
                foreach (var bread in breads.Items.Take(breads.Items.Count - 1)) { breadCrumb += string.Format("<a href='{0}'>{1}</a> >> ", bread.Url, bread.Crumb); };
              
                breadCrumb += string.Format("<strong>{0}</strong>", breads.Items.LastOrDefault().Crumb);
            }

            return breadCrumb;
        }


        public BreadCrumbList() { Reset(); }
        
        /// <summary>
        /// Any given name - (Commonly we can use GUID for the same)
        /// </summary>
        public string Name { get; set; } //any random name for a start.
        /// <summary>
        /// Each click of user, when browser moves pagse, for every page a single item will be created.
        /// </summary>
        public IList<BreadCrumbItem> Items { get; set; }

        internal int Count { get { return Items.Count;  }}

        public void Reset()
        {
            Items = new List<BreadCrumbItem>();           
        }
        
        public void Add(string url, string Description)
        {
            var foundBread = Items.FirstOrDefault(i => i.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase));
            if (foundBread != null) //If found to be there, remove all other beyound that.            
                Items = Items.TakeWhile(i => i.Index <= foundBread.Index).ToList(); 
            else
                Items.Add(new BreadCrumbItem { Url = url, Index = Count + 1, Crumb = SeparateWords(Description) });

            SessionManager.Set<BreadCrumbList>(BreadCrumbList.Session, this);
        }


        /// <summary>
        /// Private method to format description.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string SeparateWords(string str)
        {
            var final = "";
            bool firstCharacterCheckIsDone = false;
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                {
                    if (str.IndexOf(c) == 0 && !firstCharacterCheckIsDone)
                    {
                        final += " " + c.ToString();
                        firstCharacterCheckIsDone = true;
                    }
                    else
                        final += " " + c.ToString().ToUpper();
                }
                else
                {
                    if (str.IndexOf(c) == 0 && !firstCharacterCheckIsDone)
                        final += c.ToString().ToUpper();
                    else
                        final += c.ToString();
                }
            }
            return final;
        }
    }
    
    ///IMPLEMETATION
    public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            CaptureBreadCrumbs(requestContext);

            return base.CreateController(requestContext, controllerName);
        }

        private void CaptureBreadCrumbs(RequestContext context)
        {
            string breadName = "Dashboard";
            string vasRedirectionPage = "EEORedirect.aspx";
            var req = HttpContext.Current.Request;
            var url = req.Url.OriginalString;
            var refUrl = req.UrlReferrer != null ? req.UrlReferrer.OriginalString : "";

            var action = context.RouteData.Values["action"].ToString();
            var controller = context.RouteData.Values["controller"].ToString();

            if ( ( req.HttpMethod != "GET" && !refUrl.Contains(vasRedirectionPage) ) || new System.Web.HttpRequestWrapper(req).IsAjaxRequest() ) return;

            if (action.Equals("index", StringComparison.CurrentCultureIgnoreCase)) action = controller;
            var id = context.RouteData.Values.ContainsKey("id") ? context.RouteData.Values["id"].ToString() : "0";
            try { if (Convert.ToInt32(id) < 0) action = id; } catch { action = id.Replace("_", " "); };

            BreadCrumbList bread = null;
            if ((bread = BreadCrumbList.Get(breadName, url)) != null)
            {
                if (refUrl == "" || refUrl.Contains(vasRedirectionPage) ) bread.Reset();

                bread.Add(url, action);
            }            
        }   
        
        
        IN LAYOUT VIEW 
         <span>@Html.Raw( BreadCrumbList.GetHtml() )</span>
    
}

