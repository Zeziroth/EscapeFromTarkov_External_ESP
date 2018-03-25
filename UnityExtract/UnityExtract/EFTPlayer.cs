using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE = UnityEngine;
namespace Swoopie
{
    public class EFTPlayer
    {
        public ReflectStructure structs = null;
        private IntPtr _baseAddr = IntPtr.Zero;
        public float distance = 0f;
        public EFTPlayer(IntPtr addr)
        {
            _baseAddr = addr;

            structs = new ReflectStructure(_baseAddr, new Dictionary<string, Dictionary<int, int[]>>() {
            { "IS_PLAYER", new Dictionary<int, int[]>(){ { 32, new int[] { 0x398, 0x20 } } } },

            { "USERNAME", new Dictionary<int, int[]>(){ { 32, new int[] { 0x398, 0x28, 0x10, 0x14 } } } },
            { "WATER_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x10, 0x20 } } } },
            { "WATER_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x10, 0x24 } } } },

            { "ENERGY_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x18, 0x20 } } } },
            { "ENERGY_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x18, 0x24 } } } },

            { "HEAD_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x20, 0x20 } } } },
            { "HEAD_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x20, 0x24 } } } },

            { "CHEST_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x28, 0x20 } } } },
            { "CHEST_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x28, 0x24 } } } },

            { "STOMACH_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x30, 0x20 } } } },
            { "STOMACH_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x30, 0x24 } } } },

            { "LEFTARM_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x38, 0x20 } } } },
            { "LEFTARM_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x38, 0x24 } } } },

            { "RIGHTARM_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x40, 0x20 } } } },
            { "RIGHTARM_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x40, 0x24 } } } },

            { "LEFTLEG_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x48, 0x20 } } } },
            { "LEFTLEG_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x48, 0x24 } } } },

            { "RIGHTLEG_CUR", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x50, 0x20 } } } },
            { "RIGHTLEG_MAX", new Dictionary<int, int[]>(){ { 4, new int[] { 0x398, 0x40, 0x20, 0x10, 0x50, 0x24 } } } },

            { "POS_X", new Dictionary<int, int[]>(){ { 4, new int[] { 0x58, 0x30 } } } },
            { "POS_Y", new Dictionary<int, int[]>(){ { 4, new int[] { 0x58, 0x34 } } } },
            { "POS_Z", new Dictionary<int, int[]>(){ { 4, new int[] { 0x58, 0x38 } } } },

            { "VIEW_X", new Dictionary<int, int[]>(){ { 4, new int[] { 0x50, 0x1EC } } } },
            { "VIEW_Y", new Dictionary<int, int[]>(){ { 4, new int[] { 0x50, 0x1F0 } } } },
            { "VIEW_Z", new Dictionary<int, int[]>(){ { 4, new int[] { 0x50, 0x1F4 } } } }
            });
        }
        public string Username()
        {
            IntPtr playerNameAddr = Base.GetPtr(_baseAddr, new int[] { 0x398, 0x28, 0x10, 0x14 });
            byte[] usernameBytes = Memory.ReadBytes(playerNameAddr.ToInt64(), 32);
            List<byte> clearBytes = new List<byte>();

            for (int i = 0; i < usernameBytes.Count(); i++)
            {
                if (usernameBytes[i] == 0x0 && usernameBytes[i + 1] == 0x0)
                {
                    break;
                }
                clearBytes.Add(usernameBytes[i]);
            }
            if (clearBytes.Count() % 2 != 0) {
                clearBytes.Add(0x00);
            }
            return Encoding.Unicode.GetString(clearBytes.ToArray());
        }
        public UE.Vector3 GetVector3()
        {
            return new UE.Vector3(X(), Y(), Z());
        }
        public bool isPlayer()
        {
            int player = structs.GetValue<int>("IS_PLAYER");
            string uname = Username();
            return !(player == 0 || player == 98021344 || player == 36810720);
        }

        public float X()
        {
            return structs.GetValue<float>("POS_X");
        }
        public float Y()
        {
            return structs.GetValue<float>("POS_Y");
        }
        public float Z()
        {
            return structs.GetValue<float>("POS_Z");
        }
        public float GetCurHealth()
        {
            float hp = 0f;
            hp += structs.GetValue<float>("HEAD_CUR");
            hp += structs.GetValue<float>("CHEST_CUR");
            hp += structs.GetValue<float>("STOMACH_CUR");
            hp += structs.GetValue<float>("LEFTARM_CUR");
            hp += structs.GetValue<float>("RIGHTARM_CUR");
            hp += structs.GetValue<float>("LEFTLEG_CUR");
            hp += structs.GetValue<float>("RIGHTLEG_CUR");
            return hp;
        }
        public float GetMaxHealth()
        {
            float hp = 0f;
            hp += structs.GetValue<float>("HEAD_MAX");
            hp += structs.GetValue<float>("CHEST_MAX");
            hp += structs.GetValue<float>("STOMACH_MAX");
            hp += structs.GetValue<float>("LEFTARM_MAX");
            hp += structs.GetValue<float>("RIGHTARM_MAX");
            hp += structs.GetValue<float>("LEFTLEG_MAX");
            hp += structs.GetValue<float>("RIGHTLEG_MAX");
            return hp;
        }
        public void FillAll()
        {
            structs.SetValue("WATER_CUR", structs.GetValue<float>("WATER_MAX"));
            structs.SetValue("ENERGY_CUR", structs.GetValue<float>("ENERGY_MAX"));
            structs.SetValue("HEAD_CUR", structs.GetValue<float>("HEAD_MAX"));
            structs.SetValue("CHEST_CUR", structs.GetValue<float>("CHEST_MAX"));
            structs.SetValue("STOMACH_CUR", structs.GetValue<float>("STOMACH_MAX"));
            structs.SetValue("LEFTARM_CUR", structs.GetValue<float>("LEFTARM_MAX"));
            structs.SetValue("RIGHTARM_CUR", structs.GetValue<float>("RIGHTARM_MAX"));
            structs.SetValue("LEFTLEG_CUR", structs.GetValue<float>("LEFTLEG_MAX"));
            structs.SetValue("RIGHTLEG_CUR", structs.GetValue<float>("RIGHTLEG_MAX"));
        }
    }
}
