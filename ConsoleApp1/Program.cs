using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                ////////////////////
                //Парсинг страницы//
                ////////////////////
                //string url = "http://www.cic.gc.ca/english/express-entry/past-rounds.asp";
                Uri url = new Uri("http://www.cic.gc.ca/english/express-entry/past-rounds.asp");
                WebClient wc = new WebClient();
                var str = wc.DownloadString(url); //DownloadString(url);
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(str);
                
                var table = doc.DocumentNode;
                ////////////////////////////////////////////
                /////////Парсинг полученных данных//////////
                ////////////////////////////////////////////
                var trNodes = table.SelectNodes("//details");
                
                FileInfo fi = new FileInfo(@"C:\canada.csv");
                if (fi.Exists) fi.Delete();
                FileStream f = fi.Create();
                f.Close();
                foreach (var node in trNodes)
                {
                    var date_str = node.ChildNodes["summary"].ChildNodes["h3"].InnerText;
                    var prog = node.ChildNodes["p"].InnerText;
                    string j = "";
                    try
                    {
                        var invit = node.ChildNodes["table"].ChildNodes["tbody"].ChildNodes["tr"].ChildNodes["td"].InnerText.ToString();
                        
                        j = invit;
                    }
                    catch
                    {
                        var invit = node.ChildNodes["table"].ChildNodes[3].ChildNodes[1].InnerHtml.ToString();
                        j = invit;

                    }
                    if (j != null)
                    {
                        if(!j.Contains(","))
                        { 
                            j = j.Replace("\r\n","").Trim();
                            j = System.Text.RegularExpressions.Regex.Match(j, @"^\d+.*?").Value;
                        }
                        else
                        {
                            if (j.Contains("<sup"))
                            {
                                j = System.Text.RegularExpressions.Regex.Match(j, @"^.*?(?=\<sup)").Value;
                                string[] ss = j.Split(',');
                                string s1 = ss[0];
                                string s2 = System.Text.RegularExpressions.Regex.Match(ss[1], @"^\d+.*?").Value;
                                j = s1 + s2;
                            }

                            else
                            {
                                string[] ss = j.Split(',');
                                string s1 = ss[0];
                                string s2 = System.Text.RegularExpressions.Regex.Match(ss[1], @"^\d+.*?").Value;
                                j = s1 + s2;
                            }
                        }
                    }

                    string k = "";
                    try
                    {
                        var point = node.ChildNodes["table"].ChildNodes[3].ChildNodes[1].ChildNodes[3].InnerHtml;
                        if (point.Contains("&nbsp"))
                        {
                            point = System.Text.RegularExpressions.Regex.Match(point, @".*?(?=\&nbsp;)").Value;
                        }
                        else point = System.Text.RegularExpressions.Regex.Match(point, @"^\d+").Value;
                        k = point;
                    }
                    catch
                    {
                        var point = node.ChildNodes["table"].ChildNodes[3].ChildNodes[3].InnerHtml;
                        if(point.Contains(","))
                        { 
                            string[] ss = point.Split(',');
                            string s1 = ss[0];
                            string s2 = System.Text.RegularExpressions.Regex.Match(ss[1], @"^\d+.*?").Value;
                            k = s1 + s2;
                        }
                        else k = System.Text.RegularExpressions.Regex.Match(point, @"^\d+.*?").Value;
                    }
                    string l = "";
                    try
                    {
                        var preps = node.ChildNodes["aside"].ChildNodes["dl"].ChildNodes["dd"].ChildNodes["p"].InnerHtml;
                        l = preps;
                    }
                    catch
                    {
                        l = "";
                    }
                        File.AppendAllText(fi.FullName, "\"" + date_str + "\",\"" + prog + "\",\"" + j + "\",\"" + k + "\",\"" + l + "\""+"\n",System.Text.Encoding.UTF8);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n\n" + e.StackTrace);
            }
        }
    }
}
