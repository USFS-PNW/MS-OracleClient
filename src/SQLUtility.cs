using System;
using System.Collections.Generic;

namespace Oracle.ADO
{
  public class SQLUtility
  {
    public static string InsertParameters(ref List<MySQLParameter> orclParameters, List<int> valueList, string uniqueParName)
    {
      string IN_List_Str = string.Empty;

      for (int i = 0; i <= valueList.Count - 1; i++)
      {
        IN_List_Str += i == 0 ? ":" + uniqueParName + (i+1) : ", :" + uniqueParName + (i+1);

        MySQLParameter param = new MySQLParameter(uniqueParName + (i+1).ToString(), valueList[i]);
        orclParameters.Add(param);
      }
      return IN_List_Str;
    }
    public struct MySQLParameter
    {

      public string name;
      public string value;
      public object date;
      public object nullValue;
      public string[] valueArray;

      public MySQLParameter(string nameIn, string valueIn)
      {
        name = nameIn;
        value = valueIn;
        date = null;
        nullValue = null;
        valueArray = null;
      }
      public MySQLParameter(string nameIn, DateTime dateIn)
      {
        name = nameIn;
        date = dateIn;
        value = null;
        nullValue = null;
        valueArray = null;
      }
      public MySQLParameter(string nameIn)
      {
        name = nameIn;
        value = null;
        date = null;
        nullValue = null;
        valueArray = null;
      }
      public MySQLParameter(string nameIn, int? valueIn)
      {
        name = nameIn;
        value = null;
        date = null;
        nullValue = null;
        valueArray = null;

        if (valueIn != null)
        {
          value = valueIn.ToString();
        }
        else
        {
          nullValue = DBNull.Value;
        }
      }
      public MySQLParameter(string nameIn, decimal? valueIn)
      {
        name = nameIn;
        value = null;
        date = null;
        nullValue = null;
        valueArray = null;

        if (valueIn != null)
        {
          value = valueIn.ToString();
        }
        else
        {
          nullValue = DBNull.Value;
        }
      }
      public MySQLParameter(string nameIn, string[] valueArrayIn)
      {
        name = nameIn;
        value = null;
        date = null;
        nullValue = null;
        valueArray = valueArrayIn;

      }

    }
  }
}