using Microsoft.Office.Interop.Excel;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace GeoFilials.Scripts
{
    class Excel
    {
        public void ExportDataToExcel(System.Data.DataTable dt)
        {
            Application App = new Application();
            App.Visible = true;
            App.Workbooks.Add();
            _Worksheet MySheet = (Worksheet)App.ActiveSheet;

            object[,] dannie = new object[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j == 6)
                    {
                        dannie[i, j] = "'" + dt.Rows[i].ItemArray[j];
                    }
                    else
                    {
                        dannie[i, j] = dt.Rows[i].ItemArray[j];
                    }
                }

            }
            string[,] shapka = new string[1, dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                shapka[0, i] = dt.Columns[i].Caption;
            }

            Nazvanie_Yecheyki naz = new Nazvanie_Yecheyki();
            MySheet.get_Range("A1", naz.Name_yacheiki(dt.Columns.Count) + "1").Value2 = shapka;
            MySheet.get_Range("A2", naz.Name_yacheiki(dt.Columns.Count) + (dt.Rows.Count + 1).ToString()).Value2 = dannie;
            MySheet.get_Range("A1", naz.Name_yacheiki(dt.Columns.Count) + "1").Borders.Weight = XlBorderWeight.xlThin;

            App.Visible = true;
            App.UserControl = true;
        }

        public void ExportDataToExcel(System.Data.DataTable dt, int Param)
        {
            Application App = new Application();
            App.Visible = true;
            App.Workbooks.Add();
            _Worksheet MySheet = (Worksheet)App.ActiveSheet;
            Range range = MySheet.get_Range("D1").EntireColumn;
            range.NumberFormat = "@";
            

            object[,] dannie = new object[dt.Rows.Count, dt.Columns.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j == 6)
                    {
                        dannie[i, j] = "'" + dt.Rows[i].ItemArray[j];
                    }
                    else
                    {
                        dannie[i, j] = dt.Rows[i].ItemArray[j];
                    }
                }

            }
            string[,] shapka = new string[1, dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                shapka[0, i] = dt.Columns[i].Caption;
            }

            Nazvanie_Yecheyki naz = new Nazvanie_Yecheyki();
            MySheet.get_Range("A1", naz.Name_yacheiki(dt.Columns.Count) + "1").Value2 = shapka;
            MySheet.get_Range("A2", naz.Name_yacheiki(dt.Columns.Count) + (dt.Rows.Count + 1).ToString()).Value2 = dannie;
            MySheet.get_Range("A1", naz.Name_yacheiki(dt.Columns.Count) + "1").Borders.Weight = XlBorderWeight.xlThin;

            App.Visible = true;
            App.UserControl = true;
        }
    }
}
