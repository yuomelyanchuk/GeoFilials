using System.Data;
using System.Security.Principal;
using GeoFilials.Scripts;

namespace GeoFilials.Scripts
{
    public class CurrentUser
    {
        private string user;
        private DataTable uvcs;
        private bool showAllColumns;

        public string User { get { return user; } }
        public DataTable UserVisibleColumnSettings { get { return uvcs; } }
        public bool IsAllColumnsVisible { get { return showAllColumns; } }

        public CurrentUser()
        {
            user = WindowsIdentity.GetCurrent().Name.ToString();
            this.uvcs = new SQL().GetUserVisibleColumnsData(this.user);
            this.showAllColumns = IsAllColumnsShowed();
        }

        private bool IsAllColumnsShowed()
        {
            int index = 0;

            foreach (DataRow row in this.uvcs.Rows)
            {
                if (row["Видимость"].ToString() == "False")
                {
                    index++;
                }
            }

            if (index == 0)
                return true;
            else
                return false;
        }

        public void ChangeVisibleColumns(DataTable NewColumnsVisibleSettings)
        {
            this.uvcs = NewColumnsVisibleSettings;
        }
    }
}
