using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Lab_2
{
    
    public partial class Form1 : Form
    {
        Panel[] panels = new Panel[100];
        public string[] tab1;   
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Bazadanych siema = new Bazadanych(@"DAMIAN\SQLS", @"Northwind");
            string[] t = new string[1];
            t = siema.GetTableNames();
            foreach (var item in t)
            {
                dataGridView1.Rows.Add(item.ToString());
            }
            dataGridView2.DataSource = siema.GetTableData(t[1]);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Bazadanych siema = new Bazadanych(@"DAMIAN\SQLS", @"Northwind");
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            string temp = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            dataGridView2.DataSource =siema.GetTableData(temp);
            label2.Text = temp;
        }

        

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label4.Text = Convert.ToString(dataGridView2.Rows[e.RowIndex].Index + 1);
        }

       

        private void button4_Click(object sender, EventArgs e)
        {
            Bazadanych siema = new Bazadanych(@"DAMIAN\SQLS", @"Northwind");
            DataTable dv = (DataTable)(dataGridView2.DataSource);
           // siema.SaveTableToServer(dv, label2.Text);
           
        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Bazadanych siema = new Bazadanych(@"DAMIAN\SQLS", @"Northwind");
            //siema.SaveRowToBase((DataTable)(dataGridView2.DataSource), label2.Text, e.RowIndex);
        }
    }

    class Bazadanych
    {
        SqlConnection conn;
        public Bazadanych(string ServerName, string DbName)
        {
            conn = new SqlConnection("Data Source=" + ServerName + ";" + "Initial Catalog=" + DbName + ";" + @"Trusted_Connection=true");
            conn.Open();
        }
        public string[] GetTableNames()
        {
            DataTable Tables = conn.GetSchema("Tables");
            List<string> TablesNames = new List<string>();
            foreach (DataRow row in Tables.Rows)
            {
                if (row[3].ToString() == "BASE TABLE") TablesNames.Add(row[2].ToString());
            }
            string[] TablesS = new string[TablesNames.Count];
            
            int i = 0;
            foreach (var item in TablesNames)
            {
                TablesS[i] = item.ToString();
                i++;
            }
            return TablesS;
        }
        public DataTable GetTableData(string TableName)
        {
            DataTable t = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + TableName, conn))
            {
                da.Fill(t);
            };
            return t;
        }
        public Boolean SaveRowToBase(DataTable Table, String TableName, int RowIndex)
        {
            using (SqlCommand comm = new SqlCommand())
            {
            string cmdString = "INSERT INTO " + TableName  + " (";
            
            for (int i = 0; i < Table.Columns.Count; i++)
			{
			    cmdString += Table.Columns[i].ColumnName + ", ";
			}
            cmdString += ") VALUES (";
            for (int i = 0; i < Table.Columns.Count; i++)
			{
                if (i +1 == Table.Columns.Count) cmdString += Table.Columns[i].ToString() + " ";
               
                    //cmdString += Table.Row[RowIndex].Cell[i].Value.ToString() + ", ";
			}
            cmdString += ");";
            //MessageBox.Show(cmdString);
            
            }
            return true;
        }
    }
}
