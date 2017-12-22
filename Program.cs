using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script;

namespace ResDowner
{
    class Program
    {

        static void Main(string[] args)
        {

            //var filePath = System.Configuration.ConfigurationManager.AppSettings["RES:FilePath"];

            //if (filePath.StartsWith("~/"))
            //    filePath = filePath.Replace("~", AppDomain.CurrentDomain.BaseDirectory);

            var res = new Dictionary<string, string>();
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            using (var reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/res.json"))
            {
                res = serializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
            }
            using (var reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/urls.txt"))
            {
                var line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var url = line.Replace("http://localhost:7066/", "/");
                    if (!res.ContainsKey(url))
                        res.Add(url, url);

                    line = reader.ReadLine();
                }


                //res = serializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
            }

            //var jsBase = "https://res.wx.qq.com/mmocbiz/zh_CN/scripts/";
            //var cssBase = "https://res.wx.qq.com/mmocbiz/zh_CN/";
            var jsBase = "https://www.eteams.cn";
            var cssBase = "https://www.eteams.cn";
            var jsLocal = AppDomain.CurrentDomain.BaseDirectory + "/estems/";
            var cssLocal = AppDomain.CurrentDomain.BaseDirectory + "/estems/";

            // 使用多线程处理
            Parallel.ForEach(res, (p) =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var address = p.Key.EndsWith(".css") ? cssBase + p.Value : jsBase + p.Value;
                        var fileName = p.Key.EndsWith(".css") ? cssLocal + p.Key : jsLocal + p.Key;
                        EnsurePathExists(fileName.Substring(0, fileName.LastIndexOf("/")));
                        Console.WriteLine("正在下载{0}", new Uri(address));
                        client.DownloadFile(new Uri(address), fileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("下载出错，{0},{1}\r\n{2}", p.Key, p.Value, ex.Message);
                }
            });

            Console.WriteLine("Ok,资源下载完成");
            Console.Read();
        }

        private static void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
