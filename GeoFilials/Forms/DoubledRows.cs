using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class DoubledRows : Form
    {
        #region Поля 
        private volatile DataTable Data;
        private Stopwatch Stopwatch = new Stopwatch();
        private Dictionary<string, string> FilterData;
        #endregion

        #region Конструктор
        public DoubledRows()
        {
            InitializeComponent();
            Stopwatch.Start();
            this.pictureBox1.Dock = DockStyle.Fill;
            backgroundWorker1.RunWorkerAsync();
        }
        #endregion

        #region Методы
        private string GetTimeTaken()
        {
            TimeSpan time = this.Stopwatch.Elapsed;
            return string.Format("{0:00}:{1:00}", time.TotalMinutes, time.TotalSeconds);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Data = new SQL().GetDoubledRowsDataTable();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingComplited();
        }

        private void ChangeComponentsSize()
        {
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.groupBox1.Width = this.dataGridView1.Width + 10;
            this.Width = groupBox1.Width + 10;
            this.Height = 520;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.FilterData = new Dictionary<string, string>
            {
                { "city", dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value.ToString() },
                { "streettype", dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[1].Value.ToString() },
                { "street", dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[2].Value.ToString()},
                { "house", dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[3].Value.ToString()}
            };
            Dobby.GetFilterString(FilterData);
            try
            {
                Form form = Application.OpenForms[0];
                form.Activate();
            }
            catch (Exception)
            {
            }
            this.Close();
        }

        private void LoadingComplited()
        {
            ShowComponents();
            this.dataGridView1.DataSource = Data;
            ChangeComponentsSize();
            Stopwatch.Stop();
            this.toolStripStatusLabel1.Text = string.Format("Затраченое время : {0}", GetTimeTaken());
        }

        private void ShowComponents()
        {
            this.pictureBox1.Visible = false;
            this.groupBox1.Visible = true;
            this.toolStrip1.Visible = true;
            this.statusStrip1.Visible = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new Excel().ExportDataToExcel(Data,0);
        }
        #endregion
    }
}
