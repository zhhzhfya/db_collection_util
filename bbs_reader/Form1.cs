
using DotNet.Utilities;
using NSoup.Nodes;
using NSoup.Select;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace bbs_reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Document doc = NSoup.NSoupClient.Connect("http://example.com")
                                  .Data("query", "Java")
                                  .UserAgent("Mozilla")
                                  .Cookie("auth", "token")
                                  .Timeout(3000)
                                  .Post();
            Elements body = doc.Select("body");
            this.textBox3.Text = body.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine("开始");
            for (int i = 0; i < 1000; i++)
            {
                ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback((object s) =>
                {
                    HttpItem itemCheck = new HttpItem()
                    {
                        URL = "http://m.baidu.com/",
                        Timeout = 50000,
                    };
                    HttpHelper helperCheck = new HttpHelper();
                    HttpResult resultCheck = helperCheck.GetHtml(itemCheck);
                    string checkOut = resultCheck.Html;
                    if (checkOut.Contains("百度"))
                    {
                        Random r = new Random();

                        Console.WriteLine(r.ToString());
                        return;
                    }
                }), null);        //线程池
            }
        }
    }
}
