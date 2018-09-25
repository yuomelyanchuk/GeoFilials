using System;
using System.Data;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class ColumnsVisibleSetings : Form
    {
        #region Поля 
        private DataTable Table;
        private int SelectedRadioButton = 1;
        #endregion

        #region Делегаты/События
        public delegate void GetCVSHandler(DataTable data, int RadioButtonSelected);
        public event GetCVSHandler Get_CVS;
        #endregion

        #region Конструктор
        public ColumnsVisibleSetings(DataTable UserVisibleColumnsSettings)
        {
            InitializeComponent();
            this.Table = UserVisibleColumnsSettings;
            FillDataGridView();
            new ToolTip().SetToolTip(this.radioButton1, "При следующем запуске программы\nбудут отображены все поля");
            new ToolTip().SetToolTip(this.radioButton2, "При следующем запуске программы\nбудут отображены все поля\nвыбор которых будет осуществлен сейчас");
            this.radioButton1.Checked = true;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Нажатие кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Get_CVS(Table, SelectedRadioButton);
            this.Close();
        }

        /// <summary>
        /// Изменение статуса RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = (RadioButton)sender;

            if (button.Checked)
            {
                SelectedRadioButton = Dobby.GetButtonIndex(button.Name);
            }
        }

        /// <summary>
        /// Нажатие ячейки типа CheckBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() == "False")
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value = true;
                else
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value = false;

                UpdateDataTable();
            }
        }

        /// <summary>
        /// Заполнить компонент DataGridView
        /// </summary>
        private void FillDataGridView()
        {
            this.dataGridView1.DataSource = Table;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ClearSelection();
            this.dataGridView1.AllowUserToAddRows = false;
        }

        /// <summary>
        /// Обновить компонент DataGridView
        /// </summary>
        private void UpdateDataTable()
        {
            foreach (DataGridViewRow gRow in this.dataGridView1.Rows)
            {
                foreach (DataRow dRow in Table.Rows)
                {
                    if (gRow.Cells[0].Value.ToString() == dRow["Название Поля"].ToString())
                    {
                        dRow["Видимость"] = (bool)gRow.Cells[1].Value;
                    }
                }
            }
        }

        #endregion
    }
}
