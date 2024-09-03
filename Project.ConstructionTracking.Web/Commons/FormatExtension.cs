using HeyRed.Mime;
using Microsoft.VisualBasic;
using System.Globalization;

namespace Project.ConstructionTracking.Web.Commons
{
    public static class FormatExtension
    {
        public static string StandardNumberFormat = "{0:#,##0.00}";
        public static string StandardNumberFormat2 = "#,##0";
        public static string StandardCulture = "en-US";
        public static string StandardDateFormat = "yyyy-MM-dd";
        public static string StandardDateTimeFormat = "yyyy-MM-dd HH:mm";
        public static Guid AsGuid(this Guid? value1)
        {
            return value1.HasValue ? value1.Value : Guid.Empty;
        }
        public static int AsInt(this int? value1)
        {
            return value1.HasValue ? value1.Value : 0;
        }
        public static long AsLong(this long? value1)
        {
            return value1.HasValue ? value1.Value : 0;
        }
        public static DateTime AsDate(this DateTime? value1)
        {
            return value1.HasValue ? value1.Value : DateTime.Now;
        }
        public static bool AsBool(this bool? value1)
        {
            return value1.HasValue ? value1.Value : false;
        }
        public static decimal AsDecimal(this decimal? value1)
        {
            return value1.HasValue ? value1.Value : 0;
        }

        public static int? ToInt(this string str)
        {
            int result;
            if (int.TryParse(str, out result))
                return result;

            return null;
        }
        public static DateTime? ToDate(this string str)
        {
            DateTime result;
            if (!string.IsNullOrEmpty(str.ToStringNullable()))
            {
                if (DateTime.TryParse(str, out result))
                    return result;
            }
            return null;
        }
        public static string? ToStringNullable(this string? param)
        {
            return string.IsNullOrEmpty(param) ? null : param.Trim();
        }
        public static string ToStringEmpty(this string param)
        {
            return string.IsNullOrEmpty(param) ? string.Empty : param.Trim();
        }
        public static string ToStringNumber(this decimal number)
        {
            return string.Format(StandardNumberFormat, number);
        }
        public static string ToStringNumber(this long number)
        {
            return string.Format(StandardNumberFormat, number);
        }
        public static string? ToStringNumber(this decimal? number)
        {
            if (number == null)
            {
                return null;
            }
            return string.Format(StandardNumberFormat, number);
        }
        public static string ToStringNumber(this int number)
        {
            return number.ToString(StandardNumberFormat2);
        }
        public static string ToStringDateTime(this DateTime? dateValue)
        {
            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.HasValue ? dateValue.Value.ToString(StandardDateTimeFormat, culture) : string.Empty;
        }
        public static string ToStringDateTime(this DateTime dateValue)
        {
            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.ToString(StandardDateTimeFormat, culture);
        }
        public static string ToStringDate(this DateTime? dateValue)
        {

            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.HasValue ? dateValue.Value.ToString(StandardDateFormat, culture) : string.Empty;
        }
        public static string ToStringDate(this DateTime dateValue)
        {

            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.ToString(StandardDateFormat, culture);
        }
        public static string Right(this string sValue, int iMaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(sValue))
            {
                //Set valid empty string as string could be null
                sValue = string.Empty;
            }
            else if (sValue.Length > iMaxLength)
            {
                //Make the string no longer than the max length
                sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
            }

            //Return the string
            return sValue;
        }

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public static string GetExtension(this string contentType)
        {
            return MimeTypesMap.GetExtension(contentType);
        }

        /// <summary>
        /// NullToString
        /// คือ Function สำหรับแปลง Object ใดๆ เป็น string
        /// </summary>
        /// <param name="obj">Object ใดๆที่ต้องการเปลี่ยนเป็น string </param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public static string NullToString(object obj, string defaultString = "")
        {
            return NullToAnyString(obj, defaultString);
        }
        public static string NullToAnyString(object obj, string defaultString = " - ")
        {
            string temp = defaultString;
            if (obj == null)
            {
                temp = defaultString;
            }
            else if (System.Convert.IsDBNull(obj))
            {
                temp = defaultString;
            }
            else if (obj.ToString().Trim() == "")
            {
                temp = defaultString;
            }
            else
            {
                temp = obj.ToString();
            }
            return temp;
        }
        /// <summary>
        /// คือ Function สำหรับแปลง Object ใดๆ เป็น int
        /// </summary>
        /// <param name="obj">Object ใดๆที่ต้องการเปลี่ยนเป็น int</param>
        /// <param name="defaultint"></param>
        /// <returns></returns>
        public static int Nulltoint(object obj, int defaultint = 0)
        {
            if (obj != null)
            { /* เผื่อสำหรับกรณีที่ส่งเข้ามาแบบ  1,123  ให้ตัดสตริง , ออกก่อน เพราะว่าเวลาไม่ตัดแล้ว convert ออกมาเป็น int จะได้ 0  */
                try { obj = obj.ToString().Replace(",", ""); } catch { }
            }

            int Temp = defaultint;
            if (obj == null)
            {
                Temp = defaultint;
            }
            else if (Information.IsDBNull(obj))
            {
                Temp = defaultint;
            }
            else if (obj.ToString().Trim() == "")
            {
                Temp = defaultint;
            }
            else if (Information.IsNumeric(obj.ToString()) == false)
            {
                Temp = defaultint;
            }
            else
            {
                int.TryParse(obj.ToString(), out Temp);
            }
            return Temp;
        }

        public static string ConvertToDateString(object dateObject)
        {
            if (dateObject == null || dateObject == DBNull.Value)
            {
                return string.Empty;
            }

            if (DateTime.TryParse(dateObject.ToString(), out DateTime date))
            {
                return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

    }
}
