using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bbs_reader
{
    public partial class Form2 : Form
    {
        string strconn = "User ID=hdeam_product;Password=d3B68Apk29v34Dj;Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST=192.168.6.10)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=orcl)))";
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Show(button1, 0, this.button1.Height);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 192.168.6.10:1521/orcl
            if (!DbUtil.testConnection(this.db.Text, this.user.Text, this.password.Text))
            {
                MessageBox.Show("连接数据库失败！");
                return;
            }
            Form3 f3 = new Form3();
            f3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
