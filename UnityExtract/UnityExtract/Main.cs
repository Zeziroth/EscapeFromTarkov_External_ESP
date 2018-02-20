using System;
using System.Windows.Forms;

namespace Swoopie
{
    public partial class Main : Form
    {
        private Overlay overlay;
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case "OPEN":
                    overlay = new Overlay();
                    overlay.Show();
                    button1.Text = "CLOSE";
                    break;

                case "CLOSE":
                    Overlay.ingame = false;
                    overlay.Close();
                    overlay = null;
                    button1.Text = "OPEN";
                    break;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }
}
