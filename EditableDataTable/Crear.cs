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
    public partial class Crear : Form
    {
      
        public Crear(ComponenteTabla dataGrid)
        {
            InitializeComponent();
           
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (var conexion = new SQLiteConnection("Data Source=mydb.db;Version=3;"))
            {

                try
                {
                    var query = new StringBuilder();
                    query.Append(" INSERT INTO Test ");
                    query.Append("(Nombre,Edad)");
                    query.Append("VALUES(@Nombre,@Edad);");
                    using (var commando = new SQLiteCommand(query.ToString(), conexion))
                    {
                        commando.Parameters.AddWithValue("@Edad", textBox2.Text);
                        commando.Parameters.AddWithValue("@Nombre", textBox1.Text);
                        conexion.Open();
                        int resultado = commando.ExecuteNonQuery();
                        conexion.Close();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Guardado");
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

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


