using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Banka.Forme
{
    /// <summary>
    /// Interaction logic for frmTipKartice.xaml
    /// </summary>
    public partial class frmTipKartice : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        bool azuriraj;
        DataRowView pomocniRed;

        public frmTipKartice(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
        }
        public frmTipKartice()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            txtNazivTipaKartice.Focus();
            
        }

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazivTipaKartice.Text))
            {
                MessageBox.Show("Popunite naziv tipa kartice pre nego što sačuvate.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                konekcija.Open();
                //kreiramo novu komandu koriscenjem objekta klase sqlcommands
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija//nad kojom bazom treba dase izvrsi ova komanda, znaci izvrsi upit nad ovom konekcijom koju smo prosledili
                };
                //sta su parametri ovog upita, sta ubacujemo u bazu
                cmd.Parameters.Add("@NazivTipaKartice", System.Data.SqlDbType.NVarChar).Value = txtNazivTipaKartice.Text;//kada je @ u navodnicima - parametar



                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblTipKartice Set NazivTipaKartice=@NazivTipaKartice Where TipKarticeID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblTipKartice(NazivTipaKartice)
                                    values(@NazivTipaKartice);";
                    //ako je @ ispred sve sto je pod navodnicima je string

                }
                cmd.ExecuteNonQuery();//izvrsi ovu komandu kao upit nad bazom
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {
                MessageBox.Show("Unos odredjenih vrednosti nije validan", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }

            }
        }
    }
}
