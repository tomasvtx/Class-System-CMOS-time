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
            bool isElevated = false;

            // Kontrola oprávnění správce
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                isElevated = new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!isElevated)
            {
                throw new Exception("Vyžadovány jsou oprávnění správce.");
            }

            SystemTime st = new SystemTime();
            st.Year = (short)localDateTime.Year; // musí být short
            st.Month = (short)localDateTime.Month;
            st.Day = (short)localDateTime.Day;
            st.Hour = (short)localDateTime.Hour;
            st.Minute = (short)localDateTime.Minute;
            st.Second = (short)localDateTime.Second;

            // Zápis do CMOS
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
        /// Zapíše čas do systémového CMOS - vyžaduje oprávnění správce.
        /// </summary>
        /// <param name="st">Struktura SYSTEMTIME s časovými údaji.</param>
        /// <returns>True, pokud byl zápis úspěšný; jinak false.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SystemTime systemTime);


        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public short Year;
            public short Month;
            public short DayOfWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short Milliseconds;
        }

    }
}
