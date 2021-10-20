using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.IO;
using System.Net;
using System.Web;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            //WebApi();
            StringLos();

        }

        static void StringLos()
        {
            //bruges til indsættelse i database
            int toc = 0;
            int frc = 1;
            int rat = 2;
            //bruges til at kigge på indholdet på en hjemmeside
            WebClient client = new WebClient();
            //den bestemte hjemmeside
            String downloadedString = client.DownloadString("https://valutakurser.azurewebsites.net/valutakurs");
            //til at fjerne bestemte tegn fra stringen
            var chartorem = new string[] {"valutaKurser", "'", "{", "}","fromCurrency", "toCurrency", "rate", "[", "]", "\"" };
            foreach (var c in chartorem)
            {
                downloadedString = downloadedString.Replace(c, string.Empty);
            }
            //bruges til at finde datoen i stringen
            int datePos = downloadedString.IndexOf("2021");
            string UpdatedAt = downloadedString.Substring(datePos);
            chartorem = new string[] {":"};
            foreach (var c in chartorem)
            {
                downloadedString = downloadedString.Replace(c, string.Empty);
            }
            //sat alt data fra stringen ind i et array
            string[] colle = downloadedString.Split(',');
            //send data fra array til database indsættelse
            int lang = (colle.Length - 1) / 3;
            for (int x = 0; x < lang; x++)
            {
                Console.WriteLine("{0} {1} {2}", colle[toc], colle[frc], colle[rat]);
                CreateCommand(colle[frc], colle[toc], UpdatedAt, colle[rat]);
                toc += 3;
                frc += 3;
                rat += 3;
            }
        }

        static void WebApi()
        {
            Console.WriteLine("hej");


        }

        private static void CreateCommand(string FromCurrency, string ToCurrency, string UpdatedAt, string Rate)
        {
            string SQL = "Select * from ValutaKurser";
            DateTime now = DateTime.Now;
            //string nu = "2021-10-16T22:24:54.3969037Z";
            string nu = Convert.ToString(UpdatedAt);
            string SQLInsert = "INSERT INTO ValutaKurser(FromCurrency, ToCurrency, UpdatedAt, Rate) VALUES( '"+FromCurrency+"', '"+ToCurrency+"', '"+nu+"', '"+Rate+"')";
            Console.WriteLine("Getting Connection ...");
            //your connection string 
            string connString = "Data Source=40.68.244.59, 1433; Initial Catalog = Martin; User ID = Martin; Password = TestCase";
            //create instanace of database connection
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                Console.WriteLine("Openning Connection ...");
                //open connection
                conn.Open();
                using (SqlCommand command = new SqlCommand(SQL, conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0} {1} {2} {3} {4}",reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDecimal(4));
                    }
                }
                Console.WriteLine("Connection successful!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            //Console.Read();
        }

    }

}

