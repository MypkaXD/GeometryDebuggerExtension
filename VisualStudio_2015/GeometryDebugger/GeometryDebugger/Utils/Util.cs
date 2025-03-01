using System.Collections.ObjectModel;

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
                           left.m_C_Color == right.m_C_Color &&
                           left.m_S_Addres == right.m_S_Addres &&
                           left.m_S_Name == right.m_S_Name &&
                           left.m_S_Source == right.m_S_Source &&
                           left.m_S_Type == right.m_S_Type)
                return true;
            else
                return false;
        }

        static public string getPathOfVariable(string path, Variable variable)
        {
            return path + variable.m_S_Name + "_" + variable.m_S_Source + "_" + variable.m_S_Addres;
        }
    }
}
