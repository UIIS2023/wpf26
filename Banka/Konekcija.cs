using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Banka
{
     class Konekcija
    {
        public SqlConnection KreirajKonekciju()//sqlconnection nam omogucava da postavimo parametre za konekciju na bazu
        {
            SqlConnectionStringBuilder ccnSb = new SqlConnectionStringBuilder()
            {
                DataSource = @"jana\SQLEXPRESS",//lokalni server racunara
                InitialCatalog = "Banka",//baza na lokalnom serveru
                IntegratedSecurity = true// autentizacija-ukoliko se nalazi na lokalnoj masnini onda se postavi na true
            };
            string con= ccnSb.ToString();
            SqlConnection konekcija= new SqlConnection(con);//prosledjujemo parametre za konekciju
            return konekcija; //Overa metoda vraca konekciju
        }
    }
}
