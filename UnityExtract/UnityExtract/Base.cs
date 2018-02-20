using System;

namespace Swoopie
{
    public static class Base
    {
        public static IntPtr baseAddress = IntPtr.Zero;

        public static IntPtr GetPtr(IntPtr pointer, int[] offsets, bool debug = false)
        {
            try
            {
                IntPtr pointedto = pointer;
                foreach (int offset in offsets)
                {
                    IntPtr tmpPointed = (IntPtr)(Memory.Read<long>((long)pointedto));
                    pointedto = IntPtr.Add(tmpPointed, offset);
                    if (debug)
                    {
                        System.Windows.Forms.MessageBox.Show("0x" + tmpPointed.ToString("X") + "+ 0x" + offset.ToString("X") + " => 0x" + (Memory.Read<long>((long)pointedto)).ToString("X"));
                    }
                }

                return pointedto;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return IntPtr.Zero;
            }
        }
    }
}
