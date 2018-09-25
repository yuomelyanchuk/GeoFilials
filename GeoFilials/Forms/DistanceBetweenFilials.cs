using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class DistanceBetweenFilials : Form
    {
        #region Поля
        private List<string> types;
        private int version;
        private volatile string selectedItem;
        private volatile int selectedItemIndex;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public DistanceBetweenFilials()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            ShowComponents();
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Пользовательский конструктор
        /// </summary>
        /// <param name="BusinessTypes"></param>
        public DistanceBetweenFilials(List<string> BusinessTypes)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.types = BusinessTypes;
            this.version = 1;
            ShowComponents();
            FillComboBox();
        }
        #endregion

        #region Методы
        /// <summary>
        /// Запуск процесса в фоновом режиме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable table = (version == 1) ? new SQL().GetDistanceBetweenFilials(selectedItem) : new SQL().GetDistanceBetweenFilials();
            new Excel().ExportDataToExcel(table);
            MessageBox.Show(Dobby.GetDialogText(3), Dobby.GetMessageBoxTitle(3));
        }

        /// <summary>
        /// Завершение работы фонового процесса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Нажатие на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex>0)
            {
                this.label1.Visible = false;
                this.comboBox1.Visible = false;
                this.button1.Visible = false;
                this.button2.Visible = false;
                this.pictureBox1.Visible = true;
                this.pictureBox1.Dock = DockStyle.Fill;
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show(Dobby.GetErrorText(14),Dobby.GetMessageBoxTitle(1));
            }
        }

        /// <summary>
        /// Подтверждение осуществления выбора пользователем, элемента в компоненте ComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.selectedItem = this.comboBox1.SelectedItem.ToString();
            this.selectedItemIndex = this.comboBox1.SelectedIndex;
        }

        /// <summary>
        /// Заполнить компонент ComboBox данными
        /// </summary>
        private void FillComboBox()
        {
            if (types != null)
            {
                comboBox1.Items.Add(Dobby.GetDialogText(4));

                foreach (var item in types)
                {
                    comboBox1.Items.Add(item.ToString());
                }
                this.comboBox1.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Отображение/сокрытие компонентов.
        /// </summary>
        private void ShowComponents()
        {
            if (version == 0)
            {
                this.label1.Visible = false;
                this.comboBox1.Visible = false;
                this.button1.Visible = false;
                this.button2.Visible = false;
                this.pictureBox1.Visible = true;
                this.pictureBox1.Dock = DockStyle.Fill;
            }
            else
            {
                this.label1.Visible = true;
                this.comboBox1.Visible = true;
                this.button1.Visible = true;
                this.button2.Visible = true;
                this.pictureBox1.Visible = false;
            }
        }
        #endregion
    }
}
