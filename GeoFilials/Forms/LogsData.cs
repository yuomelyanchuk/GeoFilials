using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class LogsData : Form
    {
        private Dictionary<string, string> DataIn;
        private DataTable Table;

        public delegate void GetFilialIDHandler(int FiliD);
        public event GetFilialIDHandler GetFilialID;

        public LogsData(DataTable dataTableIn)
        {
            InitializeComponent();
            this.Table = dataTableIn;
            GetDictionary();
            FillLabels();
            FillDataGridView();
        }


        private void GetDictionary()
        {
            try
            {
                if (Table != null)
                {
                    if (Table.Rows.Count == 1)
                    {
                        DataIn = new Dictionary<string, string>();

                        DataRow row = Table.Rows[0];

                        foreach (DataColumn col in Table.Columns)
                        {
                            string val = row[col.ColumnName].ToString();

                            if (val != null && val != string.Empty && val.Length > 0)
                            {
                                DataIn.Add(col.ColumnName, row[col.ColumnName].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Dobby.GetMessageBoxTitle(2));
            }
        }

        private void FillLabels()
        {
            label2.Text = (DataIn.ContainsKey("id")) ? DataIn["id"] : "[N/A]";
            label4.Text = (DataIn.ContainsKey("upr_company")) ? DataIn["upr_company"] : "[N/A]";
            label6.Text = (DataIn.ContainsKey("name")) ? DataIn["name"] : "[N/A]";
            label8.Text = (DataIn.ContainsKey("working")) ? DataIn["working"] : "[N/A]";
            label10.Text = (DataIn.ContainsKey("format")) ? DataIn["format"] : "[N/A]";
            label12.Text = (DataIn.ContainsKey("region")) ? DataIn["region"] : "[N/A]";
            label14.Text = (DataIn.ContainsKey("city")) ? DataIn["city"] : "[N/A]";
            label16.Text = (DataIn.ContainsKey("streettype")) ? DataIn["streettype"] : "[N/A]";
            label18.Text = (DataIn.ContainsKey("street")) ? DataIn["street"] : "[N/A]";
            label20.Text = (DataIn.ContainsKey("house")) ? DataIn["house"] : "[N/A]";
        }

        private void FillDataGridView()
        {
            DataTable table = new SQL().GetLogsDataTable(Convert.ToInt32(DataIn["id"]));

            if (table != null)
            {
                this.dataGridView1.DataSource = table;
            }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            GetFilialID(Convert.ToInt32(DataIn["id"]));
            this.Close();
        }

        
    }
}
