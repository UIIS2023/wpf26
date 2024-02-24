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
    /// Interaction logic for frmKartica.xaml
    /// </summary>
    public partial class frmKartica : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        bool azuriraj;
        DataRowView pomocniRed;

        public frmKartica(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            PopuniPadajuceListe();
        }
        public frmKartica()
        { 
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            PopuniPadajuceListe();
        }
        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija.Open();
                string VratiRacunID = @"select RacunID,BrojRacuna from tblRacun;";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daRacunID = new SqlDataAdapter(VratiRacunID, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtRacunID = new DataTable();//privremena promenljiva tipa datatable
                daRacunID.Fill(dtRacunID);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbRacunID.ItemsSource = dtRacunID.DefaultView;
                dtRacunID.Dispose();
                daRacunID.Dispose();//zasto dva put

                string VratiTipKarticeID = @"select TipKarticeID,NazivTipaKartice from tblTipKartice;";//upit nad bazom koji ce nam iscitati vozila
                SqlDataAdapter daTipKarticeID = new SqlDataAdapter(VratiTipKarticeID, konekcija);//konstruktoru prosledimo koju komandu zelimo da izvrsimo i nad kojom bazom
                DataTable dtTipKarticeID = new DataTable();//privremena promenljiva tipa datatable
                daTipKarticeID.Fill(dtTipKarticeID);// sada smo prebacili podatke u privremenu
                //izvor podataka padajuce liste- pozivamo preko nejma -izvor je ono sto se trenutno nalzi u datatable-dtRacuna
                cbTipKarticeID.ItemsSource = dtTipKarticeID.DefaultView;
                dtTipKarticeID.Dispose();
                daTipKarticeID.Dispose();//zasto dva put

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
            cbRacunID.Focus();

        }
        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (cbRacunID.SelectedValue == null || cbTipKarticeID.SelectedValue == null)
            {
                MessageBox.Show("Popunite sva polja pre nego što sačuvate karticu.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                konekcija.Open();
                SqlCommand cmd = new SqlCommand
                {
                    Connection = konekcija
                };
                cmd.Parameters.Add("@RacunID", SqlDbType.Int).Value = cbRacunID.SelectedValue;
                cmd.Parameters.Add("@TipKarticeID", SqlDbType.Int).Value = cbTipKarticeID.SelectedValue;
               
                
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblKartica Set RacunID=@RacunID, TipKarticeID=@TipKarticeID
                                       Where KarticaID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblKartica(RacunID, TipKarticeID)
                values(@RacunID,@TipKarticeID);";

                }

                cmd.ExecuteNonQuery();
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
