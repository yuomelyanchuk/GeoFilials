using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GeoFilials.Scripts;
using GeoFilials.Forms;

namespace GeoFilials
{
    public partial class MainWindow : Form
    {
        #region Поля
        private bool FilterActivated;
        private LogsData GetLogsDataForm;
        private DistanceBetweenFilials DistanceBetweenFilials;
        private ColumnsVisibleSetings GetColumnsVisibleSetings;
        private CurrentUser GetCurrent = null;
        private SQL sQL;

        internal SQL SQL { get => sQL; set => sQL = value; }
        private int SelectedFillID, CurrentRowIndex;
        #endregion

        #region конструктор
        public MainWindow(CurrentUser user)
        {
            GetCurrent = user;
            SQL = new SQL();
            SQL.GetDataFromSQL();
            InitializeComponent();
            FillDataGridAllData();
            FillComboBoxes();
            this.groupBox1.Visible = FilterActivated;
            DataGridViewChangePosition();
            this.Text = string.Format("Geo Filials : {0}", GetCurrent.User);
            SetToolStripButtonImageScaling();
            this.dataGridView1.ClearSelection();
            new ToolTip().SetToolTip(this.button1, "Убрать фильтр");
            new ToolTip().SetToolTip(this.button2, "Выбрать столбцы");
            ShowColumns(GetCurrent.UserVisibleColumnSettings);
        }
        #endregion

        #region Методы

        #region DataTable
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTable GetRegionCityDataTable()
        {
            var q = (from row in sQL.GetData.AsEnumerable()
                     select new { Region = row.Field<string>("region"), City = row.Field<string>("city") }
                                ).Distinct();

            DataTable RegionCity = new DataTable();
            RegionCity.Columns.Add(new DataColumn("region", typeof(string)));
            RegionCity.Columns.Add(new DataColumn("city", typeof(string)));

            foreach (var item in q)
            {
                DataRow row = RegionCity.NewRow();
                row["region"] = item.Region;
                row["city"] = item.City;
                RegionCity.Rows.Add(row);
            }
            return RegionCity;
        }
        #endregion

        #region List<T>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        private List<string> GetListFromDataTableFieldByName(string FieldName)
        {
            return (from row in SQL.GetData.AsEnumerable()
                    orderby row.Field<string>(FieldName)
                    select row.Field<string>(FieldName)).Distinct().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ObjectList> GetObjectLists()
        {
            List<ObjectList> objectLists = new List<ObjectList>();

            objectLists.Add(new ObjectList
            {
                Name = "upr_company",
                Data = GetListFromDataTableFieldByName("upr_company"),
                DataTable = null
            }); // Управляющая компания
            objectLists.Add(new ObjectList
            {
                Name = "RegionCityDataTable",
                Data = null,
                DataTable = GetRegionCityDataTable()
            }); // DataTable [region][city]
            objectLists.Add(new ObjectList
            {
                Name = "name",
                Data = GetListFromDataTableFieldByName("name"),
                DataTable = null
            }); // Сеть
            objectLists.Add(new ObjectList
            {
                Name = "streettype",
                Data = GetListFromDataTableFieldByName("streettype"),
                DataTable = null
            }); // Тип улицы
            objectLists.Add(new ObjectList
            {
                Name = "working",
                Data = GetListFromDataTableFieldByName("working"),
                DataTable = null
            }); // Статус
            objectLists.Add(new ObjectList
            {
                Name = "format",
                Data = GetListFromDataTableFieldByName("format"),
                DataTable = null
            }); // Формат
            objectLists.Add(new ObjectList
            {
                Name = "food",
                Data = GetListFromDataTableFieldByName("food"),
                DataTable = null
            }); // Тип Бизнеса

            return objectLists;
        }

        #endregion

        #region void

        /// <summary>
        /// Нажатие кнопки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Button))
                {
                    Button button = (Button)sender;
                    int index = Dobby.GetButtonIndex(button.Name);

                    switch (index)
                    {
                        case 1:
                            FillDataGridAllData();
                            foreach (var item in groupBox1.Controls)
                            {
                                if ((item as ComboBox) != null)
                                {
                                    ComboBox box = (ComboBox)item;
                                    box.SelectedIndex = 0;
                                }
                            }
                            FillComboBoxes();
                            break;
                        case 2:
                            GetColumnsVisibleSetings = new ColumnsVisibleSetings(GetCurrent.UserVisibleColumnSettings);
                            GetColumnsVisibleSetings.Get_CVS += new ColumnsVisibleSetings.GetCVSHandler(ChangeColumnsVisible);
                            GetColumnsVisibleSetings.Show();
                            break;
                    }
                }
                else if(sender.GetType() == typeof(ToolStripButton))
                {
                    ToolStripButton button = (ToolStripButton)sender;
                    int index = Dobby.GetButtonIndex(button.Name);
                    switch (index)
                    {
                        case 1:
                            FilterActivated = FilterActivated ? false : true;
                            groupBox1.Visible = FilterActivated;
                            DataGridViewChangePosition();
                            break;
                        case 2:
                            object Data = (object)GetObjectLists();
                            Form addNF = new EditFilial(Data, 1, null);
                            addNF.StartPosition = FormStartPosition.CenterScreen;
                            addNF.ShowDialog();
                            break;
                        case 3:
                            Form DoubleRowsForm = new DoubledRows();
                            DoubleRowsForm.StartPosition = FormStartPosition.CenterScreen;
                            DoubleRowsForm.ShowDialog();
                            break;
                        case 4:
                            if (SelectedFillID > 0)
                            {
                                DataTable table = (from row in SQL.GetData.AsEnumerable()
                                                   where row.Field<int>("id") == SelectedFillID
                                                   select row).Distinct().CopyToDataTable();

                                GetLogsDataForm = new LogsData(table);
                                GetLogsDataForm.StartPosition = FormStartPosition.CenterScreen;
                                GetLogsDataForm.Show();
                                GetLogsDataForm.GetFilialID += new LogsData.GetFilialIDHandler(GetFilialID);
                            }
                            else
                            {
                                MessageBox.Show(Dobby.GetErrorText(12), Dobby.GetMessageBoxTitle(1));
                            }
                            break;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Изменить цвет выделенной строки.
        /// </summary>
        private void CellsColor()
        {
            if (CurrentRowIndex > 0)
            {
                this.dataGridView1.CurrentCell = this.dataGridView1[0, CurrentRowIndex];
            }
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.Rows.Cast<DataGridViewRow>().ToList().ForEach(a => { a.DefaultCellStyle.BackColor = Color.White; });
            dataGridView1.Rows.Cast<DataGridViewRow>().ToList().Find(a=> ( a == dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y])).DefaultCellStyle.BackColor= Color.LightGreen;

            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    if (row == dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y])
            //    {
            //        row.DefaultCellStyle.BackColor = Color.LightGreen;
            //    }
            //    else
            //    {
            //        row.DefaultCellStyle.BackColor = Color.White;
            //    }
            //}
        }

        /// <summary>
        /// Нажатие клавишей мыши на строке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            CurrentRowIndex = dataGridView1.CurrentRow.Index;
            SelectedFillID = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value);
            CellsColor();
        }

        /// <summary>
        /// Двойной клик, на строке в компоненте DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CurrentRowIndex = dataGridView1.CurrentRow.Index;
            OpenEditFilialForm(Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[0].Value));
        }

        /// <summary>
        /// Перерисовка компонента DataGridView
        /// </summary>
        private void DataGridViewChangePosition()
        {
            if (FilterActivated)
            {
                //this.dataGridView1.Size = new Size(1256, 790);
                //this.dataGridView1.Size = new Size(this.Size.Width - 20, this.Size.Height - 278);
                //MessageBox.Show( Screen.PrimaryScreen.Bounds.Height.ToString());
                //MessageBox.Show(this.Size.Height.ToString() + ";" + this.dataGridView1.Size.ToString());
                //MessageBox.Show(groupBox1.Size.ToString());
                this.dataGridView1.Size = new Size(this.Size.Width - 20, this.Size.Height - Convert.ToInt32(groupBox1.Size.Height*2));
                this.dataGridView1.Location = new Point(12, 162);
                this.toolStripButton1.Image = GeoFilials.Properties.Resources.Zoom_Out_icon;
            }
            else
            {
                //this.dataGridView1.Size = new Size(1256, 890);
                this.dataGridView1.Size = new Size(this.Size.Width - 20, this.Size.Height - Convert.ToInt32(groupBox1.Size.Height * 1.2));
                this.dataGridView1.Location = new Point(12, 62);
                this.toolStripButton1.Image = GeoFilials.Properties.Resources.Zoom_In_icon;
                //MessageBox.Show(groupBox1.Size.ToString());
            }
            this.dataGridView1.ClearSelection();
        }

        /// <summary>
        /// Заполнить компоненты ComboBox
        /// </summary>
        private void FillComboBoxes()
        {
            foreach (var item in groupBox1.Controls)
            {
                if ((item as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)item;
                    List<string> DataForCombo = new List<string>();
                    string SelectedValue = null;
                    var data = (from row in SQL.GetData.DefaultView.ToTable().AsEnumerable()
                                orderby row.Field<string>(box.Tag.ToString())
                                select row.Field<string>(box.Tag.ToString())).Distinct().ToList();

                    if (box.SelectedIndex > 0)
                    {
                        SelectedValue = box.SelectedItem.ToString();
                    }

                    box.Items.Clear();
                    box.Items.Add("Все");
                    foreach (var row in data)
                    {
                        box.Items.Add(row);
                    }

                    if (SelectedValue != null)
                    {
                        box.SelectedIndex = box.Items.IndexOf(SelectedValue);
                    }
                    else
                    {
                        box.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Отобразить данные в компоненте DataGridView
        /// </summary>
        private void FillDataGridAllData()
        {
            SQL.GetData.DefaultView.RowFilter = string.Empty;
            Dobby.lastFilterParams = string.Empty;
            SQL.GetData.DefaultView.Sort = "id DESC";
            this.dataGridView1.DataSource = SQL.GetData;
            this.dataGridView1.ClearSelection();
        }

        /// <summary>
        /// Получить ИД Филиала, который выбран для редактирования.
        /// </summary>
        /// <param name="FilialID"></param>
        private void GetFilialID(int FilialID)
        {
            OpenEditFilialForm(FilialID);
        }

        /// <summary>
        /// Форма активирована.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (Dobby.lastFilterParams != string.Empty)
            {
                SQL.GetData.DefaultView.RowFilter = Dobby.lastFilterParams;
            }
            else
            {
                UpdateAllData();
            }
            if (CurrentRowIndex > 0)
            {
                this.dataGridView1.FirstDisplayedScrollingRowIndex = CurrentRowIndex;
                CellsColor();
            }
        }

        /// <summary>
        /// Закрытие программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();

            foreach (var item in Application.OpenForms)
            {
                Form form = (Form)item;
                form.Hide();
            }

            System.Environment.Exit(0);
        }

        /// <summary>
        /// Открыть форму редактирования филиала
        /// </summary>
        /// <param name="Id"></param>
        private void OpenEditFilialForm(int Id)
        {
            DataTable table = (from row in SQL.GetData.AsEnumerable()
                               where row.Field<int>("id") == Id
                               select row).Distinct().CopyToDataTable();
            object Data = (object)GetObjectLists();
            Form addNF = new EditFilial(Data, 2, table);
            addNF.StartPosition = FormStartPosition.CenterScreen;
            addNF.ShowDialog();
        }

        /// <summary>
        /// Выбор элементов в компоненте ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangeCommitted(object sender, EventArgs e)
        {
            List<ComboBox> boxes = new List<ComboBox>();

            foreach (var item in groupBox1.Controls)
            {
                if ((item as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)item;
                    if (box.SelectedIndex > 0)
                    {
                        boxes.Add(box);
                    }
                }
            }

            if (boxes.Count > 0)
            {
                SQL.GetData.DefaultView.RowFilter = Dobby.GetFilterString(ref boxes);
            }
            else
            {
                FillDataGridAllData();
            }
            FillComboBoxes();
        }

        /// <summary>
        /// Настройки изображения, на компоненте ToolStripButton.
        /// </summary>
        private void SetToolStripButtonImageScaling()
        {
            foreach (var item in Controls)
            {
                if ((item as ToolStripButton) != null)
                {
                    ToolStripButton stripButton = (ToolStripButton)item;
                    stripButton.ImageScaling = ToolStripItemImageScaling.None;
                }
            }
        }

        /// <summary>
        /// Обработка выбора элемента на кнопке "Выгрузить в Excel".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = Dobby.GetButtonIndex(e.ClickedItem.Name.ToString());

            switch (index)
            {
                case 1:
                    new Excel().ExportDataToExcel(SQL.GetData);
                    break;
                case 2:
                    if (Dobby.lastFilterParams is null || Dobby.lastFilterParams == string.Empty)
                    {
                        MessageBox.Show(Dobby.GetErrorText(11), Dobby.GetMessageBoxTitle(1));
                    }
                    else
                    {
                        DataView view = new DataView();
                        view = SQL.GetData.DefaultView;
                        view.RowFilter = Dobby.lastFilterParams;
                        DataTable table = view.ToTable();
                        new Excel().ExportDataToExcel(table);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Обработка выбора элемента на кнопке "Файл для карты".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButton2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int rez = sQL.CreateMapFile(Dobby.GetButtonIndex(e.ClickedItem.Name.ToString()));
            if (rez > 0)
                MessageBox.Show(Dobby.GetDialogText(3), Dobby.GetMessageBoxTitle(3));
            else
                MessageBox.Show(Dobby.GetErrorText(13), Dobby.GetMessageBoxTitle(1));
        }

        /// <summary>
        /// Обработка выбора элемента на кнопке "Расстояние между филиалами"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripDropDownButton3_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int index = Dobby.GetButtonIndex(e.ClickedItem.Name.ToString());

            switch (index)
            {
                case 1:
                    DistanceBetweenFilials = new DistanceBetweenFilials();
                    DistanceBetweenFilials.Show();
                    break;
                case 2:
                    DistanceBetweenFilials = new DistanceBetweenFilials(GetBisinessTypes());
                    DistanceBetweenFilials.Show();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Обновить все.
        /// </summary>
        private void UpdateAllData()
        {
            SQL.GetDataFromSQL();
            FillDataGridAllData();
            FillComboBoxes();

        }

        /// <summary>
        /// Получить список BisinessTypes. Для передачи на форму DistanceBetweenFilials.
        /// </summary>
        /// <returns></returns>
        private List<string> GetBisinessTypes()
        {
            return (from row in SQL.GetData.AsEnumerable()
                    select row.Field<string>("food")).Distinct().ToList();
        }

        /// <summary>
        /// Отобразить коллонки
        /// </summary>
        /// <param name="data"></param>
        private void ShowColumns(DataTable data)
        {
            foreach (DataRow row in data.Rows)
            {
                foreach (DataGridViewColumn column in this.dataGridView1.Columns)
                {
                    if (row["Название Поля"].ToString() == column.Name)
                    {
                        column.Visible = (bool)row["Видимость"];
                    }
                }
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            DataGridViewChangePosition();
        }

        /// <summary>
        /// Принять данные с дочерней формы. Список коллонок и статус отображения.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RadioButtonSelected"></param>
        private void ChangeColumnsVisible(DataTable data, int RadioButtonSelected)
        {
            ShowColumns(data);
            if (RadioButtonSelected == 2)
            {
                GetCurrent.ChangeVisibleColumns(data);
                new SQL().SetUserVisibleColumns(GetCurrent);
            }
        }


        #endregion

        #endregion

    }
}
