using Banka.Forme;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Banka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Select upiti
        string klijentSelect = @"Select KlijentID as ID, ImeKlijenta as Ime, PrezimeKlijenta as Prezime, JMBGKlijenta as JMBG, KontaktKlijenta as Kontakt, AdresaKlijenta as Adresa, GradKlijenta as Grad from tblKlijent;";

        string zaposleniSelect = @"Select ZaposleniID as ID, ImeZaposlenog as 'Ime', PrezimeZaposlenog as 'Prezime', GradZaposlenog as 'Grad', JMBGZaposlenog as 'JMBG', KontaktZaposlenog as Kontakt from tblZaposleni;";

        string racunSelect = @"Select RacunID as ID, StanjeNaRacunu as 'Stanje na racunu', BrojRacuna as 'Broj racuna', NazivTipaRacuna 'Tip racuna', ImeKlijenta + ' ' +PrezimeKlijenta as Klijent
                                from tblRacun join tblTipRacuna on tblRacun.TipRacunaID=tblTipRacuna.TipRacunaID
                                              join tblKlijent on tblRacun.KlijentID=tblKlijent.KlijentID;";

        string tipRacunaSelect = @"Select TipRacunaID as ID, NazivTipaRacuna as 'Tip racuna' from tblTipRacuna;";

        string karticaSelect = @"Select KarticaID as ID, BrojRacuna as 'Broj racuna', NazivTipaKartice as 'Tip kartice'
                                from tblKartica join tblRacun on tblKartica.RacunID=tblRacun.RacunID
                                              join tblTipKartice on tblKartica.TipKarticeID=tblTipKartice.TipKarticeID;";

        string tipKarticeSelect = @"Select TipKarticeID as ID, NazivTipaKartice as 'Tip kartice' from tblTipKartice;";

        string transakcijaSelect = @"Select TransakcijaID as ID, DatumTransakcije as 'Datum transakcije', VremeTransakcije as 'Vreme transakcije', IznosTransakcije as 'Iznos', BrojRacuna as 'Racun', ImeZaposlenog + ' ' + PrezimeZaposlenog as Zaposleni, NazivTipaTransakcije as 'Tip transakcije'
                                from tblTransakcija join tblRacun on tblTransakcija.RacunID=tblRacun.RacunID
                                              join tblZaposleni on tblTransakcija.ZaposleniID=tblZaposleni.ZaposleniID
                                                join tblTipTransakcije on tblTransakcija.TipTransakcijeID=tblTipTransakcije.TipTransakcijeID;";

        string tipTransakcijeSelect = @"Select TipTransakcijeID as ID, NazivTipaTransakcije as 'Tip transakcije' from tblTipTransakcije;";

        string kreditSelect = @"Select KreditID as ID, IznosKredita as 'Iznos kredita', RataKredita as 'Rata', KamataKredita as Kamata, PocetakOtplacivanja as 'Pocetak otplacivanja', ZavrsetakOtplacivanja as 'Zavrsetak otplacivanja', BrojRacuna as 'Racun'
                                from tblKredit join tblRacun on tblKredit.RacunID=tblRacun.RacunID;";




        #endregion

        #region Select upiti sa uslovom

        string selectUslovKlijent = @"Select * from tblKlijent where KlijentID=";

        string selectUslovZaposleni = @"Select * from tblZaposleni where ZaposleniID=";

        string selectUslovRacun = @"Select * from tblRacun where RacunID=";

        string selectUslovTipRacuna = @"Select * from tblTipRacuna where TipRacunaID=";

        string selectUslovKartica = @"Select * from tblKartica where KarticaID=";

        string selectUslovTipKartice = @"Select * from tblTipKartice where TipKarticeID=";

        string selectUslovTransakcija = @"Select * from tblTransakcija where TransakcijaID=";

        string selectUslovTipTransakcije = @"Select * from tblTipTransakcije where TipTransakcijeID=";

        string selectUslovKredit = @"Select * from tblKredit where KreditID=";




        #endregion

        #region Delete upiti
        string klijentDelete = @"Delete from tblKlijent where KlijentID=";
        string zaposleniDelete = @"Delete from tblZaposleni where ZaposleniID=";
        string racunDelete = @"Delete from tblRacun where RacunID=";
        string tipRacunaDelete = @"Delete from tblTipRacuna where TipRacunaID=";
        string karticaDelete = @"Delete from tblKartica where KarticaID=";
        string tipKarticeDelete = @"Delete from tblTipKartice where TipKarticeID=";
        string transakcijaDelete = @"Delete from tblTransakcija where TransakcijaID=";
        string tipTransakcijeDelete = @"Delete from tblTipTransakcije where TipTransakcijeID=";
        string kreditDelete = @"Delete from tblKredit where KreditID=";


        #endregion

        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        string ucitanaTabela;
        bool azuriraj;
        DataRowView pomocni;
        private DataRowView? pomocniRed;

        public MainWindow()
        {
            

            InitializeComponent();
            konekcija = kon.KreirajKonekciju();

            // Otvaranje prozora za prijavljivanje
            Forme.Login loginWindow = new Forme.Login();
            loginWindow.ShowDialog(); // Blokirajući poziv, čeka dok se prozor za prijavljivanje ne zatvori

            if (loginWindow.DialogResult == true)
            {
                // Ako je rezultat prijavljivanja tačan, otvori MainWindow
                UcitajPodatke(dataGridCentral, klijentSelect);
            }
            else
            {
                // Ako korisnik otkaže prijavljivanje, zatvori aplikaciju
                Application.Current.Shutdown();
            }



        }
        void UcitajPodatke (DataGrid grid, string selectUpit)//izvucemo iz baze i smestimo u ovaj grid
        {
            try
            {//slicno kao smestanje u padajucu listu
               
                konekcija.Open();//inicijalizujemo objekat klase SqlDataadapter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectUpit, konekcija);//konstruktoru prosledimo koji upit i nad kojom bazom treba da se izvrsi
                DataTable dataTable = new DataTable();//privremmena tabela i u nju smestimo podatke koji su izvuceni iz baze
                //
                dataAdapter.Fill(dataTable);
                //
                ucitanaTabela = selectUpit;
               
                if(grid != null)//treba da kazemo da je izvor podataka za pocetnu tabelu ono sto smo dobili iz baze i sto se nalazi u pomocnoj
                {
                    grid.ItemsSource = dataTable.DefaultView;//ucitali smo podatke koji su nam potrebni
                }
                
              

            }
            catch (SqlException)
            {
            MessageBox.Show("Neuspesno ucitani podaci", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
        private void btnKlijent_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, klijentSelect);
        }

        private void btnZaposleni_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, zaposleniSelect);
        }

        private void btnRacun_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, racunSelect);
        }

        private void btnTipRacuna_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, tipRacunaSelect);
        }

        private void btnKartica_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, karticaSelect);
        }

        private void btnTipKartice_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, tipKarticeSelect);
        }

        private void btnTransakcija_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, transakcijaSelect);
        }

        private void btnTipTransakcije_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, tipTransakcijeSelect);
        }

        private void btnKredit_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentral, kreditSelect);
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            Window prozor;
           if(ucitanaTabela.Equals(klijentSelect))
            {
                prozor = new frmKlijent();//kako je moguce da ovo napisem- zato sto forma klijent je podklasa mejnwindow
                prozor.ShowDialog();//modalnost dijaloga
                UcitajPodatke(dataGridCentral, klijentSelect);
            }else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                prozor = new frmZaposleni();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                prozor = new frmRacun();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, racunSelect);
            }
            else if (ucitanaTabela.Equals(tipRacunaSelect))
            {
                prozor = new frmTipRacuna();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, tipRacunaSelect);
            }
            else if (ucitanaTabela.Equals(karticaSelect))
            {
                prozor = new frmKartica();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, karticaSelect);
            }
            else if (ucitanaTabela.Equals(tipKarticeSelect))
            {
                prozor = new frmTipKartice();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, tipKarticeSelect);
            }
            else if (ucitanaTabela.Equals(transakcijaSelect))
            {
                prozor = new frmTransakcije();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, transakcijaSelect);
            }
            else if (ucitanaTabela.Equals(tipTransakcijeSelect))
            {
                prozor = new frmTipTransakcije();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, tipTransakcijeSelect);
            }
            else if (ucitanaTabela.Equals(kreditSelect))
            {
                prozor = new frmKredit();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentral, kreditSelect);
            }


        }

        private void btnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(klijentSelect))
            {
                ObrisiZapis(dataGridCentral, klijentDelete);
                UcitajPodatke(dataGridCentral, klijentSelect);

            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                ObrisiZapis(dataGridCentral, zaposleniDelete);
                UcitajPodatke(dataGridCentral, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                ObrisiZapis(dataGridCentral, racunDelete);
                UcitajPodatke(dataGridCentral, racunSelect);
            }
            else if (ucitanaTabela.Equals(tipRacunaSelect))
            {
                ObrisiZapis(dataGridCentral, tipRacunaDelete);
                UcitajPodatke(dataGridCentral, tipRacunaSelect);
            }
            else if (ucitanaTabela.Equals(karticaSelect))
            {
                ObrisiZapis(dataGridCentral, karticaDelete);
                UcitajPodatke(dataGridCentral, karticaSelect);
            }
            else if (ucitanaTabela.Equals(tipKarticeSelect))
            {
                ObrisiZapis(dataGridCentral, tipKarticeDelete);
                UcitajPodatke(dataGridCentral, tipKarticeSelect);
            }
            else if (ucitanaTabela.Equals(transakcijaSelect))
            {
                ObrisiZapis(dataGridCentral, transakcijaDelete);
                UcitajPodatke(dataGridCentral, transakcijaSelect);
            }
            else if (ucitanaTabela.Equals(tipTransakcijeSelect))
            {
                ObrisiZapis(dataGridCentral, tipTransakcijeDelete);
                UcitajPodatke(dataGridCentral, tipTransakcijeSelect);
            }
            else if (ucitanaTabela.Equals(kreditSelect))
            {
                ObrisiZapis(dataGridCentral, kreditDelete);
                UcitajPodatke(dataGridCentral, kreditSelect);
            }
        }

        private void ObrisiZapis(DataGrid grid, object deleteUpit)
        {
            try
            {
                konekcija.Open();
                DataRowView red = (DataRowView)grid.SelectedItems[0];//da nadjemo selektovani red

                MessageBoxResult rezultat = MessageBox.Show("Da li ste sigurni?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (rezultat == MessageBoxResult.Yes)
                {
                    //treba nam sql komanda , inicijalizujemo objetak klse u konstruktoru prosledjujemo konekciju
                    SqlCommand cmd = new SqlCommand
                    {
                        Connection = konekcija
                    };
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];//iz selektovanog reda da ono sto se nalazi u polju kolone ID
                    cmd.CommandText = deleteUpit + "@ID";
                    cmd.ExecuteNonQuery();//izvrsavamo upit
                }


            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(SqlException)
            {
                MessageBox.Show("Postoje povezani podaci u drugim tabelama", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
        void PopuniFormu (DataGrid grid, string selectUslov)
        {
            try
            {
                konekcija.Open();
                azuriraj = true;
                DataRowView red = (DataRowView)grid.SelectedItems[0];
                pomocniRed = red;
                SqlCommand komanda = new SqlCommand
                {
                    Connection = konekcija
                };
                komanda.Parameters.Add("ID", SqlDbType.Int).Value = red["ID"];
                komanda.CommandText = selectUslov + "@ID";
                SqlDataReader citac = komanda.ExecuteReader();
                komanda.Dispose();
                while(citac.Read())
                {
                    if (ucitanaTabela.Equals(klijentSelect, StringComparison.Ordinal))
                    {
                        frmKlijent prozorKlijent= new frmKlijent(azuriraj, pomocniRed);
                        prozorKlijent.txtIme.Text = citac["ImeKlijenta"].ToString();
                        prozorKlijent.txtPrezime.Text = citac["PrezimeKlijenta"].ToString();
                        prozorKlijent.txtJMBG.Text = citac["JMBGKlijenta"].ToString();
                        prozorKlijent.txtKontakt.Text = citac["KontaktKlijenta"].ToString();
                        prozorKlijent.txtAdresa.Text = citac["AdresaKlijenta"].ToString();
                        prozorKlijent.txtGrad.Text = citac["GradKlijenta"].ToString();
                        prozorKlijent.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(zaposleniSelect, StringComparison.Ordinal))
                    {
                        frmZaposleni prozorZaposleni = new frmZaposleni(azuriraj, pomocniRed);
                        prozorZaposleni.txtImeZaposlenog.Text = citac["ImeZaposlenog"].ToString();
                        prozorZaposleni.txtPrezimeZaposlenog.Text = citac["PrezimeZaposlenog"].ToString();
                        prozorZaposleni.txtGradZaposlenog.Text = citac["GradZaposlenog"].ToString();
                        prozorZaposleni.txtJMBGZaposlenog.Text = citac["JMBGZaposlenog"].ToString();
                        prozorZaposleni.txtKontaktZaposlenog.Text = citac["KontaktZaposlenog"].ToString();
                        prozorZaposleni.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(racunSelect, StringComparison.Ordinal))
                    {
                        frmRacun prozorRacun = new frmRacun(azuriraj, pomocniRed);
                        prozorRacun.txtStanjeNaRacunu.Text = citac["StanjeNaRacunu"].ToString();
                        prozorRacun.txtBrojRacuna.Text = citac["BrojRacuna"].ToString();
                       prozorRacun.cbTipRacunaID.SelectedValue = citac["TipRacunaID"].ToString();
                        prozorRacun.cbKlijentID.SelectedValue = citac["KlijentID"].ToString();
                        prozorRacun.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(tipRacunaSelect, StringComparison.Ordinal))
                    {
                        frmTipRacuna prozorTipRacuna = new frmTipRacuna(azuriraj, pomocniRed);
                        prozorTipRacuna.txtNazivTipaRacuna.Text = citac["NazivTipaRacuna"].ToString();
                        prozorTipRacuna.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(karticaSelect, StringComparison.Ordinal))
                    {
                        frmKartica prozorKartica = new frmKartica(azuriraj, pomocniRed);
                        prozorKartica.cbRacunID.SelectedValue = citac["RacunID"].ToString();
                        prozorKartica.cbTipKarticeID.SelectedValue = citac["TipKarticeID"].ToString();
                        prozorKartica.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(tipKarticeSelect, StringComparison.Ordinal))
                    {
                        frmTipKartice prozorTipKartice = new frmTipKartice(azuriraj, pomocniRed);
                        prozorTipKartice.txtNazivTipaKartice.Text = citac["NazivTipaKartice"].ToString();
                        prozorTipKartice.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(transakcijaSelect, StringComparison.Ordinal))
                    {
                        frmTransakcije prozorTransakcija = new frmTransakcije(azuriraj, pomocniRed);
                        prozorTransakcija.dpDatumTransakcije.SelectedDate= (DateTime) citac["DatumTransakcije"];
                        prozorTransakcija.txtVremeTransakcije.Text = citac["VremeTransakcije"].ToString();
                        prozorTransakcija.txtIznosTransakcije.Text = citac["IznosTransakcije"].ToString();
                        prozorTransakcija.cbRacunID.SelectedValue = citac["RacunID"].ToString();
                        prozorTransakcija.cbZaposleniID.SelectedValue = citac["ZaposleniID"].ToString();
                        prozorTransakcija.cbTipTransakcijeID.SelectedValue = citac["TipTransakcijeID"].ToString();
                        prozorTransakcija.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(tipTransakcijeSelect, StringComparison.Ordinal))
                    {
                        frmTipTransakcije prozorTipTransakcije = new frmTipTransakcije(azuriraj, pomocniRed);
                        prozorTipTransakcije.txtNazivTipaTransakcije.Text = citac["NazivTipaTransakcije"].ToString();
                        prozorTipTransakcije.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(kreditSelect, StringComparison.Ordinal))
                    {
                        frmKredit prozorKredit = new frmKredit(azuriraj, pomocniRed);
                        prozorKredit.txtIznos.Text = citac["IznosKredita"].ToString();
                        prozorKredit.txtRata.Text = citac["RataKredita"].ToString();
                        prozorKredit.txtKamata.Text = citac["KamataKredita"].ToString();
                        prozorKredit.dpPocetakOtplacivanja.SelectedDate = (DateTime)citac["PocetakOtplacivanja"];
                        prozorKredit.dpZavrsetakOtplacivanja.SelectedDate = (DateTime)citac["ZavrsetakOtplacivanja"];
                        prozorKredit.cbRacunID.SelectedValue = citac["RacunID"].ToString();
                        prozorKredit.ShowDialog();
                    }
                }



            }
            catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(konekcija != null)
                {
                    konekcija.Close();
                }
                azuriraj = false;
            }
        }
        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(klijentSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovKlijent);
                UcitajPodatke(dataGridCentral, klijentSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovZaposleni);
                UcitajPodatke(dataGridCentral, zaposleniSelect);
            }
            else if (ucitanaTabela.Equals(racunSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovRacun);
                UcitajPodatke(dataGridCentral, racunSelect);
            }
            else if (ucitanaTabela.Equals(tipRacunaSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovTipRacuna);
                UcitajPodatke(dataGridCentral, tipRacunaSelect);
            }
            else if (ucitanaTabela.Equals(karticaSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovKartica);
                UcitajPodatke(dataGridCentral, karticaSelect);
            }
            else if (ucitanaTabela.Equals(tipKarticeSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovTipKartice);
                UcitajPodatke(dataGridCentral, tipKarticeSelect);
            }
            else if (ucitanaTabela.Equals(transakcijaSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovTransakcija);
                UcitajPodatke(dataGridCentral, transakcijaSelect);
            }
            else if (ucitanaTabela.Equals(tipTransakcijeSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovTipTransakcije);
                UcitajPodatke(dataGridCentral, tipTransakcijeSelect);
            }
            else if (ucitanaTabela.Equals(kreditSelect))
            {
                PopuniFormu(dataGridCentral, selectUslovKredit);
                UcitajPodatke(dataGridCentral, kreditSelect);
            }
        }
    }
}
