using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        private Tracker Tracker = new Tracker();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Tracker.Read();
                    RefreshTrack();
                    System.Threading.Thread.Sleep(3 * 1000);
                }
            });
        }

        void RefreshTrack()
        {
            if (this.IsDisposed)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { RefreshTrack(); }));
            }
            else
            {
                label1.Text = Tracker.Ask.ToString();
                label2.Text = Tracker.Bid.ToString();
                label3.Text = Tracker.Diff.ToString();

                var t = DateTime.Now;
                this.Text = t.ToString();
                label4.Text = t.ToLongTimeString();
            }
        }
    }

    class Tracker
    {

        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Diff { get { return this.Bid - this.Ask; } }

        public void Read()
        {
            try
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

                this.Ask = decimal.Parse(askStr);
                this.Bid = decimal.Parse(bidStr);
            }
            catch (Exception)
            {
            }
        }


    }
}
