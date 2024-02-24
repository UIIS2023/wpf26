using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for frmKlijent.xaml
    /// </summary>
    public partial class frmKlijent : Window
    {
        SqlConnection konekcija = new SqlConnection();
         bool azuriraj;
        DataRowView pomocniRed;
        Konekcija kon = new Konekcija();
     
        public frmKlijent()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            txtIme.Focus();
            txtKontakt.Text = "(+381)";

        }

        public frmKlijent(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
        }
       
        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)//dva klika na button kreira se funkcija klik i u u button sacuvaj doda atribut klik
        {
            string kontaktBroj = txtKontakt.Text.Substring(6); // Preskačemo "(+381)"
            if (!txtKontakt.Text.StartsWith("(+381)") || !System.Text.RegularExpressions.Regex.IsMatch(kontaktBroj, "^[0-9]{9}$"))
            {
                MessageBox.Show("Broj telefona mora počinjati sa (+381) i sadržavati tačno 9 cifara nakon toga.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!kontaktBroj.All(char.IsDigit))
            {
                MessageBox.Show("Broj telefona mora sadržavati samo cifre.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtIme.Text) ||
                  string.IsNullOrEmpty(txtPrezime.Text) ||
                  string.IsNullOrEmpty(txtJMBG.Text) ||
                  string.IsNullOrEmpty(txtKontakt.Text) ||
                  string.IsNullOrEmpty(txtAdresa.Text) ||
                  string.IsNullOrEmpty(txtGrad.Text))
            {
                MessageBox.Show("Molimo popunite sva polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Provera jedinstvenosti JMBG-a pre dodavanja ili ažuriranja
            if (!ProveriJMBGJedinstvenost(txtJMBG.Text))
            {
                MessageBox.Show("JMBG već postoji u bazi podataka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
          

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtJMBG.Text, "^[0-9]{13}$"))
            {
                MessageBox.Show("JMBG mora sadržavati tačno 13 cifara i biti broj.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                konekcija.Open();//otvorili konekkciju
                SqlCommand cmd = new SqlCommand()
                {
                    Connection = konekcija,//dodelili konekciju nad kojom ce se izvrsavati komanda
                };
            



                    cmd.Parameters.Add("@ImeKlijenta", SqlDbType.NVarChar).Value = txtIme.Text;//definisali koje parametere cemo imati
                cmd.Parameters.Add("@PrezimeKlijenta", SqlDbType.NVarChar).Value = txtPrezime.Text;
                cmd.Parameters.Add("@JMBGKlijenta", SqlDbType.NVarChar).Value = txtJMBG.Text;
                cmd.Parameters.Add("@KontaktKlijenta", SqlDbType.NVarChar).Value = txtKontakt.Text;
                cmd.Parameters.Add("@AdresaKlijenta", SqlDbType.NVarChar).Value = txtAdresa.Text;
                cmd.Parameters.Add("@GradKlijenta", SqlDbType.NVarChar).Value = txtGrad.Text;
               
            
                if(this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblKlijent Set ImeKlijenta=@ImeKlijenta, PrezimeKlijenta=@PrezimeKlijenta,
                                       JMBGKlijenta=@JMBGKlijenta, KontaktKlijenta=@KontaktKlijenta, AdresaKlijenta=@AdresaKlijenta,
                                        GradKlijenta=@GradKlijenta Where KlijentID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into
                    tblKlijent (ImeKlijenta,PrezimeKlijenta, JMBGKlijenta, KontaktKlijenta, AdresaKlijenta, GradKlijenta)
                    values(@ImeKlijenta, @PrezimeKlijenta, @JMBGKlijenta, @KontaktKlijenta,@AdresaKlijenta, @GradKlijenta);";
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
            finally
            {
                if( konekcija != null )
                {
                    konekcija.Close();
                }
            }
        }

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
     
        private bool ProveriJMBGJedinstvenost(string jmbg)
        {
            try
            {
                konekcija.Open();

                SqlCommand cmd;
                if (this.azuriraj)
                {
                    // Ako ažuriramo, dozvoli isti JMBG za trenutnog klijenta
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblKlijent WHERE JMBGKlijenta = @JMBG AND KlijentID <> @ID", konekcija);
                    cmd.Parameters.AddWithValue("@ID", this.pomocniRed["ID"]);
                }
                else
                {
                    // Inače, proveri jedinstvenost JMBG-a
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblKlijent WHERE JMBGKlijenta = @JMBG", konekcija);
                }

                cmd.Parameters.AddWithValue("@JMBG", jmbg);

                int brojKlijenataSaJMBG = (int)cmd.ExecuteScalar();

                return brojKlijenataSaJMBG == 0; // Ako je 0, JMBG je jedinstven
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL greška: {ex}");
                MessageBox.Show($"Greška pri proveri jedinstvenosti JMBG-a.\nDetalji greške:\n{ex}", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
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
