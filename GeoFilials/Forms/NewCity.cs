using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class NewCity : Form
    {
        #region Поля класса
        private string NewCityName;
        private string selectedRgion;
        private string RegionIn;
        private bool RegionChecked;
        private DataTable RegionCityData;
        private NewCityToDB newCity;
        #endregion

        #region Делегаты
        public delegate void GetNewCityNameHandler(NewCityToDB newCity);
        public event GetNewCityNameHandler GetNewCityName;
        #endregion

        #region Конструктор
        public NewCity(DataTable dataTable, string RegionIn)
        {
            this.RegionCityData = dataTable;
            this.RegionIn = RegionIn;
            InitializeComponent();
            FillComboBox();
            if (RegionIn != null)
            {
                selectedRgion = RegionIn;
                RegionChecked = true;
            }
        }
        #endregion

        #region Методы

        #region bool
        /// <summary>
        /// Проверка. Наличие нового города в БД
        /// </summary>
        /// <returns></returns>
        private bool IfCityExist()
        {
            bool answer = false;
            var Cities = (from row in RegionCityData.AsEnumerable()
                          select row.Field<string>("city")).Distinct().ToList();
            foreach (var item in Cities)
            {
                if (item.ToString().ToLower() == NewCityName.ToLower())
                {
                    answer = true;
                    break;
                }
            }
            return answer;
        }
        #endregion

        #region string
        /// <summary>
        /// Диалоги
        /// </summary>
        /// <param name="ErrorID"></param>
        /// <returns></returns>
        private string GetExistCityInfo()
        {
            var querry = (from row in RegionCityData.AsEnumerable()
                          where row.Field<string>("city") == NewCityName
                          select new { Region = row.Field<string>("region"), City = row.Field<string>("city") }).Distinct().ToList();

            StringBuilder builder = new StringBuilder();
            builder.Append("В базе имеются следующие значения : \nОбласть\t\t| Город\n");
            builder.Append(new string('-', 50));
            builder.Append("\n");
            foreach (var item in querry)
            {
                builder.Append(string.Format("{0}\t| {1}\n", item.Region.ToString(), item.City.ToString()));
            }
            builder.Append(new string('-', 50));
            builder.Append(string.Format("\nВы уверены, что хотите добавить : \nОбласть : {0}\nГород : {1}", selectedRgion, NewCityName));

            return builder.ToString();
        }
        
        #endregion

        #region void

        /// <summary>
        /// Нажатие клавиши
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
                    if (!RegionChecked)
                    {
                        MessageBox.Show(Dobby.GetErrorText(9), Dobby.GetMessageBoxTitle(1), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (IfCityExist())
                        {
                            DialogResult result = MessageBox.Show(GetExistCityInfo(), Dobby.GetMessageBoxTitle(3), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                ReturnData();
                                MessageBox.Show(Dobby.GetDialogText(1), Dobby.GetMessageBoxTitle(3));
                            }
                        }
                        else
                        {
                            ReturnData();
                            MessageBox.Show(Dobby.GetDialogText(1), Dobby.GetMessageBoxTitle(3));
                        }
                    }
                    break;
                default:
                    Close();
                    break;
            }
        }

        /// <summary>
        /// Заполнить КомбоБокс
        /// </summary>
        private void FillComboBox()
        {
            var q = (from row in RegionCityData.AsEnumerable()
                     select row.Field<string>("region")).Distinct().ToList();

            comboBox1.Items.Add("            ------ <Выберите элемент> ------");
            foreach (var item in q)
            {
                comboBox1.Items.Add(item.ToString());
            }

            if (RegionIn != null)
            {
                if (comboBox1.Items.Contains(RegionIn))
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(RegionIn);
                }
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCity_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Возвращаем данные
        /// </summary>
        private void ReturnData()
        {
            if (textBox1.Text.Length > 0)
            {
                newCity = new NewCityToDB(selectedRgion, NewCityName);
                GetNewCityName(newCity);
                Close();
            }
        }

        /// <summary>
        /// Изменение данных в КомбоБоксе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            if (box.SelectedIndex == 0)
            {
                RegionChecked = false;
                selectedRgion = string.Empty;
            }
            else
            {
                RegionChecked = true;
                selectedRgion = box.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Завершение редактирования наименования города
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (!Char.IsUpper(Convert.ToChar(textBox1.Text.Substring(0, 1))))
            {
                this.textBox1.Text = string.Format("{0}{1}", textBox1.Text.Substring(0, 1).ToUpper(), textBox1.Text.Substring(1, textBox1.Text.Length - 1));
            }
            NewCityName = textBox1.Text;
        }
        #endregion

        #endregion
    }
}
