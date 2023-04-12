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
    public partial class CheckSolution : Form
    {
        int questID;
        int userID;
        SqlConnection myCon = new SqlConnection();
        DataSet dsSolution;
        int tokens;

        public CheckSolution(int ID, int userID)
        {
            InitializeComponent();
            this.questID= ID; 
            this.userID= userID;


            myCon.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\II hw\\accesa project\\accesa project\\Database2.mdf\";Integrated Security=True";
            myCon.Open();

            dsSolution = new DataSet();
            SqlDataAdapter daSolution = new SqlDataAdapter("SELECT * FROM Solution", myCon);
            daSolution.Fill(dsSolution, "Solution");

            foreach (DataRow dr in dsSolution.Tables[0].Rows)
            {
                if (ID.ToString() == dr[0].ToString())
                {
                    textBox1.Text = dr[1].ToString();
                    textBox3.Text = dr[2].ToString();
                    questID = Convert.ToInt32(dr[4].ToString());
                }
            }

            DataSet dsQuest = new DataSet();
            SqlDataAdapter daQuest= new SqlDataAdapter("SELECT * FROM Quests", myCon);
            daQuest.Fill(dsQuest, "Quests");

            foreach (DataRow dr in dsSolution.Tables[0].Rows)
            {
                if (questID.ToString() == dr[0].ToString())
                {
                    tokens = Convert.ToInt32(dr[4].ToString());
                }
            }

                myCon.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myCon.Open();

            SqlCommand sql_cmnd = new SqlCommand("Delete_Quest", myCon);
            sql_cmnd.CommandType = System.Data.CommandType.StoredProcedure;
            sql_cmnd.Parameters.AddWithValue("@QuestID", SqlDbType.Int).Value = questID;

            sql_cmnd.ExecuteNonQuery();

            foreach (DataRow dr in dsSolution.Tables[0].Rows)
            {
                if (dr[3].ToString() == userID.ToString() && dr[4].ToString()==questID.ToString())
                {
                    SqlCommand sql_cmnd2 = new SqlCommand("Modify_Tokens", myCon);
                    sql_cmnd2.CommandType = System.Data.CommandType.StoredProcedure;
                    sql_cmnd2.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = userID;
                    sql_cmnd2.Parameters.AddWithValue("@Tokens", SqlDbType.Int).Value = Convert.ToInt32(dr[6].ToString()) + tokens;
                    sql_cmnd2.Parameters.AddWithValue("@TotalTokens", SqlDbType.Int).Value = Convert.ToInt32(dr[7].ToString()) + tokens;

                    sql_cmnd2.ExecuteNonQuery();
                }
            }

            myCon.Close();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
