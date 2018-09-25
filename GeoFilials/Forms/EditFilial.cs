using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{

    public partial class EditFilial : Form
    {
        #region Поля
        private NewCity NewCity;
        private int version;
        private int row_FilialID { get; set; }
        private object DataIn;
        private DataTable RegionCityData;
        private DataTable Edit_Filial;
        private NewCityToDB NewCityToDB;
        private List<TextBox> TextBoxesForTab;
        //private NewFilialToDB FilialToDB;
        private DateTime CloseDate;
        private bool CloseDatePicked;
        private int[] IsFiledMatrix = new int[10];
        [Flags]
        private enum NecessarilyComponentsToFill {upr_company = 0, name = 1 , region = 2, city = 3 , streettype = 4, street = 5, house = 6, working = 7 , format = 8 , food = 9 };
        #endregion

        #region Конструктор
        public EditFilial(object Data, int Version , DataTable FilialToEdit)
        {
            Edit_Filial = FilialToEdit;
            this.version = Version;
            this.DataIn = Data;
            GetDataTable();
            InitializeComponent();
            FillComboBoxes();
            InitializeTextBoxexForTabArray();
            IsOkActive();
            this.label20.Visible = false;
            this.label21.Visible = false;
            this.textBox12.Visible = false;
            this.dateTimePicker1.Visible = false;
            SwitchVersion();
        }
        #endregion

        #region Методы

        #region Bool

        /// <summary>
        /// Проверка на наличие текста в координатах.
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        private bool CheckCoords(string dataIn)
        {
            Regex regex = new Regex("[a-z]|[A-Z]|[а-я]|[А-Я]");
            return regex.IsMatch(dataIn);
        }

        /// <summary>
        /// Проверка значения. Является ли числом.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckValue(string value)
        {
            int i;
            if (Int32.TryParse(value, out i))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Проверка. "Управляющая компания" = "Fozzy Group".
        /// </summary>
        /// <returns></returns>
        private bool FozzyChecked()
        {
            if (comboBox1.SelectedItem.ToString() == "Fozzy Group")
            {
                int i;
                if (Int32.TryParse(textBox12.Text, out i))
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        #endregion

        #region String

        private string GetSelectedRegionName ()
        {
            return (comboBox3.SelectedIndex > 0) ? comboBox3.SelectedItem.ToString() : null;
        }


        /// <summary>
        /// Работа с точками координат.
        /// </summary>
        /// <param name="dataIn"></param>
        /// <returns></returns>
        private string WorkWithDots(string dataIn)
        {
            string data = dataIn.Replace(',', '.');
            data = Regex.Replace(data, @"\s+", "");
            if (Convert.ToChar(data.Substring(0, 1)) != '.')
            {
                int k = 0;
                foreach (char item in data)
                {
                    if (item == '.')
                    {
                        k++;
                    }
                }
                if (k > 1)
                {
                    string[] mas = data.Split('.');

                    if (CheckValue(mas[0]) && CheckValue(mas[1]))
                        return string.Format("{0}.{1}", mas[0].Trim(), mas[1].Trim());
                    else
                        return string.Empty;
                }
                else
                {
                    if (data.Length > 1 && Convert.ToChar(data.Substring(data.Length - 1, 1)) != '.')
                        return data;
                    else
                        return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region Void

        /// <summary>
        /// Нажатие кнопок.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int index = Dobby.GetButtonIndex(button.Name);
            switch (index)
            {
                case 1:
                    if (version == 1)
                    {
                        if (FozzyChecked())
                        {
                            ExportNewFilial(PrepareFilialRow());
                        }
                    }
                    else if (version == 2)
                    {
                        UpdateFilial(PrepareFilialRow(), row_FilialID);
                        this.Close();
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(Dobby.GetErrorText(7), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (result == DialogResult.OK)
                        {
                            textBox12.Focus();
                            textBox12.SelectionStart = textBox12.Text.Length;
                        }
                    }
                    break;
                case 2:
                    this.Hide();
                    this.Dispose();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Изменения значения в комбоБоксе "Область".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e) // ComboBox Область
        {
            List<string> DataForBox = new List<string>();

            if (comboBox3.SelectedIndex > 0)
            {
                DataForBox = (from row in RegionCityData.AsEnumerable()
                              where row.Field<string>("region") == comboBox3.SelectedItem.ToString()
                              orderby row.Field<string>("city")
                              select row.Field<string>("city")
                             ).Distinct().ToList();
            }
            else
            {
                DataForBox = (from row in RegionCityData.AsEnumerable()
                              orderby row.Field<string>("city")
                              select row.Field<string>("city")
                             ).Distinct().ToList();
            }

            string selectedCity = (comboBox4.SelectedIndex > 0) ? comboBox4.SelectedItem.ToString() : string.Empty;

            comboBox4.Items.Clear();
            comboBox4.Items.Add("            ------ <Выберите элемент> ------");
            comboBox4.Items.Add("            ------ <Добавить новый город> ------");
            foreach (var item in DataForBox)
            {
                comboBox4.Items.Add(item);
            }

            if (comboBox3.SelectedIndex == 0)
            {
                comboBox4.SelectedIndex = 0;
            }
 
            WorkWithFiledMatrix();
        }

        /// <summary>
        /// Изменения значения в комбоБоксе "Город".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox4_SelectionChangeCommitted(object sender, EventArgs e) // ComboBox Город
        {
            ComboBox box = (ComboBox)sender;
            if (box.SelectedIndex == 1)
            {
                NewCity = new NewCity(RegionCityData, GetSelectedRegionName());
                NewCity.Show();
                NewCity.GetNewCityName += new NewCity.GetNewCityNameHandler(GetNewCityName_);
            }
            else
            {
                List<string> DataForBox = new List<string>();

                DataForBox = (from row in RegionCityData.AsEnumerable()
                              where row.Field<string>("city") == comboBox4.SelectedItem.ToString()
                              orderby row.Field<string>("region")
                              select row.Field<string>("region")
                             ).Distinct().ToList();
                if (DataForBox.Count == 1)
                {
                    comboBox3.SelectedIndex = comboBox3.Items.IndexOf(DataForBox[0].ToString());
                }
                WorkWithFiledMatrix();
            }
        }

        /// <summary>
        /// Закрытие формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFilial_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Dispose();
        }

        /// <summary>
        /// Добавить новый филиал в БД.
        /// </summary>
        private void ExportNewFilial(NewFilialToDB FilialToDB)
        {
            if (new SQL().IsFilialExist(FilialToDB.city, FilialToDB.streettype, FilialToDB.street, FilialToDB.house, FilialToDB.name))
            {
                MessageBox.Show(Dobby.GetErrorText(6), Dobby.GetMessageBoxTitle(1));
            }
            else
            {
                try
                {
                    int rez = new SQL().AddNewFilial(FilialToDB);
                    if (rez == 1)
                    {
                        MessageBox.Show(Dobby.GetDialogText(1), Dobby.GetMessageBoxTitle(3));
                        Form form = Application.OpenForms[0];
                        form.Activate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Dobby.GetMessageBoxTitle(2));
                }
            }
        }

        /// <summary>
        /// Заполнить элементы ComboBox значениями.
        /// </summary>
        private void FillComboBoxes()
        {
            List<ObjectList> objects = (List<ObjectList>)DataIn;

            foreach (var Control in this.Controls)
            {
                if ((Control as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)Control;

                    if (box.Items.Count > 0)
                    {
                        box.Items.Clear();
                    }
                    box.Items.Add("            ------ <Выберите элемент> ------");

                    if (box.Tag.ToString() == "city")
                    {
                        box.Items.Add("            ------ <Добавить новый город> ------");
                    }

                    foreach (var MyObject in objects)
                    {
                        if (MyObject.Name.ToString() == box.Tag.ToString())
                        {
                            foreach (var row in MyObject.Data)
                            {
                                box.Items.Add(row);
                            }
                        }
                        else
                        {
                            if (MyObject.DataTable != null)
                            {
                                List<string> Columns = RegionCityData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
                                foreach (var ColName in Columns)
                                {
                                    if (ColName == box.Tag.ToString())
                                    {
                                        var q = (from row in RegionCityData.AsEnumerable()
                                                 orderby row.Field<string>(ColName)
                                                 select row.Field<string>(ColName)
                                            ).Distinct().ToList();
                                        foreach (var row in q)
                                        {
                                            box.Items.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    box.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Получить DataTable, из входящего объекта.
        /// </summary>
        private void GetDataTable()
        {
            List<ObjectList> objects = (List<ObjectList>)DataIn;

            foreach (var item in objects)
            {
                if (item.Name == "RegionCityDataTable")
                {
                    this.RegionCityData = item.DataTable;
                }
            }
        }

        /// <summary>
        /// Получить данные, для нового города.
        /// </summary>
        /// <param name="newCityToDB"></param>
        private void GetNewCityName_(NewCityToDB newCityToDB)
        {
            this.NewCityToDB = newCityToDB;
            try
            {
                comboBox3.SelectedIndex = comboBox3.Items.IndexOf(NewCityToDB.Region);
                comboBox4.Items.Add(newCityToDB.City);
                comboBox4.SelectedIndex = comboBox4.Items.IndexOf(NewCityToDB.City);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Dobby.GetMessageBoxTitle(2));
            }
        }

        /// <summary>
        /// Инициализация массива элементов TextBox, которые участвуют в настроенной табуляции.
        /// </summary>
        private void InitializeTextBoxexForTabArray()
        {
            TextBoxesForTab = new List<TextBox>();
            TextBoxesForTab.Add(this.textBox9);
            TextBoxesForTab.Add(this.textBox10);
            TextBoxesForTab.Add(this.textBox7);
            TextBoxesForTab.Add(this.textBox1);
            TextBoxesForTab.Add(this.textBox2);
            TextBoxesForTab.Add(this.textBox3);
            TextBoxesForTab.Add(this.textBox4);
            TextBoxesForTab.Add(this.textBox5);
            TextBoxesForTab.Add(this.textBox12);
        }

        /// <summary>
        /// Отображение элементов, при выборе "Управляющая компания" = Fozzy Group.
        /// </summary>
        /// <param name="BoxSelectedItem"></param>
        private void IsFozzyGroupSelected(string BoxSelectedItem)
        {
            if (BoxSelectedItem == "Fozzy Group")
            {
                this.label20.Visible = true;
                this.textBox12.Visible = true;
            }
            else
            {
                this.label20.Visible = false;
                this.textBox12.Visible = false;
            }
        }

        /// <summary>
        /// Отображение элементов, при выборе "Статус" = "Закрыт" или "Временно закрыт".
        /// </summary>
        /// <param name="BoxSelectedItem"></param>
        private void IsFilialClosed(string BoxSelectedItem)
        {
            if (BoxSelectedItem == "закрыт" || BoxSelectedItem == "временно закрыт")
            {
                this.label21.Visible = true;
                this.dateTimePicker1.Visible = true;
                this.button1.Location = new System.Drawing.Point(354, 555);
                this.button2.Location = new System.Drawing.Point(477, 555);
                this.Size = new System.Drawing.Size(609, 629);
            }
            else
            {
                this.label21.Visible = false;
                this.dateTimePicker1.Visible = false;
                this.button1.Location = new System.Drawing.Point(354, 494);
                this.button2.Location = new System.Drawing.Point(477, 494);
                this.Size = new System.Drawing.Size(609, 577);
            }
        }

        /// <summary>
        /// Отображение кнопки "Ок".
        /// </summary>
        private void IsOkActive()
        {
            int sum = 0;

            foreach (var item in IsFiledMatrix)
            {
                sum += item;
            }

            if (sum==10)
            {
                this.button1.Visible = true;
            }
            else
            {
                this.button1.Visible = false;
            }

        }

        /// <summary>
        /// Сбросить значения элементов ComboBox.Tag {"region","city"}, на значения по умолчанию.
        /// </summary>
        private void ResetComboRegionAndCity()
        {
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();

            comboBox3.Items.Add("            ------ <Выберите элемент> ------");
            comboBox4.Items.Add("            ------ <Выберите элемент> ------");

            var reg = (from row in RegionCityData.AsEnumerable()
                       orderby row.Field<string>("region")
                       select row.Field<string>("region")
                      ).Distinct().ToList();

            var cit = (from row in RegionCityData.AsEnumerable()
                       orderby row.Field<string>("city")
                       select row.Field<string>("city")
                      ).Distinct().ToList();

            foreach (var item in reg)
            {
                comboBox3.Items.Add(item);
            }
            foreach (var item in cit)
            {
                comboBox4.Items.Add(item);
            }
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }

        /// <summary>
        /// Выбор значения в элементе ComboBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangeCommitted(object sender, EventArgs e)
        {
            foreach (var item in this.Controls)
            {
                if ((item as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)item;

                    if (box.Tag.ToString() == "upr_company")
                    {
                        IsFozzyGroupSelected(box.SelectedItem.ToString());
                    }
                    else if (box.Tag.ToString() == "working")
                    {
                        IsFilialClosed(box.SelectedItem.ToString());
                    }
                    WorkWithFiledMatrix();
                }
            }
        }

        /// <summary>
        /// Изменение внешнего вида, в зависимости от версии формы.
        /// </summary>
        private void SwitchVersion()
        {
            switch (version)
            {
                case 2:
                    this.Text = "Редактирование филиала";
                    FillDataFilialToEdit();
                    this.button1.Visible = true; 
                    break;
                default:
                    this.Text = "Добавить новый филиал";
                    this.button1.Location = new System.Drawing.Point(354, 494);
                    this.button2.Location = new System.Drawing.Point(477, 494);
                    this.Size = new System.Drawing.Size(609, 577);
                    break;
            }
        }

        /// <summary>
        /// Нажатие клавишы в компоненте TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyCode == Keys.Enter)
            {
                if (TextBoxesForTab.Contains((TextBox)sender))
                {
                    if (TextBoxesForTab[TextBoxesForTab.IndexOf((TextBox)sender) + 1].Visible)
                    {
                        TextBoxesForTab[TextBoxesForTab.IndexOf((TextBox)sender) + 1].Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Покидаем компонент TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxLeave(object sender, EventArgs e)
        {
            TextBox text = (TextBox)sender;

            if (text.Tag.ToString() == "street" || text.Tag.ToString() == "house")
            {
                WorkWithFiledMatrix();
            }
            else if (text.Tag.ToString() == "year_opening")
            {
                try
                {
                    int year;
                    if (Int32.TryParse(text.Text, out year))
                    {
                        if (year < 1991)
                        {
                            MessageBox.Show(Dobby.GetErrorText(1), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            text.Text = string.Empty;
                            text.Focus();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(Dobby.GetErrorText(2), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    text.Text = string.Empty;
                }
            }
            else if (text.Tag.ToString() == "date_opening")
            {
                if (text.Text.Length == 5)
                {
                    text.Text = text.Text + '.';
                }
                else if (text.Text.Length < 5)
                {
                    MessageBox.Show(Dobby.GetErrorText(10), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    text.Focus();
                    text.SelectionStart = text.Text.Length;
                }
            }
        }

        /// <summary>
        /// Изменение текста в компоненте TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Tag.ToString() == "C" || textBox.Tag.ToString() == "D")
            {
                try
                {
                    string coordsFromUser = Clipboard.GetText();
                    Clipboard.Clear();

                    if (!CheckCoords(coordsFromUser))
                    {
                        WorkWithCoords(coordsFromUser);
                    }
                    else
                    {
                        textBox5.Clear();
                        textBox6.Clear();
                        MessageBox.Show(Dobby.GetErrorText(8), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                }
            }
            else if (textBox.Tag.ToString() == "date_opening")
            {
                int Value;
                char Char;
                switch (textBox4.Text.Length)
                {
                    case 1:
                        if (!Int32.TryParse(textBox4.Text, out Value))
                        {
                            textBox4.Text = "";
                        }
                        break;
                    case 2:
                        Char = Convert.ToChar(textBox4.Text.Substring(1, 1));
                        if (Char == ',' || Char == '.')
                        {
                            textBox4.Text = string.Format("0{0}", Convert.ToChar(textBox4.Text.Substring(0, 1)));
                            textBox4.SelectionStart = textBox4.Text.Length;
                        }
                        else
                        {
                            if (Int32.TryParse(textBox4.Text, out Value))
                            {
                                if (Value > 31)
                                {
                                    MessageBox.Show(Dobby.GetErrorText(4), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    textBox4.Text = "";
                                    textBox4.Focus();
                                    textBox4.SelectionStart = textBox4.Text.Length;
                                }
                                else if (Value > 0 && Value < 10)
                                {
                                    textBox4.Text = string.Format("0{0}", Value.ToString());
                                    textBox4.SelectionStart = textBox4.Text.Length;
                                }
                            }
                            else
                            {
                                textBox4.Text = textBox4.Text.Substring(0, 1);
                                textBox4.SelectionStart = textBox4.Text.Length;
                            }
                        }
                        break;
                    case 3:
                        Char = Convert.ToChar(textBox4.Text.Substring(2, 1));
                        if (Char != '.')
                        {
                            textBox4.Text = textBox4.Text.Substring(0, 2) + '.';
                            textBox4.SelectionStart = textBox4.Text.Length;
                        }
                        break;
                    case 4:
                        if (Int32.TryParse(textBox4.Text.Substring(3, 1), out Value))
                        {
                            break;
                        }
                        else
                        {
                            textBox4.Text = textBox4.Text.Substring(0, 3);
                            textBox4.SelectionStart = textBox4.Text.Length;
                        }
                        break;
                    case 5:
                        Char = Convert.ToChar(textBox4.Text.Substring(4, 1));
                        if (Char == '.' || Char == ',')
                        {
                            if (Int32.TryParse(textBox4.Text.Substring(3, 1), out Value))
                            {
                                textBox4.Text = string.Format("{0}0{1}", textBox4.Text.Substring(0, 3), Value);
                                textBox4.SelectionStart = textBox4.Text.Length;
                            }
                        }
                        else
                        {
                            Int32.TryParse(textBox4.Text.Substring(3, 2), out Value);
                            if (Value > 0 && Value < 10)
                            {
                                textBox4.Text = string.Format("{0}0{1}", textBox4.Text.Substring(0, 3), Value);
                                textBox4.SelectionStart = textBox4.Text.Length;
                            }
                            else if (Value > 12)
                            {
                                MessageBox.Show(Dobby.GetErrorText(3), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textBox4.Text = textBox4.Text.Substring(0, 3);
                                textBox4.SelectionStart = textBox4.Text.Length;
                            }
                        }
                        break;
                    case 6:
                        textBox4.Text = textBox4.Text.Substring(0, 5) + '.';
                        textBox4.SelectionStart = textBox4.Text.Length;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Обработка координат.
        /// </summary>
        /// <param name="dataIn"></param>
        private void WorkWithCoords(string dataIn)
        {
            string[] LatLong = dataIn.Replace(" ", string.Empty).Split(',').ToArray();

            if (Convert.ToInt32(LatLong[0].Substring(0, 2)) < Convert.ToInt32(LatLong[1].Substring(0, 2)))
            {
                textBox5.Text = LatLong[1];
                textBox6.Text = LatLong[0];
            }
            else
            {
                textBox5.Text = LatLong[0];
                textBox6.Text = LatLong[1];
            }
        }

        /// <summary>
        /// Работа с компонентами "Обязательными для заполнения".
        /// </summary>
        private void WorkWithFiledMatrix()
        {
            foreach (var item in this.Controls)
            {
                if ((item as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)item;

                    if (Enum.IsDefined(typeof(NecessarilyComponentsToFill),box.Tag.ToString()))
                    {
                        if (box.SelectedIndex > 0 && IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill),box.Tag.ToString())]==0)
                        {
                            IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), box.Tag.ToString())] = 1;
                        }
                        else if (box.SelectedIndex == 0 && IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), box.Tag.ToString())] != 0)
                        {
                            IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), box.Tag.ToString())] = 0;
                        }
                    }
                }
                else if ((item as TextBox) != null)
                {
                    TextBox text = (TextBox)item;
                    if (Enum.IsDefined(typeof(NecessarilyComponentsToFill), text.Tag.ToString()))
                    {
                        if (text.Text.Length > 0 && IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), text.Tag.ToString())] == 0)
                        {
                            IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), text.Tag.ToString())] = 1;
                        }
                        else if (text.Text.Length == 0 && IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), text.Tag.ToString())] != 0)
                        {
                            IsFiledMatrix[(int)Enum.Parse(typeof(NecessarilyComponentsToFill), text.Tag.ToString())] = 0;
                        }
                    }
                }
                IsOkActive();
            }

        }
        #endregion

        #endregion

        private void FillDataFilialToEdit()
        {
            try
            {
                Dictionary<string, string> valuePairs = null;
                if (Edit_Filial != null)
                {
                    if (Edit_Filial.Rows.Count == 1)
                    {
                        valuePairs = new Dictionary<string, string>();

                        DataRow row = Edit_Filial.Rows[0];

                        foreach (DataColumn col in Edit_Filial.Columns)
                        {
                            string val = row[col.ColumnName].ToString();

                            if (val != null && val != string.Empty && val.Length > 0)
                            {
                                valuePairs.Add(col.ColumnName, row[col.ColumnName].ToString());
                            }
                        }

                        foreach (KeyValuePair<string, string> pair in valuePairs)
                        {
                            foreach (Control control in this.Controls)
                            {
                                if ((control as ComboBox) != null)
                                {
                                    ComboBox box = (ComboBox)control;
                                    if (box.Tag.ToString() == pair.Key)
                                    {
                                        box.SelectedIndex = box.Items.IndexOf(pair.Value);
                                        if (pair.Key.ToString() == "upr_company")
                                        {
                                            IsFozzyGroupSelected(pair.Value);
                                        }
                                        else if (pair.Key.ToString() == "working")
                                        {
                                            IsFilialClosed(pair.Value);
                                        }
                                    }
                                }
                                else if ((control as TextBox) != null)
                                {
                                    TextBox textBox = (TextBox)control;
                                    if (textBox.Tag.ToString() == pair.Key)
                                    {
                                        textBox.Text = pair.Value;
                                    }
                                }
                            }
                        }

                        if (valuePairs.ContainsKey("id"))
                        {
                            row_FilialID = Convert.ToInt32(valuePairs["id"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Dobby.GetMessageBoxTitle(2));
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.CloseDate = new DateTime();
            this.CloseDatePicked = true;
            this.CloseDate = dateTimePicker1.Value;
        }

        private NewFilialToDB PrepareFilialRow()
        {
            NewFilialToDB filialToDB = new NewFilialToDB();
            foreach (var item in this.Controls)
            {
                if ((item as ComboBox) != null)
                {
                    ComboBox box = (ComboBox)item;
                    if (box.SelectedIndex > 0)
                    {
                        string value = box.SelectedItem.ToString();
                        switch (box.Tag)
                        {
                            case "upr_company":
                                filialToDB.upr_company = value;
                                break;
                            case "name":
                                filialToDB.name = value;
                                break;
                            case "region":
                                filialToDB.region = value;
                                break;
                            case "city":
                                filialToDB.city = value;
                                break;
                            case "streettype":
                                filialToDB.streettype = value;
                                break;
                            case "working":
                                filialToDB.working = value;
                                break;
                            case "format":
                                filialToDB.format = value;
                                break;
                            case "food":
                                filialToDB.food = value;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if ((item as TextBox) != null)
                {
                    TextBox box = (TextBox)item;
                    if (box.Text.Length > 0)
                    {
                        switch (box.Tag)
                        {
                            case "street":
                                filialToDB.street = box.Text;
                                break;
                            case "house":
                                filialToDB.house = box.Text;
                                break;
                            case "areaall":
                                filialToDB.areaall = WorkWithDots(box.Text);
                                break;
                            case "trading_area":
                                filialToDB.trading_area = WorkWithDots(box.Text);
                                break;
                            case "year_opening":
                                filialToDB.year_opening = box.Text;
                                break;
                            case "date_opening":
                                filialToDB.date_opening = box.Text;
                                break;
                            case "note":
                                filialToDB.note = box.Text;
                                break;
                            case "comments":
                                filialToDB.comments = box.Text;
                                break;
                            case "C":
                                filialToDB.c = box.Text;
                                break;
                            case "D":
                                filialToDB.d = box.Text;
                                break;
                            case "count_kass":
                                filialToDB.count_kass = box.Text;
                                break;
                            case "filid_in_base":
                                if (comboBox1.SelectedItem.ToString() == "Fozzy Group")
                                {
                                    filialToDB.filid_in_base = box.Text;
                                }
                                else
                                {
                                    if (box.Text.Length > 0)
                                    {
                                        box.Text = "";
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (CloseDatePicked)
                {
                    filialToDB.Date_Close = CloseDate.ToShortDateString();
                }
            }

            return filialToDB;
        }

        private void UpdateFilial(NewFilialToDB FilialToUpdate, int TableRowID)
        {
            try
            {
                int rez = new SQL().UpdateFilial(FilialToUpdate, TableRowID);
                if (rez == 1)
                {
                    MessageBox.Show(Dobby.GetDialogText(2), Dobby.GetMessageBoxTitle(3));
                    Form form = Application.OpenForms[0];
                    form.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Dobby.GetMessageBoxTitle(2));
            }

        }



    }
}

