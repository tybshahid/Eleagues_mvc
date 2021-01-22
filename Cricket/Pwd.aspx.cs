using Cricket.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cricket.Utilities
{
    public partial class Pwd : System.Web.UI.Page
    {
        private CricketEntities db;
        public Pwd()
        {
            db = new CricketEntities();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<PlayersMaster> lst = db.PlayersMaster.Where(p => p.RecordID != 1).OrderBy(p => p.Name).ToList();

                ddlPlayer.DataValueField = "RecordID";
                ddlPlayer.DataTextField = "Name";
                ddlPlayer.DataSource = lst;
                ddlPlayer.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // validate admin password first
            string enteredAdminPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(txtAdminPassword.Text, "MD5");

            if (enteredAdminPwd == ConfigurationManager.AppSettings["AdminPwd"].ToString())
            {
                long _RecordID = long.Parse(ddlPlayer.SelectedItem.Value);
                PlayersMaster objPlayersExisting = db.PlayersMaster.FirstOrDefault(p => p.RecordID == _RecordID);
                if (objPlayersExisting != null)
                {
                    objPlayersExisting.UserID = txtUserName.Text;
                    objPlayersExisting.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPassword.Text, "MD5");

                    db.SaveChanges();

                    divSetPassword.Visible = false;
                    divPasswordSet.Visible = true;
                }
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            // validate admin password first
            lblPwdHash.Text = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPlainPwd.Text, "MD5");
        }
    }
}