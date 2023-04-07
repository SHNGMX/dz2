using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dz2
{
     class Class1
    {
        SqlConnection con = new SqlConnection(@"Server = db.edu.cchgeu.ru; User = 201s_Tkachenko; Password = QazWsxEdc123;Database = 201s_Tkachenko");



        public void openConnection()
        {
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
        }

        public void closeConnection()
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }

        public SqlConnection getConnection()
        {
            return con;
        }
    }
}
