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
    public partial class Form1 : Form
    {
        ComponenteTabla  tabla;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
        private void Test(object sender, EventArgs e)
        {
            var valores = new StringBuilder();
            var button = (Button)sender;
            var ValoresDelasColumnas = (Dictionary<string, object>)button.Tag;
            foreach (var val in ValoresDelasColumnas)
                valores.Append(" " + (string)Convert.ChangeType(val.Value, Nullable.GetUnderlyingType(typeof(string)) ?? typeof(string)));
            MessageBox.Show(valores.ToString());
        }
        private void MostraVentanaCrear(object sender, EventArgs e)
        {
            
            var ventana = new Crear(tabla);
            this.panel1.Controls.Remove(tabla);
            ventana.ShowDialog();
            tabla.LeerDatos("Test");
            this.panel1.Controls.Add(tabla);
        }

        private void MostraVentanaEditar(object sender, EventArgs e)
        {
            var buton = (Button)sender;
            var valores = (Dictionary<string, object>)buton.Tag;
            var ventana = new Editar(valores);
            ventana.ShowDialog();
            this.panel1.Controls.Remove(tabla);
            tabla.LeerDatos("Test");
            this.panel1.Controls.Add(tabla);
        }

        private void BorrarElemento(object sender, EventArgs e)
        {

         
            
            var button = (Button)sender;
            var valores = (Dictionary<string,object>)button.Tag;
            DialogResult dialogResult = MessageBox.Show("Estas Seguro de Borrar Esto?", "Borrrar", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.panel1.Controls.Remove(tabla);
            
                using (var conexion = new SQLiteConnection("Data Source=mydb.db;Version=3;"))
                {
                    try
                    {
                        var query = new StringBuilder();
                        query.Append(" DELETE FROM Test  ");
                        query.Append(" WHERE ID=@Id ");
                        using (var commando = new SQLiteCommand(query.ToString(), conexion))
                        {

                          
                            commando.Parameters.AddWithValue("@Id", valores["ID"]);
                            conexion.Open();
                            int resultado = commando.ExecuteNonQuery();
                            if (resultado > 0)
                            {
                                MessageBox.Show("Borrado");
                            }


                        }
                    }


                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }

                }

                tabla.LeerDatos("Test");
                this.panel1.Controls.Add(tabla);
            }
           
           
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            tabla = new ComponenteTabla(new SQLiteConnection("Data Source=mydb.db;Version=3;"), new SQLiteCommand());
            tabla.EditClick += Test;
            tabla.EditClick += MostraVentanaEditar;
            tabla.CreateClick += MostraVentanaCrear;
            tabla.DeleteClick += BorrarElemento;
            tabla.ConfigurarColumnas(new ConlumsCollection("ID", "Nombre", "Edad"));
            tabla.LeerDatos("Test");

            this.panel1.Controls.Add(tabla);
            this.button1.Visible = false;
        }
    }
   
}
