using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace dz2
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted

    }
    public partial class Form1 : Form
    {

        Class1 dataBase = new Class1();
        int selectedRow;
        private string result = "";

        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "ID");
            dataGridView1.Columns.Add("contact_name", "Имя");
            dataGridView1.Columns.Add("contuct_number", "Номер телефона");
            dataGridView1.Columns.Add("IsNew", String.Empty);
        }
        private void ReadSingleRow(DataGridView dqw, IDataRecord record)
        {
            dqw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), RowState.ModifiedNew);

        }
        private void RefreshDataGrid(DataGridView dqw)
        {
            dqw.Rows.Clear();
            string queryString = $"select * from contacts";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());
            dataBase.openConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ReadSingleRow(dqw, reader);
            }
            reader.Close();
            button4.Visible = true;
        }

        private void contacts_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
            textBox4.Visible = false;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                textBox4.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();

            }
        }
        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from contacts where concat (id,contact_name,contact_number) like '%" + textBox1.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[index].Visible = false;
            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[3].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[3].Value = RowState.Deleted;
        }
        private void Update()
        {
            try
            {
                dataBase.openConnection();
                for (int index = 0; index < dataGridView1.Rows.Count; index++)
                {
                    var rowState = (RowState)dataGridView1.Rows[index].Cells[3].Value;
                    if (rowState == RowState.Existed)
                        continue;

                    if (rowState == RowState.Deleted)
                    {
                        var ID = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value); //добавление
                        var DeleteQuery = $"delete from contacts where id = {ID}";
                        var command = new SqlCommand(DeleteQuery, dataBase.getConnection());
                        command.ExecuteNonQuery();
                    }
                    if (rowState == RowState.Modified)
                    {
                        var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                        var name = dataGridView1.Rows[index].Cells[1].Value.ToString();
                        var number = dataGridView1.Rows[index].Cells[2].Value.ToString();
                        var changeQuery = $"update contacts set contact_name= '{name}',contact_namuber='{number}' where id='{id}' ";
                        var command = new SqlCommand(changeQuery, dataBase.getConnection());
                        command.ExecuteNonQuery();
                    }
                }
                dataBase.closeConnection();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка" + ex);
            }
        }
        private void Change()//Метод изменения
        {


            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var id = textBox4.Text;
            var name = textBox2.Text;
            var number = textBox3.Text;
            dataGridView1.Rows[selectedRowIndex].SetValues(id, name, number);
            dataGridView1.Rows[selectedRowIndex].Cells[3].Value = RowState.Modified;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                sb.Append(dataGridView1.Columns[i].HeaderText);
                sb.Append(",");
            }
            sb.AppendLine();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    sb.Append(dataGridView1.Rows[i].Cells[j].Value.ToString());
                    sb.Append(",");
                }
                sb.AppendLine();
            }

            //Сохранение данных в файл CSV
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV(*.csv)|*.csv";
            sfd.FileName = "data.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, sb.ToString());
            }
        }
    }
}