using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace accesa_project
{
    public partial class Form1 : Form
    {
        SqlConnection myCon = new SqlConnection();
        DataSet dsUsers;
        DataSet dsQuests;
        DataSet dsSolution;

        int userID;
        public Form1()
        {
            InitializeComponent();
            panel3.Hide();
            myCon.ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Database2.mdf;Integrated Security=True";

            Update();                   

        }

        public void Update()    //updating the elements for both the Quest List and Solution List
        {
            myQuestList.Items.Clear();  //Quest list getting cleared
            listQuests.Items.Clear();   //Solution list which holds the entries to quests
            textBox1.Clear();

            myCon.Open();
            dsUsers = new DataSet();
            SqlDataAdapter daUsers = new SqlDataAdapter("SELECT * FROM Users", myCon);
            daUsers.Fill(dsUsers, "Users");    //creating a local copy of of the Users table

            dsQuests = new DataSet();
            SqlDataAdapter daQuests = new SqlDataAdapter("SELECT * FROM Quests", myCon);
            daQuests.Fill(dsQuests, "Quests"); //local copy of the Quests table

            dsSolution = new DataSet();
            SqlDataAdapter daSolution = new SqlDataAdapter("SELECT * FROM Solution", myCon);
            daSolution.Fill(dsSolution, "Solution"); //local copy of the Solution table

            foreach (DataRow dr in dsQuests.Tables[0].Rows) //Adding elements to the Quest List
            {
                listQuests.Items.Add(dr[1].ToString() + "-for " + dr[4].ToString() + " tokens #" + dr[0].ToString());
            }

            foreach (DataRow dr in dsSolution.Tables[0].Rows) //Adding elements to the Solution List
            {
                if (dr[5].ToString() == userID.ToString()) // but only for the logged in user's Quest
                {
                    DataRow userRow = dsUsers.Tables[0].AsEnumerable().SingleOrDefault(r => r.Field<int>("UserID") == dr.Field<int>("UserID"));
                    myQuestList.Items.Add(userRow[1].ToString()+"'s solution #" + dr[0].ToString());
                }
            }

            int count = 0;
            DataView sortedUsers = dsUsers.Tables[0].DefaultView;  //sorting all users by tokens earned
            sortedUsers.Sort = "TotalTokens DESC";
            DataTable users = sortedUsers.ToTable();
            if (dsUsers.Tables[0].Rows.Count < 10)
            {
                count = dsUsers.Tables[0].Rows.Count; //adjusting the top 10 to a shorter list
                                                      //if there are less than 10 users
            }
            else 
                count = 10;
            for(int i=0; i<count;i++)
            {
                DataRow user = users.Rows[i]; //creating the leaderboards
                textBox1.AppendText(i+1+"." + user[1].ToString() + user[2].ToString() + "---" + user[7].ToString() +" Tokens \r\n");
            }

            DataRow userRow2 = dsUsers.Tables[0].AsEnumerable().SingleOrDefault(r => r.Field<int>("UserID") == userID);
            if (userRow2 != null)
            {
                label11.Text = userRow2[1].ToString() + " " + userRow2[1].ToString();
                label10.Text = userRow2[4].ToString();
                label12.Text = userRow2[6].ToString() + " tokens/" + userRow2[7].ToString() + " Total Tokens";
            }   

            myCon.Close();
        }

        //creating a secondary window where users can respond to the quest
        //also parsing the value of the Quest ID
        public void openQuestWindow(int ID)
        {
            int ownerID = 0;
            foreach (DataRow dr in dsQuests.Tables[0].Rows)
            {
                if (ID.ToString() == dr[0].ToString())
                {
                    ownerID = Convert.ToInt32(dr[5].ToString());
                }
            }


            QuestTab secondaryWindow = new QuestTab(userID,ID,ownerID);
            secondaryWindow.ShowDialog();
        }


        //creating a secondary window to create a new quest
        public void openQuestCreator()
        {
            CreateQuest secondaryWindow = new CreateQuest(userID,dsUsers);
            secondaryWindow.ShowDialog();
        }

        //a secondary window where you can verify the solution to your tasks
        public void openSolutionCheck(int ID)
        {
            CheckSolution secondaryWindow = new CheckSolution(ID,userID);
            secondaryWindow.ShowDialog();
        }

        //login functionality
        private void Login_Click(object sender, EventArgs e)
        {
            foreach (DataRow dr in dsUsers.Tables[0].Rows)
                {
                    if (username.Text == dr[4].ToString() && password.Text == dr[5].ToString())
                    {
                        userID = Convert.ToInt32(dr[0].ToString());
                        panel3.Show();
                        panel2.Hide();
                        Update();
                    }
            }
        }


        //selecting which quest to solve from the given list
        private void listQuests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listQuests.Text.Length > 0)
            {
                int ID = Convert.ToInt32(listQuests.Text.Split('#')[1]);
                openQuestWindow(ID);
            }
        }

        
        //selecting from the different features
        private void sideBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (sideBar.Text)  //displaying only the relevant information for each case;
            {
                case "QUESTS":
                    {
                        listQuests.Show();
                        myQuest.Hide();
                        rewards.Hide();
                        leaderboards.Hide();
                        profile.Hide();
                        break;
                    }
                case "MY QUEST":
                    {
                        myQuestList.Hide();
                        foreach (DataRow dr in dsQuests.Tables[0].Rows)
                        {
                            if (Convert.ToInt32(dr[5].ToString()) == userID)
                            {
                                myQuestList.Show();
                            }
                        }

                        listQuests.Hide();
                        myQuest.Show();
                        rewards.Hide();
                        leaderboards.Hide();
                        profile.Hide();
                        break;
                    }
                case "REWARDS":
                    {
                        listQuests.Hide();
                        myQuest.Hide();
                        rewards.Show();
                        leaderboards.Hide();
                        profile.Hide();
                        break;
                    }
                case "LEADERBOARDS":
                    {
                        listQuests.Hide();
                        myQuest.Hide();
                        rewards.Hide();
                        leaderboards.Show();
                        profile.Hide();
                        break;
                    }
                case "PROFILE":
                    {
                        listQuests.Hide();
                        myQuest.Hide();
                        rewards.Hide();
                        leaderboards.Hide();
                        profile.Show();
                        break;
                    }
                case "SIGN OUT":
                    {
                        panel2.Show();  //going back to the login page
                        panel3.Hide();
                        break;
                    }
                default:
                    break;
            }
            Update();
        }


        //checking which solution was selected
        private void myQuestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myQuestList.Text.Length > 0)
            {
                int ID = Convert.ToInt32(myQuestList.Text.Split('#')[1]);
                openSolutionCheck(ID);
            }
        }

        //creating a quest
        private void button1_Click(object sender, EventArgs e)
        {
            openQuestCreator();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
