﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GeometryDebugger.Utils
{
    public class ChildrenVariable : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;
        private Color color;
        private int index;

        public Variable m_parrent;

        public ChildrenVariable(bool isSelected, string name, Color color, int index, Variable parrent)
        {
            this.m_C_Color = color;
            this.m_b_isSelected = isSelected;
            this.m_s_Name = name;
            this.m_i_Index = index;
            this.m_parrent = parrent;
        }

        public int m_i_Index
        {
            get
            {
                return this.index;

            }
            set
            {
                if (this.index != value)
                {
                    this.index = value;
                    this.OnPropertyChanged(nameof(m_i_Index));
                }
            }
        }

        public Color m_C_Color
        {
            get
            {
                return this.color;

            }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged(nameof(m_C_Color));
                }
            }
        }

        public string m_s_Name
        {
            get
            {
                return this.name;

            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(nameof(m_s_Name));
                }
            }
        }

        public bool m_b_isSelected
        {
            get
            {
                return this.isSelected;

            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(m_b_isSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Variable : INotifyPropertyChanged
    {
        private string name;
        private string source;
        private string type;
        private string addres;

        private bool isAdded;
        private bool isSelected;
        private bool isSerialized;

        private Color color;

        private ObservableCollection<ChildrenVariable> childrens;

        public int m_i_NumberOfChilds = 0;

        public ObservableCollection<ChildrenVariable> m_OC_Childrens
        {
            get
            {
                return this.childrens;

            }
            set
            {
                if (this.childrens != value)
                {
                    this.childrens = value;
                    this.OnPropertyChanged(nameof(m_OC_Childrens));
                }
            }
        }

        public string m_S_Type
        {
            get
            {
                return this.type;

            }
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
            get
            {
                return this.addres;
            }
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
            get
            {
                return this.source;
            }
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
            get
            {
                return this.name;
            }
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
            get
            {
                return this.isAdded;
            }
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
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(m_B_IsSelected));
                }
            }
        }
        public bool m_B_IsSerialized
        {
            get
            {
                return this.isSerialized;
            }
            set
            {
                if (this.isSerialized != value)
                {
                    this.isSerialized = value;
                    this.OnPropertyChanged(nameof(m_B_IsSerialized));
                }
            }
        }

        public Color m_C_Color
        {
            get
            {
                return this.color;
            }
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

    public struct Color
    {
        public int m_i_R { get; set; }
        public int m_i_G { get; set; }
        public int m_i_B { get; set; }

        public Color(int r, int g, int b)
        {
            m_i_R = r;
            m_i_G = g;
            m_i_B = b;
        }

        public static bool operator !=(Color colorFirst, Color colorSecond)
        {
            return !(colorFirst == colorSecond);
        }

        public static bool operator ==(Color colorFirst, Color colorSecond)
        {
            return (colorFirst.m_i_R == colorSecond.m_i_R &&
                colorFirst.m_i_G == colorSecond.m_i_G &&
                colorFirst.m_i_B == colorSecond.m_i_B);
        }
    }
}
