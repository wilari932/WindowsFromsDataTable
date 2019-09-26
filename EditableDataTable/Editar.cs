using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditableDataTable
{
    public partial class Editar : Form
    {
        private readonly string ID;
        public Editar(Dictionary<string, object> valores)
        {
            InitializeComponent();
            try
            {
                textBox1.Text = Convert.ToString(valores["Nombre"]);
                textBox2.Text = Convert.ToString(valores["Edad"]);
                ID = Convert.ToString(valores["ID"]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (var conexion = new SQLiteConnection("Data Source=mydb.db;Version=3;"))
            {
                try
                {
                    var query = new StringBuilder();
                    query.Append(" UPDATE Test SET ");
                    query.Append("Nombre=@Nombre,Edad=@Edad");
                    query.Append(" WHERE ID=@Id;");
                    using (var commando = new SQLiteCommand(query.ToString(), conexion))
                    {
                        commando.Parameters.AddWithValue("@Id", ID);
                        commando.Parameters.AddWithValue("@Edad", textBox2.Text);
                        commando.Parameters.AddWithValue("@Nombre", textBox1.Text);
                        conexion.Open();
                        int resultado = commando.ExecuteNonQuery();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Actulizado");
                            this.Close();
                        }


                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }

            }
        }
    }
}
