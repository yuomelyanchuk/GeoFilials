using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Reflection;

namespace GeoFilials.Forms
{
    public struct NewFilialToDB
    {
        public string upr_company { get; set;} // *
        public string name { get; set; } // *
        public string region { get; set; } // *
        public string city { get; set; } // *
        public string streettype { get; set; } // *
        public string street { get; set; } // *
        public string house { get; set; } // *
        public string working { get; set; } // *
        public string format { get; set; } // *
        public string count_kass { get; set; }
        public string areaall { get; set; }
        public string trading_area { get; set; }
        public string year_opening { get; set; }
        public string date_opening { get; set; }
        public string note { get; set; }
        public string comments { get; set; }
        public string c { get; set; }
        public string d { get; set; }
        public string filid_in_base { get; set; }
        public string food { get; set; } // *
        public string User { get { return WindowsIdentity.GetCurrent().Name.ToString(); } }
        public string Date_Close { get; set; }

        public bool IsReadyToSend()
        {
            if (CheckValue(this.upr_company) && CheckValue(this.name) && CheckValue(this.region) && CheckValue(this.city) && CheckValue(this.streettype) && CheckValue(this.street) && CheckValue(this.working) && CheckValue(this.format) && CheckValue(this.food))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckValue(string data)
        {
            return (data != null && data.Replace(" ", string.Empty).Length > 0) ? true : false;
        }
        private bool CheckValue(int data)
        {
            return (data > 0) ? true : false;
        }
        private bool CheckValue(float data)
        {
            return (data > 0) ? true : false;
        }

        public string GetString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append((upr_company.Length > 0 && upr_company != null) ? string.Format("<upr_company = {0}>\n", upr_company.ToString()) : string.Empty);
            builder.Append((name.Length > 0 && name != null) ? string.Format("<name = {0}>\n", name.ToString()) : string.Empty);
            builder.Append((region.Length > 0 && region != null) ? string.Format("<region = {0}>\n", region.ToString()) : string.Empty);
            builder.Append((city.Length > 0 && city != null) ? string.Format("<city = {0}>\n", city.ToString()) : string.Empty);
            builder.Append((streettype.Length > 0 && streettype != null) ? string.Format("<streettype = {0}>\n", streettype.ToString()) : string.Empty);
            builder.Append((street.Length > 0 && street != null) ? string.Format("<street = {0}>\n", street.ToString()) : string.Empty);
            builder.Append((house.Length > 0 && house != null) ? string.Format("<house = {0}>\n", house.ToString()) : string.Empty);
            builder.Append((working.Length > 0 && working != null) ? string.Format("<working = {0}>\n", working.ToString()) : string.Empty);
            builder.Append((format.Length > 0 && format != null) ? string.Format("<format = {0}>\n", format.ToString()) : string.Empty);
            builder.Append((count_kass.Length > 0 && count_kass != null) ? string.Format("<count_kass = {0}>\n", count_kass.ToString()) : string.Empty);
            builder.Append((areaall.Length > 0 && areaall != null) ? string.Format("<areaall = {0}>\n", areaall.ToString()) : string.Empty);
            builder.Append((trading_area.Length > 0 && trading_area != null) ? string.Format("<trading_area = {0}>\n", trading_area.ToString()) : string.Empty);
            builder.Append((year_opening.Length > 0 && year_opening != null) ? string.Format("<year_opening = {0}>\n", year_opening.ToString()) : string.Empty);
            builder.Append((date_opening != null) ? string.Format("<date_opening = {0}>\n", date_opening.ToString()) : string.Empty);
            builder.Append((note != null) ? string.Format("<note = {0}>\n", note.ToString()) : string.Empty);
            builder.Append((comments != null) ? string.Format("<comments = {0}>\n", comments.ToString()) : string.Empty);
            builder.Append((c != null) ? string.Format("<c = {0}>\n", c.ToString()) : string.Empty);
            builder.Append((d != null) ? string.Format("<d = {0}>\n", d.ToString()) : string.Empty);
            builder.Append((filid_in_base.Length > 0 && filid_in_base != null) ? string.Format("<filid_in_base = {0}>\n", filid_in_base.ToString()) : string.Empty);
            builder.Append((food != null) ? string.Format("<food = {0}>\n", food.ToString()) : string.Empty);
            builder.Append((User.Length > 0) ? string.Format("<User = {0}>\n", User.ToString()) : string.Empty);

            return builder.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            Type type = typeof(NewFilialToDB);
            List<PropertyInfo> properties = type.GetProperties(BindingFlags.Public).ToList();

            foreach (var item in properties)
            {
                if (CheckValue(item.ToString()))
                {
                    builder.Append(string.Format("<{0} = {1}>", item.Name, item.ToString()));
                }
            }
            return builder.ToString();
        }
    }
}

