using System;
using System.Collections.Generic;
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

namespace useless
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class CurTets : Window
    {
        public CurTets()
        {
            InitializeComponent();
            foreach (var prop in typeof(Cursors).GetProperties())
            {
                if (prop.Name.StartsWith("Scroll")) continue;
                var but = new Button();
                but.Content = prop.Name;
                var cur = prop.GetValue(null) as Cursor;
                if (cur == null) but.Content += "{SOME ERROR}";
                else but.Cursor = cur;
                controls.Children.Add(but);
            }
        }
    }
}
