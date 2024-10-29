using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProjectHelloWorld.Utils
{
    public class Variable : INotifyPropertyChanged
    {
        private string name;
        private string source;
        private string type;
        private string addres;

        private bool isAdded;
        private bool isSelected;

        private SolidColorBrush color;

        public string m_S_Type
        {
            get => this.type;
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(nameof(m_S_Type));
                }
            }
        }
        public string m_S_Addres
        {
            get => this.addres;
            set
            {
                if (this.addres != value)
                {
                    this.addres = value;
                    this.OnPropertyChanged(nameof(m_S_Addres));
                }
            }
        }
        public string m_S_Source
        {
            get => this.source;
            set
            {
                if (this.source != value)
                {
                    this.source = value;
                    this.OnPropertyChanged(nameof(m_S_Source));
                }
            }
        }
        public string m_S_Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(nameof(m_S_Name));
                }
            }
        }

        public bool m_B_IsAdded
        {
            get => this.isAdded;
            set
            {
                if (this.isAdded != value)
                {
                    this.isAdded = value;
                    this.OnPropertyChanged(nameof(m_B_IsAdded));
                }
            }
        }
        public bool m_B_IsSelected
        {
            get => this.isSelected;
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(m_B_IsSelected));
                }
            }
        }

        public SolidColorBrush m_C_Color
        {
            get => this.color;
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged(nameof(m_C_Color));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
