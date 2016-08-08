using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace WeebIRCWebEdition
{
    class HtmlParser
    {

        public string html { get; set; }

        /// <HtmlParser>
        /// <summary>
        /// Constructor to setup the dictionarys and lists
        /// </summary
        /// </HtmlParser>
        public HtmlParser()
        {
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
        public string[,,] CutHtml(string uniqueString)
        {
            string[] htmlSplitted = html.Split(new string[] { uniqueString }, StringSplitOptions.None);

            string[,,] tagsAndTheirInsidesPerPart = new string[htmlSplitted.Length, 200,2];
            for (int a = 1; a < htmlSplitted.Length - 1; a++)
            {
                try
                {
                    string[] startTags = htmlSplitted[a].Split('<');
                    for (int i = 0; i < startTags.Length - 1; i++)
                    {
                        try
                        {

                            string tagName = startTags[i].Split('>')[0];
                            string dataInTag = startTags[i].Split('>')[1].Split(new string[] { "</" }, StringSplitOptions.None)[0];
                            tagsAndTheirInsidesPerPart[a, i, 0] = tagName;
                            tagsAndTheirInsidesPerPart[a, i, 1] = dataInTag;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                } catch (Exception e)
                {

                }                
            }
            return tagsAndTheirInsidesPerPart;

        }
    }
}
