﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
namespace EBookViewver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string isconnected = "0";
        string ismacaddresschanged = "0";
        string previousmacaddress = "0";
        string usercanuse = "0";
        string idforclosed = "0";
        private void Form1_Shown(object sender, EventArgs e)
        {
            //AutoScroll = true;
            try
            {
                string txtuser;
                string txtpassword;
                try
                {
                    System.IO.StreamReader logfile = new System.IO.StreamReader("logwt.txt");
                    txtuser = logfile.ReadLine();
                    txtpassword = logfile.ReadLine();
                    logfile.Close();
                }
                catch
                {
                    txtuser = Microsoft.VisualBasic.Interaction.InputBox("User?", "Enter your user name?", "");
                    txtpassword = Microsoft.VisualBasic.Interaction.InputBox("Password?", "Enter your password?", "");
                    using (System.IO.StreamWriter createdfile = System.IO.File.AppendText("logwt.txt"))
                    {
                        createdfile.WriteLine(txtuser);
                        createdfile.WriteLine(txtpassword);
                    }
                }
                string myConnection = "datasource=sql7.freesqldatabase.com;port=3306;username=sql7298466;password=9QF4YIgrcv";
                MySqlConnection myConn = new MySqlConnection(myConnection);
                MySqlDataAdapter myDataAdapter = new MySqlDataAdapter();
                myDataAdapter.SelectCommand = new MySqlCommand(" select * from sql7298466.authentification where users='" + txtuser.ToString() + "' and passwords='" + txtpassword.ToString() + "';", myConn);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(myDataAdapter);
                myConn.Open();
                DataSet ds = new DataSet("accounts");
                myDataAdapter.Fill(ds);
                MessageBox.Show("connected");
                string id = retrieveRow(ds);
                if (id != "0")
                {
                    MySqlCommand cmd;
                    if (isconnected == "0")
                    {
                        cmd = new MySqlCommand(" update sql7298466.authentification set connections=1 where id=" + id + ";", myConn);
                    }
                    else
                    {
                        Int64 alreadyconnected = Convert.ToInt64(isconnected) + 1;
                        cmd = new MySqlCommand(" update sql7298466.authentification set connections=" + alreadyconnected.ToString() + " where id=" + id + ";", myConn);
                    }
                    cmd.ExecuteNonQuery();
                    String firstMacAddress = System.Net.NetworkInformation.NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault();
                    cmd = new MySqlCommand(" update sql7298466.authentification set ips='" + firstMacAddress + "' where id=" + id + ";", myConn);
                    cmd.ExecuteNonQuery();
                    if (previousmacaddress != firstMacAddress & previousmacaddress != "0")
                    {
                        Int64 macaddresschanged = Convert.ToInt64(ismacaddresschanged) + 1;
                        cmd = new MySqlCommand(" update sql7298466.authentification set ipschanges=" + macaddresschanged.ToString() + " where id=" + id + ";", myConn);
                        cmd.ExecuteNonQuery();
                    }
                    if (usercanuse == "0" & idforclosed != "0")
                    {
                        AutoScroll = true;
                    }
                    else
                    {
                        cmd = new MySqlCommand(" update sql7298466.authentification set allows=" + usercanuse.ToString() + " where id=" + id + ";", myConn);
                        cmd.ExecuteNonQuery();
                        AutoScroll = false;
                    }
                }
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                AutoScroll = false;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private string retrieveRow(DataSet dataSet)
        {
            // For each table in the DataSet, print the row values.
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        MessageBox.Show(row[1].ToString());
                        isconnected = row[3].ToString();
                        if (row[6].ToString() == "1")
                        {
                            return "0";
                        }
                        previousmacaddress = row[4].ToString();
                        ismacaddresschanged = row[5].ToString();
                        if (Convert.ToInt64(row[3].ToString()) <= 10 & Convert.ToInt64(row[5].ToString()) <= 5)
                        {
                            usercanuse = "0";
                        }
                        else
                        {
                            usercanuse = "1";
                        }
                        idforclosed = row[0].ToString();
                        return row[0].ToString();
                    }
                }
            }
            return "0";
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (usercanuse == "0")
            {
                try
                {
                    string myConnection = "datasource=sql7.freesqldatabase.com;port=3306;username=sql7298466;password=9QF4YIgrcv";
                    MySqlConnection myConn = new MySqlConnection(myConnection);
                    MySqlCommand cmd;
                    myConn.Open();
                    cmd = new MySqlCommand(" update sql7298466.authentification set connections=0 where id=" + idforclosed + ";", myConn);
                    cmd.ExecuteNonQuery();
                    myConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "PrintScreen")
            {
                Clipboard.Clear();
                return;
            }
        }
    }
}