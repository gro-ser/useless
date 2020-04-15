using System;
using System.Windows.Forms;

internal class TabFormControl : TabControl
{
    private static Form Default() => new Form();
    public static Func<Form> FormCreator { get; set; } = Default;
    public TabFormControl() => MouseDoubleClick += TabFormControl_MouseDoubleClick;

    private static void CopyControls(Control.ControlCollection collection, Control.ControlCollection from)
    {
        for (int i = 0, length = from.Count; i < length; i++)
            collection.Add(from[0]);
    }

    private void TabFormControl_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        TabControl control = sender as TabControl;
        TabPage tab = control.SelectedTab;
        Form form = FormCreator();
        tab.FindForm().AddOwnedForm(form);
        if (tab.BackColor.A == 255)
            form.BackColor = tab.BackColor;
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
        Form form = sender as Form;
        TabPage tab = new TabPage(form.Text)
        {
            Size = form.ClientSize,
            BackColor = form.BackColor
        };
        CopyControls(tab.Controls, form.Controls);
        (form.Tag as TabControl).TabPages.Add(tab);
        tab.Select();
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        ResumeLayout(false);
    }
}