using SharpGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VSIXProjectHelloWorld
{
    /// <summary>
    /// Interaction logic for UserControl.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public ObservableCollection<MyObject> objects;
        public string txtbEnterExpressionText = "";

        public UserControl1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtbEnterExpressionText = txtbEnterExpression.Text;
        }
        double alpha = 0.0d;

        private void OpenGLControl_OpenGLDraw(object sender, RoutedEventArgs args)
        {
            alpha += 0.07;
            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Color(0f, 1f, 0f);
            gl.Vertex(-Math.Cos(alpha), -1f);
            gl.Vertex(0f, 1f);
            gl.Vertex(Math.Cos(alpha), -1f);
            gl.End();
        }


        private void OpenGLControl_OpenGLInitialized(object sender, RoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.ClearColor(0.3f, 0.3f, 0.3f, 0.3f);
        }

        private void OpenGLControl_Resized(object sender, RoutedEventArgs args)
        {
        }
    }
    public class MyObject : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;
        private Color color;
        private string type;
        private string addres;

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

        public Color m_C_Color
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
