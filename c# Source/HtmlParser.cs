using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace WeebIRCWebEdition
{
    class HtmlParser
    {

        public List<Dictionary<string, List<string>>> seperateDictionary { get; set; }
        public Dictionary<string, List<string>> tagsAndTheInside{ get; set;}
        public List<string> AllTags { get; set; }
        public string html { get; set; }

        /// <HtmlParser>
        /// <summary>
        /// Constructor to setup the dictionarys and lists
        /// </summary
        /// </HtmlParser>
        public HtmlParser()
        {
            seperateDictionary = new List<Dictionary<string, List<string>>>();
            tagsAndTheInside = new Dictionary<string, List<string>>();
            AllTags = new List<string>();
        }

        /// <ParseUrl>
        /// <summary>
        /// Gets html content from url
        /// </summary
        /// </ParseUrl>
        public string ParseUrl(string url)
        {
            using (WebClient webclient = new WebClient())
            {
                html = webclient.DownloadString(url);
                return html;
            }
        }

        /// <CutHtml>
        /// <summary>
        /// Cuts the html into an array, using a unique string, then parses all tags and text inside those tags into a dictionary, which is publicly available
        /// </summary
        /// </CutHtml>
        public void CutHtml(string uniqueString)
        {
            string[] htmlSplitted = html.Split(new string[] { uniqueString }, StringSplitOptions.None);

            Console.WriteLine("htmltoparseparts: " + htmlSplitted.Length);
            for (int b = 1; b < htmlSplitted.Length-1; b++)
            {
                string htmlToParse = htmlSplitted[b];
                htmlToParse = Regex.Replace(htmlToParse, @"\r\n?|\n", "");
                htmlToParse = Regex.Replace(htmlToParse, @"\s+", " ");
                Regex regex = new Regex(@"(?<=<)(.*?)(?=>)");
                Match match = regex.Match(htmlToParse);

                Dictionary<string, List<string>> tagsAndTheInsideinPart = new Dictionary<string, List<string>>();
                List<string> tagsInParts = new List<string>();

                if (match.Success)
                {
                    int i = 0;
                    while (true)
                    {
                        if (i == 0)
                        {

                            if (match.Value == "")
                            {
                                break;
                            }
                            if (!tagsInParts.Contains(match.Value) && !match.Value.Substring(0, 1).Contains("/"))
                            {
                                tagsInParts.Add(match.Value);
                            }
                        }
                        else
                        {
                            match = match.NextMatch();
                            if (match.Success)
                            {
                                if (match.Value == "")
                                {
                                    break;
                                }

                                if (!tagsInParts.Contains(match.Value) && !match.Value.Substring(0, 1).Contains("/"))
                                {
                                    tagsInParts.Add(match.Value);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        i++;
                    }
                    for (int d = 0; d < tagsInParts.Count; d++)
                    {
                        string tag = tagsInParts[d];
                        string endTag = tag.Split(' ')[0];
                        Regex regex2 = new Regex(@"(?<=" + tag + ">)(.*?)(?=</" + endTag + ")");
                        Match match2 = regex2.Match(htmlToParse);
                        List<string> insideTags = new List<string>();
                        if (match2.Success)
                        {
                            int a = 0;
                            while (true)
                            {
                                if (a == 0)
                                {

                                    string text = match2.Value.Replace(tag, "").Replace("</", "");
                                    insideTags.Add(text);
                                }
                                else
                                {
                                    match2 = match2.NextMatch();
                                    if (match2.Success)
                                    {

                                        string text = match2.Value.Replace(tag, "").Replace("</", "");
                                        insideTags.Add(text);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                a++;
                            }
                            tagsAndTheInsideinPart.Add(tag, insideTags);
                        }
                        else
                        {
                            tagsAndTheInsideinPart.Add(tag, new List<string>() { "NOTHING INSIDE" });
                        }

                        seperateDictionary.Add(tagsAndTheInsideinPart);
                    }
                }
            }
        }
    }
}
