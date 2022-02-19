using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTCC_PPD
{
    class Program
    {
        /*  GLOBAL VARS     */
        public static string ROOTDIRECTORY = "C:\\YOUR\\FILES\\HERE\\";
        public static string USERAGENT = "Edit your User Agent String";

        static void Main(string[] args)
        {
            Menu();
        }
        static void Menu() 
        {
            int choice = -1;
            string rootDirectory = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Menu: ");
                Console.WriteLine("\t 1. Download All");
                Console.WriteLine("\t 2. Unzip All");
                Console.WriteLine("\t 3. Ingest All");
                Console.WriteLine("");
                Console.WriteLine("\t 0. Exit");

                choice = Int32.Parse(Console.ReadLine());

                if (choice == 1) { DownloadAll(); }
                if (choice == 2) { unzip_directory(rootDirectory); }
                if (choice == 3) { }
                if (choice == 0) { System.Environment.Exit(0); }
            }
        }

        // Iterate through current day to past
        static void DownloadAll()
        {
            DateTime d = DateTime.Today;
            DateTime earliest = new DateTime(2020, 11, 23);
            DateTime CA_earliest = new DateTime(2022, 01, 20);
            DateTime CTFC_earliest = new DateTime(2021, 01, 04);
            DateTime CTFC_Commodities_earliest = new DateTime(2020, 11, 23);
            DateTime SEC_earliest = new DateTime(2022, 02, 11);
            

            while (d >= earliest)
            {
                if (d.DayOfWeek != DayOfWeek.Sunday && d.DayOfWeek != DayOfWeek.Saturday)
                {
                    string sDate = d.Year.ToString("0000") + "_" + d.Month.ToString("00") + "_" + d.Day.ToString("00");

                    if (d > SEC_earliest) //2022-FEB-11
                    {
                        download_sec(sDate);
                    }

                    if (d > CA_earliest) //2022-JAN-20
                    {
                        download_ca(sDate);
                    }

                    if (d > CTFC_earliest) //2021-JAN-04
                    {
                        download_cftc(sDate);
                    }

                    // The CTFC Commodities goes back the furthest, so it's separated from the rest of CTFC downloads
                    if (d > CTFC_Commodities_earliest) //2020-NOV-23
                    {
                        download_cftc_commodities(sDate);
                    }
                }

                d = d.AddDays(-1);
            }

            Console.WriteLine("Downloads completed.  Press [return] to return to menu...");
            Console.Read();
        }

        // DOWNLOAD GROUPS BY PPD DASHBOARD
        public static void download_ca(string sDate) 
        {
            download_ca_credits(sDate);
            download_ca_equities(sDate);
            download_ca_rates(sDate);
        }
        public static void download_cftc(string sDate) 
        {
            //download_cftc_commodities(sDate); Removed, see above
            download_cftc_credits(sDate);
            download_cftc_equities(sDate);
            download_cftc_forex(sDate);
            download_cftc_rates(sDate);
        }
        public static void download_sec(string sDate) 
        {
            download_sec_credits(sDate);
            download_sec_equities(sDate);
            download_sec_rates(sDate);
        }

        // CFTC PPD DASHBOARD
        public static void download_cftc_commodities(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/cftc/eod/CFTC_CUMULATIVE_COMMODITIES_2022_02_18.zip
            string source = "CFTC";
            string product = "COMMODITIES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_cftc_credits(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/cftc/eod/CFTC_CUMULATIVE_CREDITS_2022_02_18.zip
            string source = "CFTC";
            string product = "CREDITS";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
         }
        public static void download_cftc_equities(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/cftc/eod/CFTC_CUMULATIVE_EQUITIES_2022_02_18.zip
            string source = "CFTC";
            string product = "EQUITIES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
          }
        public static void download_cftc_forex(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/cftc/eod/CFTC_CUMULATIVE_FOREX_2022_02_18.zip
            string source = "CFTC";
            string product = "FOREX";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_cftc_rates(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/cftc/eod/CFTC_CUMULATIVE_RATES_2022_02_18.zip
            string source = "CFTC";
            string product = "RATES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }   

        // CANADIAN PPD DASHBOARD
        public static void download_ca_credits(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/ca/eod/CA_CUMULATIVE_CREDITS_2022_02_18.zip
            string source = "CA";
            string product = "CREDITS";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_ca_equities(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/ca/eod/CA_CUMULATIVE_EQUITIES_2022_02_18.zip
            string source = "CA";
            string product = "EQUITIES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_ca_rates(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/ca/eod/CA_CUMULATIVE_RATES_2022_02_18.zip
            string source = "CA";
            string product = "RATES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }

        // SEC PPD DASHBOARD
        public static void download_sec_credits(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/sec/eod/SEC_CUMULATIVE_CREDITS_2022_02_18.zip
            string source = "SEC";
            string product = "CREDITS";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_sec_equities(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/sec/eod/SEC_CUMULATIVE_EQUITIES_2022_02_18.zip
            string source = "SEC";
            string product = "EQUITIES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }
        public static void download_sec_rates(string sDate) 
        {
            // https://kgc0418-tdw-data-0.s3.amazonaws.com/sec/eod/SEC_CUMULATIVE_RATES_2022_02_18.zip
            string source = "SEC";
            string product = "RATES";
            string file = source + "_CUMULATIVE_" + product + "_" + sDate + ".zip";
            string url = "https://kgc0418-tdw-data-0.s3.amazonaws.com/" + source.ToLower() + "/eod/" + file;

            download(url, file, source, product);
        }

        // DOWNLOAD INDIVIDUAL FILE
        public static void download(string uri, string file, string source, string product)
        {
            string download_to = ROOTDIRECTORY + source.ToString() + "\\" + product.ToString() + "\\" + file.ToString();
            if (!File.Exists(download_to) && !File.Exists(download_to.Replace(".zip", ".csv")))
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp";
                        wc.Headers["Accept-Language"] = "en-US,en;q=0.5";
                        wc.Headers["DNT"] = "1";
                        wc.Headers["Host"] = "kgc0418-tdw-data-0.s3.amazonaws.com";
                        wc.Headers["Sec-Fetch-Dest"] = "document";
                        wc.Headers["Sec-Fetch-Mode"] = "navigate";
                        wc.Headers["Sec-Fetch-Site"] = "none";
                        wc.Headers["Sec-Fetch-User"] = "?1";
                        wc.Headers["Upgrade-Insecure-Requests"] = "1";
                        wc.Headers["User-Agent"] = USERAGENT;

                        Console.WriteLine("Downloading: {0}", uri);
                        wc.DownloadFile(new System.Uri(uri), download_to);
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine(file);
                    Console.WriteLine(e.ToString());
                    Console.Read();
                    return;
                }
            }
        }
    }
}
