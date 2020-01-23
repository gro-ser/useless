using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace useless
{
    public partial class RegexSolver : Form
    {
        class Settings
        {
            public string CharRange { get; set; }
                = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ\\";
            public int TimeOut { get; set; } = 5000;
            public int MaxRepeats { get; set; } = 5;
            public string FullCharRange => " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
            public string ShortCharRange => " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public string DefaultCharRange
                => " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ\\";
            public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;
        }

        int width, height, col, row;
        Regex[] top, bottom, left, right;
        readonly Settings settings = new Settings();

        public RegexSolver()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = settings;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ClearGrid();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            left = right = top = bottom = null;

            width = int.Parse(textBox1.Text);
            height = int.Parse(textBox2.Text);

            dgv.Columns.Clear();
            dgv.Columns.Add("left", "left");
            for (int i = 1; i <= width; i++)
                dgv.Columns.Add(i + "", "#" + i);
            dgv.Columns.Add("right", "right");

            dgv.Columns[0].DefaultCellStyle.BackColor = Color.LightCyan;
            dgv.Columns[width + 1].DefaultCellStyle.BackColor = Color.LightCyan;

            dgv.Rows.Add(height + 2);

            dgv.Rows[0].DefaultCellStyle.BackColor = Color.LightCyan;
            dgv.Rows[height + 1].DefaultCellStyle.BackColor = Color.LightCyan;
            //dgv.Rows[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            for (int x = 0; x < width + 2; x += width + 1)
                for (int y = 0; y < height + 2; y += height + 1)
                {
                    dgv[x, y].Style.BackColor = Color.Silver;
                    dgv[x, y].ReadOnly = true;
                }

            //for (int x = 1; x <= width; ++x)
            //    for (int y = 1; y <= height; ++y)
            //        dgv[x, y].ReadOnly = true;

            //ClearGrid();

            foreach (DataGridViewColumn col in dgv.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            top = new Regex[width + 1];
            bottom = new Regex[width + 1];
            for (int i = 1; i <= width; i++)
            {
                top[i] = CreateRegex((string)dgv[i, 0].Value);
                bottom[i] = CreateRegex((string)dgv[i, height + 1].Value);
            }

            left = new Regex[height + 1];
            right = new Regex[height + 1];
            for (int i = 1; i <= height; i++)
            {
                left[i] = CreateRegex((string)dgv[0, i].Value);
                right[i] = CreateRegex((string)dgv[width + 1, i].Value);
            }
        }

        private Regex CreateRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                pattern = ".*";
            return new Regex($"^{pattern}$", settings.RegexOptions);
        }

        private void ClearGrid()
        {
            for (int x = 1; x <= width; ++x)
                for (int y = 1; y <= height; ++y)
                    dgv[x, y].Value = null;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            settings.CharRange = settings.ShortCharRange;
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            settings.CharRange = settings.DefaultCharRange;
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            settings.CharRange = settings.FullCharRange;
        }

        private void Dgv_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int col = e.ColumnIndex;
            UpdateByColumn(col);
        }

        private void UpdateByColumn(int col)
        {
            UseWaitCursor = true;

            if (col == 0 || col > width) return;

            var sources = new string[height];
            for (int i = 1; i <= height; i++)
                sources[i - 1] = dgv[col, i].FormattedValue.ToString();

            var res = SafeGetStringsByRegex(height, sources, new[] { top[col], bottom[col] });
            if (res != null)
                for (int i = 1; i <= height; i++)
                    dgv[col, i].Value = res[i - 1];

            UseWaitCursor = !true;
        }

        private void Dgv_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int row = e.RowIndex;
            UpdateByRow(row);

        }

        private void UpdateByRow(int row)
        {
            UseWaitCursor = true;

            if (row == 0 || row > height) return;

            var sources = new string[width];
            for (int i = 1; i <= width; i++)
                sources[i - 1] = dgv[i, row].FormattedValue.ToString();

            var res = SafeGetStringsByRegex(width, sources, new[] { left[row], right[row] });
            if (res != null)
                for (int i = 1; i <= width; i++)
                    dgv[i, row].Value = res[i - 1];

            UseWaitCursor = !true;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = dgv;
        }

        private void Button4_Click_1(object sender, EventArgs e)
        {
            AutoFind();
        }

        private void AutoFind()
        {
            if (top == null) Setup();
            int repeats = 0;
            (col, row) = (1, 1);
            if (height > width) col = width + 1;
            Save();
            while (!IsEnded())
            {
                if (col > width)
                    if (row > height)
                        if (IsFailed())
                        {
                            MessageBox.Show("IT IS FAIL!");
                            return;
                        }
                        else if (repeats == settings.MaxRepeats)
                        {
                            MessageBox.Show("Max reapeats come now!");
                            return;
                        }
                        else
                            (col, row, repeats) = (1, 1, repeats + 1);
                    else UpdateByRow(row++);
                else UpdateByColumn(col++);
                Refresh();
            }
            MessageBox.Show("DONE!");
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            using var sw = new StreamWriter("temp.csv");
            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    var str = dgv[j, i].FormattedValue.ToString();
                    if (str.Contains(';')) str = @$"""{str}""";
                    sw.Write(str);
                    if (j < dgv.ColumnCount - 1)
                        sw.Write(";\0");
                }
                sw.WriteLine();
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Apply();
        }

        private void Apply()
        {
            using var sr = new StreamReader("temp.csv");
            for (int i = 0; i <= height + 1; i++)
            {
                var tmp = sr.ReadLine()?.Split(new[] { ";\0" }, StringSplitOptions.None);
                if (tmp == null) return;
                for (int j = 0; j <= width + 1; j++)
                {
                    if (j < tmp.Length)
                        dgv[j, i].Value = tmp[j];
                }
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            settings.RegexOptions = checkBox2.Checked ?
                settings.RegexOptions | RegexOptions.IgnoreCase :
                settings.RegexOptions & ~RegexOptions.IgnoreCase;
        }

        private void BSetRange_Click(object sender, EventArgs e)
        {
            char a = ParseChar(tbStart.Text),
                b = ParseChar(tbEnd.Text, a);
            settings.CharRange = Enumerable.Range(a, b - a + 1).Select(Convert.ToChar).Aggregate("", (a, b) => a + b);
        }

        private char ParseChar(string chr, char start = '\0')
        {
            try
            {
                if (chr == null) throw new Exception("chr is null!");
                if(chr.Length>2&&chr[0]==('\\'))
                    chr = Regex.Unescape(chr);
                if (chr.Length == 1) return chr[0];
                return (char)(int.Parse(chr) + start);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return '\0';
            }
        }

        private void Label2_DoubleClick(object sender, EventArgs e)
        {
            var tmp = textBox1.Text;
            textBox1.Text = textBox2.Text;
            textBox2.Text = tmp;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            ClearGrid();
            Setup();
            AutoFind();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            dgv.AutoSizeColumnsMode = checkBox1.Checked ?
                DataGridViewAutoSizeColumnsMode.ColumnHeader :
                DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void TextBoxEnter(object sender, EventArgs e)
        {
            var tb = (sender as TextBox);
            if (tb.SelectionStart == tb.TextLength)
                tb.SelectAll();
        }

        private bool IsEnded()
        {
            for (int x = 1; x <= width; ++x)
                for (int y = 1; y <= height; ++y)
                    if (dgv[x, y].FormattedValue.ToString().Length != 1)
                        return false;
            return true;
        }

        private bool IsFailed()
        {
            for (int x = 1; x <= width; ++x)
                for (int y = 1; y <= height; ++y)
                    if (!string.IsNullOrEmpty(dgv[x, y].FormattedValue.ToString()))
                        return false;
            return true;
        }

        public string[] GetStringsByRegex(
            int length, string[] sources, Regex[] regexes)
        {
            if (sources == null)
                sources = new string[length];
            else
            if (sources.Length != length)
                throw new Exception("Length != length");
            for (int i = 0; i < length; i++)
                if (string.IsNullOrEmpty(sources[i]))
                    sources[i] = settings.CharRange;

            var results = new List<char>[length];

            for (int i = 0; i < length; i++)
                results[i] = new List<char>();

            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(sources[i][0]);

            while (true)
            {
                var str = sb.ToString();
                if (regexes.All(r => r.IsMatch(str)))
                    for (int i = 0; i < length; i++)
                        results[i].AddDistinct(str[i]);

                bool lastOfRange = true;

                for (int i = 0; lastOfRange && (i < length); ++i)
                {
                    int ind = sources[i].IndexOf(sb[i]) + 1;
                    if (ind < sources[i].Length)
                        lastOfRange = false;
                    else ind = 0;
                    sb[i] = sources[i][ind];
                }

                if (lastOfRange) break;
            }

            var tmp = new string[length];
            for (int i = 0; i < length; i++)
                tmp[i] = string.Concat(results[i]);

            return tmp;
        }

        public string[] SafeGetStringsByRegex(
            int length, string[] sources, Regex[] regexes)
        {
            string[] result = null;
            var container = new { result = null as string[] };
            
            var thread = new Thread(obj =>
            {
                var (length, sources, regexes) =
                    ((int, string[], Regex[]))obj;
                result = GetStringsByRegex(length, sources, regexes);
            });            
            thread.Start((length, sources, regexes));
            for (int i = settings.TimeOut / 100; i > 0; --i)
                if (thread.IsAlive)
                    Thread.Sleep(100);
                else return result;
            thread.Abort();
            return null;
        }
    }
}