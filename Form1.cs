using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;

namespace experiment5
{
   

    public partial class Form1 : Form
    {
        OleDbConnection connection;
        OleDbDataAdapter dataAdapter1;
        OleDbDataAdapter dataAdapter2;
        OleDbDataAdapter dataAdapter3;
        DataTable DataTable1;
        DataTable DataTable2;
        DataTable DataTable3;
        OleDbCommand cmd;
        OleDbCommand cmd2;
        OleDbCommand cmd3;
        TreeNode t_list = new TreeNode("");
        TreeNode subTreeNode = new TreeNode("");
        ArrayList sub_list = new ArrayList();
        TreeNode treeNode;
      
        
          
        public Form1()
        {
            makeExchange();
            makeUpDatabase();
            make_io();
          
            InitializeComponent();
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
                     
        }
        public void make_io() 
        {
            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            string createQuery3 = "CREATE TABLE Io(" + " ID INTEGER NOT NULL, "+"IO_TYPE VARCHAR(10) NOT NULL,"+"IO_VALUE VARCHAR(10) NOT NULL)";
            cmd3 = new OleDbCommand(createQuery3, connection);
            try
            {
                connection.Open();
                cmd3.ExecuteNonQuery();
                MessageBox.Show("IO table oluşturuldu");
            }

            catch (Exception)
            {
                MessageBox.Show("IO table olusturulamadi.Ya da daha önceden olusturulmus!");

            }
            connection.Close();
            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            connection.Open();

            dataAdapter3 = new OleDbDataAdapter();
            DataTable3 = new DataTable();
            dataAdapter3.SelectCommand = connection.CreateCommand();
            dataAdapter3.SelectCommand.CommandText = "Select * from Io\n";
            dataAdapter3.SelectCommand.CommandText += "where Id=0";
            DataTable3.Clear();

            dataAdapter3.Fill(DataTable3);
            dataAdapter3.InsertCommand = connection.CreateCommand();

            dataAdapter3.InsertCommand.CommandText = "insert into Io (ID , IO_TYPE ,IO_VALUE) values (?,?,?)";
            dataAdapter3.InsertCommand.Parameters.Add("ID", OleDbType.Numeric, 50, "ID");
            dataAdapter3.InsertCommand.Parameters.Add("IO_TYPE ", OleDbType.Char, 10, "IO_TYPE");
            dataAdapter3.InsertCommand.Parameters.Add("IO_VALUE", OleDbType.Char, 10, "IO_VALUE");
          
        }
        public void makeExchange() 
        {
            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            string createQuery2 = "CREATE TABLE Exchange(" + " ID INTEGER NOT NULL, " + " NAME VARCHAR(250) NOT NULL,"+"NUM INTEGER NOT NULL,"+ " CATEGORY VARCHAR(250) NOT NULL,"+ " SUBCATEGORY VARCHAR(250) NOT NULL,"+"MEANVALUE VARCHAR(20) NOT NULL,"+"STANDARTDEV95 VARCHAR(20))";
            cmd2 = new OleDbCommand(createQuery2, connection);
            try
            {
                connection.Open();
                cmd2.ExecuteNonQuery();
                MessageBox.Show("Exchange table olusturuldu");
            }

            catch (Exception)
            {
                MessageBox.Show("Exchange table olusturulamadi.Ya da daha önceden olusturulmus!");

            }
            connection.Close();
            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            connection.Open();

            dataAdapter2 = new OleDbDataAdapter();
            DataTable2 = new DataTable();
            dataAdapter2.SelectCommand = connection.CreateCommand();
            dataAdapter2.SelectCommand.CommandText = "Select * from Exchange\n";
            dataAdapter2.SelectCommand.CommandText += "where Id=0";
            DataTable2.Clear();

            dataAdapter2.Fill(DataTable2);
            dataAdapter2.InsertCommand = connection.CreateCommand();

            dataAdapter2.InsertCommand.CommandText = "insert into Exchange (ID , NAME ,NUM,CATEGORY,SUBCATEGORY ,MEANVALUE, STANDARTDEV95  ) values (?,?,?,?,?,?,?)";
            dataAdapter2.InsertCommand.Parameters.Add("ID", OleDbType.Numeric, 50, "ID");
            dataAdapter2.InsertCommand.Parameters.Add("NAME ", OleDbType.Char, 150, "NAME");
            dataAdapter2.InsertCommand.Parameters.Add("NUM", OleDbType.Numeric, 10, "NUM");
            dataAdapter2.InsertCommand.Parameters.Add("CATEGORY ", OleDbType.Char, 250, "CATEGORY");
            dataAdapter2.InsertCommand.Parameters.Add("SUBCATEGORY ", OleDbType.Char, 250, "SUBCATEGORY");
            dataAdapter2.InsertCommand.Parameters.Add("MEANVALUE", OleDbType.Char, 50, "MEANVALUE");
            dataAdapter2.InsertCommand.Parameters.Add("STANDARTDEV95", OleDbType.Char, 50, "STANDARTDEV95");

        }
        private void button1_Click(object sender, EventArgs e)
        {

            string pro_no = "";
            string dizin = "Process_infra_raw";
            string[] dizi;
            XmlTextReader reader;
            foreach (string dosya in System.IO.Directory.GetFiles(dizin, "*.XML"))
            {
                dizi = dosya.Split('\\');
                dizin = dizi[0] + "\\" + dizi[1];
              
                reader = new XmlTextReader(dizin);//for reading xml files
                try
                {
                    while (reader.Read())//read all document
                    {
                        switch (reader.NodeType)//node found
                        {
                            case XmlNodeType.Element: // The node is an element.
                                if (reader.Name.Equals("dataset"))
                                    pro_no = reader.GetAttribute("number");

                                if (reader.Name.Equals("referenceFunction"))
                                {
                                    addDatabaseProcess(Int32.Parse(pro_no), reader.GetAttribute("name").ToString(), reader.GetAttribute("category").ToString(), reader.GetAttribute("subCategory").ToString(), reader.GetAttribute("localName").ToString(), reader.GetAttribute("amount").ToString(), reader.GetAttribute("unit").ToString(), reader.GetAttribute("generalComment").ToString(), reader.GetAttribute("infrastructureProcess").ToString(), reader.GetAttribute("infrastructureIncluded").ToString(), reader.GetAttribute("includedProcesses").ToString());
                                }
                                break;
                        }

                    }
                }
                catch (Exception)
                {
                    addDatabaseProcess(Int32.Parse(pro_no), "fff", "fff", "fff", "fff", "0", "fff", "fff", "fff", "fff", "fff");
                }

            }
            MessageBox.Show("Loaded successfully");

            makeTree();

        }
        public void makeUpDatabase()
        {
            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            string createQuery = "CREATE TABLE Process(" + " ID INTEGER PRIMARY KEY, " + " NAME VARCHAR(250) NOT NULL, " + " CATEGORY VARCHAR(250) NOT NULL, " + " SUBCATEGORY VARCHAR(250) NOT NULL, " + " LOCALNAME VARCHAR(250) NOT NULL, " + " AMOUNT INTEGER NOT NULL, " + " UNIT VARCHAR(250) NOT NULL, " + " GENERALCOMMENT MEMO NOT NULL, " + " INFRASTRUCTUREPROCESS VARCHAR(250) NOT NULL, " + " INFRASTRUCTUREINCLUDED VARCHAR(250) NOT NULL, " + " INCLUDEDPROCESSES VARCHAR(250) NOT NULL)";
            cmd = new OleDbCommand(createQuery , connection);
           
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Process table olusturuldu");
            }

            catch (Exception)
            {
                MessageBox.Show("Process table olusturulamadi.Ya da daha önceden olusturulmus!");

            }
            connection.Close();

            connection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;data source=eco.mdb");
            connection.Open();
            
            dataAdapter1 = new OleDbDataAdapter();
            DataTable1 = new DataTable();
            dataAdapter1.SelectCommand = connection.CreateCommand();
            dataAdapter1.SelectCommand.CommandText = "Select * from Process\n";
            dataAdapter1.SelectCommand.CommandText += "where Id=0";
            DataTable1.Clear();
            

            dataAdapter1.Fill(DataTable1);
            dataAdapter1.InsertCommand = connection.CreateCommand();
           
            dataAdapter1.InsertCommand.CommandText = "insert into Process (ID , NAME, CATEGORY ,SUBCATEGORY, LOCALNAME, AMOUNT, UNIT, GENERALCOMMENT,INFRASTRUCTUREPROCESS,INFRASTRUCTUREINCLUDED,INCLUDEDPROCESSES ) values (?,?,?,?,?,?,?,?,?,?,?)";
            dataAdapter1.InsertCommand.Parameters.Add("ID", OleDbType.Numeric, 50, "ID");
            dataAdapter1.InsertCommand.Parameters.Add("NAME ", OleDbType.Char, 150, "NAME");
            dataAdapter1.InsertCommand.Parameters.Add("CATEGORY", OleDbType.Char, 50, "CATEGORY");
            dataAdapter1.InsertCommand.Parameters.Add("SUBCATEGORY", OleDbType.Char, 50, "SUBCATEGORY");
            dataAdapter1.InsertCommand.Parameters.Add("LOCALNAME", OleDbType.Char, 50, "LOCALNAME");
            dataAdapter1.InsertCommand.Parameters.Add("AMOUNT", OleDbType.Numeric, 50, "AMOUNT");
            dataAdapter1.InsertCommand.Parameters.Add("UNIT", OleDbType.Char, 50, "UNIT");
            dataAdapter1.InsertCommand.Parameters.Add("GENERALCOMMENT", OleDbType.LongVarChar, 50, "GENERALCOMMENT");
            dataAdapter1.InsertCommand.Parameters.Add("INFRASTRUCTUREPROCESS", OleDbType.Char, 50, "INFRASTRUCTUREPROCESS");
            dataAdapter1.InsertCommand.Parameters.Add("INFRASTRUCTUREINCLUDED", OleDbType.Char, 50, "INFRASTRUCTUREINCLUDED");
            dataAdapter1.InsertCommand.Parameters.Add("INCLUDEDPROCESSES", OleDbType.Char, 50, "INCLUDEDPROCESSES");
        }


        public void addDatabaseProcess(int id, string name, string category, string subcategory, string localname, string amount, string unit, string generalcomment, string inf_process, string inf_included, string inc_processes)
        {
    
            DataRow yeni = DataTable1.NewRow();
            yeni[0] = id;
            yeni[1] = name;
            yeni[2] = category;
            yeni[3] = subcategory;
            yeni[4] = localname;
            yeni[5] = Int32.Parse(amount);
            yeni[6] = unit;
            yeni[7] = "generalcomment_";
            yeni[8] = inf_process;
            yeni[9] = inf_included;
            yeni[10] = "inc_processes_";

            DataTable1.Rows.Add(yeni);
            dataAdapter1.Update(DataTable1);
            DataTable1.AcceptChanges();
        }

       

        private void listView1_DoubleClick(object sender, System.EventArgs e) 
        {
            Form form2 = new Form();
            form2.Show();
            form2.Size = new Size(600,600);
            string id=listView1.FocusedItem.Text;
            string name=listView1.FocusedItem.SubItems[1].Text;
            string unit = listView1.FocusedItem.SubItems[2].Text;
            string type = listView1.FocusedItem.SubItems[3].Text;
            if (type == "U")
                type = "Unit";
            else if (type == "S")
                type = "System";
 

            TabControl tabControl1 = new TabControl();
            TabPage tabPageDocumentation = new TabPage("Documentation");
            TabPage tabPageIO = new TabPage("Input/Output");

            tabControl1.TabPages.Add(tabPageDocumentation);
            tabControl1.TabPages.Add(tabPageIO);
            tabControl1.Size = new Size(500,400); 
            tabControl1.Location = new Point(40, 40);
            TextBox tb1 = new TextBox();
            tb1.Text = type;
            form2.Controls.Add(tabControl1);
            tb1.Location = new Point(150, 15);
            
            Label l1 = new Label();
            l1.Text = "Process Type:";
            l1.Location = new Point(10,15);

            Label l2 = new Label();
            l2.Text = "Category:";
            l2.Location = new Point(10, 45);

           
            tabPageDocumentation.Controls.Add(tb1);
            tabPageDocumentation.Controls.Add(l1);
            tabPageDocumentation.Controls.Add(l2);


            

        }

        private void treeView1_AfterSelect(System.Object sender,System.Windows.Forms.TreeViewEventArgs e)
        {  

            string selected=treeView1.SelectedNode.Text;
            string[] data=null;
            ListViewItem lv=null;
            label2.Text = selected;
            listView1.Items.Clear();
            
            
            for (int i = 0; i < sub_list.Count; i++)
            {
                if (sub_list[i].Equals(selected)) 
                {
                   
                    data=t_list.Nodes[i].Nodes[0].ToString().Split(':','~');
                    lv = new ListViewItem(data[1]);
                    lv.SubItems.Add(data[2]);
                    lv.SubItems.Add(data[3]);
                    lv.SubItems.Add("U");
                    listView1.Items.AddRange(new ListViewItem[]{lv});
                    lv = new ListViewItem(data[1]);
                    lv.SubItems.Add(data[2]);
                    lv.SubItems.Add(data[3]);
                    lv.SubItems.Add("S");
                    listView1.Items.AddRange(new ListViewItem[] { lv });
 
                }

            }
            listView1.FullRowSelect = true;
            
           
        }

        public void makeTree()
        {
            string id = "", name = "", unit = "", category = "", subcategory = "";
            ArrayList list = new ArrayList();
            treeNode = new TreeNode("Click here to view categories");
            dataAdapter1.SelectCommand.CommandText = "Select ID,CATEGORY,SUBCATEGORY,NAME,UNIT from Process\n";
            DataTable1.Clear();
            dataAdapter1.Fill(DataTable1);
            
            DataRow veri = null;
            for (int i = 0; i < DataTable1.Rows.Count; i++)
            {
                veri = DataTable1.Rows[i];
                category = veri[2].ToString();
                subcategory = veri[3].ToString();
                id = veri[0].ToString();
                name = veri[1].ToString();
                unit = veri[6].ToString();
                id=id+"~"+name+"~"+unit;

               
                if (treeNode.Nodes.Find(category, false).Count() != 1)
                {
                    list.Add(category);
                    treeNode.Nodes.Add(category, category);
                    treeNode.Nodes[list.IndexOf(category)].Nodes.Add(subcategory, subcategory);
   
                }
                else
                {
                    if (treeNode.Nodes[list.IndexOf(category)].Nodes.Find(subcategory, true).Count() < 1)//there is a node with same name(subcategory)
                        treeNode.Nodes[list.IndexOf(category)].Nodes.Add(subcategory, subcategory);   
                }

              
                t_list.Nodes.Add(subcategory,subcategory);
                t_list.Nodes[i].Nodes.Add(id,id);
                sub_list.Add(subcategory);

            }
            treeView1.Nodes.Add(treeNode);
            this.Controls.Add(treeView1);
            
        }

        private void addDatabaseExchange(int pro_id, int ex_number, string ex_name,string ex_category,string ex_subcategory, string ex_meanvalue, string ex_sd95)
        {
            //MessageBox.Show(pro_id.ToString() + "\n" + ex_number.ToString() + "\n" + ex_name + "\n" + ex_category + "\n" + ex_subcategory + "\n" + ex_meanvalue + "\n" + ex_sd95);
 
            DataRow yeni = DataTable2.NewRow();
            yeni[0] = pro_id;
            yeni[1] = ex_name;
            yeni[2] = ex_number;
            yeni[3] = ex_category;
            yeni[4] = ex_subcategory;
            yeni[5] = ex_meanvalue;
            yeni[6] = ex_sd95;
         
           
            DataTable2.Rows.Add(yeni);
            dataAdapter2.Update(DataTable2);
            DataTable2.AcceptChanges();
        }

        private void addDatabaseIo(int id, string io_type, string io_value)
        {
           
            DataRow yeni = DataTable3.NewRow();
            yeni[0] = id;
            yeni[1] = io_type;
            yeni[2] = io_value;
          

            DataTable3.Rows.Add(yeni);
            dataAdapter3.Update(DataTable3);
            DataTable3.AcceptChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pro_no = "";
            string number = "";
            string i_o = "";
            string io_value ="";
            string dizin = "Process_infra_raw";
            string[] dizi;
            XmlTextReader reader;
            foreach (string dosya in System.IO.Directory.GetFiles(dizin, "*.XML"))
            {
                dizi = dosya.Split('\\');
                dizin = dizi[0] + "\\" + dizi[1];
                //addDatabase();
                reader = new XmlTextReader(dizin);//for reading xml files
                try
                {
                    while (reader.Read())//read all document
                    {
                        switch (reader.NodeType)//node found
                        {
                            case XmlNodeType.Element: // The node is an element.
                                if (reader.Name.Equals("dataset"))
                                {
                                    pro_no = reader.GetAttribute("number");
                                }
                             
                                
                                if (reader.Name.Equals("exchange"))
                                {
                                    number = reader.GetAttribute("number");
                                    
                                    addDatabaseExchange(Int32.Parse(pro_no), Int32.Parse(number), reader.GetAttribute("name").ToString(), reader.GetAttribute("category").ToString(), reader.GetAttribute("subCategory").ToString(), reader.GetAttribute("meanValue").ToString(), reader.GetAttribute("standardDeviation95").ToString());
                                  
                                }
                                if (reader.Name.Equals("outputGroup"))
                                {
                                  
                                    i_o = "o";
                                    io_value=reader.ReadString();
                                    addDatabaseIo(Int32.Parse(number), i_o ,io_value);
                                }
                                if (reader.Name.Equals("inputGroup"))
                                {
                                   
                                     i_o = "i";
                                     io_value = reader.ReadString();
                                     addDatabaseIo(Int32.Parse(number), i_o, io_value);
                                   
                                }
                              
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    addDatabaseExchange(Int32.Parse(pro_no), Int32.Parse(number), reader.GetAttribute("name").ToString(), reader.GetAttribute("category").ToString(),reader.GetAttribute("subCategory").ToString(),reader.GetAttribute("meanValue").ToString(), " ");
                }

            }
            MessageBox.Show("Loaded successfully");
        }





    }
}
