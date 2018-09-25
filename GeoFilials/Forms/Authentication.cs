using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoFilials.Scripts;

namespace GeoFilials.Forms
{
    public partial class Authentication : Form
    {
        public Authentication()
        {
            SQL sQL = new SQL();
            CurrentUser currentUser = new CurrentUser();

            InitializeComponent();
            if (!sQL.UserRights(currentUser))
            {
                //AccessDenied(sQL.User);
                AccessDenied(currentUser.User);
            }
            else
            {
                //Form mw = new MainWindow(sQL.User);
                Form mw = new MainWindow(currentUser);
                this.Hide();
                mw.ShowDialog();
            }
        }

        private void AccessDenied(string user)
        {
            foreach (var item in this.Controls)
            {
                Label label;

                if ((item as Label)!= null)
                {
                    label = (Label)item;
                    label.Visible = true;
                }
            }

            this.label3.Text = user;
            label3.Left = this.Width / 2 - label3.Width / 2;
            this.button1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void CloseForm()
        {
            this.Hide();
            System.Environment.Exit(0);
        }

        private void Authentication_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseForm();
        }
    }
}
