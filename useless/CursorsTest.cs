using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace useless
{
    public partial class CursorsTest : Form
    {
        public CursorsTest()
        {
            InitializeComponent();
            foreach (System.Reflection.PropertyInfo prop in typeof(Cursors).GetProperties())
            {
                if (Regex.IsMatch(prop.Name, "split|no|pan", RegexOptions.IgnoreCase))
                    continue;
                GroupBox gb = new GroupBox() { Width = 100 };
                gb.Text = prop.Name;
                Cursor cur = prop.GetValue(null) as Cursor;
                if (cur == null)
                    gb.Text += "{SOME ERROR}";
                else
                    gb.Cursor = cur;
                flowLayoutPanel1.Controls.Add(gb);
            }
        }

        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
