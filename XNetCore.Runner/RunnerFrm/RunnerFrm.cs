using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XNetCore.Runner
{
    partial class RunnerFrm : Form
    {
        Form runnerfrm;
        public RunnerFrm(Form frm)
        {
            InitializeComponent();
            this.runnerfrm = frm;
            frm.TextChanged += Frm_TextChanged;
            this.SizeChanged += RunnerFrm_SizeChanged;
            this.Shown += RunnerFrm_Shown;
        }

        private void RunnerFrm_SizeChanged(object sender, EventArgs e)
        {
            var frm = this.runnerfrm;
            frm.Size = this.panel1.Size;
        }

        private void Frm_TextChanged(object sender, EventArgs e)
        {
            var frm = sender as Form;
            if (frm==null)
            {
                return;
            }
            this.Text = frm.Text;
        }

        private void RunnerFrm_Shown(object sender, EventArgs e)
        {
            this.Icon = XNetCore.Runner.RunHelper.Instance.AppIcon;
            XNetCore.Runner.RunHelper.Instance.IconChanged += (icon) => { this.Icon = icon; };
            var frm = this.runnerfrm;
            frm.TopLevel = false;
            frm.Parent = this.panel1;
            this.panel1.Controls.Add(frm);
            frm.Location =this.panel1.Location;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Show();
            frm.Size = this.panel1.Size;
        }
    }
}
