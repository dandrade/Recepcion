using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


using Recepcion.Common;
using Recepcion.Logic;

namespace Recepcion
{
    public partial class AltaHuella : Form
    {
        GriauleFingerprintLibrary.FingerprintCore core;
        GriauleFingerprintLibrary.DataTypes.FingerprintRawImage huella;
        GriauleFingerprintLibrary.DataTypes.FingerprintTemplate template;

        private static MySqlCommand command;
        private static Conexion_MySQL Conexion;

        List<Servicio> servicios;
        Rules logica = new Rules();

        string idAdmin = "0";

        Usuario usuario;
        bool isEdit = false;

        public AltaHuella(string id)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            cargarServicios();
            idAdmin = id;

            //fechaNacimiento.Value = DateTime.Now;

            //mensualidad.Text = this.getFecha();
        }

        public AltaHuella(Usuario usr)
        {
            InitializeComponent();
            usuario = usr;
            comboBox1.SelectedIndex = 0;
            asignarValores();
            cargarServicios(usr.idUsuario.ToString());
            button1.Text = "Editar Usuario";
            this.Text = "Editar Usuario";
            isEdit = true;
            fotoEditar.ImageLocation = @"C:\wamp\www\gym\fotos\" + usuario.Foto;
            fotoEditar.Show();
            label15.Hide();
            label16.Hide();
            subtotal.Hide();
            descuento.Hide();
            groupBox3.Hide();
            groupBox1.Text = "Fotografia";
            //groupBox1.Hide();
            pictureBox1.Hide();
            //fechaNacimiento.Text = this.getFecha();
            //mensualidad.Text = this.getFecha();
        }

        private void asignarValores()
        {
            nombre.Text = usuario.Nombre;
            apellidoPaterno.Text = usuario.ApellidoPaterno;
            apellidoMaterno.Text = usuario.ApellidoMaterno;
            lugarNacimiento.Text = usuario.LugarNacimiento;
            fechaNacimiento.Text = usuario.FechaNacimiento;
            direccion.Text = usuario.Direccion;
            colonia.Text = usuario.Colonia;
            CP.Text = usuario.CP;
            municipio.Text = usuario.Municipio;
            telefono.Text = usuario.Telefono;
            celular.Text = usuario.Celular;
            telefonoAdicional.Text = usuario.TelefonoAdicional;
            comboBox1.SelectedItem = usuario.RolUser;
            mensualidad.Text = usuario.Mensualidad;
        }

        public void cargarServicios()
        {
            servicios = new List<Servicio>();
            servicios = logica.getServices();
            dataGridView1.DataSource = servicios;
        }

        public void cargarServicios(string idUsr)
        {
            servicios = new List<Servicio>();
            servicios = logica.getServices(idUsr);
            dataGridView1.DataSource = servicios;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!isEdit)
            {
                try
                {
                    core = new GriauleFingerprintLibrary.FingerprintCore();

                    core.onStatus += new GriauleFingerprintLibrary.StatusEventHandler(core_onStatus);
                    core.onImage += new GriauleFingerprintLibrary.ImageEventHandler(core_onImage);
                    core.Initialize();
                    core.CaptureInitialize();
                }
                catch
                {
                }
            }
        }


        bool HuellaOK = false;
        void core_onImage(object source, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
        {
            try 
            {
                huella = ie.RawImage;
                core.Extract(huella, ref template);

                pictureBox1.Image = huella.Image;
                
                switch (template.Quality)
                {
                    case 0:
                        MessageBox.Show("Huella de mala calidad favor de volver a poner el dedo");
                        return;
                    case 1:
                        MessageBox.Show("La huella es de una calidad media, intente nuevamente");
                        return;
                    case 2:
                        MessageBox.Show("Huella con buena calidad");
                        HuellaOK = true;
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void core_onStatus(object source, GriauleFingerprintLibrary.Events.StatusEventArgs se)
        {
            if (se.StatusEventType == GriauleFingerprintLibrary.Events.StatusEventType.SENSOR_PLUG)
            {
                core.StartCapture(source);
                mensajeBarraEstado.Text = "Lector Conectado";
            }
            else
            {
                mensajeBarraEstado.Text = "Lector Desconectado";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty(nombre.Text) && HuellaOK)
            if (isEdit)
            {
                editarUsuarioClick();
            }
            else
            {
                agregarUsuarioClick();
            }

        }

        private void agregarUsuarioClick()
        {
            if (!String.IsNullOrEmpty(nombre.Text) && HuellaOK)
            {
                List<int> servicios = new List<int>();
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    try
                    {
                        bool isSelected = (bool)item.Cells["Selected"].Value;
                        if (isSelected)
                        {
                            int idServicio = (int)item.Cells["idServicio"].Value;

                            servicios.Add(idServicio);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                try
                {
                    if (!String.IsNullOrEmpty(textBox1.Text))
                    {
                        //System.IO.File.Copy(foto, @"c:\wamp\www\gym\fotos\", true);
                        Fotografia = GetFileName(textBox1.Text).Replace(".", DateTime.Now.Ticks.ToString() + ".");
                        System.IO.File.Move(textBox1.Text, @"c:\wamp\www\gym\fotos\" + Fotografia);

                        //System.IO.File.Copy(textBox1.Text, @"C:\xampp\xampplite\htdocs\gym\fotos\"+Fotografia, true);


                    }
                }
                catch
                {
                }

                Conexion = new Conexion_MySQL();

                string query = @"INSERT INTO usuarios (
                                    nombre, 
                                    apellidoPaterno, 
                                    apellidoMaterno,
                                    lugarNacimiento,
                                    fechaNacimiento,
                                    direccion,
                                    colonia,
                                    cp,
                                    municipio,
                                    telefono,
                                    celular,
                                    telefonoAdicional,
                                    foto,
                                    rol,
                                    activo,
                                    template,
                                    calidad_template,
                                    mensualidad
                                )
                                VALUES 
                                (
                                    ?nombre, 
                                    ?apellidoPaterno, 
                                    ?apellidoMaterno,
                                    ?lugarNacimiento,
                                    ?fechaNacimiento,
                                    ?direccion,
                                    ?colonia,
                                    ?cp,
                                    ?municipio,
                                    ?telefono,
                                    ?celular,
                                    ?telefonoAdicional,
                                    ?foto,
                                    ?rol,
                                    ?activo,
                                    ?template,
                                    ?calidad_template,
                                    ?mensualidad
                                ); select LAST_INSERT_ID()";

                MySqlCommand cmd = new MySqlCommand(query, Conexion.Cnx);

                cmd.Parameters.AddWithValue("?nombre", nombre.Text);
                cmd.Parameters.AddWithValue("?apellidoPaterno", apellidoPaterno.Text);
                cmd.Parameters.AddWithValue("?apellidoMaterno", apellidoMaterno.Text);
                cmd.Parameters.AddWithValue("?lugarNacimiento", lugarNacimiento.Text);
                cmd.Parameters.AddWithValue("?fechaNacimiento", fechaNacimiento.Text);
                cmd.Parameters.AddWithValue("?direccion", direccion.Text);
                cmd.Parameters.AddWithValue("?colonia", colonia.Text);
                cmd.Parameters.AddWithValue("?cp", CP.Text);
                cmd.Parameters.AddWithValue("?municipio", municipio.Text);
                cmd.Parameters.AddWithValue("?telefono", telefono.Text);
                cmd.Parameters.AddWithValue("?celular", celular.Text);
                cmd.Parameters.AddWithValue("?telefonoAdicional", telefonoAdicional.Text);
                cmd.Parameters.AddWithValue("?foto", Fotografia);
                cmd.Parameters.AddWithValue("?rol", comboBox1.SelectedItem);
                cmd.Parameters.AddWithValue("?activo", "1");
                cmd.Parameters.AddWithValue("?calidad_template", template.Quality.ToString());
                cmd.Parameters.AddWithValue("?mensualidad", mensualidad.Text);


                MySqlParameter templateParam = cmd.Parameters.Add("?template", MySqlDbType.Blob);
                templateParam.Value = (object)template.Buffer;


                try
                {
                    int newID = Convert.ToInt32(cmd.ExecuteScalar());

                    if (newID > 0)
                    {
                        if (comboBox1.SelectedItem != "Usuario")
                        {
                            foreach (var item in servicios)
                            {
                                string q = "insert into servicios_usuarios (servicio,usuario,tipo) values (" + item.ToString() + "," + newID + ",'" + comboBox1.SelectedItem + "')";
                                MySqlCommand cmd2 = new MySqlCommand(q, Conexion.Cnx);
                                cmd2.ExecuteNonQuery();
                            }

                            if (comboBox1.SelectedItem == "Cliente")
                            {
                                string pago = "insert into pagos (monto, descuento, concepto, usuario, cobro, fecha, mensualidad) values('" + subtotal.Text + "', '" + descuento.Text + "', 'Inscripcion', " + newID + ", " + idAdmin + ", '" + this.getFecha() + "', '" + mensualidad.Text + "' )";
                                MySqlCommand cmd3 = new MySqlCommand(pago, Conexion.Cnx);
                                cmd3.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Usuario Agregado");
                        clean();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo agregar el usuario");
                    }

                }
                catch (MySqlException ex)
                {

                }
            }
            else
            {
                MessageBox.Show("Por favor capture su huella.");
            }
        }


        private void editarUsuarioClick()
        {
            if (!String.IsNullOrEmpty(nombre.Text))
            {
                try
                {
                    if (!String.IsNullOrEmpty(textBox1.Text))
                    {
                        //System.IO.File.Copy(foto, @"c:\wamp\www\gym\fotos\", true);
                        Fotografia = GetFileName(textBox1.Text).Replace(".", DateTime.Now.Ticks.ToString() + ".");
                        System.IO.File.Move(textBox1.Text, @"c:\wamp\www\gym\fotos\" + Fotografia);

                        //System.IO.File.Copy(textBox1.Text, @"C:\xampp\xampplite\htdocs\gym\fotos\"+Fotografia, true);


                    }
                }
                catch
                {
                }
                
                Conexion = new Conexion_MySQL();

                string query = @"UPDATE usuarios set
                                    nombre = ?nombre, 
                                    apellidoPaterno = ?apellidoPaterno, 
                                    apellidoMaterno = ?apellidoMaterno,
                                    lugarNacimiento = ?lugarNacimiento,
                                    fechaNacimiento = ?fechaNacimiento,
                                    direccion = ?direccion,
                                    colonia = ?colonia,
                                    cp = ?cp,
                                    municipio = ?municipio,
                                    telefono = ?telefono,
                                    celular = ?celular,
                                    telefonoAdicional = ?telefonoAdicional,
                                    foto = ?foto,
                                    rol = ?rol,
                                    mensualidad = ?mensualidad
                                    where id = ?id
                                ";

                MySqlCommand cmd = new MySqlCommand(query, Conexion.Cnx);

                cmd.Parameters.AddWithValue("?nombre", nombre.Text);
                cmd.Parameters.AddWithValue("?apellidoPaterno", apellidoPaterno.Text);
                cmd.Parameters.AddWithValue("?apellidoMaterno", apellidoMaterno.Text);
                cmd.Parameters.AddWithValue("?lugarNacimiento", lugarNacimiento.Text);
                cmd.Parameters.AddWithValue("?fechaNacimiento", fechaNacimiento.Text);
                cmd.Parameters.AddWithValue("?direccion", direccion.Text);
                cmd.Parameters.AddWithValue("?colonia", colonia.Text);
                cmd.Parameters.AddWithValue("?cp", CP.Text);
                cmd.Parameters.AddWithValue("?municipio", municipio.Text);
                cmd.Parameters.AddWithValue("?telefono", telefono.Text);
                cmd.Parameters.AddWithValue("?celular", celular.Text);
                cmd.Parameters.AddWithValue("?telefonoAdicional", telefonoAdicional.Text);
                cmd.Parameters.AddWithValue("?foto", Fotografia);
                cmd.Parameters.AddWithValue("?rol", comboBox1.SelectedItem);
                cmd.Parameters.AddWithValue("?mensualidad", mensualidad.Text);
                cmd.Parameters.AddWithValue("?id", usuario.idUsuario);

                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(textBox1.Text))
                            {
                                //System.IO.File.Copy(foto, @"c:\wamp\www\gym\fotos\", true);
                                Fotografia = GetFileName(textBox1.Text).Replace(".", DateTime.Now.Ticks.ToString() + ".");
                                System.IO.File.Move(textBox1.Text, @"c:\wamp\www\gym\fotos\" + Fotografia);

                                //System.IO.File.Copy(textBox1.Text, @"C:\xampp\xampplite\htdocs\gym\fotos\"+Fotografia, true);


                            }
                        }
                        catch
                        {
                        }

                        MessageBox.Show("Usuario Editado");
                        //clean();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo editar el usuario");
                    }

                }
                catch (MySqlException ex)
                {

                }
            }
            else
            {
                MessageBox.Show("El nombre/huella no puede estar en blanco.");
            }
        }


        private string getFecha()
        {
            string[] fechaA = DateTime.Now.ToShortDateString().Split(new char[] { '/' });
            string dia = fechaA[0].ToString();
            string mes = fechaA[1].ToString();
            string ano = fechaA[2].ToString().Replace("20", "");
            string fecha = dia + "/" + mes + "/" + ano;
            return fecha;
        }


        private void clean()
        {
            nombre.Text = ""; 
            apellidoPaterno.Text = ""; 
            apellidoMaterno.Text = "";
            lugarNacimiento.Text = "";
            fechaNacimiento.Text = "";
            direccion.Text = "";
            colonia.Text = "";
            CP.Text = "";
            municipio.Text = "";
            telefono.Text = "";
            celular.Text = "";
            telefonoAdicional.Text = "";
            textBox1.Text = "";
            cargarServicios();
             
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != "Cliente")
            {
                costo = 0;
                subtotal.Text = "";
            }
        }

        private void AltaHuella_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                core.Finalizer();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void AltaHuella_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                core.Finalizer();
            }
            catch (Exception ex)
            {
                
            }
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        string Fotografia;
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult resultado = openFileDialog1.ShowDialog();

            if (resultado == DialogResult.OK)
            {

                textBox1.Text = openFileDialog1.FileName;

                
            }
        }

        public string GetFileName(string path)
        {
            string ruta, nombre;
            ruta = System.IO.Path.GetDirectoryName(path);
            nombre = path.ToString().Replace(ruta, "");
            return nombre.Replace("\\", "");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        public double costo;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (comboBox1.SelectedItem == "Cliente")
            {

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Selected")
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    DataGridViewCheckBoxCell seleccion = row.Cells["Selected"] as DataGridViewCheckBoxCell;

                    if (Convert.ToBoolean(seleccion.Value))
                    {
                        costo += Convert.ToDouble(row.Cells["Costo"].Value);
                        subtotal.Text = costo.ToString();
                    }
                }
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void descuento_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void descuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
    }
}
