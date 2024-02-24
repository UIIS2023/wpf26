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
    /// Interaction logic for frmTipTransakcije.xaml
    /// </summary>
    public partial class frmTipTransakcije : Window
    {
        //kreiramo konekciju na bazu
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();//objekat klase konekcija
        bool azuriraj;
        DataRowView pomocniRed;
        public frmTipTransakcije(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
        }
        public frmTipTransakcije()
        {
            InitializeComponent();
                //konekciji moramo dodeliti parametre, odnosno pozvati metodu kreiraj konekciju
                konekcija = kon.KreirajKonekciju();
            txtNazivTipaTransakcije.Focus();
            }

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNazivTipaTransakcije.Text))
            {
                MessageBox.Show("Molimo popunite sva polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                cmd.Parameters.Add("@NazivTipaTransakcije", System.Data.SqlDbType.NVarChar).Value = txtNazivTipaTransakcije.Text;//kada je @ u navodnicima - parametar

                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblTipTransakcije Set NazivTipaTransakcije=@NazivTipaTransakcije Where TipTransakcijeID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblTipTransakcije(NazivTipaTransakcije)
                                    values(@NazivTipaTransakcije);";
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
