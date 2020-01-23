using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class TabFormControl:TabControl
{
    static Form Default() => new Form();
    public static Func<Form> FormCreator { get; set; } = Default;
    public TabFormControl()
    {
        MouseDoubleClick += TabFormControl_MouseDoubleClick;
    }

    static void CopyControls(Control.ControlCollection collection, Control.ControlCollection from)
    {
        for (int i = 0, length = from.Count; i < length; i++)
            collection.Add(from[0]);
    }

    private void TabFormControl_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        var control = sender as TabControl;
        var tab = control.SelectedTab;
        var form = FormCreator();
        tab.FindForm().AddOwnedForm(form);
        if (tab.BackColor.A == 255) form.BackColor = tab.BackColor;
        form.ClientSize = tab.Size;
        form.Text = tab.Text;
        form.Tag = control;
        CopyControls(form.Controls, tab.Controls);
        control.TabPages.RemoveAt(control.SelectedIndex);
        form.FormClosing += Form_FormClosing;
        form.Show();
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        var form = sender as Form;
        var tab = new TabPage(form.Text);
        tab.Size = form.ClientSize;
        tab.BackColor = form.BackColor;
        CopyControls(tab.Controls, form.Controls);
        (form.Tag as TabControl).TabPages.Add(tab);
        tab.Select();
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            this.ResumeLayout(false);
    }
}