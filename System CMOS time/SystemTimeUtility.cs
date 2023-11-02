using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SystemCmosTime
{
    public static class SystemTimeUtility
    {
        /// <summary>
        /// Zapíše čas do systémového CMOS - vyžaduje oprávnění správce.
        /// </summary>
        /// <param name="localDateTime">Lokální DateTime, který bude zapsán do CMOS.</param>
        /// <param name="error">Výstupní zpráva v případě chyby.</param>
        /// <returns>True, pokud byl zápis úspěšný; jinak false.</returns>
        public static bool WriteToCmos(this DateTime localDateTime, ref string error)
        {
            bool isElevated = CheckForAdministratorPrivileges();

            if (!isElevated)
            {
                throw new Exception("Vyžadováno je oprávnění správce.");
            }

            SystemTime st = ConvertLocalDateTimeToSystemTime(localDateTime);

            if (SetSystemTime(ref st))
            {
                error = "Systémový čas byl nastaven.";
                return true;
            }
            else
            {
                error = "Nepodařilo se nastavit systémový čas. Chybový kód: " + Marshal.GetLastWin32Error();
                return false;
            }
        }

        /// <summary>
        /// Zkontroluje, zda aktuální uživatel má oprávnění správce.
        /// </summary>
        /// <returns>
        /// True, pokud aktuální uživatel má oprávnění správce; jinak false.
        /// </returns>
        private static bool CheckForAdministratorPrivileges()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Konvertuje místní DateTime na strukturu SystemTime, která se používá pro nastavení systémového času.
        /// </summary>
        /// <param name="localDateTime">Místní DateTime k konverzi.</param>
        /// <returns>Struktura SystemTime reprezentující místní čas.</returns>
        private static SystemTime ConvertLocalDateTimeToSystemTime(DateTime localDateTime)
        {
            SystemTime st = new SystemTime
            {
                Year = (short)localDateTime.Year,
                Month = (short)localDateTime.Month,
                Day = (short)localDateTime.Day,
                Hour = (short)localDateTime.Hour,
                Minute = (short)localDateTime.Minute,
                Second = (short)localDateTime.Second
            };

            return st;
        }

        /// <summary>
        /// Zapíše čas do systémového CMOS - vyžaduje oprávnění správce.
        /// </summary>
        /// <param name="st">Struktura SYSTEMTIME s časovými údaji.</param>
        /// <returns>True, pokud byl zápis úspěšný; jinak false.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SystemTime systemTime);

        /// <summary>
        /// Reprezentuje systémový čas v podobě struktury se sekvenčním uspořádáním položek.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            /// <summary>
            /// Obsahuje hodnotu pro rok.
            /// </summary>
            public short Year;

            /// <summary>
            /// Obsahuje hodnotu pro měsíc.
            /// </summary>
            public short Month;

            /// <summary>
            /// Obsahuje hodnotu pro den v týdnu.
            /// </summary>
            public short DayOfWeek;

            /// <summary>
            /// Obsahuje hodnotu pro den v měsíci.
            /// </summary>
            public short Day;

            /// <summary>
            /// Obsahuje hodnotu pro hodiny.
            /// </summary>
            public short Hour;

            /// <summary>
            /// Obsahuje hodnotu pro minuty.
            /// </summary>
            public short Minute;

            /// <summary>
            /// Obsahuje hodnotu pro sekundy.
            /// </summary>
            public short Second;

            /// <summary>
            /// Obsahuje hodnotu pro milisekundy.
            /// </summary>
            public short Milliseconds;
        }

    }
}
