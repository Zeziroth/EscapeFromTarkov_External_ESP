using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using System.Text;
using UE = UnityEngine;
using System.Collections.Generic;
using Numerics = System.Numerics;

namespace Swoopie
{
    public partial class Overlay : Form
    {
        #region Declaration
        internal struct Margins
        {
            public int Left, Right, Top, Bottom;
        }
        private Margins marg;

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        private static WindowRenderTarget _device;
        private HwndRenderTargetProperties _renderProperties;
        private Factory _factory;
        public static bool ingame = false;
        // Fonts
        private FontFactory _fontFactory = new FontFactory();

        private IntPtr _handle;
        private Thread _threadDx = null;

        private float[] _viewMatrix = new float[16];
        private UE.Vector3 _worldToScreenPos = new UE.Vector3();

        private bool _running = false;

        public static class Colors
        {
            public static RawColor4 WHITE = new RawColor4(Color.White.R, Color.White.G, Color.White.B, Color.White.A);
            public static RawColor4 BLACK = new RawColor4(Color.Black.R, Color.Black.G, Color.Black.B, Color.Black.A);
            public static RawColor4 RED = new RawColor4(Color.Red.R, Color.Red.G, Color.Red.B, Color.Red.A);
            public static RawColor4 GREEN = new RawColor4(Color.Green.R, Color.Green.G, Color.Green.B, Color.Green.A);
            public static RawColor4 BLUE = new RawColor4(Color.Blue.R, Color.Blue.G, Color.Blue.B, Color.Blue.A);
            public static RawColor4 TRANSPARENCY = new RawColor4(Color.Black.R, Color.Black.G, Color.Black.B, 255);
        }
        public class Brushes
        {
            public static SolidColorBrush WHITE = new SolidColorBrush(_device, Colors.WHITE);
            public static SolidColorBrush BLACK = new SolidColorBrush(_device, Colors.BLACK);
            public static SolidColorBrush RED = new SolidColorBrush(_device, Colors.RED);
            public static SolidColorBrush GREEN = new SolidColorBrush(_device, Colors.GREEN);
            public static SolidColorBrush BLUE = new SolidColorBrush(_device, Colors.BLUE);
            public static SolidColorBrush TRANSPARENCY = new SolidColorBrush(_device, Colors.TRANSPARENCY);
        }
        #endregion
        #region Start
        public Overlay()
        {
            _handle = Handle;
            InitializeComponent();
        }
        Factory factory = new Factory();
        private void LoadOverlay(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            _factory = new Factory();
            _renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new SharpDX.Size2(Size.Width, Size.Height),
                PresentOptions = PresentOptions.None
            };

            marg.Left = 0;
            marg.Top = 0;
            marg.Right = this.Width;
            marg.Bottom = this.Height;

            DwmExtendFrameIntoClientArea(this.Handle, ref marg);
            // Initialize DirectX
            _device = new WindowRenderTarget(_factory, new RenderTargetProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), _renderProperties);

            _threadDx = new Thread(new ParameterizedThreadStart(DirectXThread))
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };

            _running = true;
            this.TopMost = true;
            _threadDx.Start();

        }
        #endregion

        private void DirectXThread(object sender)
        {
            float npcLimit = 200f;
            float playerLimit = 750f;
            float playerLimitBox = 200f;
            bool newRound = false;
            EFTCore.Init();
            while (_running)
            {
                try
                {
                    Invoker.BringToFront(this);

                    int i = EFTCore.PlayerCount();
                    string hex = EFTCore.gameWorld.ToString("X");

                    while (!Memory.isRunning() || EFTCore.PlayerCount() == 0)
                    {
                        EFTCore.Init();
                    }

                    _device.BeginDraw();
                    _device.Clear(SharpDX.Color.Transparent);
                    _device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;
                    StringBuilder strBuild = new StringBuilder();
                    List<EFTPlayer> players = EFTCore.Players();
                    players.Sort((x, y) => x.distance.CompareTo(y.distance));

                    foreach (EFTPlayer player in players)
                    {
                        if (player.isPlayer())
                        {
                            strBuild.AppendLine(player.Username() + " " + Math.Round(player.distance, 0) + "m");
                        }

                        if (player.isPlayer() && player.distance <= playerLimit || !player.isPlayer() && player.distance <= npcLimit)
                        {
                            UE.Vector3 coords;
                            WorldToScreen(player.GetVector3(), out coords);
                            if (coords.x > 0 || coords.y > 0 || coords.z > 0)
                            {

                                if (player.isPlayer())
                                {
                                    if (player.distance <= playerLimitBox)
                                    {
                                        _device.DrawRectangle(new RawRectangleF(coords.x - 15, coords.y + 25, coords.x, coords.y - 25), Brushes.BLACK);
                                    }
                                    if (player.distance <= playerLimit)
                                    {
                                        WriteText(player.Username() + Environment.NewLine + Math.Round(player.distance, 0) + "m", coords.x + 5, coords.y - 25, Brushes.WHITE);
                                    }
                                }
                                else
                                {
                                    WriteText("BOT", coords.x + 5, coords.y - 25, Brushes.RED);
                                    WriteText(Math.Round(player.distance, 0) + "m", coords.x + 5, coords.y - 15, Brushes.WHITE);
                                }
                            }
                        }
                    }

                    //WriteCenterText("Gameworld: " + EFTCore.gameWorld.ToString("X"), this.Height / 2, Brushes.WHITE);
                    //WriteCenterText("FPS Camera: " + EFTCore.fpsCamera.ToString("X"), (this.Height / 2) + 50, Brushes.WHITE);
                    WriteBottomText(strBuild.ToString(), 50, Brushes.WHITE, 16);
                    _device.Flush();
                    _device.EndDraw();
                }
                catch
                {
                    try
                    {
                        _device.Flush();
                        _device.EndDraw();
                    }
                    catch { }
                    continue;
                }
            }
        }
        #region DrawFunctions
        private SolidColorBrush CreateBrush(RawColor4 color)
        {
            return new SolidColorBrush(_device, color);
        }
        private void DrawLine(int x, int y, int xTo, int yTo, SolidColorBrush color)
        {
            _device.DrawLine(new RawVector2(x, y), new RawVector2(xTo, yTo), color);

        }
        private void WriteText(string msg, float x, float y, SolidColorBrush color, float fontSize = 13, string fontFamily = "Arial Unicode MS")
        {
            Size measure = PredictSize(msg, fontSize, fontFamily);
            _device.DrawText(msg, new TextFormat(_fontFactory, fontFamily, fontSize), new RawRectangleF(x, y, x + measure.Width, y + measure.Height), color);
        }
        private void WriteTextExact(string msg, float x, float y, SolidColorBrush color, float fontSize = 13, string fontFamily = "Arial Unicode MS")
        {
            Size measure = PredictSize(msg, fontSize, fontFamily);
            _device.DrawText(msg, new TextFormat(_fontFactory, fontFamily, fontSize), new RawRectangleF(x, y, x + measure.Width, y + measure.Height), color);
        }
        private void WriteCenterText(string msg, float y, SolidColorBrush color, float fontSize = 13, string fontFamily = "Arial Unicode MS")
        {
            Size measure = PredictSize(msg, fontSize, fontFamily);
            int x = this.Width / 2 - measure.Width / 2;
            WriteText(msg, x, y, color, fontSize, fontFamily);
        }
        private void WriteBottomText(string msg, float x, SolidColorBrush color, float fontSize = 13, string fontFamily = "Arial Unicode MS")
        {
            Size measure = PredictSize(msg, fontSize, fontFamily);
            int y = this.Height - measure.Height;
            WriteTextExact(msg, x, y, color, fontSize, fontFamily);
        }
        private Size PredictSize(string msg, float fontSize = 13, string fontFamily = "Arial Unicode MS")
        {
            return System.Windows.Forms.TextRenderer.MeasureText(msg, new System.Drawing.Font(fontFamily, fontSize - 3));
        }
        #endregion
        #region Quit
        private void ClosedOverlay(object sender, FormClosingEventArgs e)
        {
            try
            {
                _running = false;
                _device.Flush();
                _device.EndDraw();
                _device.Dispose();
                _device = null;
            }
            catch { }
        }
        #endregion
        #region Functions
        private bool WorldToScreen(UE.Vector3 _Enemy, out UE.Vector3 _Screen)
        {
            _Screen = new UE.Vector3(0, 0, 0);
            Numerics.Matrix4x4 temp = Numerics.Matrix4x4.Transpose(Memory.Read<Numerics.Matrix4x4>(Base.GetPtr(EFTCore.fpsCamera, new int[] { 0x30, 0x18, 0xC0 }).ToInt64()));

            UE.Vector3 translationVector = new UE.Vector3(temp.M41, temp.M42, temp.M43);
            UE.Vector3 up = new UE.Vector3(temp.M21, temp.M22, temp.M23);
            UE.Vector3 right = new UE.Vector3(temp.M11, temp.M12, temp.M13);

            float w = D3DXVec3Dot(translationVector, _Enemy) + temp.M44;

            if (w < 0.098f)
                return false;

            float y = D3DXVec3Dot(up, _Enemy) + temp.M24;
            float x = D3DXVec3Dot(right, _Enemy) + temp.M14;

            _Screen.x = (this.Width / 2) * (1f + x / w);
            _Screen.y = (this.Height / 2) * (1f - y / w);
            _Screen.z = w;

            return true;

        }

        private float D3DXVec3Dot(UE.Vector3 a, UE.Vector3 b)
        {
            return (a.x * b.x +
                    a.y * b.y +
                    a.z * b.z);
        }
        bool WorldToScreen(UE.Vector3 from, UE.Vector3 to)
        {
            float w = 0.0f;

            to.x = _viewMatrix[0] * from.x + _viewMatrix[1] * from.y + _viewMatrix[2] * from.z + _viewMatrix[3];
            to.y = _viewMatrix[4] * from.x + _viewMatrix[5] * from.y + _viewMatrix[6] * from.z + _viewMatrix[7];

            w = _viewMatrix[12] * from.x + _viewMatrix[13] * from.y + _viewMatrix[14] * from.z + _viewMatrix[15];

            if (w < 0.01f)
                return false;

            to.x *= (1.0f / w);
            to.y *= (1.0f / w);

            int width = Size.Width;
            int height = Size.Height;

            float x = width / 2;
            float y = height / 2;

            x += 0.5f * to.x * width + 0.5f;
            y -= 0.5f * to.y * height + 0.5f;

            to.x = x;
            to.y = y;

            _worldToScreenPos.x = to.x;
            _worldToScreenPos.y = to.y;

            return true;
        }
        #endregion
    }
}