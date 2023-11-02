using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace SystemCmosTime
{
    /// <summary>
    /// Třída pro získávání aktuálního času ze serveru time.nist.gov pomocí poskytnutého rozhraní IDateTimeStringProvider.
    /// </summary>
    public sealed class TimeProvider
    {
        private readonly IDateTimeProvider _dateTimeStringProvider;

        /// <summary>
        /// Inicializuje novou instanci třídy TimeProvider s použitím zadaného poskytovatele textového datového řetězce.
        /// </summary>
        /// <param name="dateTimeStringProvider">Rozhraní pro získávání textových datových řetězců, obsahujících UTC datum a čas.</param>
        public TimeProvider(IDateTimeProvider dateTimeStringProvider)
        {
            _dateTimeStringProvider = dateTimeStringProvider;
        }

        /// <summary>
        /// Získá aktuální UTC datum a čas ze zadaného datového zdroje a převede je na DateTime. 
        /// Pokud dojde k chybě při získávání datového řetězce nebo při konverzi na DateTime, metoda vrátí null.
        /// </summary>
        /// <returns>Objekt DateTime reprezentující aktuální UTC datum a čas nebo null v případě chyby.</returns>
        public DateTime? ZiskejDatumZeServeruNistGov()
        {
            try
            {
                string utcDateTimeString = _dateTimeStringProvider.ZiskejUtcDateTimeString();

                if (utcDateTimeString != null)
                {
                    return DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// Rozhraní pro získávání textových datových řetězců, které obsahují UTC datum a čas.
    /// </summary>
    public interface IDateTimeProvider
    {
        string ZiskejUtcDateTimeString();
    }

    /// <summary>
    /// Implementace rozhraní IDateTimeStringProvider pro získávání UTC datum a čas ze serveru time.nist.gov.
    /// </summary>
    public class TcpDateTimeStringProvider : IDateTimeProvider
    {
        /// <summary>
        /// Získá UTC datum a čas ve formě textového řetězce ze serveru time.nist.gov.
        /// </summary>
        /// <returns>Textový řetězec obsahující UTC datum a čas, nebo null v případě chyby.</returns>
        public string ZiskejUtcDateTimeString()
        {
            try
            {
                using (TcpClient klient = new TcpClient("time.nist.gov", 13))
                using (StreamReader ctecka = new StreamReader(klient.GetStream()))
                {
                    return ctecka.ReadToEnd().Substring(7, 17);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
