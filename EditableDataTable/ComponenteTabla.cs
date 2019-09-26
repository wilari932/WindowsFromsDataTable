using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Data.SQLite;

namespace EditableDataTable
{
    public class ConlumsCollection
    {
        public readonly HashSet<string> Colums;

        public ConlumsCollection(params string[] columns)
        {
            Colums = new HashSet<string>(columns);
        }
    }
    public class ComponenteTabla : TableLayoutPanel
    {
       
        public event EventHandler EditClick;
        public event EventHandler DeleteClick;
        public event EventHandler CreateClick;
        private TableLayoutPanel childPanel { get; set; }
        private readonly IDbConnection conexion;
        private IDbCommand commando;

        private HashSet<string> _totalColumns;
        public ComponenteTabla(IDbConnection connection, IDbCommand dbCommand)
        {
            conexion = connection;
            commando = dbCommand;
            
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            this.ColumnCount = 1;
            this.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.AutoScroll = true;
            this.RowCount = 2;
            this.RowStyles.Add(new RowStyle(SizeType.Percent,15));
            this.RowStyles.Add(new RowStyle(SizeType.Percent, 85));

            _totalColumns = new HashSet<string>();

        }
      
        public void ConfigurarColumnas(ConlumsCollection Columnas)
        {
            _totalColumns = new HashSet<string>(Columnas.Colums);

            Init();
          
             
        
        }
        public void RefreshTable()
        {
            this.Controls.Clear();
            Init();
        }
        private void Init()
        {
            childPanel = new TableLayoutPanel();
            childPanel.Dock = DockStyle.Fill;
            childPanel.ColumnCount = _totalColumns.Count + 1;
            childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
            childPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
            childPanel.AutoScroll = true;
            childPanel.RowCount = 1;
            childPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            
            for (int i = 0; i < _totalColumns.Count; i++)
            {
                childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
                childPanel.Controls.Add(new Label
                {
                    Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                    Size = new Size(70, 25),
                    Dock = DockStyle.Fill,
                    Text = _totalColumns.ElementAt(i)
                }, i + 1, 0);

            }

            var actionslabel = new Label();
            actionslabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            actionslabel.Size = new Size(70, 25);
            actionslabel.Text = "Actiones";

           this.Controls.Add(childPanel, 0, 1);

        }

   
        //private void button1_Click(object sender, EventArgs e)
        //{
        //  
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        // 
        //}

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    using (var conexion = new SQLiteConnection(Recurso.Cadena))
        //    {
        //        try
        //        {
        //            var query = new StringBuilder();
        //            query.Append(" SELECT  ");
        //            query.Append("Id,Nombre,Appellidos");
        //            query.Append(" FROM MITABLA ");
        //            query.Append(" WHERE Id=@Id ");
        //            using (var commando = new SQLiteCommand(query.ToString(), conexion))
        //            {
        //                commando.Parameters.AddWithValue("@Id", textBoxID.Text);
        //                conexion.Open();
        //                var reader = commando.ExecuteReader();
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        textBoxID.Text = reader["Id"].ToString();
        //                        textBoxNombre.Text = reader["Nobre"].ToString();
        //                        textBoxApellidos.Text = reader["Appellidos"].ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("No se encontro nada");
        //                }


        //            }
        //        }
        //        catch (Exception error)
        //        {
        //            MessageBox.Show(error.Message);
        //        }

        //    }
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    
        //}

        //private void button5_Click(object sender, EventArgs e)
        //{

        //        try
        //        {
        //            var query = new StringBuilder();
        //            query.Append(" SELECT  ");
        //            query.Append("Id,Nombre,Appellidos");
        //            query.Append(" FROM MITABLA ");
        //            query.Append(" WHERE Id=@Id ");

        //                commando.Parameters.AddWithValue("@Id", textBoxID.Text);
        //                conexion.Open();

        //                SQLiteDataAdapter da = new SQLiteDataAdapter();

        //                da.SelectCommand = commando;

        //                DataTable dt = new DataTable();

        //                da.Fill(dt);

        //                dataGridView1.DataSource = dt;

        //                conexion.Close();

        //            }
        //        }
        //        catch (Exception error)
        //        {
        //            MessageBox.Show(error.Message);
        //        }

        //    }
        //}

        public void LeerDatos(string tabla)
        {

            try
            {
                RefreshTable();
                var query = new StringBuilder();
                query.Append(" SELECT  ");
                for (int i = 0; i < _totalColumns.Count; i++)
                {
                    query.Append(_totalColumns.ElementAt(i));
                    if (i + 1 < _totalColumns.Count)
                        query.Append(",");
                }
                query.Append(" FROM  ");
                query.Append(tabla);

                conexion.Open();

                commando.CommandText = query.ToString();
                commando.Connection = conexion;
                var reader = commando.ExecuteReader();
                while (reader.Read())
                {
                    childPanel.RowCount = childPanel.RowCount + 1;
                    childPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    var values = new Dictionary<string, object>();
                    for (int i = 0; i < _totalColumns.Count; i++)
                    {
                        values.Add(_totalColumns.ElementAt(i), reader[_totalColumns.ElementAt(i)]);
                        childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5));
                        childPanel.Controls.Add(new Label
                        {
                            Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                            Size = new Size(70, 25),
                            Dock = DockStyle.Fill,
                            Text = (string)Convert.ChangeType(reader[_totalColumns.ElementAt(i)],Nullable.GetUnderlyingType(typeof(string))??typeof(string))
                        }, i + 1, childPanel.RowCount-1);

                    }
                   
                    var editbutton = new Button();
              
                    editbutton.BackColor = System.Drawing.Color.DarkOrchid;
                    editbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    editbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    
                    editbutton.Size = new System.Drawing.Size(100,30);
                    editbutton.Text = "Editar";
                    editbutton.UseVisualStyleBackColor = false;
                    
                    editbutton.Tag = values;
                    editbutton.Click += EditClick;

                    var deletebtn = new Button();
                    //editbutton.Tag = 
                    deletebtn.BackColor = System.Drawing.Color.Red;
                    deletebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    deletebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    deletebtn.Size = new System.Drawing.Size(100, 30);

                    deletebtn.Text = "Delete";
                    deletebtn.UseVisualStyleBackColor = false;
                    
                    deletebtn.Tag = values;
                    deletebtn.Click += DeleteClick;

                    var p = new FlowLayoutPanel();
                    p.Controls.Add(deletebtn);
                    p.Controls.Add(editbutton);
                    childPanel.Controls.Add(p, 0, childPanel.RowCount - 1);
                }
                reader.Close();
                conexion.Close();
                TableLayoutPanel createPanel = new TableLayoutPanel();
                var emptyPanel = new Panel();
                emptyPanel.Size = new System.Drawing.Size(118, 42);
                emptyPanel.Dock = DockStyle.Fill;
                createPanel.Dock = DockStyle.Fill;
                createPanel.ColumnCount = 2;
                createPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
                createPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85));
                createPanel.AutoScroll = true;
                createPanel.RowCount = 1;
                createPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var createbtn = new Button();
                //editbutton.Tag = 
                createbtn.BackColor = System.Drawing.Color.DarkOrchid;
                createbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                createbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                createbtn.Size = new System.Drawing.Size(118, 42);
                createbtn.Text = "Crear";
                createbtn.Dock = DockStyle.Fill;
                createbtn.UseVisualStyleBackColor = false;
                createbtn.Click += CreateClick;
                createPanel.Controls.Add(createbtn,0,0);
                createPanel.Controls.Add(emptyPanel, 1, 0);

                this.Controls.Add(createPanel, 0,0);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }


        }


    }
}
