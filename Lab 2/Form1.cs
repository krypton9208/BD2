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
        Bazadanych bt;
        Bazadanych Baza;
        bool StartedAddingRow = false;
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            string temp = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            dataGridView2.DataSource = Baza.GetTableData(temp);
            label2.Text = temp;
        }

        

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label4.Text = Convert.ToString(dataGridView2.Rows[e.RowIndex].Index + 1);
        }

       

        

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!StartedAddingRow)
            Baza.UpdateCell(comboBox1.Text,label2.Text, e.ColumnIndex+1, Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()), dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Połącz")
            {
                label1.Enabled = true;
                label2.Enabled = true;
                label3.Enabled = true;
                label4.Enabled = true;
                label5.Enabled = true;
                label6.Enabled = true;
                dataGridView1.Enabled = true;
                dataGridView2.Enabled = true;
                comboBox1.Enabled = true;
                button1.Text = "Rozłącz";

                bt = new Bazadanych(@textBox1.Text);
                foreach (var item in bt.GetBaseNames())
                {
                    comboBox1.Items.Add(item.ToString());
                }
            }
            else
            {
                label1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                label5.Enabled = false;
                label6.Enabled = false;
                dataGridView1.Enabled = false;
                dataGridView2.Enabled = false;
                comboBox1.Enabled = false;
                button1.Text = "Połącz";
                comboBox1.Text = "";
                comboBox1.Items.Clear();
                Baza.Zamknij();
            }
           
            
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bt.Zamknij();
            Baza = new Bazadanych(textBox1.Text, comboBox1.Text);
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            string[] t = new string[1];
            t = Baza.GetTableNames();
            foreach (var item in t)
            {
                dataGridView1.Rows.Add(item.ToString());
            }
            dataGridView2.DataSource = Baza.GetTableData(t[0]);
            label2.Text = t[0];
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView2_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            StartedAddingRow = true;
            
        }

        private void dataGridView2_RowValidated(object sender, DataGridViewCellEventArgs e)
        {

            if (StartedAddingRow)
            {
                StartedAddingRow = false;
                if (Baza.InsertRow(comboBox1.Text, label2.Text, dataGridView2.Rows[e.RowIndex])) MessageBox.Show("Row added successful");
            }
            
        }

        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView2_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //var d = (sender as DataRow);
            

        }

        private void dataGridView2_AllowUserToDeleteRowsChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            
        }

        private void dataGridView2_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            MessageBox.Show("Usunięto Wiersz: " +dataGridView2.Rows[e.Row.Index].Cells[0].Value.ToString());
            Baza.RemoveByIndex(comboBox1.Text, label2.Text, dataGridView2.Rows[e.Row.Index].Cells[0].Value.ToString());
            dataGridView2.DataSource = null;
            
            dataGridView2.Rows.Clear();
            string[] t = new string[1];
            t = Baza.GetTableNames();
            dataGridView2.DataSource = Baza.GetTableData(t[0]);
            
        }
    }

    class Bazadanych
    {
        SqlConnection conn;
        public Bazadanych(string ServerName)
        {
            conn = new SqlConnection("Data Source=" + ServerName + "; Trusted_Connection=True;");
            conn.Open();
        }
        public Bazadanych(string ServerName, string BaseName)
        {
            conn = new SqlConnection("Data Source=" + ServerName + "; " + " Initial Catalog=" + BaseName + "; Trusted_Connection=True;");
            conn.Open();
        }
        public void Zamknij()
        {
            conn.Close();
        }
        public string[] GetBaseNames()
        {
            DataTable Base = conn.GetSchema("Databases");
            List<string> BaseNames = new List<string>();
            foreach (DataRow row in Base.Rows)
            {
                BaseNames.Add(row[0].ToString());
            }
            string[] nazwy = new string[BaseNames.Count];
            int i = 0;
            foreach (var item in BaseNames)
            {
                nazwy[i] = item.ToString();
                i++;
            }
            return nazwy;
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
            string temp = TableName;
            for (int i = 0; i < TableName.Count(); i++)
            {
                if (TableName[i] == Convert.ToChar(32)) temp = "[" + TableName + "]";
                
            }
            DataTable t = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + @temp, conn))
            {
                da.Fill(t);
            };
            return t;
        }

        private string[] GetColumnNames(string TableName, string BaseName)
        {

            var Tables = conn.GetSchema("Columns", new[] { BaseName, null, TableName });
            List<string> TablesNames = new List<string>();
            
            foreach (DataRow row in Tables.Rows)
            {
                 TablesNames.Add(row[3].ToString());
            }
            return TablesNames.ToArray();
        }

        public void UpdateCell(string BaseName,string TableName, int Col, int id, string Content)
        {
            string[] columns = GetColumnNames(TableName, BaseName);
            
            using (SqlCommand comm = conn.CreateCommand())
            {
                comm.CommandText = "use " + @BaseName + " UPDATE  " + @TableName + " SET " + @columns[Col-1] + " = @Content  WHERE Id = @ID";
                comm.Parameters.AddWithValue("@Content", Content);
                comm.Parameters.AddWithValue("@ID", id);
                comm.ExecuteNonQuery();
            }
        }
       
        public bool RemoveByIndex( string BaseName, String TableName, string index)
        {
            using (SqlCommand comm = conn.CreateCommand())
            {
                comm.CommandText = "use " + @BaseName + " DELETE FROM  " + @TableName + " WHERE ID = " + @index;

                comm.ExecuteNonQuery();
                return true;
            }
            
        }
        public bool InsertRow(string BaseName, string TableName, DataGridViewRow Data)
        {
            string[] columns = GetColumnNames(TableName, BaseName);
            string kolumny, value;
            string[] parametry = new string[Data.Cells.Count];
            kolumny = "(";
            for (int i = 1; i < Data.Cells.Count; i++)
			{
                if (i + 1 == Data.Cells.Count)
                {
                    kolumny += columns[i] + ")";
                    parametry[i] = "@" + columns[i];
                }
                else
                {
                    kolumny += columns[i] + ", ";
                    parametry[i] = "@" + columns[i];
                }
			}
            value = "(";
            for (int i = 1; i < Data.Cells.Count; i++)
			{
                string temp = columns[i];
                bool t = false;
                for (int j = 0; j < temp.Length; j++)
                {
                   if (temp[j] == Convert.ToChar(32)) t = true ;
                 
                }
                if (t) temp = "[" + temp + "]";
                t = false;
                if (temp != string.Empty)
                {
                    if (i + 1 == Data.Cells.Count) value += "@" + temp + ")";
                    else
                        value += "@" + temp + ", ";
                }
			}
            
            using (SqlCommand comm = conn.CreateCommand())
            {
                comm.CommandText = "use " + @BaseName + " INSERT INTO  " + @TableName + " " + kolumny + " VALUES " + value;
                for (int i = 1; i < Data.Cells.Count; i++)
                {
                    string temp = Data.Cells[i].Value.ToString();
                    if (temp == string.Empty) temp = "NULL";
                    comm.Parameters.AddWithValue(parametry[i], Data.Cells[i].Value.ToString());
                }
                 comm.ExecuteNonQuery();
                 return true;
            }
        
            return false;
        }
    }
}
