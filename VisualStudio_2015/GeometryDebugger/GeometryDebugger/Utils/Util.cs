using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace GeometryDebugger.Utils
{
    public class Util
    {
        static public bool isContainVariable(ref ObservableCollection<Variable> variables, string name)
        {
            foreach (var variable in variables)
            {
                if (variable.m_S_Name == name)
                    return true;
            }
            return false;
        }
        static public bool isEqualVariables(Variable left, Variable right)
        {
            if (left.m_B_IsAdded == right.m_B_IsAdded &&
                           left.m_B_IsSelected == right.m_B_IsSelected &&
                           left.m_B_IsSerialized == right.m_B_IsSerialized &&
                           left.m_C_Color == right.m_C_Color &&
                           left.m_S_Addres == right.m_S_Addres &&
                           left.m_S_Name == right.m_S_Name &&
                           left.m_S_Source == right.m_S_Source &&
                           left.m_S_Type == right.m_S_Type)
                return true;
            else
                return false;
        }

        static string SanitizeFileName(string name)
        {
            List<string> forbiddenChars = new List<string>() { "<", ">", ":", "\\", "/", "|", "?", "*", "&" };

            string result_string = name;

            for (int i = 0; i < forbiddenChars.Count; ++i)
            {
                while (result_string.Contains(forbiddenChars[i]))
                {
                    result_string = result_string.Replace(System.Convert.ToChar(forbiddenChars[i]), '_');
                }
            }

            return result_string;
        }

        static public string getPathOfVariable(string path, Variable variable)
        {
            string name = SanitizeFileName(variable.m_S_Name);
            return path + name + "_" + variable.m_S_Source + "_" + variable.m_S_Addres + "_depth" + variable.m_i_NumberOfChilds;
        }
    }
}
