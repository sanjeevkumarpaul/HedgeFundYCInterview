using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIUtilCallDemo
{
    public class ParallelismTry
    {
        private List<WebModal> models = new List<WebModal>();

        public async Task DownloadWebSites()
        {
            Progress<DownloadProgress> progress = new Progress<DownloadProgress>();
            progress.ProgressChanged += (source, target) => 
            {
                Console.WriteLine($"{target.Percentage}% completed, Total Bytes: {target.TotalLength}" );
            };

            await DownloadParallelAsync(progress);
        }


        private async Task<List<WebModal>> DownloadParallelAsync(IProgress<DownloadProgress> progress)
        {
            //List<WebModal> models = new List<WebModal>();

            await Task.Run(() => 
            {
                Parallel.ForEach<string>(Websites, (site) =>
                {
                    var model = DownloadWebsiteContent(site);
                    models.Add(model);

                    var _prg = new DownloadProgress { Percentage = (int)((models.Count * 100) / Websites.Count), TotalLength = model.Content.Length };
                    progress.Report(_prg);
                });
            });

            return models;
        }

        private List<string> Websites = new List<string>
        {
            "https://www.yahoo.com/",
            "https://www.apple.com/",
            "https://www.microsoft.com/en-us/",
            "https://www.facebook.com/",
            "https://www.google.com/"
        };

        private WebModal DownloadWebsiteContent(string url)
        {
            WebModal modal = new WebModal { url = url };

            WebClient client = new WebClient();
            modal.Content = client.DownloadString(url);
            
            return modal;
        }
        
    }

    public class DownloadProgress
    {
        public int Percentage { get; set; }
        public long TotalLength { get; set; }
    }

    public class WebModal
    {
        public string url;
        public string Content;        
    }
}
