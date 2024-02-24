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
    /// Interaction logic for frmZaposleni.xaml
    /// </summary>
    public partial class frmZaposleni : Window
    {
        SqlConnection konekcija = new SqlConnection();
        Konekcija kon = new Konekcija();
        DataRowView pomocniRed;
        bool azuriraj;
        public frmZaposleni()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            txtImeZaposlenog.Focus();
            txtKontaktZaposlenog.Text = "(+381)";
        }
        public frmZaposleni(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            
        }
        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            string kontaktBroj = txtKontaktZaposlenog.Text.Substring(6); // Preskačemo "(+381)"
            if (!txtKontaktZaposlenog.Text.StartsWith("(+381)") || !System.Text.RegularExpressions.Regex.IsMatch(kontaktBroj, "^[0-9]{9}$"))
            {
                MessageBox.Show("Broj telefona mora počinjati sa (+381) i sadržavati tačno 9 cifara nakon toga.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!kontaktBroj.All(char.IsDigit))
            {
                MessageBox.Show("Broj telefona mora sadržavati samo cifre.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtImeZaposlenog.Text) ||
              string.IsNullOrEmpty(txtPrezimeZaposlenog.Text) ||
              string.IsNullOrEmpty(txtGradZaposlenog.Text) ||
              string.IsNullOrEmpty(txtJMBGZaposlenog.Text) ||
              string.IsNullOrEmpty(txtKontaktZaposlenog.Text))
            {
                MessageBox.Show("Molimo popunite sva polja.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!ProveriJMBGJedinstvenost(txtJMBGZaposlenog.Text))
            {
                MessageBox.Show("JMBG već postoji u bazi podataka.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtJMBGZaposlenog.Text, "^[0-9]{13}$"))
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
                cmd.Parameters.Add("@ImeZaposlenog", SqlDbType.NVarChar).Value = txtImeZaposlenog.Text;              
                cmd.Parameters.Add("@PrezimeZaposlenog", SqlDbType.NVarChar).Value = txtPrezimeZaposlenog.Text;
                cmd.Parameters.Add("@GradZaposlenog", SqlDbType.NVarChar).Value = txtGradZaposlenog.Text;
                cmd.Parameters.Add("@JMBGZaposlenog", SqlDbType.NVarChar).Value = txtJMBGZaposlenog.Text;
                cmd.Parameters.Add("@KontaktZaposlenog", SqlDbType.NVarChar).Value = txtKontaktZaposlenog.Text;


                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"Update tblZaposleni Set ImeZaposlenog=@ImeZaposlenog, PrezimeZaposlenog=@PrezimeZaposlenog,
                                       GradZaposlenog=@GradZaposlenog, JMBGZaposlenog=@JMBGZaposlenog,     
                                        KontaktZaposlenog=@KontaktZaposlenog Where ZaposleniID=@ID";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into
                    tblZaposleni (ImeZaposlenog,PrezimeZaposlenog, GradZaposlenog,JMBGZaposlenog, KontaktZaposlenog)
                    values(@ImeZaposlenog, @PrezimeZaposlenog, @GradZaposlenog, @JMBGZaposlenog,@KontaktZaposlenog);";
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
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
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
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblZaposleni WHERE JMBGZaposlenog = @JMBG AND ZaposleniID <> @ID", konekcija);
                    cmd.Parameters.AddWithValue("@ID", this.pomocniRed["ID"]);
                }
                else
                {
                    // Inače, proveri jedinstvenost JMBG-a
                    cmd = new SqlCommand("SELECT COUNT(*) FROM tblZaposleni WHERE JMBGZaposlenog = @JMBG", konekcija);
                }

                cmd.Parameters.AddWithValue("@JMBG", jmbg);

                int brojZaposlenihSaJMBG = (int)cmd.ExecuteScalar();

                return brojZaposlenihSaJMBG == 0; // Ako je 0, JMBG je jedinstven
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
