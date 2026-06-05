using System.Globalization;

namespace Domain;

public static class Extension
{
    public static int ToInteger(this string strToConvert)
    {
        int strConvert;
        int.TryParse(strToConvert, out strConvert);
        return strConvert;
    }

    public static int ToInteger(this object objToConvert)
    {
        int objConvert;
        int.TryParse(objToConvert.ToString(), out objConvert);
        return objConvert;
    }

    public static string ToDateBr(this DateTime objToConvert)
    {
        CultureInfo cultureInfo = new CultureInfo("pt-BR");
        string formattedDate = objToConvert.ToString("dd/MM/yyyy", cultureInfo);
        return formattedDate;
    }

    public static DateTime ToDateTimeBr(this string objToConvert)
    {
        DateTime obj;
        CultureInfo cultureInfo = new CultureInfo("pt-BR");
        DateTime.TryParse(objToConvert, cultureInfo, out obj);

        return obj;
    }

    public static decimal ToDecimal(this string objToConvert)
    {
        decimal obj;
        CultureInfo cultureInfo = new CultureInfo("pt-BR");
        decimal.TryParse(objToConvert, cultureInfo, out obj);

        return obj;
    }

    public static bool ToBoolean(this string strToConvert)
    {
        if (string.IsNullOrWhiteSpace(strToConvert))
            return false;

        strToConvert = strToConvert.Trim();

        if (strToConvert == "0") return false;
        if (strToConvert == "1") return true;

        if (bool.TryParse(strToConvert, out bool result))
            return result;

        return false;
    }

    public static bool ToBoolean(this int value)
    {
        return value == 1;
    }

    public static bool ToBoolean(this ushort value)
    {
        return value == 1;
    }

    public static bool ToBoolean(this object value)
    {
        if (value == null) return false;

        switch (value)
        {
            case string s: return s.ToBoolean();
            case int i: return i.ToBoolean();
            case ushort u: return u.ToBoolean();
            case bool b: return b;
            default:
                return false;
        }
    }
}