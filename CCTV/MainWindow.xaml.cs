using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
//using Camera;


namespace CCTV
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NVR nvr = new NVR();
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            initNVR();
            pingNVR();
        }
        public string recordPath;

        private bool[] control = new bool[4] { false, false, false, false };

        private void initNVR()
        {
            for (int i = 0; i < NVR.NVRMaxCams; ++i)
            {
                string path = @".\Utils\config\cam" + (i + 1).ToString();
                if (File.Exists(path + @"\setup") || File.Exists(path + @"\credentials"))
                {
                    // Read a text file line by line.

                    string setupBit;
                    string[] cred;

                    setupBit = File.ReadAllText(path + @"\setup");
                    cred = File.ReadAllLines(path + @"\credentials");

                    //MessageBox.Show(setupBit[0]);
                    if (setupBit[0].Equals('1'))
                    {
                        control[i] = true;
                        Cam aux = new Cam((i + 1).ToString(), cred[2], cred[0], cred[1]);
                        nvr.addUpdate(aux);
                        if (i == 0)
                        {
                            C1.Text = "Càmera " + (i + 1).ToString();
                            IP1.Text = aux.ipString;
                            U1.Text = aux.user;
                            PASS1.Text = aux.password;
                        }
                        if (i == 1)
                        {
                            C2.Text = "Càmera " + (i + 1).ToString();
                            IP2.Text = aux.ipString;
                            U2.Text = aux.user;
                            PASS2.Text = aux.password;
                        }
                        if (i == 2)
                        {
                            C3.Text = "Càmera " + (i + 1).ToString();
                            IP3.Text = aux.ipString;
                            U3.Text = aux.user;
                            PASS3.Text = aux.password;
                        }
                        if (i == 3)
                        {
                            C4.Text = "Càmera " + (i + 1).ToString();
                            IP4.Text = aux.ipString;
                            U4.Text = aux.user;
                            PASS4.Text = aux.password;
                        }
                    }
                }
            }

        }

        private async void pingNVR() {
            await nvr.ping();
            Console.WriteLine("Després?");
            for (int i = 0; i < NVR.NVRMaxCams; ++i)
            {
                if (nvr.camList[i].connection == 0)
                {
                    if (i == 0) IP1.Background = Brushes.Yellow;
                    if (i == 1) IP2.Background = Brushes.Yellow;
                    if (i == 2) IP3.Background = Brushes.Yellow;
                    if (i == 3) IP4.Background = Brushes.Yellow;
                }
                else if (nvr.camList[i].connection == 1)
                {
                    if (i == 0) IP1.Background = Brushes.Green;
                    if (i == 1) IP2.Background = Brushes.Green;
                    if (i == 2) IP3.Background = Brushes.Green;
                    if (i == 3) IP4.Background = Brushes.Green;
                }
                else {
                    if (i == 0) IP1.Background = Brushes.Red;
                    if (i == 1) IP2.Background = Brushes.Red;
                    if (i == 2) IP3.Background = Brushes.Red;
                    if (i == 3) IP4.Background = Brushes.Red;
                }
            }
        }

        private void Afegir_Click(object sender, RoutedEventArgs e)
        {
            int cam = -1;
            string _ip1 = ip1.Text;
            string _ip2 = ip2.Text;
            string _ip3 = ip3.Text;
            string _ip4 = ip4.Text;
            string _user = user.Text;
            string _pass = pass.Password;
            if (cam1.IsChecked == true) cam = 1;
            if (cam2.IsChecked == true) cam = 2;
            if (cam3.IsChecked == true) cam = 3;
            if (cam4.IsChecked == true) cam = 4;
            /*ERR Handling*/
            if (cam == -1)
            {
                System.Windows.MessageBox.Show("No has seleccionat CÀMERA");
                return;
            }

            if (!(Int16.TryParse(_ip1, out short num) && Int16.TryParse(_ip2, out short num_) && Int16.TryParse(_ip3, out short num__) && Int16.TryParse(_ip4, out short num___)))
            {
                System.Windows.MessageBox.Show("IP ha de ser números");
                return;
            }

            if (Int16.Parse(_ip1) > 255 || Int16.Parse(_ip1) < 0 || Int16.Parse(_ip2) > 255 || Int16.Parse(_ip2) < 0 || Int16.Parse(_ip3) > 255 || Int16.Parse(_ip3) < 0 || Int16.Parse(_ip4) > 255 || Int16.Parse(_ip4) < 0)
            {
                System.Windows.MessageBox.Show("El rang de cada una és de 0 a 255");
                return;
            }

            if (user.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Introdueixi usuari");
                return;
            }
            if (pass.Password == String.Empty)
            {
                System.Windows.MessageBox.Show("Introdueixi contrasenya");
                return;
            }

            Cam aux = new Cam(cam.ToString(), _ip1, _ip2, _ip3, _ip4, _user, _pass);

            /*Update config*/
            aux.setCam();

            user.Text = String.Empty;
            pass.Password = String.Empty;
            System.Windows.MessageBox.Show("Success!");
            initNVR();
            pingNVR();
        }

        private void Treure_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {

            if (pathBox.Text == String.Empty)
            {
                System.Windows.MessageBox.Show("Seleccioni el directori de grabacions");
                return;
            }
            nvr.changeRecordPath(pathBox.Text);
            if(nvr.isConfigured()) nvr.start();
            pingNVR();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            nvr.reset();
            pingNVR();
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {

            string path = "c:\\Utils\\config\\ini_end_time";
            /*Escriptura a fitxer*/
            try
            {
                File.WriteAllText(path, ini.Text + ' ' + end.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex is IOException)
                {
                    System.Windows.MessageBox.Show("Ha hagut un error intern torna a intentar");
                    return;
                }
                throw;
            }
            System.Windows.MessageBox.Show("Sucess");
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "Select folder";
            fbd.ShowNewFolderButton = false;

            if (fbd.ShowDialog().ToString().Equals("OK"))
            {
                pathBox.Text = fbd.SelectedPath;
                recordPath = fbd.SelectedPath;
            }

            File.WriteAllText(@".\Utils\path", pathBox.Text);
        }

        private void sizeControl(string cam)
        {

        }
    }
}
