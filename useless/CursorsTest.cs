using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace useless
{
    public partial class CursorsTest : Form
    {
        public CursorsTest()
        {
            InitializeComponent();
            foreach (var prop in typeof(Cursors).GetProperties())
            {
                if (Regex.IsMatch(prop.Name, "split|no|pan", RegexOptions.IgnoreCase)) continue;
                var gb = new GroupBox() { Width = 100 };
                gb.Text = prop.Name;
                var cur = prop.GetValue(null) as Cursor;
                if (cur == null) gb.Text += "{SOME ERROR}";
                else gb.Cursor = cur;
                flowLayoutPanel1.Controls.Add(gb);
            }
        }

        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
