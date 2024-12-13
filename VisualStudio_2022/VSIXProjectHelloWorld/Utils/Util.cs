using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSIXProjectHelloWorld.Utils;

namespace VSIXProjectHelloWorld
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
