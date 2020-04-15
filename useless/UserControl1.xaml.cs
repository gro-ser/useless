using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            foreach (System.Reflection.PropertyInfo prop in typeof(Cursors).GetProperties())
            {
                if (prop.Name.StartsWith("Scroll"))
                    continue;
                Button but = new Button
                {
                    Content = prop.Name
                };
                Cursor cur = prop.GetValue(null) as Cursor;
                if (cur == null)
                    but.Content += "{SOME ERROR}";
                else
                    but.Cursor = cur;
                controls.Children.Add(but);
            }
        }
    }
}
