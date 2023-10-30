using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace SystemCmosTime
{
    public sealed class TimeProvider
    {
        /// <summary>
        /// Získá aktuální čas ze serveru time.nist.gov.
        /// </summary>
        /// <returns>DateTime s aktuálním časem, null pokud se nepodařilo získat čas.</returns>
        public static DateTime? ZiskejDatumZeServeruNistGov()
        {
            try
            {
                using (TcpClient klient = new TcpClient("time.nist.gov", 13))
                {
                    using (StreamReader ctecka = new StreamReader(klient.GetStream()))
                    {
                        string odpoved = ctecka.ReadToEnd();
                        string utcDateTimeString = odpoved.Substring(7, 17);

                        DateTime lokalniCas = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();

                        return lokalniCas;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
