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
            this.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            this.AutoScroll = true;
            this.RowCount = 2;
            this.RowStyles.Add(new RowStyle(SizeType.Percent,15));
            this.RowStyles.Add(new RowStyle(SizeType.Percent,82));

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
            childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            childPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            childPanel.AutoScroll = true;
            childPanel.RowCount = 1;
            childPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
            
            for (int i = 0; i < _totalColumns.Count; i++)
            {
                childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
                childPanel.Controls.Add(new Label
                {
                    Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                    Size = new Size(70, 25),
                    Dock = DockStyle.Fill,
                    Text = _totalColumns.ElementAt(i)
                }, i + 1, 0);

            }

            var actionslabel = new Label();
            actionslabel.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            actionslabel.Size = new Size(70, 25);
            actionslabel.Text = "Actiones";
			childPanel.Controls.Add(actionslabel, 0,0);
           this.Controls.Add(childPanel, 0, 1);

        }


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
                    childPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
                    var values = new Dictionary<string, object>();
                    for (int i = 0; i < _totalColumns.Count; i++)
                    {
                        values.Add(_totalColumns.ElementAt(i), reader[_totalColumns.ElementAt(i)]);
                        childPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
						childPanel.Controls.Add(new RichTextBox
						{
							Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
							Dock = DockStyle.Fill,
							Enabled = false,
							BorderStyle = BorderStyle.None,
			
                            Text = (string)Convert.ChangeType(reader[_totalColumns.ElementAt(i)],Nullable.GetUnderlyingType(typeof(string))??typeof(string))
                        }, i + 1, childPanel.RowCount-1);

                    }
                   
                    var editbutton = new Button();
              
                    editbutton.BackColor = System.Drawing.Color.Green;
                    editbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
					editbutton.ForeColor = System.Drawing.Color.WhiteSmoke;
					editbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    //
                   
                    editbutton.Text = "Editar";
                    editbutton.UseVisualStyleBackColor = false;
					editbutton.Dock = DockStyle.Top;
					editbutton.Tag = values;
                    editbutton.Click += EditClick;

                    var deletebtn = new Button();
                    //editbutton.Tag = 
                    deletebtn.BackColor = System.Drawing.Color.Red;
                    deletebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    deletebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
					deletebtn.Dock = DockStyle.Top;
			
					deletebtn.ForeColor = System.Drawing.Color.WhiteSmoke;
					
					deletebtn.Text = "Borrar";
                    deletebtn.UseVisualStyleBackColor = false;
                    
                    deletebtn.Tag = values;
                    deletebtn.Click += DeleteClick;

                    var p = new TableLayoutPanel();
					p.RowCount = 1;
					p.ColumnCount = 2;
					p.Dock = DockStyle.Fill;
					p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
					p.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
					p.Controls.Add(deletebtn,0,0);
                    p.Controls.Add(editbutton,1,0);
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
                createPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
                var createbtn = new Button();
                //editbutton.Tag = 
                createbtn.BackColor = System.Drawing.Color.Transparent;
                createbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                createbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                createbtn.Size = new System.Drawing.Size(60, 35);
                createbtn.Text = "+";
				
				createbtn.ForeColor = Color.Black;
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
