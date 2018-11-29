using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace enparaTrack
{
    class Program
    {
        static void Main(string[] args)
        {
            var tracker = new Tracker();
            tracker.Track();

        }
    }

    class Tracker
    {

        public void Track()
        {
            var client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            var response = client.DownloadString("https://www.qnbfinansbank.enpara.com/doviz-kur-bilgileri/doviz-altin-kurlari.aspx");

            //var xDoc = System.Xml.Linq.XDocument.Parse(response);

            var index1 = response.IndexOf("pnlContent");

            var index2 = response.IndexOf("</span>", index1);
            index2 = response.IndexOf("</span>", index2 + 1);
            index2 = response.IndexOf("</span>", index2 + 1);

            var docPart = response.Substring(index1, index2 - index1);
            docPart = docPart.Replace(Environment.NewLine, "");

            var matches = System.Text.RegularExpressions.Regex.Matches(docPart, @"<span>[0-9\,]+");

            var askStr = matches[0].Value.Replace("<span>", "");
            var bidStr = matches[1].Value.Replace("<span>", "");

            var ask = decimal.Parse(askStr);
            var bid = decimal.Parse(bidStr);
            var diff = bid - ask;


        }


    }
}
