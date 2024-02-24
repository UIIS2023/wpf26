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
    /// Interaction logic for frmRacun.xaml
    /// </summary>
    public partial class frmRacun : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        bool azuriraj;
        DataRowView pomocniRed;

        public frmRacun(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            PopuniPadajuceListe();
        }
        public frmRacun()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
        }
        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();
                string VratiTipRacuna = @"select TipRacunaID,NazivTipaRacuna from tblTipRacuna;";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daTipRacuna = new SqlDataAdapter(VratiTipRacuna, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtTipRacuna = new DataTable();//privremena promenljiva tipa datatable
                daTipRacuna.Fill(dtTipRacuna);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbTipRacunaID.ItemsSource = dtTipRacuna.DefaultView;
                dtTipRacuna.Dispose();
                daTipRacuna.Dispose();

                string VratiKlijenta = @"select KlijentID,ImeKlijenta+' '+PrezimeKlijenta+ ' ' + JMBGKlijenta as Klijent from tblKlijent;";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daKlijenta = new SqlDataAdapter(VratiKlijenta, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtKlijenta = new DataTable();//privremena promenljiva tipa datatable
                daKlijenta.Fill(dtKlijenta);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbKlijentID.ItemsSource = dtKlijenta.DefaultView;
                dtKlijenta.Dispose();
                daKlijenta.Dispose();

            }
            catch
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
            txtStanjeNaRacunu.Focus();
        }
        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtStanjeNaRacunu.Text) ||
           string.IsNullOrEmpty(txtBrojRacuna.Text) ||
           cbTipRacunaID.SelectedValue == null ||
           cbKlijentID.SelectedValue == null)
            {
                MessageBox.Show("Molimo popunite sva polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }
            if (!txtStanjeNaRacunu.Text.All(char.IsDigit))
            {
                MessageBox.Show("Stanje na računu mora sadržavati samo cifre.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ProveriBrojRacunaJedinstvenost(txtBrojRacuna.Text))
            {
                MessageBox.Show("Broj računa već postoji u bazi podataka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
          
            try
            {
                konekcija.Open();
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
                cmd.Parameters.Add("@StanjeNaRacunu", SqlDbType.Money).Value = txtStanjeNaRacunu.Text;
                cmd.Parameters.Add("@BrojRacuna", SqlDbType.NVarChar).Value = txtBrojRacuna.Text;
                cmd.Parameters.Add("@TipRacunaID", SqlDbType.Int).Value = cbTipRacunaID.SelectedValue;
                cmd.Parameters.Add("@KlijentID", SqlDbType.Int).Value = cbKlijentID.SelectedValue;
             
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblRacun Set StanjeNaRacunu=@StanjeNaRacunu, BrojRacuna=@BrojRacuna,
                                            TipRacunaID=@TipRacunaID, KlijentID=@KlijentID  Where RacunID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblRacun(StanjeNaRacunu, BrojRacuna, TipRacunaID, KlijentID)
                values(@StanjeNaRacunu,@BrojRacuna,@TipRacunaID,@KlijentID);";

                }
            
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
         /*   catch(SqlException)
            {
                MessageBox.Show("Unos odredjenih vrednosti nije validan", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);

            }*/
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL greška: {ex.ToString()}");
                MessageBox.Show($"Unos određenih vrednosti nije validan\nDetalji greške:\n{ex.ToString()}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }

            }
        }

        private bool ProveriBrojRacunaJedinstvenost(string brojRacuna)
        {
            try
            {
                konekcija.Open();

                SqlCommand cmd;
                if (this.azuriraj)
                {
                    // Ako ažuriramo, dozvoli isti broj računa za trenutni račun
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblRacun WHERE BrojRacuna = @BrojRacuna AND RacunID <> @ID", konekcija);
                    cmd.Parameters.AddWithValue("@ID", this.pomocniRed["ID"]);
                }
                else
                {
                    // Inače, proveri jedinstvenost broja računa
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblRacun WHERE BrojRacuna = @BrojRacuna", konekcija);
                }

                cmd.Parameters.AddWithValue("@BrojRacuna", brojRacuna);

                int brojRacunaSaBrojem = (int)cmd.ExecuteScalar();

                return brojRacunaSaBrojem == 0; // Ako je 0, broj računa je jedinstven
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL greška: {ex}");
                MessageBox.Show($"Greška pri proveri jedinstvenosti broja računa.\nDetalji greške:\n{ex}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Vratite false u slučaju greške
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
