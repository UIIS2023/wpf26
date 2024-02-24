using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for frmKredit.xaml
    /// </summary>
    public partial class frmKredit : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        bool azuriraj;
        DataRowView pomocniRed;

        public frmKredit(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            PopuniPadajuceListe();
        }
        public frmKredit()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
        }
        private void PopuniPadajuceListe()
        {
            try//ovde se prikazuju kombo boxovi
            {
                konekcija.Open();
                string VratiRacune = @"select RacunID,BrojRacuna from tblRacun";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daRacuna = new SqlDataAdapter(VratiRacune, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtRacuna = new DataTable();//privremena promenljiva tipa datatable
                daRacuna.Fill(dtRacuna);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbRacunID.ItemsSource = dtRacuna.DefaultView;
                daRacuna.Dispose();
            }
            catch (SqlException)
            {
                MessageBox.Show("Padajuce liste nisu popunjene!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
            txtIznos.Focus();
        }
        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIznos.Text) || string.IsNullOrEmpty(txtRata.Text) || string.IsNullOrEmpty(txtKamata.Text) || cbRacunID.SelectedValue == null)
            {
                MessageBox.Show("Molimo popunite sva polja i odaberite datume.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!txtRata.Text.All(char.IsDigit))
            {
                MessageBox.Show("Rata kredita mora biti broj.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!txtIznos.Text.All(char.IsDigit))
            {
                MessageBox.Show("Iznos kredita mora biti broj.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                konekcija.Open();
                DateTime date = (DateTime)dpPocetakOtplacivanja.SelectedDate;
                string datum = date.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
                DateTime date1 = (DateTime)dpZavrsetakOtplacivanja.SelectedDate;
                string datum1 = date1.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
                cmd.Parameters.Add("@IznosKredita", SqlDbType.Money).Value = txtIznos.Text;//da li tu treba da se promeni
                cmd.Parameters.Add("@RataKredita", SqlDbType.Money).Value = txtRata.Text;
                cmd.Parameters.Add("@KamataKredita", SqlDbType.NVarChar).Value = txtKamata.Text;
                cmd.Parameters.Add("@datum", SqlDbType.DateTime).Value = datum;
                cmd.Parameters.Add("@datum1", SqlDbType.DateTime).Value = datum1;
                cmd.Parameters.Add("@RacunID", SqlDbType.Int).Value = cbRacunID.SelectedValue;

                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblKredit Set IznosKredita=@IznosKredita, RataKredita=@RataKredita,
                                       KamataKredita=@KamataKredita, PocetakOtplacivanja=@datum, ZavrsetakOtplacivanja=@datum1,
                                        RacunID=@RacunID Where KreditID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblKredit(IznosKredita, RataKredita, KamataKredita, PocetakOtplacivanja, ZavrsetakOtplacivanja, RacunID)
                values(@IznosKredita,@RataKredita,@KamataKredita,@datum,@datum1,@RacunID);";

                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
           /* catch(SqlException)
            {
                MessageBox.Show("Unos odredjenih vrednosti nije validan", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

            }*/
            catch (InvalidOperationException)
            {
                MessageBox.Show("odaberite datum", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL greška: {ex}");
                MessageBox.Show($"Unos određenih vrednosti nije validan\nDetalji greške:\n{ex}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
