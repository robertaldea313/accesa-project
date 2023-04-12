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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace accesa_project
{
    public partial class QuestTab : Form
    {
        SqlConnection myCon = new SqlConnection();
        DataSet dsQuests;

        int userID;
        int questID;
        int ownerID;

        public QuestTab(int userID,int ID, int ownerID)
        {
            questID = ID;
            this.userID = userID;
            this.ownerID = ownerID;
            InitializeComponent();

            myCon.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\II hw\\accesa project\\accesa project\\Database2.mdf\";Integrated Security=True";
            myCon.Open();

            dsQuests = new DataSet();
            SqlDataAdapter daQuests = new SqlDataAdapter("SELECT * FROM Quests", myCon);
            daQuests.Fill(dsQuests, "Quests");

            foreach (DataRow dr in dsQuests.Tables[0].Rows)
            {
                if (questID.ToString() == dr[0].ToString())
                {
                    textBox1.Text = dr[3].ToString();
                    title.Text = dr[1].ToString();
                    textBox3.Text = dr[2].ToString();
                }
            }
            myCon.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myCon.Open();

            SqlCommand sql_cmnd = new SqlCommand("Add_Solution", myCon);
            sql_cmnd.CommandType = CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@SolutionDesc", SqlDbType.Text).Value = textBox2.Text;
            sql_cmnd.Parameters.AddWithValue("@Solution", SqlDbType.Text).Value = textBox1.Text;
            sql_cmnd.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = userID;
            sql_cmnd.Parameters.AddWithValue("@QuestID", SqlDbType.Int).Value = questID;
            sql_cmnd.Parameters.AddWithValue("@OwnerID", SqlDbType.Int).Value = ownerID;

            sql_cmnd.ExecuteNonQuery();

            myCon.Close();
            this.Close();
        }
    }
}
