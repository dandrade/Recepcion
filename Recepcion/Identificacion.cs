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
using System.Configuration;

namespace Recepcion
{
    public partial class Identificacion : Form
    {
        GriauleFingerprintLibrary.FingerprintCore core;
        GriauleFingerprintLibrary.DataTypes.FingerprintRawImage huella;
        GriauleFingerprintLibrary.DataTypes.FingerprintTemplate template;

        private static MySqlCommand command;
        private static Conexion_MySQL Conexion;

        public string Usuario { get; set; }
        public string Rol = String.Empty;

        public Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        

        private string getDay(string day)
        {
            switch (day)
            {
                case "Sunday":
                    return "dom";
                case "Monday":
                    return "lun";
                case "Tuesday":
                    return "mar";
                case "Wednesday":
                    return "mie";
                case "Thursday":
                    return "jue";
                case "Friday":
                    return "vie";
                case "Saturday":
                    return "sab";
                default:
                    break;
            }
            return "";
        }

        private List<Horario> getHorarios(string usuario)
        {
            string day = this.getDay(DateTime.Today.DayOfWeek.ToString());
            //return new Rules().getHorarios("lun","24");//.getHorarios(day, usuario);
            return new Rules().getHorarios(day, usuario);
        }

        public Identificacion()
        {
            InitializeComponent();
            //string id = new Rules().isInOut("38");
            //MessageBox.Show(new Rules().registrarSalida("38").ToString());
            
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.Height = Screen.PrimaryScreen.Bounds.Height;
            //this.Width = Screen.PrimaryScreen.Bounds.Width;
            //this.Left = (Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2);
            //this.Top = (Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2);

           // this.getDay(DateTime.Today.DayOfWeek.ToString());
            //Usuario = "24";
            //testApp();
        }

        private void testApp()
        { 
            // Ya encontre el usuario por su huella entonces
            List<Horario> horarios = this.getHorarios("");

            if (horarios.Count > 0)
            {
                dataGridView1.DataSource = horarios;
                label2.Text = "Bienvenido Daniel Andrade";
                //panel2.Show();
            }
            else
            {
                // no hay horarios
                MessageBox.Show("Lo sentimos no contamos con horarios disponibles");
            }
        }

        private void Identificacion_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            core = new GriauleFingerprintLibrary.FingerprintCore();

            core.onStatus += new GriauleFingerprintLibrary.StatusEventHandler(core_onStatus);
            core.onImage += new GriauleFingerprintLibrary.ImageEventHandler(core_onImage);

            core.Initialize();
            core.CaptureInitialize();
            //fotoGrafia.ImageLocation = @"C:\wamp\www\gym\fotos\Jellyfish634579710106958000.jpg";
        }

        void core_onImage(object source, GriauleFingerprintLibrary.Events.ImageEventArgs ie)
        {
            try
            {
                huella = ie.RawImage;
                core.Extract(huella, ref template);
                label4.Text = "Espere, identificando...";
                bool haveHorarios = false;
                string consulta;
                byte[] dataTemp;
                GriauleFingerprintLibrary.DataTypes.FingerprintTemplate templateTemp;
                int precision, calidad;

                // selecciono 
                consulta = "select id, rol, concat_ws(' ', nombre, apellidoPaterno, apellidoMaterno) as nombreCompleto, template, calidad_template, foto from usuarios where template is not null and 1 = 1";

                MySqlDataReader reader = this.EjecutarQuery(consulta);
                core.IdentifyPrepare(template);

                while (reader.Read())
                {
                    dataTemp = (byte[])reader["template"];
                    calidad = (int)reader["calidad_template"];
                    templateTemp = new GriauleFingerprintLibrary.DataTypes.FingerprintTemplate();
                    templateTemp.Buffer = dataTemp;
                    templateTemp.Size = dataTemp.Length;
                    templateTemp.Quality = calidad;

                    int result = core.Identify(templateTemp, out precision);

                    
                    if (result == 1)
                    {
                        //
                        Usuario = reader["id"].ToString();
                        string nombreCompleto = reader["nombreCompleto"].ToString();
                        string rol = reader["rol"].ToString();
                        string foto = reader["foto"].ToString();
                        Rol = rol;
                        string inout = new Rules().isInOut(Usuario);
                        if (inout == "In")
                        {

                            if (Rol == "Cliente")
                            {
                                List<Horario> horarios = this.getHorarios(Usuario);

                                if (horarios.Count > 0)
                                {
                                    haveHorarios = true;
                                }
                                else
                                {
                                    // no hay horarios
                                    MessageBox.Show("Lo sentimos no contamos con horarios disponibles");
                                    clean();
                                }

                                if (haveHorarios)
                                {
                                    label4.Text = "";
                                    dataGridView1.DataSource = horarios;
                                    label2.Text = "Bienvenido " + nombreCompleto;
                                    mensualidad.Text = new Rules().ProximoPago(Usuario);

                                    if (!String.IsNullOrEmpty(foto))
                                    {
                                        fotoGrafia.ImageLocation = @"c:\wamp\www\gym\fotos\" + foto;
                                    }

                                }
                            }
                            else if (Rol == "Instructor")
                            {
                                label4.Text = "";
                                dataGridView1.DataSource = null;
                                label2.Text = "Bienvenido " + nombreCompleto;

                                if (!String.IsNullOrEmpty(foto))
                                {
                                    fotoGrafia.ImageLocation = @"c:\wamp\www\gym\fotos\" + foto;
                                }
                            }


                        }
                        else
                        {
                            if (new Rules().registrarSalida(Usuario))
                            {
                                label2.Text = "Hasta pronto " + nombreCompleto;

                                if (!String.IsNullOrEmpty(foto))
                                {
                                    fotoGrafia.ImageLocation = @"c:\wamp\www\gym\fotos\" + foto;
                                }

                                clean();
                            }

                        }

                        break;
                    }
                    else
                    {

                        label4.Text = "Usuario no registrado, te invitamos a darte de alta";
                        dataGridView1.DataSource = null;
                        label2.Text = "";
                    }
                }
                //pictureBox1.Image = huella.Image;

                //switch (template.Quality)
                //{
                //    case 0:
                //        label4.Text = "Mala calidad intente nuevamente.";
                //        dataGridView1.DataSource = null;
                //        label2.Text = "";
                //        //MessageBox.Show("Huella de mala calidad favor de volver a poner el dedo");
                //        return;
                //    case 1:
                //        label4.Text = "Mediana calidad intente nuevamente.";
                //        //MessageBox.Show("Huella de mala calidad favor de volver a poner el dedo");
                //        dataGridView1.DataSource = null;
                //        label2.Text = "";
                //        return;
                //    case 2:
                        
                        
                //        break;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clean()
        {
            int miliseconds = int.Parse(config.AppSettings.Settings["Clean"].Value.ToString());
            System.Threading.Thread.Sleep(miliseconds * 1000);
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            mensualidad.Text = "";
            dataGridView1.DataSource = null;
            fotoGrafia.ImageLocation = null;
                        
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

        private MySqlDataReader EjecutarQuery(string query)
        {
            Conexion = new Conexion_MySQL();

            MySqlCommand cmd = new MySqlCommand(query, Conexion.Cnx);

            try
            {
                return cmd.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Rol == "Cliente")
            {
                List<int> horarios = new List<int>();
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    try
                    {
                        bool isSelected = (bool)item.Cells["Selected"].Value;
                        if (isSelected)
                        {
                            int idHorario = (int)item.Cells["ID"].Value;

                            horarios.Add(idHorario);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (horarios.Count > 0)
                {
                    if (new Rules().registrarEntrada(horarios, Usuario))
                    {
                        clean();
                        //panel2.Hide();
                    }
                    else
                    {
                        MessageBox.Show("No hemos podido registrar su entrada");
                    }
                }
                else
                {
                    MessageBox.Show("Por favor seleccione el o los horario(s) al que desea ingresar");
                }
            }
            else if (Rol == "Instructor")
            {
                if (new Rules().registrarEntrada(Usuario))
                {
                    clean();

                        //panel2.Hide();
                }
                else
                {
                     MessageBox.Show("No hemos podido registrar su entrada");
                }
                
            }
            clean();
        }

        private void Identificacion_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //core.Finalizer();
                core.CaptureFinalize();
            }
            catch
            { 
                
            }
        }

    }
}
