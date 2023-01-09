using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlTransactUtility.Utilities
{
    internal class TimeUtils
    {
        //Date query builder
        public string dateQuery(DateTime to, DateTime from, string region, string dateFormat)
        {
            //set datetimes with correct formatting - uk = "en-GB" = "dd-MMM-yyyy HH:mm"
            string start = to.ToString(dateFormat, new CultureInfo(region));
            string end = from.ToString(dateFormat, new CultureInfo(region));

            string query = "DECLARE @start DateTime = '" + start + "' \r\n" +
                                "DECLARE @end DateTime = '" + end + "' \r\n";

            return query;
        }
        //Duration query builder
        public string durationQuery(string increment, double duration, string region, string dateFormat)
        {
            DateTime origin;

            switch (increment)
            {
                case "Days":
                    origin = DateTime.Now.AddDays(-duration);
                    break;
                case "Hours":
                    origin = DateTime.Now.AddHours(-duration);
                    break;
                case "Min":
                    origin = DateTime.Now.AddMinutes(-duration);
                    break;
                default:
                    origin = DateTime.Now.AddHours(-1);
                    break;
            }

            //set datetimes with correct formatting - uk = "en-GB" = "dd-MMM-yyyy HH:mm"
            string start = origin.ToString(dateFormat, new CultureInfo(region));
            string end = DateTime.Now.ToString(dateFormat, new CultureInfo(region));

            string query = "DECLARE @start DateTime = '" + start + "' \r\n" +
                                "DECLARE @end DateTime = '" + end + "' \r\n";

            return query;
        }

    }
}
