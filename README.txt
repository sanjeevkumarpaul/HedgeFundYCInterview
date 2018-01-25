# HedgeFundYCInterview
Just Practice.



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
		      

        public static void Compress(string filepath)
        {
            
        }
    }    
   