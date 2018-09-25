using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using GeoFilials.Forms;

namespace GeoFilials.Scripts
{
    class SQL
    {
        #region Поля Класса
        private const string connectionString = "";
        private DataTable Data;
        private Dictionary<string, string> ServerDataBase = new Dictionary<string, string> { { "Server", "X27_test" }, { "DataBase", "marketing_test" } };
        #endregion

        #region Свойства
        //public string User
        //{
        //    get
        //    {
        //        if (user != null)
        //        {
        //            return user;
        //        }
        //        else
        //        {
        //            return "Пользователь не опознан";
        //        }
        //    } }

        public DataTable GetData
        {
            get
            {
                if (Data != null)
                {
                    return Data;
                }
                else
                {
                    Data = new DataTable();
                    Data.Columns.Add("Ошибка", typeof(string));

                    Data.Rows.Add(new object[] { "Ошибка чтения данных" });
                    return Data;
                }
            } }
        #endregion

        #region Конструктор
        public SQL()
        {
        }
        #endregion

        #region Методы класса

        #region Bool
        /// <summary>
        /// Проверка, наличие филиала в БД
        /// </summary>
        /// <param name="City"></param>
        /// <param name="StreetType"></param>
        /// <param name="Street"></param>
        /// <param name="House"></param>
        /// <returns></returns>
        public bool IsFilialExist(string City, string StreetType, string Street, string House, string Name)
        {
            int rez = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand com = new SqlCommand(GetQerry("dbo", "geo_new_fil_if_exists"), con))
                {
                    con.Open();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter { ParameterName = "@city", Value = City ?? string.Empty });
                    com.Parameters.Add(new SqlParameter { ParameterName = "@streettype", Value = StreetType ?? string.Empty });
                    com.Parameters.Add(new SqlParameter { ParameterName = "@street", Value = Street ?? string.Empty });
                    com.Parameters.Add(new SqlParameter { ParameterName = "@house", Value = House ?? string.Empty });
                    com.Parameters.Add(new SqlParameter { ParameterName = "@name", Value = Name ?? string.Empty });
                    rez = (int)com.ExecuteScalar();
                    con.Close();
                }
            }
            return (rez == 0) ? false : true;
        }

        /// <summary>
        /// Проверка прав доступа пользователя
        /// </summary>
        /// <returns>bool</returns>
        public bool UserRights(CurrentUser user)
        {
            int Level_right;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                //using (SqlCommand com = new SqlCommand("Select isnull(max(level_right), 0) from users_for_my_program where prg_name = 'geo_filials_v5' and user_login = '" + User + "'", con))
                using (SqlCommand com = new SqlCommand("Select isnull(max(level_right), 0) from users_for_my_program where prg_name = 'geo_filials_v5' and user_login = '" + user.User + "'", con))
                {
                    Level_right = Convert.ToInt32(com.ExecuteScalar());
                }
                con.Close();
            }

            if (Level_right == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region DataTables
        /// <summary>
        /// Получить DataTable "Растояние между филиалами" (Все данные)
        /// </summary>
        /// <returns></returns>
        public DataTable GetDistanceBetweenFilials()
        {
            DataTable data = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter("EXEC X27_test.marketing_test.dbo.rastoyanie_mezdu_filialami", con))
                    {
                        sda.Fill(data);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                data = null;
            }
            return data;
        }

        /// <summary>
        /// Получить DataTable "Растояние между филиалами" (по типу бизнеса)
        /// </summary>
        /// <param name="bussinessType"></param>
        /// <returns></returns>
        public DataTable GetDistanceBetweenFilials(string bussinessType)
        {
            DataTable data = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand com = new SqlCommand("X27_test.marketing_test.dbo.rastoyanie_mezdu_filialami_by_business_type ", con))
                    {
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.Add(new SqlParameter { ParameterName = "@business_type", Value = bussinessType });
                        SqlDataAdapter adapter = new SqlDataAdapter(com);
                        adapter.Fill(data);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                data = null;
            }
            return data;
        }

        /// <summary>
        /// Получить DataTable "Задвоенные записи"
        /// </summary>
        /// <returns></returns>
        public DataTable GetDoubledRowsDataTable()
        {
            #region v1
            DataTable ReturnData = null;

            try
            {
                ReturnData = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM [MarketingTest].[dbo].[zadvoennie]", con))
                    {
                        sda.Fill(ReturnData);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, Dobby.GetMessageBoxTitle(1));
            }
            return ReturnData;
            #endregion
        }

        /// <summary>
        /// Получить DataTable "Логи по филиалу"
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public DataTable GetLogsDataTable(int rowId)
        {
            if (rowId > 0)
            {
                DataTable tmp = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(GetLoqsQuerry(rowId), con))
                    {
                        sda.Fill(tmp);
                    }
                    con.Close();
                }
                return tmp;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить DataTable "Пользователь, отображаемые поля"
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public DataTable GetUserVisibleColumnsData(string userLogin)
        {
            DataTable tmp = new DataTable();
            DataTable rez = new DataTable();

            rez.Columns.Add(new DataColumn("Название Поля", typeof(string)));
            rez.Columns.Add(new DataColumn("Видимость", typeof(bool)));

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand com = new SqlCommand("MarketingTest.dbo.GetUserVisibleColumns", con))
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter { ParameterName = "@UserLogin", Value = userLogin });

                    try
                    {
                        con.Open();
                        SqlDataReader reader = com.ExecuteReader();
                        tmp.Load(reader);
                        con.Close();
                    }
                    catch (Exception)
                    {
                        tmp = null;
                    }
                }
            }

            if (tmp != null)
            {
                foreach (DataRow row in tmp.Rows)
                {
                    bool st;
                    if ((int)row["VisibleStatusID"] == 1)
                        st = true;
                    else
                        st = false;
                    DataRow newRow = rez.NewRow();
                    newRow["Название Поля"] = row["ColumnName"];
                    newRow["Видимость"] = st;
                    rez.Rows.Add(newRow);
                }
            }
            return rez;
        }
        #endregion

        #region Int.... но не совсем

        /// <summary>
        /// Добавляем филиал в БД
        /// </summary>
        /// <param name="newFilial"></param>
        /// <returns>1 - Успешно / 0 - ошибка</returns>
        public int AddNewFilial(NewFilialToDB newFilial)
        {
            int rez = 0;
            if (newFilial.IsReadyToSend())
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand com = new SqlCommand(GetQerry("dbo", "dobavit_noviy_filial"), con))
                        {
                            con.Open();
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.Add(new SqlParameter { ParameterName = "@upr_compani", Value = newFilial.upr_company ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@name", Value = newFilial.name ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@streettype", Value = newFilial.streettype ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@street", Value = newFilial.street ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@house", Value = newFilial.house ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@city", Value = newFilial.city ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@region", Value = newFilial.region ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@working", Value = newFilial.working ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@format", Value = newFilial.format ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@count_kass", Value = newFilial.count_kass ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@areaall", Value = newFilial.areaall ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@trading_area", Value = newFilial.trading_area ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@year_opening", Value = newFilial.year_opening ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@date_opening", Value = newFilial.date_opening ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@note", Value = newFilial.note ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@comments", Value = newFilial.comments ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@c", Value = newFilial.c ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@d", Value = newFilial.d ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@polzovatel", Value = newFilial.User });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@filid", Value = newFilial.filid_in_base ?? string.Empty });
                            com.Parameters.Add(new SqlParameter { ParameterName = "@is_non_food", Value = newFilial.food ?? string.Empty });
                            com.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    rez = 1;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, Dobby.GetMessageBoxTitle(1), System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return rez;
        }

        /// <summary>
        /// Создать файл для карты (kml)
        /// </summary>
        /// <param name="index"></param>
        /// <returns>1 - Успешно / 0 - ошибка</returns>
        public int CreateMapFile(int index)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand com = new SqlCommand(GetMapFileQuerry(index), con))
                    {
                        SqlDataReader reader = com.ExecuteReader();
                        StreamWriter writer = new StreamWriter(new FileStream(Dobby.GetMapFileName(index), FileMode.Create, FileAccess.Write));
                        while (reader.Read())
                        {
                            writer.WriteLine(reader[1].ToString(), Encoding.UTF8);
                        }
                        writer.Close();
                        reader.Close();
                    }
                    con.Close();
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Редактируем филиал.
        /// </summary>
        /// <param name="filialToDB"></param>
        /// <param name="TableRowID"></param>
        /// <returns>1 - Успешно / 0 - ошибка</returns>
        public int UpdateFilial(NewFilialToDB filialToDB, int TableRowID)
        {
            int rez = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand com = new SqlCommand(GetQerry("dbo", "update_filial"), con))
                    {
                        con.Open();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.Add(new SqlParameter { ParameterName = "@upr_compani", Value = filialToDB.upr_company ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@name", Value = filialToDB.name ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@streettype", Value = filialToDB.streettype ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@street", Value = filialToDB.street ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@house", Value = filialToDB.house ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@city", Value = filialToDB.city ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@region", Value = filialToDB.region ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@working", Value = filialToDB.working ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@format", Value = filialToDB.format ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@count_kass", Value = filialToDB.count_kass ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@areaall", Value = filialToDB.areaall ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@trading_area", Value = filialToDB.trading_area ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@year_opening", Value = filialToDB.year_opening ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@date_opening", Value = filialToDB.date_opening ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@note", Value = filialToDB.note ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@comments", Value = filialToDB.comments ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@c", Value = filialToDB.c ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@d", Value = filialToDB.d ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@polzovatel", Value = filialToDB.User ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@id", Value = TableRowID.ToString() });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@filid", Value = filialToDB.filid_in_base ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@is_non_food", Value = filialToDB.food ?? string.Empty });
                        com.Parameters.Add(new SqlParameter { ParameterName = "@date_close", Value = filialToDB.Date_Close ?? string.Empty });
                        com.ExecuteNonQuery();
                        con.Close();
                    }
                }
                rez = 1;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, Dobby.GetMessageBoxTitle(2));
            }
            return rez;
        }

        #endregion

        #region String

        /// <summary>
        /// Получить строку запроса, для просмотра Логов по филиалу
        /// </summary>
        /// <param name="RowId"></param>
        /// <returns></returns>
        private string GetLoqsQuerry(int RowId)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ")
                .Append("table_id,username,case when typ=1 then 'create' when typ=2 then 'update' end type,field,fromvalue,tovalue,date ")
                .Append("FROM [x27_test].[Marketing_TEST].[dbo].[logs_for_my_program] ")
                .Append("WHERE table_id=")
                .Append(string.Format("{0} ", RowId))
                .Append("ORDER BY date");
            return builder.ToString();
        }

        /// <summary>
        /// Получить строку запроса, для создания файла с картой
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetMapFileQuerry(int index)
        {
            switch (index)
            {
                case 1:
                    return string.Format("EXEC {0}.{1}.dbo.sozdat_kml_dlya_filialov 'продовольственный'", ServerDataBase["Server"], ServerDataBase["DataBase"]);
                case 2:
                    return string.Format("EXEC {0}.{1}.dbo.sozdat_kml_dlya_filialov 'непродовольственный'", ServerDataBase["Server"], ServerDataBase["DataBase"]);
                case 3:
                    return string.Format("EXEC {0}.{1}.dbo.sozdat_kml_dlya_filialov_silpo", ServerDataBase["Server"], ServerDataBase["DataBase"]);
                case 4:
                    return string.Format("EXEC {0}.{1}.dbo.sozdat_kml_dlya_filialov 'аптека'", ServerDataBase["Server"], ServerDataBase["DataBase"]);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Получить строку запроса
        /// </summary>
        /// <param name="Schema">Схема</param>
        /// <param name="DataBaseObject">Таблица/Хранимая Процедура/...</param>
        /// <returns>Строка Запроса</returns>
        private string GetQerry(string Schema, string DataBaseObject)
        {
            return string.Format("{0}.{1}.{2}.{3}", ServerDataBase["Server"], ServerDataBase["DataBase"], Schema, DataBaseObject);
        }

        /// <summary>
        /// Получить строку запроса, для обновления статуса отображения поля 
        /// </summary>
        /// <param name="User">Пользователь</param>
        /// <param name="ColumnName">Наименование поля</param>
        /// <param name="Status">Статус</param>
        /// <returns></returns>
        private string GetQuerryStringForUpdateUserColumnsVisibleStatus(string User, string ColumnName, bool Status)
        {
            int status = (Status) ? 1 : 0;

            return string.Format(@"UPDATE [x27_test].[Marketing_TEST].[dbo].[GeoFilials_User_Visivble_Columns] SET VisibleStatusID = {0} WHERE UserLogin = '{1}' AND ColumnName = '{2}'", status, User, ColumnName);
        }

        #endregion

        #region Void

        /// <summary>
        /// Получить данные от SQL
        /// </summary>
        public void GetDataFromSQL()
        {
            var Filials = Task.Factory.StartNew(() => GetFilialsData());
            Task.WaitAll(Filials);
        }

        /// <summary>
        /// Получить данные по филиалам 
        /// </summary>
        private void GetFilialsData()
        {
            Data = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT DISTINCT * FROM geo_base_new", con))
                {
                    sda.Fill(Data);
                }
                con.Close();
            }
        }

        /// <summary>
        /// Изменить "Видимость поля"
        /// </summary>
        /// <param name="user"></param>
        public void SetUserVisibleColumns(CurrentUser user)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (DataRow row in user.UserVisibleColumnSettings.Rows)
                {
                    using (SqlCommand com = new SqlCommand(GetQuerryStringForUpdateUserColumnsVisibleStatus(user.User, row["Название Поля"].ToString(), (bool)row["Видимость"]), con))
                    {
                        com.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
        }
        #endregion

        #endregion

    }
}
