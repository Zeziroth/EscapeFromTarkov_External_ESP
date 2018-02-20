using System.Drawing;
using System.Windows.Forms;

namespace Swoopie
{
    static class Invoker
    {

        public enum Mode
        {
            SetMaximum = 0,
            SetValue = 1

        }

        //Forms
        private delegate void _ChangeVisibilityForm(Form frm, bool state);
        private delegate void _BringToFront(Form frm);
        private delegate void _CloseForm(Form frm);
        public static void CloseForm(Form frm)
        {
            try
            {
                if (frm.InvokeRequired)
                {
                    frm.Invoke(new _CloseForm(CloseForm), frm);
                }
                else
                {
                    frm.Close();
                }
            }
            catch (System.Exception)
            {

            }
        }
        public static void BringToFront(Form frm)
        {
            try
            {
                if (frm.InvokeRequired)
                {
                    frm.Invoke(new _BringToFront(BringToFront), frm);
                }
                else
                {
                    frm.BringToFront();
                }
            }
            catch (System.Exception)
            {

            }
        }

        public static void ChangeVisibilityForm(Form frm, bool state)
        {
            try
            {
                if (frm.InvokeRequired)
                {
                    frm.Invoke(new _ChangeVisibilityForm(ChangeVisibilityForm), frm, state);
                }
                else
                {
                    if (state)
                    {
                        frm.Show();
                    }
                    else
                    {
                        frm.Hide();
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }
    }
}
