using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace bbs_reader
{
    public partial class Form3 : Form
    {
        static DataTable dt;
        static string sql = @"select f.module_code  as 所属模块,
                                   f.func_code    as 功能编码,
                                   f.func_name    as 功能名称,
                                   f.func_comment as 备注
                              from sy_func f, sy_form_def m, sy_table_def t
                             where f.func_info = m.form_code
                               and m.table_code_action = t.table_code
                               and t.table_type = 1
                             order by f.module_code, f.func_code";
        static string SelectedPath = "";
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            genDataToView();
            if (!this.tableCodeQry.Text.Equals(""))
            {
                var q1 = from dt1 in dt.AsEnumerable()//查询
                         where dt1.Field<string>("功能编码").Contains(this.tableCodeQry.Text.ToUpper())//条件
                         select dt1;
                dataGridView1.DataSource = q1.AsDataView();
            }
        }
        // 填充datagridview
        private void genDataToView()
        {
            dt = DbUtil.query(sql);
            dataGridView1.DataSource = dt;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                this.dataGridView1.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (dt == null)
            {
                genDataToView();
            }
            var q1 = from dt1 in dt.AsEnumerable()//查询
                     where dt1.Field<string>("功能编码").Contains(this.tableCodeQry.Text.ToUpper())//条件
                     select dt1;
            dataGridView1.DataSource = q1.AsDataView();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("数据收集工具 v1.0", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void 导出数据收集模版ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = "D:";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.Cancel || this.folderBrowserDialog1.SelectedPath.Equals(""))
            {
                return;
            }
            foreach (DataGridViewRow dv in dataGridView1.SelectedRows)
            {
                string funcCode = dv.Cells["功能编码"].Value.ToString();
                string sql1 = @"select t.fitem_code,
                                   t.fitem_name,
                                   t.fitem_data_type,
                                   t.fitem_length,
                                   t.fitem_input_type,
                                   t.fitem_card_default,
                                   t.dic_code,
                                   t.fitem_comment
                              from sy_form_item t,
                                   (select f.form_code
                                      from sy_form_def f, sy_func c
                                     where f.form_code = c.func_info
                                       and c.func_code = '" + funcCode + @"'
                                       and rownum = 1) fm
                             where t.fitem_flag = 1
                               and t.form_code = fm.form_code
                               and t.fitem_type = 'TAB'
                             order by t.fitem_card_order asc";
                exportTemplates(funcCode, DbUtil.query(sql1), dataGridView1.SelectedRows.Count);
            }
            this.tProgressBar1.Value = 0;
            this.toolStripStatusLabel1.Text = "全部导出完成";
        }
        private void exportTemplates(string funcCode, DataTable dt, int select_rows_count)
        {
            // FITEM_CODE,FITEM_NAME,FITEM_DATA_TYPE,FITEM_LENGTH,FITEM_INPUT_TYPE,FITEM_CARD_DEFAULT,DIC_CODE,FITEM_COMMENT
            HSSFWorkbook wk = new HSSFWorkbook();
            ISheet tb = wk.CreateSheet("Sheet");
            IRow row0 = tb.CreateRow(0);
            IRow row1 = tb.CreateRow(1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                tb.SetColumnWidth(i, (int)((12 + 0.72) * 256));// 宽为20
                ICell cell0 = row0.CreateCell(i);
                cell0.SetCellValue(row["FITEM_CODE"].ToString());
                ICell cell1 = row1.CreateCell(i);
                cell1.SetCellValue(row["FITEM_NAME"].ToString());
            }
            using (FileStream fs = File.OpenWrite(this.folderBrowserDialog1.SelectedPath + "/" + funcCode + ".xls"))
            {
                wk.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
                this.tProgressBar1.Value += 100 / select_rows_count;
                this.toolStripStatusLabel1.Text = funcCode + "导出完成";
            }
        }


        private void 导出ERPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("将用ERP模版样式导出设备台帐的数据【带参数】");

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            //saveFileDialog.FilterIndex = 0;
            //saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.CreatePrompt = true;
            //saveFileDialog.Title = "**数据";
            //saveFileDialog.ShowDialog();
            //if (saveFileDialog.FileName == "")
            //    return;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(this.dataGridView1.CurrentRow.Cells["功能编码"].Value.ToString());
        }

        
    }
}
