using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace accesa_project
{
    public partial class CreateQuest : Form
    {
        SqlConnection myCon = new SqlConnection();
        int userID;
        DataSet user;
        public CreateQuest(int ID,DataSet user)
        {
            InitializeComponent();
            userID = ID;
            this.user = user;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(DataRow dr in user.Tables[0].Rows)
            {
                if (dr[0].ToString()==userID.ToString())
                {
                    if (Convert.ToInt32(dr[6].ToString())>= Convert.ToInt32(textBox4.Text))
                    {
                        myCon.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\II hw\\accesa project\\accesa project\\Database2.mdf\";Integrated Security=True";
                        myCon.Open();

                        SqlCommand sql_cmnd = new SqlCommand("Add_Quest", myCon);
                        sql_cmnd.CommandType = CommandType.StoredProcedure;
                        sql_cmnd.Parameters.AddWithValue("@QuestTitle", SqlDbType.Text).Value = textBox1.Text;
                        sql_cmnd.Parameters.AddWithValue("@QuestDesc", SqlDbType.Text).Value = textBox2.Text;
                        sql_cmnd.Parameters.AddWithValue("@Task", SqlDbType.Text).Value = textBox3.Text;
                        sql_cmnd.Parameters.AddWithValue("@QuestReward", SqlDbType.Int).Value = Convert.ToInt32(textBox4.Text);
                        sql_cmnd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = userID;


                        sql_cmnd.ExecuteNonQuery();

                        SqlCommand sql_cmnd2 = new SqlCommand("Modify_Tokens", myCon);
                        sql_cmnd2.CommandType = System.Data.CommandType.StoredProcedure;
                        sql_cmnd2.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = userID;
                        sql_cmnd2.Parameters.AddWithValue("@Tokens", SqlDbType.Int).Value = Convert.ToInt32(dr[6].ToString()) - Convert.ToInt32(textBox4.Text);
                        sql_cmnd2.Parameters.AddWithValue("@TotalTokens", SqlDbType.Int).Value = Convert.ToInt32(dr[7].ToString());

                        sql_cmnd2.ExecuteNonQuery();

                        myCon.Close();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Not enough tokens!");
                    }
                }
            }
        }
    }
}
