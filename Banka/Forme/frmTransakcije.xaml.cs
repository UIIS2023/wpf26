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
    /// Interaction logic for frmTransakcije.xaml
    /// </summary>
    public partial class frmTransakcije : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        bool azuriraj;
        DataRowView pomocniRed;

        public frmTransakcije(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            PopuniPadajuceListe();
        }
        public frmTransakcije()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();//uspostavljamo konekciju
            PopuniPadajuceListe();
        }
        void PopuniPadajuceListe()
        {
            try
            {
                konekcija.Open();
                string vratiZaposleniID = @"select ZaposleniID,ImeZaposlenog+ ' ' + PrezimeZaposlenog+ ' ' + JMBGZaposlenog as Zaposleni from tblZaposleni";
                SqlDataAdapter daZaposlenog = new SqlDataAdapter(vratiZaposleniID, konekcija);
                DataTable dtZaposlenog = new DataTable();
                daZaposlenog.Fill(dtZaposlenog);
                cbZaposleniID.ItemsSource = dtZaposlenog.DefaultView;
                daZaposlenog.Dispose();
                daZaposlenog.Dispose();//zasto dva puta

                string VratiRacune = @"select RacunID,BrojRacuna from tblRacun";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daRacuna = new SqlDataAdapter(VratiRacune, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtRacuna = new DataTable();//privremena promenljiva tipa datatable
                daRacuna.Fill(dtRacuna);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbRacunID.ItemsSource = dtRacuna.DefaultView;
                daRacuna.Dispose();

                dtRacuna.Dispose();

                string VratiTipTransakcijeID = @"select TipTransakcijeID,NazivTipaTransakcije from tblTipTransakcije;";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daTipTransakcijeID = new SqlDataAdapter(VratiTipTransakcijeID, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtTipTransakcijeID = new DataTable();//privremena promenljiva tipa datatable
                daTipTransakcijeID.Fill(dtTipTransakcijeID);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbTipTransakcijeID.ItemsSource = dtTipTransakcijeID.DefaultView;
                daTipTransakcijeID.Dispose();
                daTipTransakcijeID.Dispose();//zasto dva put


             

            }
            catch (SqlException)
            {
                MessageBox.Show("Padajuce liste nisu popunjene!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Odaberite datum!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }

            dpDatumTransakcije.Focus();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIznosTransakcije.Text) ||
       string.IsNullOrEmpty(txtVremeTransakcije.Text) ||
       cbRacunID.SelectedValue == null ||
       cbZaposleniID.SelectedValue == null ||
       cbTipTransakcijeID.SelectedValue == null)
            {
                MessageBox.Show("Popunite sva polja pre nego što sačuvate transakciju.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!txtIznosTransakcije.Text.All(char.IsDigit))
            {
                MessageBox.Show("Stanje na računu mora sadržavati samo cifre.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                konekcija.Open();
                DateTime date = (DateTime)dpDatumTransakcije.SelectedDate;
                string datum = date.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
           
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
               
                cmd.Parameters.Add("@datum", SqlDbType.Date).Value = DateTime.ParseExact(datum, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@IznosTransakcije", SqlDbType.Money).Value = decimal.Parse(txtIznosTransakcije.Text);
                cmd.Parameters.Add("@VremeTransakcije", SqlDbType.NVarChar, 15).Value = txtVremeTransakcije.Text;
                cmd.Parameters.Add("@RacunID", SqlDbType.Int).Value = Convert.ToInt32(cbRacunID.SelectedValue);
                cmd.Parameters.Add("@ZaposleniID", SqlDbType.Int).Value = Convert.ToInt32(cbZaposleniID.SelectedValue);
                cmd.Parameters.Add("@TipTransakcijeID", SqlDbType.Int).Value = Convert.ToInt32(cbTipTransakcijeID.SelectedValue);

              


                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblTransakcija Set DatumTransakcije=@datum, IznosTransakcije=@IznosTransakcije,
                                       VremeTransakcije=@VremeTransakcije, RacunID=@RacunID, ZaposleniID=@ZaposleniID,
                                        TipTransakcijeID=@TipTransakcijeID Where TransakcijaID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblTransakcija(DatumTransakcije, IznosTransakcije, VremeTransakcije, RacunID, ZaposleniID, TipTransakcijeID)
                values(@datum,@IznosTransakcije,@VremeTransakcije,@RacunID,@ZaposleniID,@TipTransakcijeID);";

                }

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            /* catch (SqlException)
             {
                 MessageBox.Show("Unos odredjenih vrednosti nije validan", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

             }*/
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL greška: {ex}");
                MessageBox.Show($"Unos određenih vrednosti nije validan\nDetalji greške:\n{ex}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("odaberite datum", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

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
