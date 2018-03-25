using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UE = UnityEngine;

namespace Swoopie
{
    public static class EFTCore
    {
        public static readonly string procName = "escapefromtarkov";
        public static readonly long GOM = 0x14327E0;

        public static IntPtr gameObjectManager = Base.GetPtr(new IntPtr(Memory.ImageBase().ToInt64() + GOM), new int[] { });

        public static IntPtr gameWorld = new IntPtr(0x0);

        private static readonly int[] fpsCameraStruct = new int[] { 0x30, 0x18 };
        public static IntPtr fpsCamera = new IntPtr(0x0);

        private static readonly int[] localGameWorldStruct = new int[] { 0x30, 0x18, 0x28 };
        public static IntPtr localGameWorld = new IntPtr(0x0);

        private static readonly int[] registeredPlayerStruct = new int[] { 0x60 };
        public static IntPtr registeredPlayers = new IntPtr(0x0);
        private static readonly int[] playerCountStruct = new int[] { 0x18 };

        public static void Init()
        {
            try
            {
                gameObjectManager = Base.GetPtr(new IntPtr(Memory.ImageBase().ToInt64() + GOM), new int[] { });
                GameWorld();
                FTPCamera();
                LocalGameWorld();
                RegisteredPlayers();
            }
            catch { }
        }
        public static IntPtr GameWorld()
        {
            gameWorld = new IntPtr(0x0);
            gameWorld = FindActiveObject("gameworld");
            return gameWorld;
        }
        public static IntPtr FTPCamera()
        {
            fpsCamera = new IntPtr(0x0);
            fpsCamera = FindTaggedObject("fps camera");
            return fpsCamera;
        }
        private static IntPtr FindTaggedObject(string objName)
        {
            int limit = 350;
            StringBuilder objNames = new StringBuilder();
            IntPtr output = new IntPtr(0x0);
            if (!Memory.isRunning())
            {
                return output;
            }
            for (int curObject = 0x1; curObject < limit; curObject++)
            {
                List<int> newStruct = new List<int>() { 0x8 };
                List<int> depth = Enumerable.Repeat(0x8, curObject).ToList();
                newStruct.AddRange(depth);
                newStruct.AddRange(new int[] { 0x10, 0x60, 0x0 });

                long newAddr = Base.GetPtr(gameObjectManager, newStruct.ToArray()).ToInt64();

                string objectName = Memory.ReadOld<string>(newAddr, 10);
                objNames.AppendLine(objectName);
                if (objectName.ToLower() == objName.ToLower())
                {
                    newStruct.RemoveAt(newStruct.Count() - 1);
                    newStruct.RemoveAt(newStruct.Count() - 1);
                    output = Base.GetPtr(gameObjectManager, newStruct.ToArray());
                    return output;
                }
            }
            return output;
        }
        private static IntPtr FindActiveObject(string objName)
        {
            int limit = 350;
            StringBuilder objNames = new StringBuilder();
            IntPtr output = IntPtr.Zero;
            if (!Memory.isRunning())
            {
                return output;
            }
            for (int curObject = 0x1; curObject < limit; curObject++)
            {
                List<int> newStruct = new List<int>() { 0x18 };
                List<int> depth = Enumerable.Repeat(0x8, curObject).ToList();
                newStruct.AddRange(depth);
                newStruct.AddRange(new int[] { 0x10, 0x60, 0x0 });

                long newAddr = Base.GetPtr(gameObjectManager, newStruct.ToArray()).ToInt64();

                string objectName = Memory.ReadOld<string>(newAddr, 9);
                objNames.AppendLine(objectName);
                if (objectName.ToLower() == objName.ToLower())
                {
                    newStruct.RemoveAt(newStruct.Count() - 1);
                    newStruct.RemoveAt(newStruct.Count() - 1);
                    output = Base.GetPtr(gameObjectManager, newStruct.ToArray());
                    return output;
                }
            }
            return output;
        }
        public static IntPtr LocalGameWorld()
        {
            localGameWorld = new IntPtr(0x0);
            localGameWorld = Base.GetPtr(gameWorld, localGameWorldStruct);
            return localGameWorld;
        }

        public static IntPtr RegisteredPlayers()
        {
            registeredPlayers = new IntPtr(0x0);
            registeredPlayers = Base.GetPtr(localGameWorld, registeredPlayerStruct);
            return registeredPlayers;
        }
        public static bool Ingame()
        {
            Init();
            return !(GetPlayerByName(Settings.USERNAME) == null);
        }
        public static int PlayerCount()
        {
            try
            {
                IntPtr countAddr = Base.GetPtr(registeredPlayers, playerCountStruct);
                int playerCount = Memory.Read<int>(countAddr.ToInt64());
                return playerCount;
            }
            catch
            {
                return 0;
            }
        }
        private static List<EFTPlayer> AllPlayers()
        {
            List<EFTPlayer> users = new List<EFTPlayer>();
            int limit = PlayerCount();

            for (int i = 0x0; i < limit; i++)
            {
                IntPtr playerObjAddr = Base.GetPtr(registeredPlayers, new int[] { 0x10, 0x20 + i * 0x8 });
                IntPtr playerNameAddr = Base.GetPtr(playerObjAddr, new int[] { 0x398, 0x28, 0x10, 0x14 });

                EFTPlayer member = new EFTPlayer(playerObjAddr);
                users.Add(member);
            }
            return users;
        }
        public static List<EFTPlayer> Players()
        {
            EFTPlayer localPlayer = GetPlayerByName(Settings.USERNAME);
            List<EFTPlayer> playerList = AllPlayers();
            playerList.ForEach(u =>
            {
                u.distance = GetDistance(localPlayer.GetVector3(), u.GetVector3());
            });
            return playerList;
        }

        public static EFTPlayer GetPlayerByName(string username)
        {
            try
            {
                foreach (EFTPlayer player in AllPlayers())
                {
                    string uname = player.Username();
                }
                return AllPlayers().Where(u => u.Username().ToLower() == username.ToLower()).First();
            }
            catch
            {
                Overlay.ingame = false;
                return null;
            }
        }

        public static float GetDistance(UE.Vector3 v1, UE.Vector3 v2)
        {
            return UE.Vector3.Distance(v1, v2);
        }
    }
}
