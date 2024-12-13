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
    }
}
