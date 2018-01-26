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
   
