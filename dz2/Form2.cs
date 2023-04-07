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

namespace dz2
{
    public partial class Form2 : Form
    {
        Class1 dataBase = new Class1();
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();
            var name = textBox1.Text;
            var number = textBox2.Text;
            var addQuery = $"insert into contacts (contact_name,contact_number) values ('{name}','{number}')";
            var command = new SqlCommand(addQuery, dataBase.getConnection());
            command.ExecuteNonQuery();
            this.Hide();

            dataBase.closeConnection();
        }
    }
}
