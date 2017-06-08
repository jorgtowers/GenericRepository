using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Security;
using System.Configuration;
using _W=System.Windows.Forms;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Utils
{
    public interface IDescripcionId
    {
        int Id { get; set; }
        string Descripcion { get; set; }
    }
    public class Fechas
    {
        private static DateTime _FechaHasta = DateTime.Now;
        private static DateTime _FechaDesde = DateTime.Now;
        #region Propiedades
        public static DateTime FechaHasta
        {
            set
            {
                _FechaHasta = value;
            }
        }
        public static DateTime FechaDesde
        {
            set
            {
                _FechaDesde = value;
            }
        }
        // Retorna la diferencia entre las dos fechas
        public static TimeSpan DiferenciaEntreFechas
        {
            get
            {
                return _FechaHasta - _FechaDesde;
            }
        }
        #endregion
        #region Trabajando con Fechas
        private static StringBuilder _ReturnStringBuilder;
        public static string GetFechaActual()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString();
        }
        public static string GetFechaActual(Enumeracion.FormatoFecha _Format)
        {
            switch (_Format)
            {
                case Enumeracion.FormatoFecha.YYYYMMDD:
                    {
                        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString();
                    }
                case Enumeracion.FormatoFecha.DDMMYYYY:
                    {
                        return DateTime.Now.Day.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Year.ToString();
                    }
                default:
                    {
                        return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString();
                    }
            }

        }
        public static string GetFechaActual(Enumeracion.FormatoFecha _Format, Enumeracion.SeparadorFecha _Separador)
        {
            string _split = "";
            switch (_Separador)
            {
                case Enumeracion.SeparadorFecha.Ninguno:
                    {
                        _split = "";
                        break;
                    }
                case Enumeracion.SeparadorFecha.Guion:
                    {
                        _split = "-";
                        break;
                    }
                case Enumeracion.SeparadorFecha.Slash:
                    {
                        _split = "/";
                        break;
                    }
                case Enumeracion.SeparadorFecha.UnderScore:
                    {
                        _split = "_";
                        break;
                    }
                default:
                    {
                        _split = "";
                        break;
                    }
            }
            switch (_Format)
            {
                case Enumeracion.FormatoFecha.YYYYMMDD:
                    {
                        return DateTime.Now.Year.ToString() + _split + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + _split + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString();
                    }
                case Enumeracion.FormatoFecha.DDMMYYYY:
                    {
                        return DateTime.Now.Day.ToString().PadLeft(2, '0').ToString() + _split + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + _split + DateTime.Now.Year.ToString();
                    }
                default:
                    {
                        return DateTime.Now.Year.ToString() + _split + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + _split + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString();
                    }
            }

        }
        public static string GetFechaHoraActual()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0').ToString() + DateTime.Now.Day.ToString().PadLeft(2, '0').ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
        }
        public static string GetFullDateToString()
        {
            _ReturnStringBuilder = new StringBuilder();
            #region Dias de la Semana
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        _ReturnStringBuilder.Append("Domingo, ");
                        break;
                    }
                case DayOfWeek.Monday:
                    {
                        _ReturnStringBuilder.Append("Lunes, ");
                        break;
                    }
                case DayOfWeek.Tuesday:
                    {
                        _ReturnStringBuilder.Append("Martes, ");
                        break;
                    }
                case DayOfWeek.Wednesday:
                    {
                        _ReturnStringBuilder.Append("Miércoles, ");
                        break;
                    }
                case DayOfWeek.Thursday:
                    {
                        _ReturnStringBuilder.Append("Jueves, ");
                        break;
                    }
                case DayOfWeek.Friday:
                    {
                        _ReturnStringBuilder.Append("Viernes, ");
                        break;
                    }
                case DayOfWeek.Saturday:
                    {
                        _ReturnStringBuilder.Append("Sábado, ");
                        break;
                    }
                default: { _ReturnStringBuilder.Append(""); break; }
            }
            #endregion
            _ReturnStringBuilder.Append(DateTime.Now.Day.ToString() + " de ");
            #region Mes
            switch (DateTime.Now.Month)
            {
                case 1:
                    {
                        _ReturnStringBuilder.Append("Enero ");
                        break;
                    }
                case 2:
                    {
                        _ReturnStringBuilder.Append("Febrero ");
                        break;
                    }
                case 3:
                    {
                        _ReturnStringBuilder.Append("Marzo ");
                        break;
                    }
                case 4:
                    {
                        _ReturnStringBuilder.Append("Abril ");
                        break;
                    }
                case 5:
                    {
                        _ReturnStringBuilder.Append("Mayo ");
                        break;
                    }
                case 6:
                    {
                        _ReturnStringBuilder.Append("Junio ");
                        break;
                    }
                case 7:
                    {
                        _ReturnStringBuilder.Append("Julio ");
                        break;
                    }
                case 8:
                    {
                        _ReturnStringBuilder.Append("Agosto ");
                        break;
                    }
                case 9:
                    {
                        _ReturnStringBuilder.Append("Septiembre ");
                        break;
                    }
                case 10:
                    {
                        _ReturnStringBuilder.Append("Octubre ");
                        break;
                    }
                case 11:
                    {
                        _ReturnStringBuilder.Append("Noviembre ");
                        break;
                    }
                case 12:
                    {
                        _ReturnStringBuilder.Append("Diciembre ");
                        break;
                    }
                default: { _ReturnStringBuilder.Append(""); break; }
            }
            #endregion
            _ReturnStringBuilder.Append("de " + DateTime.Now.Year.ToString());
            return _ReturnStringBuilder.ToString();
        }
        public static string GetFullDateToString(DateTime date)
        {
            _ReturnStringBuilder = new StringBuilder();
            #region Dias de la Semana
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        _ReturnStringBuilder.Append("Domingo, ");
                        break;
                    }
                case DayOfWeek.Monday:
                    {
                        _ReturnStringBuilder.Append("Lunes, ");
                        break;
                    }
                case DayOfWeek.Tuesday:
                    {
                        _ReturnStringBuilder.Append("Martes, ");
                        break;
                    }
                case DayOfWeek.Wednesday:
                    {
                        _ReturnStringBuilder.Append("Miércoles, ");
                        break;
                    }
                case DayOfWeek.Thursday:
                    {
                        _ReturnStringBuilder.Append("Jueves, ");
                        break;
                    }
                case DayOfWeek.Friday:
                    {
                        _ReturnStringBuilder.Append("Viernes, ");
                        break;
                    }
                case DayOfWeek.Saturday:
                    {
                        _ReturnStringBuilder.Append("Sábado, ");
                        break;
                    }
                default: { _ReturnStringBuilder.Append(""); break; }
            }
            #endregion
            _ReturnStringBuilder.Append(date.Day.ToString() + " de ");
            #region Mes
            switch (date.Month)
            {
                case 1:
                    {
                        _ReturnStringBuilder.Append("Enero ");
                        break;
                    }
                case 2:
                    {
                        _ReturnStringBuilder.Append("Febrero ");
                        break;
                    }
                case 3:
                    {
                        _ReturnStringBuilder.Append("Marzo ");
                        break;
                    }
                case 4:
                    {
                        _ReturnStringBuilder.Append("Abril ");
                        break;
                    }
                case 5:
                    {
                        _ReturnStringBuilder.Append("Mayo ");
                        break;
                    }
                case 6:
                    {
                        _ReturnStringBuilder.Append("Junio ");
                        break;
                    }
                case 7:
                    {
                        _ReturnStringBuilder.Append("Julio ");
                        break;
                    }
                case 8:
                    {
                        _ReturnStringBuilder.Append("Agosto ");
                        break;
                    }
                case 9:
                    {
                        _ReturnStringBuilder.Append("Septiembre ");
                        break;
                    }
                case 10:
                    {
                        _ReturnStringBuilder.Append("Octubre ");
                        break;
                    }
                case 11:
                    {
                        _ReturnStringBuilder.Append("Noviembre ");
                        break;
                    }
                case 12:
                    {
                        _ReturnStringBuilder.Append("Diciembre ");
                        break;
                    }
                default: { _ReturnStringBuilder.Append(""); break; }
            }
            #endregion
            _ReturnStringBuilder.Append("de " + date.Year.ToString());
            return _ReturnStringBuilder.ToString();
        }
        public static string GetIntervalosEntreFechas()
        {
            StringBuilder sDatos = new StringBuilder();
            sDatos.AppendLine(":: Intervalos de Tiempos >");
            sDatos.AppendLine("Días: " + DiferenciaEntreFechas.Days.ToString());
            sDatos.AppendLine("Horas: " + DiferenciaEntreFechas.Hours.ToString());
            sDatos.AppendLine("Minutos: " + DiferenciaEntreFechas.Minutes.ToString());
            sDatos.AppendLine("Segundos: " + DiferenciaEntreFechas.Seconds.ToString());
            sDatos.AppendLine(":: Intervalos de Tiempos Totales >");
            sDatos.AppendLine("Días: " + DiferenciaEntreFechas.TotalDays.ToString());
            sDatos.AppendLine("Horas: " + DiferenciaEntreFechas.TotalHours.ToString());
            sDatos.AppendLine("Minutos: " + DiferenciaEntreFechas.TotalMinutes.ToString());
            sDatos.AppendLine("Segundos: " + DiferenciaEntreFechas.TotalSeconds.ToString());
            return sDatos.ToString();
        }
        public static DateTime GetFechaFromString(string _Year, string _Month, string _Day)
        {
            return new DateTime(int.Parse(_Year), int.Parse(_Month), int.Parse(_Day));
        }
        public static DateTime GetFechaFromString(string _Year, string _Month, string _Day, string _Hours, string _Minute)
        {
            return new DateTime(int.Parse(_Year), int.Parse(_Month), int.Parse(_Day), int.Parse(_Hours), int.Parse(_Minute), 0);
        }
        public static int GetCurrentDay()
        {
            return System.DateTime.Now.Day;
        }
        public static int GetCurrentMonth()
        {
            return System.DateTime.Now.Month;
        }
        public static int GetCurrentYear()
        {
            return System.DateTime.Now.Year;
        }
        public static int GetEdad()
        {
            int anos = 0;
            // Edad en años
            anos = (int)(DiferenciaEntreFechas.Days / 365.25);
            return anos;
        }
        public static int GetLastDayOfMonth(int _Month, int _Year)
        {
            DateTime _D1, _D2;
            _D1 = Convert.ToDateTime("01/" + _Month + "/" + _Year);// pongo el 1 porque siempre es el primer día obvio
            if (_Month == 12)
            {
                _D2 = Convert.ToDateTime("01/01/" + (_Year + 1)).AddDays(-1); //resto un día al mes y con esto obtengo el ultimo día
            }
            else
            {
                _D2 = Convert.ToDateTime("01/" + (_Month + 1) + "/" + _Year).AddDays(-1); //resto un día al mes y con esto obtengo el ultimo día
            }
            return _D2.Day;
        }
        public static string GetTimeAgo(DateTime time)
        {
            DateTime startDate = DateTime.Now;
            TimeSpan deltaMinutes = startDate.Subtract(time);
            string distance = string.Empty;
            if (deltaMinutes.TotalMinutes <= (8724 * 60))
            {
                distance = GetDistanceOfTimeInWords(deltaMinutes.TotalMinutes);
                if (deltaMinutes.Minutes < 0)
                {
                    return distance + " from  now";
                }
                else
                {
                    return distance + " ago";
                }
            }
            else
            {
                return "on " + time.ToString();
            }
        }
        private static string GetDistanceOfTimeInWords(double minutes)
        {
            if (minutes < 1)
            {
                return "less than a minute";
            }
            else if (minutes < 50)
            {
                return minutes.ToString() + " minutes";
            }
            else if (minutes < 90)
            {
                return "about one hour";
            }
            else if (minutes < 1080)
            {
                return Math.Round(new decimal((minutes / 60))).ToString() + " hours";
            }
            else if (minutes < 1440)
            {
                return "one day";
            }
            else if (minutes < 2880)
            {
                return "about one day";
            }
            else
            {
                return Math.Round(new decimal((minutes / 1440))).ToString() + " days";
            }
        }

        public static string GetTimeAgoFull(DateTime dateFrom, DateTime dateTo)
        {

            // Defaults and assume if 0 is passed in that
            // its an error rather than the epoch

            if (dateFrom <= new DateTime(1900, 1, 1))
            {
                return "A long time ago";
            }

            if (dateTo == new DateTime(1900, 1, 1))
            {
                dateTo = System.DateTime.Now;
            }

            // Calculate the difference in seconds betweeen
            // the two timestamps
            TimeSpan difference;
            difference = dateTo - dateFrom;

            string interval = "";
            // If difference is less than 60 seconds,
            // seconds is a good interval of choice


            if (difference.Seconds < 60)
            {
                interval = "s";
            }

            // If difference is between 60 seconds and
            // 60 minutes, minutes is a good interval
            if (difference.Seconds >= 60 && difference.Seconds < 60 * 60)
            {
                interval = "n";
            }

            // If difference is between 1 hour and 24 hours
            // hours is a good interval
            if (difference.Seconds >= 60 * 60 && difference.Seconds < 60 * 60 * 24)
            {
                interval = "h";
            }

            // If difference is between 1 day and 7 days
            // days is a good interval
            if (difference.Seconds >= 60 * 60 * 24 && difference.Seconds < 60 * 60 * 24 * 7)
            {
                interval = "d";
            }

            // If difference is between 1 week and 30 days
            // weeks is a good interval
            if (difference.Seconds >= 60 * 60 * 24 * 7 && difference.Seconds < 60 * 60 * 24 * 30)
            {
                interval = "ww";
            }

            // If difference is between 30 days and 365 days
            // months is a good interval, again, the same thing
            // applies, if the 29th February happens to exist
            // between your 2 dates, the function will return
            // the 'incorrect' value for a day
            if (difference.Seconds >= 60 * 60 * 24 * 30 && difference.Seconds < 60 * 60 * 24 * 365)
            {
                interval = "m";
            }

            // If difference is greater than or equal to 365
            // days, return year. This will be incorrect if
            // for example, you call the function on the 28th April
            // 2008 passing in 29th April 2007. It will return
            // 1 year ago when in actual fact (yawn!) not quite
            // a year has gone by
            if (difference.Seconds >= 60 * 60 * 24 * 365)
            {
                interval = "y";
            }

            // Based on the interval, determine the
            // number of units between the two dates
            // From this point on, you would be hard
            // pushed telling the difference between
            // this function and DateDiff. If the $datediff
            // returned is 1, be sure to return the singular
            // of the unit, e.g. 'day' rather 'days'
            decimal months_difference, datediff;
            string res = "";
            switch (interval)
            {
                case "m":
                    {

                        months_difference = Math.Floor((decimal)(difference.Seconds / 60 / 60 / 24 / 29));
                        //while (mktime(date("H", $datefrom), date("i", $datefrom),
                        //  date("s", $datefrom), date("n", $datefrom) + ($months_difference),
                        //  date("j", $dateto), date("Y", $datefrom)) < $dateto) {

                        //  $months_difference++;
                        //}
                        datediff = months_difference;

                        // We need this in here because it is possible
                        // to have an 'm' interval and a months
                        // difference of 12 because we are using 29 days
                        // in a month

                        if (datediff == 12)
                        {
                            datediff--;


                            res = (datediff == 1) ? datediff.ToString() + " month ago" : datediff.ToString() + " months ago";
                        }
                    }
                    break;

                case "y":
                    {
                        datediff = Math.Floor((decimal)difference.Seconds / 60 / 60 / 24 / 365);
                        res = (datediff == 1) ? datediff.ToString() + " year ago" : datediff.ToString() + " years ago";
                    }
                    break;

                case "d":
                    {
                        datediff = Math.Floor((decimal)difference.Seconds / 60 / 60 / 24);
                        res = (datediff == 1) ? datediff.ToString() + " day ago" : datediff.ToString() + " days ago";
                    }
                    break;

                case "ww":
                    {
                        datediff = Math.Floor((decimal)difference.Seconds / 60 / 60 / 24 / 7);
                        res = (datediff == 1) ? datediff.ToString() + " week ago" : datediff.ToString() + " weeks ago";
                    }
                    break;

                case "h":
                    {
                        datediff = Math.Floor((decimal)difference.Seconds / 60 / 60);
                        res = (datediff == 1) ? datediff.ToString() + " hour ago" : datediff.ToString() + " hours ago";
                    }
                    break;

                case "n":
                    {
                        datediff = Math.Floor((decimal)difference.Seconds / 60);
                        res = (datediff == 1) ? datediff.ToString() + " minute ago" : datediff.ToString() + " minutes ago";
                    }
                    break;

                case "s":
                    {
                        datediff = difference.Seconds;
                        res = (datediff == 1) ? datediff.ToString() + " second ago" : datediff.ToString() + " seconds ago";
                    }
                    break;
            }
            return res;
        }

        public static string GetTimeAgo(DateTime fechadesde, DateTime fechahasta, bool MostrarEnLetras)
        {

            int segundos = 0, minutos = 0, horas = 0, dias = 0;
            if (DateTime.Compare(fechahasta, fechadesde) >= 0)
            {
                TimeSpan ts = fechahasta.Subtract(fechadesde);

                segundos = ts.Seconds;
                if (segundos > 60) minutos = ts.Seconds / 60; else minutos = ts.Minutes;
                if (minutos > 60) horas = minutos / 60; else horas = ts.Hours;
                if (horas > 24) dias = horas / 24; else dias = ts.Days;
            }
            if (MostrarEnLetras)
                return (dias > 0 ? dias.ToString() + " días, " : "") +
                        (horas > 0 ? horas.ToString().PadLeft(2, '0') + " hora(s) con " : "") +
                        (minutos > 0 ? minutos.ToString().PadLeft(2, '0') + " minuto(s) y " : "") +
                        (segundos > 0 ? segundos.ToString().PadLeft(2, '0') + " segundo(s)" : "");
            else
                return (dias > 0 ? dias.ToString() + ", " : "") +
                        horas.ToString().PadLeft(2, '0') + ":" +
                        minutos.ToString().PadLeft(2, '0') + ":" +
                        segundos.ToString().PadLeft(2, '0');
        }
        public static string GetTimeAgo(DateTime fechadesde, DateTime fechahasta, Enumeracion.Date.TimeAgo literal)
        {
            //Comprobamos si la fechahasta es mayor que la fechadesde
            if (DateTime.Compare(fechahasta, fechadesde) >= 0)
            {
                TimeSpan ts = fechahasta.Subtract(fechadesde);

                switch (literal)
                {
                    case Enumeracion.Date.TimeAgo.Antes:
                        {

                            if (ts.Days > 0)
                            {
                                if (ts.Days > 1)
                                {
                                    return (ts.Days + " días antes");
                                }
                                else
                                {
                                    return (ts.Days + " día antes");
                                }
                            }
                            else
                            {

                                if (ts.Hours > 0)
                                {
                                    if (ts.Hours > 1)
                                    {
                                        return (ts.Hours + " horas antes");
                                    }
                                    else
                                    {
                                        return (ts.Hours + " hora antes");
                                    }
                                }
                                else
                                {
                                    if (ts.Minutes > 0)
                                    {
                                        if (ts.Minutes > 1)
                                        {
                                            return (ts.Minutes + " minutos antes");
                                        }
                                        else
                                        {
                                            return (ts.Minutes + " minuto antes");
                                        }
                                    }
                                    else
                                    {
                                        if (ts.Seconds > 0)
                                        {
                                            if (ts.Seconds > 1)
                                            {
                                                return (ts.Seconds + " segundos antes");
                                            }
                                            else
                                            {
                                                return (ts.Seconds + " segundo antes");
                                            }
                                        }

                                    }

                                }
                            }
                        }
                        break;
                    case Enumeracion.Date.TimeAgo.Hace:
                        {
                            if (ts.Days > 0)
                            {
                                if (ts.Days > 1)
                                {
                                    return ("hace " + ts.Days + " días");
                                }
                                else
                                {
                                    return ("hace " + ts.Days + " día");
                                }
                            }
                            else
                            {

                                if (ts.Hours > 0)
                                {
                                    if (ts.Hours > 1)
                                    {
                                        return ("hace " + ts.Hours + " horas");
                                    }
                                    else
                                    {
                                        return ("hace " + ts.Hours + " hora");
                                    }
                                }
                                else
                                {
                                    if (ts.Minutes > 0)
                                    {
                                        if (ts.Minutes > 1)
                                        {
                                            return ("hace " + ts.Minutes + " minutos");
                                        }
                                        else
                                        {
                                            return ("hace " + ts.Minutes + " minuto");
                                        }
                                    }
                                    else
                                    {
                                        if (ts.Seconds > 0)
                                        {
                                            if (ts.Seconds > 1)
                                            {
                                                return ("hace " + ts.Seconds + " segundos");
                                            }
                                            else
                                            {
                                                return ("hace " + ts.Seconds + " segundo");
                                            }
                                        }

                                    }

                                }
                            }
                        }
                        break;
                }
                return "Compruebe las fechas";
            }
            else
            {

                return "Compruebe las fechas";

            }
        }
        #endregion
    }
    public class Conversiones
    {
        #region Trabajando con Conversiones

        public static int CharToInt(string _Caracter)
        {
            return (int)Convert.ToChar(_Caracter);
        }
        public static char IntToChar(int _ValueChar)
        {
            return (char)_ValueChar;
        }
        public static int GregorianaToJuliana(DateTime _GregorianDate)
        {
            string b_days;
            if (string.IsNullOrEmpty(_GregorianDate.ToString()))
            {
                b_days = "0";
            }
            else
            {
                int _A = (_GregorianDate.Year - 1900);
                string _Ano = _A.ToString().Trim();//.PadRight(3,'0');
                DateTime _D = new DateTime(_GregorianDate.Year, 1, 1);
                Fechas.FechaDesde = _D;
                Fechas.FechaHasta = _GregorianDate.AddDays(1);
                string _dif = Fechas.DiferenciaEntreFechas.Days.ToString();

                b_days = _Ano + _dif.PadLeft(3, '0');
            }
            return int.Parse(b_days);
        }
        public static DateTime JulianaToGregoriana(int _JulianDate)
        {
            int ano, b_dia, mes, dia, i, c;
            mes = 1;
            dia = 1;
            DateTime _DateReturn;
            string _FechaAjuste = _JulianDate.ToString().PadLeft(6, '0');//Ajuste en la fecha a 6 caratecteres para que no genere una fecha errada
            ano = int.Parse(_FechaAjuste.ToString().Substring(0, 3)) + 1900;
            b_dia = int.Parse(_FechaAjuste.ToString().Substring(3));
            if (b_dia <= 32)
            {
                mes = 1;
                dia = b_dia;
            }
            else
            {
                i = 2;
                c = 31 + Fechas.GetLastDayOfMonth(i, ano);
                int x = 0;
                while (x == 0)
                {
                    if (b_dia <= c)
                    {
                        dia = b_dia - (c - Fechas.GetLastDayOfMonth(i, ano));
                        mes = i;
                        break;
                    }
                    i++;
                    c = c + Fechas.GetLastDayOfMonth(i, ano);
                }
            }
            _DateReturn = new DateTime(ano, mes, dia);
            return _DateReturn;
        }
        public static double BytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        public static double KilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024f;
        }
        public static int RomanoAEntero(string s)
        {
            //pasar la entrada a mayúsculas
            //(por si acaso)
            s = s.ToUpper();
            //ultimovalor guardará el valor
            //del símbolo anterior al que
            //estemos examinando
            int ultimovalor = 0;
            //los símbolos los almacenamos en una cadena
            //la posición del símbolo coincide con su valor
            //en el vector valor
            String simbolos = "MDCLXVI";
            int[] valor = { 1000, 500, 100, 50, 10, 5, 1 };
            //Guardaremos la posición del símbolo que
            //estamos examinando dentro de la cadena
            //simbolos, y por lo tanto, servirá de índice
            //para obtener el valor
            int v;
            //el resultado
            int r = 0;
            //si detectamos un símbolo no valido lo
            //ponemos a true y nos servirá para terminar
            bool valido = true;
            //índice para movernos por la entrada
            int indice = 0;
            //el símbolo de la entrada que estamos
            //examinando
            char c;
            while (valido && (indice < s.Length))
            {
                c = s[indice];
                v = simbolos.IndexOf(c);
                if (v >= 0)
                {
                    //sumamos su valor
                    r += valor[v];
                    //si el valor es mayor que el de la
                    //vuelta anterior del bucle, entonces
                    //el anterior debíamos haberlo restado
                    if (valor[v] > ultimovalor)
                    {
                        r -= 2 * ultimovalor;
                    }
                    //preparamos el ultimo valor para
                    //la siguiente vuelta del bucle
                    ultimovalor = valor[v];
                }
                else //el símbolo no se reconoce
                {
                    valido = false;
                    r = -1;
                }
                indice++; //pasamos al siguiente simbolo
            }//while
            return r;
        }
        public static string EnteroARomano(int i)
        {
            //Un vector de valores de cada simbolo y otro
            //de símbolos. Nos moveremos por los dos con
            //el mismo índice "p".
            int[] valor ={ 1000, 900, 500, 400, 100, 90,
                  50, 40, 10, 9, 5, 4, 1 };
            String[] simbolo ={ "M", "CM", "D", "CD", "C",
                  "XC", "L", "XL", "X", 
                  "IX", "V", "IV", "I"};
            //El resultado
            String r = "";
            //inicializamos el índice a 0, es decir, al
            //símbolo de mayor valor M (1000)
            int p = 0;
            //Comprobamos que el número de entrada está en
            //un rango válido
            if (i >= 1 && i < 4000)
            {
                int x = i; //no queremos perder i
                //mientras x tenga valor
                while (x > 0)
                {
                    //probamos a descontar el valor
                    //del símbolo actual
                    while (x >= valor[p])
                    {
                        //añadir al resultado
                        r += simbolo[p];
                        //restar su valor
                        x = x - valor[p];
                    }
                    //una vez agotado un símbolo, pasamos
                    //al siguiente
                    //ojo: esto es peligroso porque nos
                    //podríamos salir del rango de los arrays,
                    //pero sabemos que no va a ser así porque
                    //cuando p esté fuera de rango x valdrá 0
                    //debido a que el último valor es 1, que
                    //siempre se puede descontar
                    p++;
                } //while
            }//if
            return r;
        }
        public static string IntToHex(int ID)
        {
            return String.Format("0x{0:X}", ID);
        }
        public static int HexToInt(string HexID)
        {
            return int.Parse(HexID.Substring(2), System.Globalization.NumberStyles.HexNumber);
        }
        #endregion
        public sealed class NumerosToLetras
        {
            #region Miembros estáticos

            private const int Unidades = 0, Diecenas = 1, Decenas = 2, Centenas = 3;
            private static string[,] _matriz = new string[Centenas + 1, 10]
        {
            {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
            {" diez"," once"," doce"," trece"," catorce"," quince"," dieciséis"," diecisiete"," dieciocho"," diecinueve"},
            {null,null,null," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
            {null,null,null,null,null," quinientos",null," setecientos",null," novecientos"}
        };

            private const Char sub = (Char)26;
            //Cambiar acá si se quiere otro comportamiento en los métodos de clase
            public const String SeparadorDecimalSalidaDefault = "con";
            public const String MascaraSalidaDecimalDefault = "00'/100.-'";
            public const Int32 DecimalesDefault = 2;
            public const Boolean LetraCapitalDefault = false;
            public const Boolean ConvertirDecimalesDefault = false;
            public const Boolean ApocoparUnoParteEnteraDefault = false;
            public const Boolean ApocoparUnoParteDecimalDefault = false;

            #endregion

            #region Propiedades

            private Int32 _decimales = DecimalesDefault;
            private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
            private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;
            private Int32 _posiciones = DecimalesDefault;
            private String _mascaraSalidaDecimal, _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
            private Boolean _esMascaraNumerica = true;
            private Boolean _letraCapital = LetraCapitalDefault;
            private Boolean _convertirDecimales = ConvertirDecimalesDefault;
            private Boolean _apocoparUnoParteEntera = false;
            private Boolean _apocoparUnoParteDecimal;

            /// <summary>
            /// Indica la cantidad de decimales que se pasarán a entero para la conversión
            /// </summary>
            /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
            public Int32 Decimales
            {
                get { return _decimales; }
                set
                {
                    if (value > 10) throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                    _decimales = value;
                }
            }

            /// <summary>
            /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
            /// </summary>
            public CultureInfo CultureInfo
            {
                get { return _cultureInfo; }
                set { _cultureInfo = value; }
            }

            /// <summary>
            /// Indica la cadena a intercalar entre la parte entera y la decimal del número
            /// </summary>
            public String SeparadorDecimalSalida
            {
                get { return _separadorDecimalSalida; }
                set
                {
                    _separadorDecimalSalida = value;
                    //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                    //por lo que apocopo el "uno" convirtiéndolo en "un"
                    if (value.Trim().IndexOf(" ") > 0)
                        _apocoparUnoParteEntera = true;
                    else _apocoparUnoParteEntera = false;
                }
            }

            /// <summary>
            /// Indica el formato que se le dara a la parte decimal del número
            /// </summary>
            public String MascaraSalidaDecimal
            {
                get
                {
                    if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                        return _mascaraSalidaDecimal;
                    else return "";
                }
                set
                {
                    //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                    //que haya al principio de la cadena, y también si es una máscara numérica
                    int i = 0;
                    while (i < value.Length
                        && (value[i] == '0')
                            | value[i] == '#')
                        i++;
                    _posiciones = i;
                    if (i > 0)
                    {
                        _decimales = i;
                        _esMascaraNumerica = true;
                    }
                    else _esMascaraNumerica = false;
                    _mascaraSalidaDecimal = value;
                    if (_esMascaraNumerica)
                        _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'"
                            + value.Substring(_posiciones)
                            .Replace("''", sub.ToString())
                            .Replace("'", String.Empty)
                            .Replace(sub.ToString(), "'") + "'";
                    else
                        _mascaraSalidaDecimalInterna = value
                            .Replace("''", sub.ToString())
                            .Replace("'", String.Empty)
                            .Replace(sub.ToString(), "'");
                }
            }

            /// <summary>
            /// Indica si la primera letra del resultado debe estár en mayúscula
            /// </summary>
            public Boolean LetraCapital
            {
                get { return _letraCapital; }
                set { _letraCapital = value; }
            }

            /// <summary>
            /// Indica si se deben convertir los decimales a su expresión nominal
            /// </summary>
            public Boolean ConvertirDecimales
            {
                get { return _convertirDecimales; }
                set
                {
                    _convertirDecimales = value;
                    _apocoparUnoParteDecimal = value;
                    if (value)
                    {// Si la máscara es la default, la borro
                        if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                            MascaraSalidaDecimal = "";
                    }
                    else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                        //Si no hay máscara dejo la default
                        MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
                }
            }

            /// <summary>
            /// Indica si de debe cambiar "uno" por "un" en las unidades.
            /// </summary>
            public Boolean ApocoparUnoParteEntera
            {
                get { return _apocoparUnoParteEntera; }
                set { _apocoparUnoParteEntera = value; }
            }

            /// <summary>
            /// Determina si se debe apococopar el "uno" en la parte decimal
            /// </summary>
            /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
            public Boolean ApocoparUnoParteDecimal
            {
                get { return _apocoparUnoParteDecimal; }
                set { _apocoparUnoParteDecimal = value; }
            }

            #endregion

            #region Constructores

            public NumerosToLetras()
            {
                MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
                SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
                LetraCapital = LetraCapitalDefault;
                ConvertirDecimales = _convertirDecimales;
            }

            public NumerosToLetras(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
            {
                if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
                    this.MascaraSalidaDecimal = MascaraSalidaDecimal;
                if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
                    _separadorDecimalSalida = SeparadorDecimalSalida;
                _letraCapital = LetraCapital;
                _convertirDecimales = ConvertirDecimales;
            }
            #endregion

            #region Conversores de instancia

            public String ToCustomCardinal(Double Numero)
            { return Convertir((Decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, _apocoparUnoParteDecimal); }

            public String ToCustomCardinal(String Numero)
            {
                Double dNumero;
                if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
                    return ToCustomCardinal(dNumero);
                else throw new ArgumentException("'" + Numero + "' no es un número válido.");
            }

            public String ToCustomCardinal(Decimal Numero)
            { return ToCardinal((Numero)); }

            public String ToCustomCardinal(Int32 Numero)
            { return Convertir((Decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, _letraCapital, _convertirDecimales, _apocoparUnoParteEntera, false); }

            #endregion

            #region Conversores estáticos

            public static String ToCardinal(Int32 Numero)
            {
                return Convertir((Decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
            }

            public static String ToCardinal(Double Numero)
            {
                return ToCardinal((Decimal)Numero);
            }

            public static String ToCardinal(String Numero, CultureInfo ReferenciaCultural)
            {
                Double dNumero;
                if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
                    return ToCardinal(dNumero);
                else throw new ArgumentException("'" + Numero + "' no es un número válido.");
            }

            public static String ToCardinal(String Numero)
            {
                return NumerosToLetras.ToCardinal(Numero, CultureInfo.CurrentCulture);
            }

            public static String ToCardinal(Decimal Numero)
            {
                return Convertir(Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
            }

            #endregion

            private static String Convertir(Decimal Numero, Int32 Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, Boolean EsMascaraNumerica, Boolean LetraCapital, Boolean ConvertirDecimales, Boolean ApocoparUnoParteEntera, Boolean ApocoparUnoParteDecimal)
            {
                Int64 Num;
                Int32 terna, centenaTerna, decenaTerna, unidadTerna, iTerna;
                String cadTerna;
                StringBuilder Resultado = new StringBuilder();

                Num = (Int64)Math.Abs(Numero);

                if (Num >= 1000000000000 || Num < 0) throw new ArgumentException("El número '" + Numero.ToString() + "' excedió los límites del conversor: [0;1.000.000.000.000)");
                if (Num == 0)
                    Resultado.Append(" cero");
                else
                {
                    iTerna = 0;
                    while (Num > 0)
                    {
                        iTerna++;
                        cadTerna = String.Empty;
                        terna = (Int32)(Num % 1000);

                        centenaTerna = (Int32)(terna / 100);
                        decenaTerna = terna % 100;
                        unidadTerna = terna % 10;

                        if ((decenaTerna > 0) && (decenaTerna < 10))
                            cadTerna = _matriz[Unidades, unidadTerna] + cadTerna;
                        else if ((decenaTerna >= 10) && (decenaTerna < 20))
                            cadTerna = cadTerna + _matriz[Diecenas, unidadTerna];
                        else if (decenaTerna == 20)
                            cadTerna = cadTerna + " veinte";
                        else if ((decenaTerna > 20) && (decenaTerna < 30))
                            cadTerna = " veinti" + _matriz[Unidades, unidadTerna].Substring(1);
                        else if ((decenaTerna >= 30) && (decenaTerna < 100))
                            if (unidadTerna != 0)
                                cadTerna = _matriz[Decenas, (Int32)(decenaTerna / 10)] + " y" + _matriz[Unidades, unidadTerna] + cadTerna;
                            else
                                cadTerna += _matriz[Decenas, (Int32)(decenaTerna / 10)];

                        switch (centenaTerna)
                        {
                            case 1:
                                if (decenaTerna > 0) cadTerna = " ciento" + cadTerna;
                                else cadTerna = " cien" + cadTerna;
                                break;
                            case 5:
                            case 7:
                            case 9:
                                cadTerna = _matriz[Centenas, (Int32)(terna / 100)] + cadTerna;
                                break;
                            default:
                                if ((Int32)(terna / 100) > 1) cadTerna = _matriz[Unidades, (Int32)(terna / 100)] + "cientos" + cadTerna;
                                break;
                        }
                        //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                        if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                            cadTerna = cadTerna.Replace("veintiuno", "veintiún");
                        else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                            cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                        //Acentúo 'veintidós', 'veintitrés' y 'veintiséis'
                        else if (decenaTerna == 22) cadTerna = cadTerna.Replace("veintidos", "veintidós");
                        else if (decenaTerna == 23) cadTerna = cadTerna.Replace("veintitres", "veintitrés");
                        else if (decenaTerna == 26) cadTerna = cadTerna.Replace("veintiseis", "veintiséis");

                        //Completo miles y millones
                        switch (iTerna)
                        {
                            case 3:
                                if (Numero < 2000000) cadTerna += " millón";
                                else cadTerna += " millones";
                                break;
                            case 2:
                            case 4:
                                if (terna > 0) cadTerna += " mil";
                                break;
                        }
                        Resultado.Insert(0, cadTerna);
                        Num = (Int32)(Num / 1000);
                    } //while
                }

                //Se agregan los decimales si corresponde
                if (Decimales > 0)
                {
                    Resultado.Append(" " + SeparadorDecimalSalida + " ");
                    Int32 EnteroDecimal = (Int32)Math.Round((Double)(Numero - (Int64)Numero) * Math.Pow(10, Decimales), 0);
                    if (ConvertirDecimales)
                    {
                        Boolean esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                        Resultado.Append(Convertir((Decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                            + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
                    }
                    else
                        if (EsMascaraNumerica) Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                        else Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
                }
                //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
                if (LetraCapital)
                    return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
                else
                    return Resultado.ToString().Substring(1);
            }
        }
    }
    public class Randoms
    {
        #region Trabajando con Aleatorios
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static string GetGuid()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Guid.NewGuid().ToString());
            return builder.ToString();
        }
        #endregion
    }
    public class Seguridad
    {
        private static String _ReturnString = "";
        #region Trabajando con Cifrado/Seguridad
        public static string GetPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Randoms.RandomString(4, true));
            builder.Append(Randoms.RandomNumber(1000, 9999));
            builder.Append(Randoms.RandomString(2, false));
            return builder.ToString();
        }
        public static string GetPasswordHexadecimal()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Conversiones.IntToHex(Randoms.RandomNumber(10000, 99999)));
            return builder.ToString();
        }
        public static string EnCode(string _Cadena)
        {
            _ReturnString = "";
            int _tot = _Cadena.Length;
            for (int i = 0; i < _tot; i++)
            {
                string s = Textos.SubString(_Cadena, i, 1);
                int v = Conversiones.CharToInt(s);
                _ReturnString += Conversiones.IntToChar((v + 84) - 39).ToString();
            }
            return _ReturnString;
        }
        public static string DeCode(string _Cadena)
        {
            _ReturnString = "";
            int _tot = _Cadena.Length;
            for (int i = 0; i < _tot; i++)
            {
                string s = Textos.SubString(_Cadena, i, 1);
                int v = Conversiones.CharToInt(s);
                _ReturnString += Conversiones.IntToChar((v - 84) + 39).ToString();
            }
            return _ReturnString;
        }
        public static string DeCode(string _Cadena, bool _SpecialCaracter)
        {
            if (_SpecialCaracter)
            {
                _ReturnString = "";
                int _tot = _Cadena.Length;
                for (int i = 0; i < _tot; i++)
                {
                    string s = Textos.SubString(_Cadena, i, 1);
                    int v = Conversiones.CharToInt(s);
                    switch (v)
                    {
                        case 7:
                            {
                                _ReturnString += Conversiones.IntToChar(44).ToString(); // Coma ( , ) 
                                break;
                            }
                        case 8:
                            {
                                _ReturnString += Conversiones.IntToChar(39).ToString(); // Comilla simple ( ' )
                                break;
                            }
                        default:
                            {
                                _ReturnString += Conversiones.IntToChar(v).ToString(); // Cadena Normal
                                break;
                            }
                    }
                }
                return _ReturnString;
            }
            else
            {
                _ReturnString = "";
                int _tot = _Cadena.Length;
                for (int i = 0; i < _tot; i++)
                {
                    string s = Textos.SubString(_Cadena, i, 1);
                    int v = Conversiones.CharToInt(s);
                    _ReturnString += Conversiones.IntToChar((v - 84) + 39).ToString();
                }
                return _ReturnString;
            }
        }
        public static string EnCode(string _Cadena, bool _SpecialCaracter)
        {
            if (_SpecialCaracter)
            {
                _ReturnString = "";
                int _tot = _Cadena.Length;
                for (int i = 0; i < _tot; i++)
                {
                    string s = Textos.SubString(_Cadena, i, 1);
                    int v = Conversiones.CharToInt(s);
                    switch (v)
                    {
                        case 44:
                            {
                                _ReturnString += Conversiones.IntToChar(7).ToString(); // Coma ( , ) 
                                break;
                            }
                        case 39:
                            {
                                _ReturnString += Conversiones.IntToChar(8).ToString(); // Comilla simple ( ' )
                                break;
                            }
                        default:
                            {
                                _ReturnString += Conversiones.IntToChar(v).ToString(); // Cadena Normal
                                break;
                            }
                    }
                }
                return _ReturnString;
            }
            else
            {
                _ReturnString = "";
                int _tot = _Cadena.Length;
                for (int i = 0; i < _tot; i++)
                {
                    string s = Textos.SubString(_Cadena, i, 1);
                    int v = Conversiones.CharToInt(s);
                    _ReturnString += Conversiones.IntToChar((v + 84) - 39).ToString();
                }
                return _ReturnString;
            }
        }
        public static SecureString MakeSecureString(string text)
        {
            SecureString secure = new SecureString();
            foreach (char c in text)
            {
                secure.AppendChar(c);
            }

            return secure;
        }
        #endregion
    }
    public class Validaciones
    {
        #region Trabajando con Validaciones
        public static bool IsBetween(int valor, int valorMinimo, int valorMaximo)
        {
            if (valor >= valorMinimo & valor <= valorMaximo)
            { return true; }
            else { return false; }
        }
        public static object IIF(object expressionValid, object expressionArgument, object resultIsTrue, object resultIsFalse)
        {
            if (object.Equals(expressionArgument, expressionValid))
            {
                return resultIsTrue;
            }
            else
            {
                return resultIsFalse;
            }
        }
        public static T IIF<T>(T expressionValid, T expressionArgument, T resultIsTrue, T resultIsFalse)
        {
            if (object.Equals(expressionArgument, expressionValid))
            {
                return resultIsTrue;
            }
            else
            {
                return resultIsFalse;
            }
        }
        public static bool IsEquals(object expressionValid, object expressionArgument)
        {
            if (object.Equals(expressionArgument, expressionValid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsEquals<T>(T expressionValid, T expressionArgument)
        {
            if (object.Equals(expressionArgument, expressionValid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string IsEmpty(string _Cadena, string _ResultTrue, string _ResultFalse)
        {
            string _Valor = (string.IsNullOrEmpty(_Cadena) != true ? _ResultTrue : _ResultFalse);
            return _Valor;
        }
        public static T IsGreaterThan<T>(T expressionValue, T expressionArgument, T resultValueIsTrue, T resultValueIsFalse)
        {
            T _Valor = (decimal.Parse(expressionValue.ToString()) > decimal.Parse(expressionArgument.ToString()) ? resultValueIsTrue : resultValueIsFalse);
            return _Valor;
        }
        public static T IsGreaterThanOrEqualTo<T>(T expressionValue, T expressionArgument, T resultValueIsTrue, T resultValueIsFalse)
        {
            T _Valor = (decimal.Parse(expressionValue.ToString()) >= decimal.Parse(expressionArgument.ToString()) ? resultValueIsTrue : resultValueIsFalse);
            return _Valor;
        }
        public static T IsLessThan<T>(T expressionValue, T expressionArgument, T resultValueIsTrue, T resultValueIsFalse)
        {
            T _Valor = (decimal.Parse(expressionValue.ToString()) < decimal.Parse(expressionArgument.ToString()) ? resultValueIsTrue : resultValueIsFalse);
            return _Valor;
        }
        public static T IsLessThanOrEqualTo<T>(T expressionValue, T expressionArgument, T resultValueIsTrue, T resultValueIsFalse)
        {
            T _Valor = (decimal.Parse(expressionValue.ToString()) <= decimal.Parse(expressionArgument.ToString()) ? resultValueIsTrue : resultValueIsFalse);
            return _Valor;
        }
        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
        public static bool IsEqualsInAndArguments(object expresionValid, object[] collectionArguments)
        {
            foreach (object item in collectionArguments)
            {
                if (!object.Equals(expresionValid, item))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsEqualsInAndArguments<T>(T expresionValid, T[] collectionArguments)
        {
            foreach (T item in collectionArguments)
            {
                if (!object.Equals(expresionValid, item))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsEqualsInOrArguments(object expresionValid, object[] collectionArguments)
        {
            foreach (object item in collectionArguments)
            {
                if (object.Equals(expresionValid, item))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsEqualsInOrArguments<T>(T expresionValid, T[] collectionArguments)
        {
            foreach (T item in collectionArguments)
            {
                if (object.Equals(expresionValid, item))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
    public class Textos
    {

        private static StringBuilder _ReturnStringBuilder;
        #region Trabajando con Textos
        public static string GetUpperLower(string _Expresion)
        {
            _ReturnStringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(_Expresion))
            {
                string[] _tmp = new string[0];
                _tmp = _Expresion.Split(' ');

                for (int i = 0; i < _tmp.Length; i++)
                {
                    if (_tmp[i].Length != 0)
                    {
                        _ReturnStringBuilder.Append(_tmp[i].Substring(0, 1).ToUpper() + _tmp[i].Substring(1).ToLower() + " ");
                    }
                }
            }
            return _ReturnStringBuilder.ToString();
        }
        public static string SubString(string _Cadena, int _Position, int _CantCaracteres)
        {
            return _Cadena.Substring(_Position, _CantCaracteres);
        }
        public static string GetParametrosSplit(string _Cadena)
        {
            string tmp1 = "", tmp2 = "", _S = "|";
            if (!string.IsNullOrEmpty(_Cadena))
            {
                string Separator = _S;
                string[] param = _Cadena.Split(Separator.ToCharArray());
                foreach (string s in param)
                {
                    tmp1 = tmp1 + "'" + s.ToString() + "',";
                }
                tmp2 = tmp1.Substring(0, tmp1.LastIndexOf(","));
            }
            return tmp2;
        }
        public static string GetParametrosSplitWithName(string _Cadena)
        {
            string tmp1 = "", tmp2 = "", _S = "|";
            if (!string.IsNullOrEmpty(_Cadena))
            {
                string Separator = _S;
                string[] param = _Cadena.Split(Separator.ToCharArray());
                foreach (string s in param)
                {
                    tmp1 = tmp1 + s.ToString().Substring(0, s.ToString().LastIndexOf('=') + 1) + "'" +
s.ToString().Substring(s.ToString().LastIndexOf('=') + 1) + "',";
                }
                tmp2 = tmp1.Substring(0, tmp1.LastIndexOf(","));
            }
            return tmp2;
        }
        public static string EliminarAcentos(string texto)
        {
            string consignos = "áàäéèëíìïóòöúùuñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑçÇ";
            string sinsignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUNcC";
            for (int v = 0; v < sinsignos.Length; v++)
            {
                string i = consignos.Substring(v, 1);
                string j = sinsignos.Substring(v, 1);
                texto = texto.Replace(i, j);
            }
            return texto;
        }
        public static string EliminarEspeciales(string s)
        {
            string Filtro = "{}[]!#$%&/()=?¡'¿|*+¨´:.;,<>“”";
            char[] caracteres = Filtro.ToCharArray();
            foreach (char item in caracteres)
            {
                s = s.Replace(item.ToString(), "");
            }
            return s;
        }
        #endregion
    }
    public class ArgumentsConsole
    {
        #region Trabajando con Argumentos desde Linea de Comandos
        public static string GetCommandLines()
        {
            return Environment.CommandLine;
        }
        public static string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
        #endregion
    }
    public class Datos
    {
        private static DataTable _ReturnDataTable;
        private static DataColumn _DataColumn;
        #region Trabajando con Tablas y Datos
        /// <summary>
        /// Metodo para crear tabla temporal
        /// </summary>
        /// <param name="_Columns">Arreglo de Columnas separadas por BDConexion.Separador</param>
        /// <param name="_Types">Arreglo de Types del tipo enum (eTipoColumna) convertidas en String separadas por BDConexion.Separador</param>
        /// <returns>Retorna una instancia de un DataTable con sus columnas</returns>
        public static DataTable GetTablaTemporal(string _Columns, string _Types)
        {
            _ReturnDataTable = new DataTable();
            string[] _arrColumns = new string[0];
            string[] _arrTypes = new string[0];
            _arrColumns = _Columns.Split(',');
            _arrTypes = _Types.Split(',');
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                _DataColumn = new DataColumn();
                _DataColumn.DataType = Type.GetType("System." + _arrTypes[i].ToString());
                _DataColumn.ColumnName = _arrColumns[i];
                _ReturnDataTable.Columns.Add(_DataColumn);
            }
            return _ReturnDataTable;
        }
        public static DataTable GetTablaTemporal(string[] nameColumns, string[] typeColumns)
        {
            _ReturnDataTable = new DataTable();
            string[] _arrColumns = nameColumns;// new string[0];
            string[] _arrTypes = typeColumns; // new string[0];
            //_arrColumns = _Columns.Split(',');
            //_arrTypes = _Types.Split(',');
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                _DataColumn = new DataColumn();
                _DataColumn.DataType = Type.GetType("System." + _arrTypes[i].ToString());
                _DataColumn.ColumnName = _arrColumns[i];
                _ReturnDataTable.Columns.Add(_DataColumn);
            }
            return _ReturnDataTable;
        }
        public static DataTable GetTablaTemporal(string _Columns, string _Types, string _Separador)
        {
            _ReturnDataTable = new DataTable();
            string[] _arrColumns = new string[0];
            string[] _arrTypes = new string[0];
            _arrColumns = _Columns.Split(_Separador.ToCharArray());
            _arrTypes = _Types.Split(_Separador.ToCharArray());
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                _DataColumn = new DataColumn();
                _DataColumn.DataType = Type.GetType("System." + _arrTypes[i].ToString());
                _DataColumn.ColumnName = _arrColumns[i];
                _ReturnDataTable.Columns.Add(_DataColumn);
            }
            return _ReturnDataTable;
        }
        /// <summary>
        /// Adición de nueva fila a una tabla, se usa el separador COMA para los valores del arreglo
        /// </summary>
        /// <param name="_dt">Tabla a la cual se adicionará la nueva fila</param>
        /// <param name="_Columns">Arreglo STRING[a,b,c,...], con los nombres de las columnas de la tabla</param>
        /// <param name="_Values">Arreglo STRING[a,b,c,...], con los valores correspondientes a cada columnas de la tabla</param>
        public static void AddRowsTemporaryTable(DataTable _dt, string _Columns, string _Values)
        {
            string[] _arrColumns = new string[0];
            string[] _arrValues = new string[0];
            _arrColumns = _Columns.Split(',');
            _arrValues = _Values.Split(',');
            DataRow dr = _dt.NewRow();
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                dr[_arrColumns[i]] = _arrValues[i];
            }
            _dt.Rows.Add(dr);
        }
        public static void AddRowsTemporaryTable(DataTable dataTable, string[] columnName, string[] valueName)
        {
            string[] _arrColumns = columnName;// string[0];
            string[] _arrValues = valueName; //new string[0];
            //_arrColumns = _Columns.Split(',');
            //_arrValues = _Values.Split(',');
            DataRow dr = dataTable.NewRow();
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                dr[_arrColumns[i]] = _arrValues[i];
            }
            dataTable.Rows.Add(dr);
        }
        /// <summary>
        /// Adición de nueva fila a una tabla, se permite cambiar el separador de los valores del arreglo
        /// </summary>
        /// <param name="_dt">Tabla a la cual se adicionará la nueva fila</param>
        /// <param name="_Columns">Arreglo STRING[a,b,c,...], con los nombres de las columnas de la tabla</param>
        /// <param name="_Values">Arreglo STRING[a,b,c,...], con los valores correspondientes a cada columnas de la tabla</param>
        /// <param name="_Separador">Caracter tipo STRING que indica el separador de los arreglos</param>
        public static void AddRowsTemporaryTable(DataTable _dt, string _Columns, string _Values, string _Separador)
        {
            string[] _arrColumns = new string[0];
            string[] _arrValues = new string[0];
            _arrColumns = _Columns.Split(_Separador.ToCharArray());
            _arrValues = _Values.Split(_Separador.ToCharArray());
            DataRow dr = _dt.NewRow();
            for (int i = 0; i < _arrColumns.Length; i++)
            {
                dr[_arrColumns[i]] = _arrValues[i];
            }
            _dt.Rows.Add(dr);
        }
        #endregion
    }
    public class CommandsMSDOS
    {
        #region Trabajando con Comandos RunAs
        public static void RunAs(string path, string username, string password)
        {
            ProcessStartInfo myProcess = new ProcessStartInfo(path);
            myProcess.UserName = username;
            myProcess.Password = Seguridad.MakeSecureString(password);
            myProcess.UseShellExecute = false;
            Process.Start(myProcess);
        }
        public static void RunAs(string path, string args, string username, string password)
        {
            ProcessStartInfo myProcess = new ProcessStartInfo(path, args);
            myProcess.UserName = username;
            myProcess.Password = Seguridad.MakeSecureString(password);
            myProcess.UseShellExecute = false;
            Process.Start(myProcess);
        }
        #endregion
    }    
    public class Web
    {        
        #region  Variables
        private static WebRequest mywebReq;
        private static WebResponse mywebResp;
        private static StreamReader sr;
        private static string strHTML;
        private static StreamWriter sw;
        private static StringBuilder _StringBuilder;
        #endregion
        /// <summary>
        /// Permite crear un páginador en una página
        /// </summary>
        /// <typeparam name="T">EntityObject que permitirá crear el listado paginado.</typeparam>
        public class Paginador<T>
        {
            int pagi = 1;
            private string _ParametroUrl = "page";

            public string ParametroUrl
            {
                private get { return _ParametroUrl; }
                set { _ParametroUrl = value; }
            }

            private int _Rango = 5;

            public int Rango
            {
                get { return _Rango; }
                set { _Rango = value; }
            }

            private List<T> _ListadoAPaginar = new List<T>();

            public List<T> ListadoAPaginar
            {
                get { return _ListadoAPaginar; }
                set { _ListadoAPaginar = value; }
            }


            
            private Page PaginaAPaginar
            {
                get {                    return HttpContext.Current.Handler as Page;                }
            }
            public List<T> GetRegistrosPaginados()
            {
                // Definimos el número de la página que queremos mostrar
                if (PaginaAPaginar.Request.QueryString.Count > 0 && !string.IsNullOrEmpty(PaginaAPaginar.Request.QueryString[ParametroUrl]))
                    pagi = int.Parse(PaginaAPaginar.Request.QueryString[ParametroUrl]);
                decimal x = ListadoAPaginar.Count() / NumeroRegistrosAMostrar;
                int tot = int.Parse(Math.Truncate(x).ToString()) + 1;
                if (pagi > tot) pagi = 1;

                int pageNumber = pagi;
                List<T> records = _ListadoAPaginar;
                // Realizamos el filtro
                records = records.Skip((pageNumber - 1) * NumeroRegistrosAMostrar).Take(NumeroRegistrosAMostrar).ToList();
                return records;
            }
            private int _NumeroRegistrosAMostrar = 5;

            public int NumeroRegistrosAMostrar
            {
                private get { return _NumeroRegistrosAMostrar; }
                set { _NumeroRegistrosAMostrar = value; }
            }

            public string MostrarPaginador()
            {
                int vrangomin = 0, vrangomax = 0;
                if (_ListadoAPaginar.Count > NumeroRegistrosAMostrar)
                {
                    string paginaInicio = "", paginaFinal = "";
                    string requestUrl = "?";


                    if (PaginaAPaginar.Request.QueryString.Count > 0)
                        requestUrl += PaginaAPaginar.Request.QueryString.ToString() + "&";


                    if (string.IsNullOrEmpty(PaginaAPaginar.Request.QueryString[ParametroUrl]))
                        paginaInicio = requestUrl + ParametroUrl + "=1";
                    else
                        paginaInicio = requestUrl.Substring(0, requestUrl.IndexOf(ParametroUrl)) + ParametroUrl + "=1";




                    StringBuilder pag = new StringBuilder();
                    pag.AppendLine("<div id='content-navegacion-paginador' class='" + ClassPaginador + "'>");
                    pag.AppendLine("<a class='" + ClassPaginas + "' href='" + paginaInicio + "' title='Muestra registros de la Primera página'>");
                    pag.AppendLine("Primera</a>");
                    decimal x = ListadoAPaginar.Count() / NumeroRegistrosAMostrar;
                    int tot = int.Parse(Math.Truncate(x).ToString()) + 1;
                    if (pagi > tot) pagi = 1;

                    if (string.IsNullOrEmpty(PaginaAPaginar.Request.QueryString[ParametroUrl]))
                        paginaFinal = requestUrl + ParametroUrl + "=" + tot;
                    else
                        paginaFinal = requestUrl.Substring(0, requestUrl.IndexOf(ParametroUrl)) + ParametroUrl + "=" + tot;

                    if (pagi <= Rango) vrangomin = Rango + 1; else vrangomin = pagi;
                    if (pagi >= (tot - Rango)) vrangomax = (tot - Rango); else vrangomax = pagi;

                    int valorMinimo = (vrangomin - Rango);
                    int valorMaximo = (vrangomax + 1 + Rango);
                    for (int i = valorMinimo; i < valorMaximo; i++)
                    {
                        if (i > 1) //Controla que no imprima la primera página
                        {
                            if (i < tot)//Controla que no imprima la última página
                            {
                                if (requestUrl.IndexOf(ParametroUrl) > 0)
                                    requestUrl = requestUrl.Substring(0, requestUrl.IndexOf(ParametroUrl));

                                if (i != pagi)
                                {
                                    string imprimeNumero = i.ToString();
                                    if ((valorMinimo > 1 && i == valorMinimo) || (valorMaximo < tot && i == valorMaximo - 1))
                                        imprimeNumero = "...";
                                    pag.AppendLine("<a class='" + ClassPaginas + "' href='" + requestUrl + ParametroUrl + "=" + (i) + "' title='Muestra registros de la página N° " + (i) + "'>");
                                    pag.AppendLine((imprimeNumero) + "</a>");
                                }
                                else
                                {
                                    pag.AppendLine("<a class='" + ClassPaginaSeleccionada + "' href='" + requestUrl + ParametroUrl + "=" + (i) + "' title='Muestra registros de la página N° " + (i) + "'>");
                                    pag.AppendLine((i) + "</a>");
                                }
                            }
                        }

                    }
                    pag.AppendLine("<a class='" + ClassPaginas + "' href='" + paginaFinal + "' title='Muestra registros de la Última página'>Última</a>");
                    pag.AppendLine("</div>");
                    pag.AppendLine("<div class='clear'>");
                    pag.AppendLine("</div>");
                    return pag.ToString();
                }
                else return "";
            }



            public string ClassPaginaSeleccionada { get; set; }

            public string ClassPaginas { get; set; }

            public string ClassPaginador { get; set; }
        }
        /// <summary>
        /// Permite llenar todos los controles que hereden de ListControl. Ej.: ListBox, CheckList, DropDonwList, RadioButtonList
        /// </summary>
        /// <typeparam name="T">Instancia de Objeto que debe implementar la interfaz IDescripcionId</typeparam>
        /// <param name="listado">Listado de tipo T, que permitirá llenar la propiedad DataSource del objeto de List</param>
        /// <param name="control">Objeto a llenar. Ej.: ListBox, CheckList, DropDonwList, RadioButtonList</param>
        /// <param name="mostrarSeleccionar">Permite controlar si se agrega al DataSource, un valor nuevo llamado Seleccionar, para mostar el siguiente dato: ( -- Seleccionar -- ) y el valor del id es "-1"</param>
        /// <param name="mostrarTodos">Permite controlar si se agrega al DataSource, un valor nuevo llamado Todos, para mostar el siguiente dato: ( -- Todos -- ) y el valor del id es "0"</param>
        public static void FillListControls<T>(IList<T> listado, ListControl control, bool mostrarSeleccionar, bool mostrarTodos) where T : IDescripcionId
        {
            if (mostrarSeleccionar)
            {
                T obj = (T)Activator.CreateInstance(listado.GetType().GetGenericArguments().First());
                obj.Id = -1;
                obj.Descripcion = "(-- Seleccionar --)";
                listado.Add(obj);
            }
            if (mostrarTodos)
            {
                T obj = (T)Activator.CreateInstance(listado.GetType().GetGenericArguments().First());
                obj.Id = 0;
                obj.Descripcion = "(-- Todos --)";
                listado.Add(obj);
            }
            control.DataSource = listado.OrderBy(x => x.Descripcion).ToList(); ;
            control.DataTextField = "Descripcion";
            control.DataValueField = "Id";
            control.DataBind();
        }

        /// <summary>
        /// Strips whitespace from a CSS file
        /// </summary>
        /// <param name="Input">css text</param>
        /// <returns>A stripped CSS file</returns>
        public static string CssCompressor(string css)
        {
            return css.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            /*
            css = css.Replace("  ", string.Empty);
            css = css.Replace(System.Environment.NewLine, string.Empty);
            css = css.Replace("\t", string.Empty);
            css = css.Replace(" {", "{");
            css = css.Replace(" :", ":");
            css = css.Replace(": ", ":");
            css = css.Replace(", ", ",");
            css = css.Replace("; ", ";");
            css = css.Replace(";}", "}");
            css = Regex.Replace(css, @"(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,}(?=&nbsp;)|(?<=&nbsp;)\s{2,}(?=[<])", string.Empty);
            css = Regex.Replace(css, "([!{}:;>+([,])s+", "$1");
            css = Regex.Replace(css, "([^;}])}", "$1;}");
            css = Regex.Replace(css, "([s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");
            css = Regex.Replace(css, ":0 0 0 0;", ":0;");
            css = Regex.Replace(css, ":0 0 0;", ":0;");
            css = Regex.Replace(css, ":0 0;", ":0;");
            css = Regex.Replace(css, "background-position:0;", "background-position:0 0;");
            css = Regex.Replace(css, "(:|s)0+.(d+)", "$1.$2");
            css = Regex.Replace(css, "[^}]+{;}", "");
            css = Regex.Replace(css, "(/" + Regex.Escape("*") + ".*?" + Regex.Escape("*") + "/)", string.Empty);
            return css;*/
        }
        /// <summary>
        /// Strips whitespace from a HTML file
        /// </summary>
        /// <param name="Input">html text</param>
        /// <returns>A stripped HTML file</returns>
        public static string HTMLCompressor(string html)
        {
            string text = html;
            text = Regex.Replace(html, @">\s+<", "><");
            return text.Replace("\r", "").Replace("\n", "");
        }
        /// <summary>
        /// Strips whitespace from a JS file
        /// </summary>
        /// <param name="Input">html text</param>
        /// <returns>A stripped JS file</returns>
        public static string JsCompressor(string js)
        {
            string text = js;
            text = Regex.Replace(js, @"/;\s+/g", ";");
            return text.Replace("\r", "").Replace("\n", "").Replace("\t", string.Empty).Replace("  ", string.Empty);
        }

        [Obsolete("Utilizar FillListControls<T>", true)]
        public static void FillDropDownList(DataSet _DataSet, DropDownList _DropDownList, string _DataTextField, string _DataValueField)
        {
            _DropDownList.DataSource = _DataSet;
            _DropDownList.DataTextField = _DataTextField;
            _DropDownList.DataValueField = _DataValueField;
            _DropDownList.DataBind();
        }
        [Obsolete("Utilizar FillListControls<T>", true)]
        public static void FillGridView(DataSet _DataSet, GridView _GridView)
        {
            _GridView.DataSource = _DataSet;
            _GridView.DataBind();
        }
        [Obsolete("Utilizar FillListControls<T>", true)]
        public static void FillListBox(DataSet _DataSet, ListBox _ListBox, string _DataTextField, string _DataValueField)
        {
            _ListBox.DataSource = _DataSet;
            _ListBox.DataTextField = _DataTextField;
            _ListBox.DataValueField = _DataValueField;
            _ListBox.DataBind();
        }
        [Obsolete("Utilizar FillListControls<T>", true)]
        public static void FillDataList(DataSet _DataSet, DataList _DataList)
        {
            _DataList.DataSource = _DataSet;
            _DataList.DataBind();
        }
        [Obsolete("Utilizar FillListControls<T>", true)]
        public static void FillRepeater(DataSet _DataSet, Repeater _Repeater)
        {
            _Repeater.DataSource = _DataSet;
            _Repeater.DataBind();
        }
        public static string GetFaceBookSocialPluging(string account, int width, int height)
        {
            _StringBuilder = new StringBuilder();
            _StringBuilder.AppendLine("<iframe src='" + account);
            _StringBuilder.AppendLine("&amp;width=" + width + "&amp;colorscheme=light&amp;show_faces=false&amp;border_color&amp;stream=true&amp;header=true");
            _StringBuilder.AppendLine("&amp;height=" + height + "' scrolling='no' frameborder='0' style='border: none; overflow: hidden; width: " + width + "px; height: " + height + "px;' ></iframe>");
            return _StringBuilder.ToString();
        }
        public static string GetTwitterSearch(string searchValue, string title, string subject, int width, int height)
        {
            _StringBuilder = new StringBuilder();
            _StringBuilder.AppendLine("<script type='text/javascript' src='http://widgets.twimg.com/j/2/widget.js'></script>");
            _StringBuilder.AppendLine("<script type='text/javascript'>");
            _StringBuilder.AppendLine("new TWTR.Widget({");
            _StringBuilder.AppendLine("version: 2,");
            _StringBuilder.AppendLine("type: 'search',");
            _StringBuilder.AppendLine("search: '" + searchValue + "',");
            _StringBuilder.AppendLine("interval: 6000,");
            _StringBuilder.AppendLine("title: '" + title + "',");
            _StringBuilder.AppendLine("subject: '" + subject + "',");
            _StringBuilder.AppendLine("width: " + width.ToString() + ",");
            _StringBuilder.AppendLine("height: " + height.ToString() + ",");
            _StringBuilder.AppendLine("theme: {");
            _StringBuilder.AppendLine("shell: {");
            _StringBuilder.AppendLine("background: '#21a4eb',");
            _StringBuilder.AppendLine("color: '#ffffff'");
            _StringBuilder.AppendLine("},");
            _StringBuilder.AppendLine("tweets: {");
            _StringBuilder.AppendLine("background: '#ffffff',");
            _StringBuilder.AppendLine("color: '#444444',");
            _StringBuilder.AppendLine("links: '#1985b5'");
            _StringBuilder.AppendLine("}");
            _StringBuilder.AppendLine("},");
            _StringBuilder.AppendLine("features: {");
            _StringBuilder.AppendLine("scrollbar: true,");
            _StringBuilder.AppendLine("loop: true,");
            _StringBuilder.AppendLine("live: true,");
            _StringBuilder.AppendLine("hashtags: true,");
            _StringBuilder.AppendLine("timestamp: true,");
            _StringBuilder.AppendLine("avatars: true,");
            _StringBuilder.AppendLine("toptweets: true,");
            _StringBuilder.AppendLine("behavior: 'default'");
            _StringBuilder.AppendLine("}");
            _StringBuilder.AppendLine("}).render().start();");
            _StringBuilder.AppendLine("</script>");
            return _StringBuilder.ToString();
        }
        public static string GetTwitterSearch(string searchValue, string title, string subject, int width, int height, int interval, bool scrollBar, bool loop, bool live, bool hashtags, bool timestamp, bool avatars, bool toptweets, string contentBackground, string contentTextColor, string tweetsContentBackground, string tweetsContentTextColor, string tweetsContentLinkColor)
        {
            _StringBuilder = new StringBuilder();
            _StringBuilder.AppendLine("<script type='text/javascript' src='http://widgets.twimg.com/j/2/widget.js'></script>");
            _StringBuilder.AppendLine("<script type='text/javascript'>");
            _StringBuilder.AppendLine("new TWTR.Widget({");
            _StringBuilder.AppendLine("version: 2,");
            _StringBuilder.AppendLine("type: 'search',");
            _StringBuilder.AppendLine("search: '" + searchValue + "',");
            _StringBuilder.AppendLine("interval: " + interval.ToString() + ",");
            _StringBuilder.AppendLine("title: '" + title + "',");
            _StringBuilder.AppendLine("subject: '" + subject + "',");
            _StringBuilder.AppendLine("width: " + width.ToString() + ",");
            _StringBuilder.AppendLine("height: " + height.ToString() + ",");
            _StringBuilder.AppendLine("theme: {");
            _StringBuilder.AppendLine("shell: {");
            _StringBuilder.AppendLine("background: '#" + contentBackground + "',");
            _StringBuilder.AppendLine("color: '#" + contentTextColor + "'");
            _StringBuilder.AppendLine("},");
            _StringBuilder.AppendLine("tweets: {");
            _StringBuilder.AppendLine("background: '#" + tweetsContentBackground + "',");
            _StringBuilder.AppendLine("color: '#" + tweetsContentTextColor + "',");
            _StringBuilder.AppendLine("links: '#" + tweetsContentLinkColor + "'");
            _StringBuilder.AppendLine("}");
            _StringBuilder.AppendLine("},");
            _StringBuilder.AppendLine("features: {");
            _StringBuilder.AppendLine("scrollbar: " + scrollBar.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("loop: " + loop.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("live: " + live.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("hashtags: " + hashtags.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("timestamp: " + timestamp.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("avatars: " + avatars.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("toptweets: " + toptweets.ToString().ToLower() + ",");
            _StringBuilder.AppendLine("behavior: 'default'");
            _StringBuilder.AppendLine("}");
            _StringBuilder.AppendLine("}).render().start();");
            _StringBuilder.AppendLine("</script>");
            return _StringBuilder.ToString();
        }
        /// <summary>
        /// Obtiene los valores de una clave en el web.config, usando las claves definidas en el AppSetting
        /// </summary>
        /// <param name="keyName">Nombre de la clave en el AppSetting</param>
        /// <returns></returns>
        public static string GetKeyValueAppSetting(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName].ToString();
        }
        public static string CreateLogFiles()
        {

            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            return sLogFormat;
        }
        public static void Notificacion(string _Message, Page _Page)
        {
            string _Key = Seguridad.GetPassword();
            _Page.RegisterClientScriptBlock(_Key, "<script>alert('" + _Message.Trim() + "')</script>");
        }
        public static void Notificacion(string _Message, Control _Control)
        {
            string _Key = Seguridad.GetPassword();
            //  ScriptManager.RegisterClientScriptBlock(_Control, _Control.GetType(), _Key, "alert('" + _Message.Trim() + "');", true);
        }
        public static void Confirmacion(string _Message, Page _Page)
        {
            string _Key = Seguridad.GetPassword();
            _Page.RegisterClientScriptBlock(_Key, "<script>if(confirm('" + _Message.Trim() + "'))this.form.submit()</script>");
        }
        public static List<TextBox> EmptyTextBoxs(Control control)
        {
            List<TextBox> textboxes = new List<TextBox>();
            foreach (object item in control.Controls)
            {
                if (item is TextBox)
                {
                    if (string.IsNullOrEmpty(((TextBox)item).Text))
                    {
                        textboxes.Add((TextBox)item);
                    }
                }
            }
            return textboxes;
        }
        public static void ClearTextBoxs(Control control)
        {
            foreach (object item in control.Controls)
            {
                if (item is TextBox)
                {
                    ((TextBox)item).Text = "";
                }
            }
        }
        public static void EventLog(string errorMessage, string errorFolder)
        {
            try
            {
                string path = "~/" + errorFolder + "/Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                    sw.WriteLine(" ");
                    sw.WriteLine("Url:");
                    sw.WriteLine(System.Web.HttpContext.Current.Request.Url.ToString());
                    sw.WriteLine(" ");
                    sw.WriteLine("Message:");
                    sw.WriteLine(errorMessage);
                    sw.WriteLine(" ");
                    sw.WriteLine("------------------------------------------------------------------------");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex, errorFolder);
            }
            finally
            {
                sw.Close();
            }
        }
        public static void EventLog(string errorMessage, string errorFolder, Enumeracion.EventLog _TypeEvent, string _LogName)
        {
            try
            {
                switch (_TypeEvent)
                {
                    case Enumeracion.EventLog.Resumida:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                sw.WriteLine(" ");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    case Enumeracion.EventLog.UnaLinea:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles() + " | Event: (" + errorMessage + ") | Url: " + System.Web.HttpContext.Current.Request.Url.ToString());
                                sw.Flush();
                                break;
                            }
                        }
                    case Enumeracion.EventLog.TodaInformacion:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                                sw.WriteLine(" ");
                                sw.WriteLine("Url:");
                                sw.WriteLine(System.Web.HttpContext.Current.Request.Url.ToString());
                                sw.WriteLine(" ");
                                sw.WriteLine("Message:");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex) { ErrorLog(ex, errorFolder); }
            finally
            {
                sw.Close();
            }


        }
        public static void EventLog(Exception errorMessage, string errorFolder, Enumeracion.EventLog _TypeEvent, string _LogName, string ipAddress)
        {
            try
            {
                switch (_TypeEvent)
                {
                    case Enumeracion.EventLog.Resumida:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                sw.WriteLine(" ");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    case Enumeracion.EventLog.UnaLinea:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {

                                sw.WriteLine(string.Format("Date: {0} \t IpAddress: {1} \t Url: {2} \t Messaje: {3} \t BaseError: {4}", new object[5] { CreateLogFiles(), ipAddress, System.Web.HttpContext.Current.Request.Url.ToString(), errorMessage.Message, errorMessage.GetBaseException().Message }));
                                //sw.WriteLine("Fecha:" + " " + CreateLogFiles() + " | Event: (" + errorMessage + ") | Url: " + System.Web.HttpContext.Current.Request.Url.ToString());
                                sw.Flush();
                                break;
                            }
                        }
                    case Enumeracion.EventLog.TodaInformacion:
                        {
                            string path = "~/" + errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                            }
                            using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                                sw.WriteLine(" ");
                                sw.WriteLine("Url:");
                                sw.WriteLine(System.Web.HttpContext.Current.Request.Url.ToString());
                                sw.WriteLine(" ");
                                sw.WriteLine("Message:");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex) { ErrorLog(ex, errorFolder); }
            finally
            {
                sw.Close();
            }


        }
        public static void ErrorLog(Exception errorMessage, string errorFolder)
        {
            try
            {
                string path = "~/" + errorFolder + "/Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                    sw.WriteLine("Fuente:" + " " + errorMessage.Source);
                    sw.WriteLine(" ");
                    sw.WriteLine("Url:");
                    sw.WriteLine(System.Web.HttpContext.Current.Request.Url.ToString());
                    sw.WriteLine(" ");
                    sw.WriteLine("Error:");
                    sw.WriteLine(errorMessage.Message);
                    sw.WriteLine(" ");
                    sw.WriteLine("BaseError:");
                    sw.WriteLine(errorMessage.GetBaseException().Message);
                    sw.WriteLine(" ");
                    sw.WriteLine("StackTrace:");
                    sw.WriteLine(errorMessage.StackTrace);
                    sw.WriteLine("------------------------------------------------------------------------");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex, errorFolder);
            }
            finally
            {
                sw.Close();
            }
        }
        public static void GenerarHtml(Page _Page, string _Folder)
        {
            mywebReq = WebRequest.Create(_Page.Request.Url.ToString());
            mywebResp = mywebReq.GetResponse();
            sr = new StreamReader(mywebResp.GetResponseStream());
            strHTML = sr.ReadToEnd();
            string _FileName = _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).Substring(0, _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).LastIndexOf(".")).Replace(" ", "_");
            string path = "~/" + _Folder + "/" + _FileName.ToString().Replace(" ", "_") + ".html";
            //sw = File.CreateText(Server.MapPath(_FileName + ".html"));
            sw = File.CreateText(System.Web.HttpContext.Current.Server.MapPath(path));
            sw.WriteLine(strHTML);
            sw.Close();
            _Page.Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath(path));
        }
        public static void GenerarHtml(string url, string folder)
        {
            StringBuilder builder = new StringBuilder();
            WebClient client = new WebClient();
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string str = "";
            str = reader.ReadLine();
            while (str != null)
            {
                builder.AppendLine(str);
                str = reader.ReadLine();
            }
            data.Close();

            try
            {
                string path = "~/" + folder + "/" + url.Replace(".aspx", ".html");
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    sw.Write(builder.ToString());
                    sw.Flush();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void GenerarHtml(string url, string folder, string nameOutPut)
        {
            StringBuilder builder = new StringBuilder();
            WebClient client = new WebClient();
            Stream data = client.OpenRead(url);
            Uri urlToCheck = new Uri(url);


            StreamReader reader = new StreamReader(data);
            string str = "";
            str = reader.ReadLine();
            while (str != null)
            {
                builder.AppendLine(str);
                str = reader.ReadLine();
            }
            data.Close();

            try
            {
                string path = "~/" + folder + "/" + nameOutPut + ".html";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath(path), builder.ToString());
                //using (sw = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                //{
                //    sw.Write(builder.ToString());
                //    sw.Flush();
                //}
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static string NamePageAspxToHtml(Page _Page)
        {
            string _FileName;
            _FileName = _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).Substring(0, _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).LastIndexOf(".")).Replace(" ", "_");
            return _FileName + ".html";
        }
        public static string NamePageAspx(Page _Page, bool addExtASPX)
        {
            string _FileName;
            if (_Page.Request.Url.LocalPath.Contains("."))
                _FileName = _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).Substring(0, _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1).LastIndexOf(".")).Replace(" ", "_");
            else
                _FileName = _Page.Request.Url.LocalPath.Substring(_Page.Request.Url.LocalPath.LastIndexOf("/") + 1);
            return _FileName + (addExtASPX ? ".aspx" : "");
        }
        public static void Redirect(Page _Page, string _Url)
        {
            _Page.Response.Redirect(_Url);
        }
        public static string FormatHtml(string _Cadena)
        {
            return _Cadena.Replace("\r", "<br/>");
        }
        public static string FormatHtml(string _Cadena, Enumeracion.EtiquetaHtml _Tag)
        {
            switch (_Tag)
            {
                case Enumeracion.EtiquetaHtml.P: { return "<p>" + _Cadena.Replace("\r", "<br/>") + "</p>"; }
                case Enumeracion.EtiquetaHtml.B: { return "<b>" + _Cadena.Replace("\r", "<br/>") + "</b>"; }
                case Enumeracion.EtiquetaHtml.Div: { return "<div>" + _Cadena.Replace("\r", "<br/>") + "</div>"; }
                case Enumeracion.EtiquetaHtml.I: { return "<i>" + _Cadena.Replace("\r", "<br/>") + "</i>"; }
                case Enumeracion.EtiquetaHtml.Span: { return "<span>" + _Cadena.Replace("\r", "<br/>") + "</span>"; }
                case Enumeracion.EtiquetaHtml.H1: { return "<h1>" + _Cadena.Replace("\r", "<br/>") + "</h1>"; }
                case Enumeracion.EtiquetaHtml.H2: { return "<h2>" + _Cadena.Replace("\r", "<br/>") + "</h2>"; }
                case Enumeracion.EtiquetaHtml.H3: { return "<h3>" + _Cadena.Replace("\r", "<br/>") + "</h3>"; }
                case Enumeracion.EtiquetaHtml.H4: { return "<h4>" + _Cadena.Replace("\r", "<br/>") + "</h4>"; }
                case Enumeracion.EtiquetaHtml.H5: { return "<h5>" + _Cadena.Replace("\r", "<br/>") + "</h5>"; }
                case Enumeracion.EtiquetaHtml.Br: { return _Cadena.Replace("\r", "<br/>"); }
                default: { return _Cadena.Replace("\r", "<br/>"); }
            }
        }
        public static string FormatHtml(string _Cadena, Enumeracion.EtiquetaHtml _Tag, string _Class)
        {
            switch (_Tag)
            {
                case Enumeracion.EtiquetaHtml.P: { return "<p class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</p>"; }
                case Enumeracion.EtiquetaHtml.B: { return "<b class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</b>"; }
                case Enumeracion.EtiquetaHtml.Div: { return "<div class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</div>"; }
                case Enumeracion.EtiquetaHtml.I: { return "<i class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</i>"; }
                case Enumeracion.EtiquetaHtml.Span: { return "<span class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</span>"; }
                case Enumeracion.EtiquetaHtml.H1: { return "<h1 class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</h1>"; }
                case Enumeracion.EtiquetaHtml.H2: { return "<h2 class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</h2>"; }
                case Enumeracion.EtiquetaHtml.H3: { return "<h3 class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</h3>"; }
                case Enumeracion.EtiquetaHtml.H4: { return "<h4 class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</h4>"; }
                case Enumeracion.EtiquetaHtml.H5: { return "<h5 class='" + _Class + "'>" + _Cadena.Replace("\r", "<br/>") + "</h5>"; }
                case Enumeracion.EtiquetaHtml.Br: { return _Cadena.Replace("\r", "<br/>"); }
                default: { return _Cadena.Replace("\r", "<br/>"); }
            }
        }
        public static string FormatHtml(string _LinkName, string _UrlLink, string _Title, string _Rel, string _Class, string _Target)
        {
            return "<a href='" + _UrlLink + "' title='" + _Title + "' rel='" + _Rel + "' class='" + _Class + "' target='" + _Target + "'>" + _LinkName + "</a>";
        }
        public static string FormatHtml(string _UrlLink, string _Title, string _Rel, string _ClassLink, string _ImgUrl, string _ClassImg, string _Target)
        {
            return "<a href='" + _UrlLink + "' title='" + _Title + "' rel='" + _Rel + "' class='" + _ClassLink + "' target='" + _Target + "'>" +
                "<img class='" + _ClassImg + "' src='" + _ImgUrl + "' />" +
                "</a>";
        }
        public static string FormatHtml(string _UrlLink, string _Title, string _Rel, string _ClassLink, string _ImgUrl, string _ClassImg, string _Target, string _Style)
        {
            return "<a href='" + _UrlLink + "' title='" + _Title + "' rel='" + _Rel + "' class='" + _ClassLink + "' target='" + _Target + "'>" +
                "<img style='" + _Style + "' class='" + _ClassImg + "' src='" + _ImgUrl + "' />" +
                "</a>";
        }
        public static string FormatHtml(string _ImgUrl, string _ClassImg)
        {
            return "<img class='" + _ClassImg + "' src='" + _ImgUrl + "' />";
        }
        public static void GridviewToExcel(GridView _GridView, Page _Page)
        {
            HtmlForm form = new HtmlForm();
            _GridView.EnableViewState = false;
            _Page.Response.Clear();
            _Page.Response.AddHeader("content-disposition", "attachment;filename=data.xls");
            _Page.Response.Charset = "UTF-8";
            _Page.Response.ContentEncoding = Encoding.Default;
            _Page.Response.ContentType = "application/vnd.ms-excel";
            StringWriter xStrinWriter = new StringWriter();
            HtmlTextWriter HtmlTextWriter = new HtmlTextWriter(xStrinWriter);
            _Page.EnableEventValidation = false;
            _Page.DesignerInitialize();
            _Page.Controls.Add(form);
            form.Controls.Add(_GridView);
            _Page.RenderControl(HtmlTextWriter);
            _Page.Response.Write(xStrinWriter.ToString());
            _Page.Response.End();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="gv"></param>
        public static void ExportGridview(string fileName, GridView gv, Page _Page)
        {
            //HttpContext.Current.Response.Clear();
            _Page.Response.Clear();
            _Page.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            _Page.Response.ContentType = "application/ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a table to contain the grid
                    Table table = new Table();

                    //  include the gridline settings
                    table.GridLines = gv.GridLines;

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);

                    //  render the htmlwriter into the response
                    _Page.Response.Write(sw.ToString());
                    _Page.Response.End();
                }
            }
        }
        /// <summary>
        /// Replace any of the contained controls with literals
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }

    }
    public class Win
    {
        private static StreamWriter sw;
        public static string CreateLogFiles()
        {

            string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString();
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            return sLogFormat;
        }
        public static void EventLog(string errorMessage, string errorFolder)
        {
            try
            {
                string path = errorFolder + "/Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                using (sw = File.AppendText(path))
                {
                    sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                    sw.WriteLine(" ");
                    sw.WriteLine(" ");
                    sw.WriteLine("Message:");
                    sw.WriteLine(errorMessage);
                    sw.WriteLine(" ");
                    sw.WriteLine("------------------------------------------------------------------------");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex, errorFolder);
            }
            finally
            {
                sw.Close();
            }
        }
        public static void EventLog(string errorMessage, string errorFolder, Enumeracion.EventLog _TypeEvent, string _LogName)
        {
            try
            {
                switch (_TypeEvent)
                {
                    case Enumeracion.EventLog.Resumida:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine(" ");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    case Enumeracion.EventLog.UnaLinea:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles() + " | Event: (" + errorMessage + ")");
                                sw.Flush();
                                break;
                            }
                        }
                    case Enumeracion.EventLog.TodaInformacion:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                                sw.WriteLine(" ");
                                sw.WriteLine("Message:");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex) { ErrorLog(ex, errorFolder); }
            finally { sw.Close(); }
        }
        public static void EventLog(string errorMessage, string errorFolder, Enumeracion.Utils.EventLog _TypeEvent, string _LogName)
        {
            try
            {
                switch (_TypeEvent)
                {
                    case Enumeracion.Utils.EventLog.Sumary:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine(" ");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    case Enumeracion.Utils.EventLog.ALine:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles() + " | Event: (" + errorMessage + ")");
                                sw.Flush();
                                break;
                            }
                        }
                    case Enumeracion.Utils.EventLog.AllInformation:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                                sw.WriteLine(" ");
                                sw.WriteLine("Message:");
                                sw.WriteLine(errorMessage);
                                sw.WriteLine(" ");
                                sw.WriteLine("------------------------------------------------------------------------");
                                sw.Flush();
                            }
                            break;
                        }
                    case Enumeracion.Utils.EventLog.Simple:
                        {
                            string path = errorFolder + "/" + _LogName + "_Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                            }
                            using (sw = File.AppendText(path))
                            {
                                sw.WriteLine(errorMessage);
                                sw.Flush();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch (Exception ex) { ErrorLog(ex, errorFolder); }
            finally { sw.Close(); }
        }
        public static void ErrorLog(Exception errorMessage, string errorFolder)
        {
            try
            {
                string path = errorFolder + "/Log_" + Fechas.GetFechaActual(Enumeracion.FormatoFecha.YYYYMMDD) + ".txt";
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                using (sw = File.AppendText(path))
                {
                    sw.WriteLine("Fecha:" + " " + CreateLogFiles());
                    sw.WriteLine("Fuente:" + " " + errorMessage.Source);
                    sw.WriteLine(" ");
                    sw.WriteLine("Error:");
                    sw.WriteLine(errorMessage.Message);
                    sw.WriteLine(" ");
                    sw.WriteLine("BaseError:");
                    sw.WriteLine(errorMessage.GetBaseException().Message);
                    sw.WriteLine(" ");
                    sw.WriteLine("StackTrace:");
                    sw.WriteLine(errorMessage.StackTrace);
                    sw.WriteLine("------------------------------------------------------------------------");
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex, errorFolder);
            }
            finally { sw.Close(); }
        }
        public static bool CreateLog(string strLogName)
        {
            bool Result = false;

            try
            {
                System.Diagnostics.EventLog.CreateEventSource(strLogName, strLogName);
                EventLog SQLEventLog = new EventLog();

                SQLEventLog.Source = strLogName;
                SQLEventLog.Log = strLogName;

                SQLEventLog.Source = strLogName;
                SQLEventLog.WriteEntry("The " + strLogName + " was successfully initialize component.", EventLogEntryType.Information);

                Result = true;
            }
            catch
            {
                Result = false;
            }

            return Result;
        }
        public static void EventLog(string strLogName, string strSource, string strErrDetail)
        {
            EventLog SQLEventLog = new EventLog();

            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(strLogName))
                {
                    CreateLog(strLogName);
                }
                SQLEventLog.Source = strLogName;
                SQLEventLog.WriteEntry(Convert.ToString(strSource) + " " + Convert.ToString(strErrDetail), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                SQLEventLog.Source = strLogName;
                SQLEventLog.WriteEntry(Convert.ToString("INFORMATION: ") + Convert.ToString(ex.Message), EventLogEntryType.Information);
            }
            finally
            {
                SQLEventLog.Dispose();
                SQLEventLog = null;
            }
        }
        public static List<_W.TextBox> EmptyTextBoxs(_W.Control[] control)
        {
            List<_W.TextBox> textboxes = new List<_W.TextBox>();
            for (int i = 0; i < control.Length; i++)
            {
                foreach (object item in control[i].Controls)
                {
                    if (item is _W.TextBox)
                    {
                        if (string.IsNullOrEmpty(((_W.TextBox)item).Text))
                        {
                            textboxes.Add((_W.TextBox)item);
                        }
                    }
                }
            }

            return textboxes;
        }
        public static bool IsEmptyTextBoxs(_W.Control[] control)
        {
            List<_W.TextBox> textboxes = new List<_W.TextBox>();
            bool _IsEmpty = false;
            for (int i = 0; i < control.Length; i++)
            {
                foreach (object item in control[i].Controls)
                {
                    if (item is _W.TextBox)
                    {
                        if (string.IsNullOrEmpty(((_W.TextBox)item).Text))
                        {
                            return true;
                        }
                    }
                }
            }

            return _IsEmpty;
        }
        public static void ClearTextBoxs(_W.Control[] control)
        {
            for (int i = 0; i < control.Length; i++)
            {
                foreach (object item in control[i].Controls)
                {
                    if (item is _W.TextBox)
                    {
                        ((_W.TextBox)item).Text = "";
                    }
                }
            }
        }
        public static List<_W.TextBox> EmptyTextBoxs(_W.Control control)
        {
            List<_W.TextBox> textboxes = new List<_W.TextBox>();
            foreach (object item in control.Controls)
            {
                if (item is _W.TextBox)
                {
                    if (string.IsNullOrEmpty(((_W.TextBox)item).Text))
                    {
                        textboxes.Add((_W.TextBox)item);
                    }
                }
            }

            return textboxes;
        }
        public static bool IsEmptyTextBoxs(_W.Control control)
        {
            bool _IsEmpty = false;
            List<_W.TextBox> textboxes = new List<_W.TextBox>();
            foreach (object item in control.Controls)
            {
                if (item is _W.TextBox)
                {
                    if (string.IsNullOrEmpty(((_W.TextBox)item).Text))
                    {
                        return true;
                    }
                }
            }

            return _IsEmpty;
        }
        public static void ClearTextBoxs(_W.Control control)
        {
            foreach (object item in control.Controls)
            {
                if (item is _W.TextBox)
                {
                    ((_W.TextBox)item).Text = "";
                }
            }
        }
        public static void CrearLicencia(string pathFolder, string empresa)
        {

            try
            {

                string path = pathFolder + "/Licencia.lic";

                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                //Open a FileStream on the file "aboutme"
                FileStream fout = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                //Create a BinaryWriter from the FileStream
                BinaryWriter bw = new BinaryWriter(fout);
                //Create some arbitrary variables
                string _empresa = empresa;
                char[] chrs = _empresa.ToCharArray();
                int _version = 20;
                DateTime _update = DateTime.Now;

                //Write the values to file
                //foreach (char item in empresa)
                //{
                //    bw.Write(item);    
                //}
                bw.Write(chrs);
                bw.Write(_empresa);
                bw.Write(_version);
                bw.Write(_update.ToString());

                //Close the file 
                bw.Close();
                fout.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //  bw.Close();
            }
        }
        public static string[] LeerLicencia(string pathFolder)
        {
            try
            {

                string path = pathFolder + "/Licencia.lic";

                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }

                //Open a FileStream in Read mode
                FileStream fin = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                //Create a BinaryReader from the FileStream
                BinaryReader br = new BinaryReader(fin);

                //Seek to the start of the file
                br.BaseStream.Seek(0, SeekOrigin.Begin);

                //Read from the file and store the values to the variables
                char[] chrs = br.ReadChars(10);
                string _empresa = br.ReadString();
                int _version = br.ReadInt32();
                DateTime _update = DateTime.Parse(br.ReadString());

                string[] _Return = new string[3] { _empresa, _version.ToString(), _update.ToString() };

                //Close the stream and free the resources
                br.Close();
                fin.Close();
                return _Return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //br.Close();
            }
        }
        public static string CompareDirectory(string _source, string _destination)
        {
            DirectoryInfo source = new DirectoryInfo(_source);
            DirectoryInfo destination = new DirectoryInfo(_destination);
            int a1 = source.GetFiles("*", SearchOption.AllDirectories).Length;
            long _tota1 = 0;
            for (int i = 0; i < a1; i++)
            {
                _tota1 += source.GetFiles("*", SearchOption.AllDirectories)[i].Length;
            }
            long _tota2 = 0;
            int a2 = destination.GetFiles("*", SearchOption.AllDirectories).Length;
            for (int i = 0; i < a1; i++)
            {
                _tota2 += destination.GetFiles("*", SearchOption.AllDirectories)[i].Length;
            }
            if (_tota1 == _tota2)
            {
                return "Los directorios son identicos...\r\n Peso: " + (Conversiones.BytesToMegabytes(_tota2)).ToString("0.00") + "MB (" + _tota2.ToString("###,###,###.##") + " bytes)";
            }
            else
            {
                return "Hay diferencias entre sus archivos...\r\n  " + _source + ": " + _tota1.ToString("###,###,###.##") + " bytes \r\n\r\n " + _destination + ": " + _tota2.ToString("###,###,###.##") + " bytes";
            }
        }

        public static void CopyDirectory(string _source, string _destination)
        {
            DirectoryInfo source = new DirectoryInfo(_source);
            DirectoryInfo destination = new DirectoryInfo(_destination);
            if (!destination.Exists)
            {
                destination.Create();
            }
            // Copy all files. 
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName,
                    file.Name));
            }
            // Process subdirectories. 
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory. 
                string destinationDir = Path.Combine(destination.FullName, dir.Name);
                // Call CopyDirectory() recursively. 
                CopyDirectory(dir.ToString(), new DirectoryInfo(destinationDir).ToString());
            }
        }

        public static void CreateXMLStructure(string _NameDataSet, string _NameKeyColumn, string _Fields, string _FilePath, string _FileName)
        {
            string[] _ListField = new string[0];
            _ListField = _Fields.Split(',');
            StringBuilder _xml = new StringBuilder();
            _xml.Append("<?xml version='1.0' encoding='ISO-8859-1'?>");
            _xml.Append("<" + _NameDataSet.ToUpper() + ">");
            _xml.Append("<" + _NameKeyColumn.ToUpper() + ">");
            for (int i = 0; i < _ListField.Length; i++)
            {
                _xml.Append("<" + _ListField[i].ToUpper() + "/>");
            }
            _xml.Append("</" + _NameKeyColumn.ToUpper() + ">");
            _xml.Append("</" + _NameDataSet.ToUpper() + ">");

            string path = _FilePath + @"\" + _FileName + ".xml";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(_xml.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        public static void ZipFolder(string _FolderSource, string _FolderDestination, string _namefile)
        {
            Process ob = new Process();
            string _S = "\"";
            string _Script = _S + _FolderDestination + _S + " -r " + _S + _FolderSource + _S;
            ob.StartInfo = new System.Diagnostics.ProcessStartInfo("ZIP", _Script);
            ob.StartInfo.UseShellExecute = false;
            ob.Start();
        }

        public static string[] GetFiles(string _PathFiles)
        {
            string[] filePaths = Directory.GetFiles(_PathFiles);
            return filePaths;
        }
        public static string[] GetFiles(string _PathFiles, string _FilterExtension)
        {
            string[] filePaths = Directory.GetFiles(_PathFiles, _FilterExtension);
            return filePaths;
        }
        public static string[] GetFiles(string _PathFiles, string _FilterExtension, SearchOption _IncluyeSubdirectorios)
        {
            string[] filePaths = Directory.GetFiles(_PathFiles, _FilterExtension, _IncluyeSubdirectorios);
            return filePaths;
        }
        public static void MoverArchivos(string _Source, string _Target)
        {
            File.Move(_Source, _Target);
        }
        public static void RenombrarArchivos(string _Source, string _Target)
        {
            File.Move(_Source, _Target);
        }
        public static string GetFileAsString(string fileName)
        {
            StreamReader sReader = null;
            string contents = null;
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sReader = new StreamReader(fileStream);
                contents = sReader.ReadToEnd();
            }
            finally
            {
                if (sReader != null)
                {
                    sReader.Close();
                }
            }
            return contents;
        }
    }
    public class Resultado
    {
        static string _NameSpace = "", _Class = "", _Metodo = "", _Descripcion = "";
        public enum eSalida { Nothing, Success, Error };
        static Exception _Excepcion = null;
        static eSalida _Salida = eSalida.Nothing;
        public static string NameSpace { get { return ("NameSpace").PadRight(20, '.') + ": " + _NameSpace; } set { _NameSpace = value; } }
        public static string Class { get { return ("Class").PadRight(20, '.') + ": " + _Class; } set { _Class = value; } }
        public static string Metodo { get { return ("Metodo").PadRight(20, '.') + ": " + _Metodo; } set { _Metodo = value; } }
        public static string Descripcion { get { return ("Descripcion").PadRight(20, '.') + ": " + _Descripcion; } set { _Descripcion = value; } }
        public static eSalida Salida { get { return _Salida; } set { _Salida = value; } }
        public static Exception Excepcion { get { return _Excepcion; } set { _Excepcion = value; } }
        public static string Resumen()
        {
            StringBuilder _return = new StringBuilder();
            _return.AppendLine(("Salida").PadRight(20, '.') + ": " + Salida.ToString());
            _return.AppendLine(("Ejecutado").PadRight(20, '.') + ": " + Fechas.GetFechaHoraActual());
            _return.AppendLine(NameSpace);
            _return.AppendLine(Class);
            _return.AppendLine(Metodo);
            _return.AppendLine(Descripcion);
            _return.AppendLine("");
            if (!object.Equals(Excepcion, null))
            {
                _return.AppendLine("Detalle de Error:");
                _return.AppendLine((" Exception Message").PadRight(25, '.') + ": " + _Excepcion.Message);
                _return.AppendLine((" Exception Source").PadRight(25, '.') + ": " + _Excepcion.Source);
                _return.AppendLine((" Exception HelpLink").PadRight(25, '.') + ": " + _Excepcion.HelpLink);
                _return.AppendLine((" Exception Inner").PadRight(25, '.') + ": " + _Excepcion.InnerException);
                _return.AppendLine((" Exception StackTrace").PadRight(25, '.') + ": " + _Excepcion.StackTrace);
            }
            return _return.ToString();
        }


    }
    public class Mensajes
    {
        public static class Errores
        {
            public const string ArgumentoInvalido = "No todos los argumentos son válidos, por favor verifiquelos y vuelva a intentarlo...";
            public const string FaltanAgumentos = "Faltan argumentos en la expresion...";
        }
        public static class Datos
        {
            public const string RegistroAgregado = "Registro(s) agregado(s) satisfactoriamente...";
            public const string RegistroActualizado = "Registro(s) actualizado(s) satisfactoriamente...";
            public const string RegistroEliminado = "Registro(s) eliminado(s) satisfactoriamente...";
        }
        public static class Warning
        {
            public const string SeguroDeAgregar = "Está seguro de agregar esta información..?";
            public const string SeguroDeActualizar = "Está seguro de actualizar esta información..?";
            public const string SeguroDeEliminar = "Está seguro de eliminar esta información..?";
        }
        public static class Info
        {
            public const string IntenteNuevamente = "Por favor vuelva a intentar la operación...";
            public const string EmailEnviado = "Se ha enviado la información vía email correctamente...";
            public const string Espere = "Espere mientras se procesa la información, este proceso puede durar varios minutos...";
            public const string NoHayRegistros = "No hay registro(s) que mostrar...";
            public const string NoEncontrado = "No se ha encontrado ningún resultado...";
            public const string OperacionCancelada = "Se ha cancelado el proceso...";
            public const string CamposVacios = "No se puede realizar el proceso, existen campos vacios, por favor verifiquelos y vuelva a intentarlo...";
            public const string ModuloNoDisponible = "Este módulo no esta disponible, comuniquese con el administrador del sistema...";
        }
    }
    public class XMLSerializable
    {
        public static void SerializeToXML<T>(T parametro, string pathFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StreamWriter(pathFile);
            serializer.Serialize(textWriter, parametro);
            textWriter.Close();
        }
        public static T DeserializeFromXML<T>(string pathFile)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            TextReader textReader = new StreamReader(pathFile);
            T parametro;
            parametro = (T)deserializer.Deserialize(textReader);
            textReader.Close();
            return parametro;
        }
    }
}

