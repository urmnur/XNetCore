using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XNetCore.STL;

namespace XNetCore.AppTest.Runner
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
            this.Text = ATest.Instance.ABC;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }



        private string CTest()
        {
            var typeName = "XNetCore.AppTest.Runner.BTest,XNetCore.AppTest.Runner";
            var type = Type.GetType(typeName);
            var method = type.GetMethod("getAbc");
            return method.Invoke(Activator.CreateInstance(type, null), null).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(CTest());
        }
    }
}
