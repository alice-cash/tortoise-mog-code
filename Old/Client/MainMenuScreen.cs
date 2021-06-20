using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tortoise.Graphics;
using Tortoise.Graphics.Input;
using Tortoise.Graphics.Rendering;
using Tortoise.Graphics.Rendering.GUI;
using StormLib;

namespace Tortoise.Client
{
    public class MainMenuScreen : Screen
    {



        //    class LoginStatusScreen : Screen
        //    {
        //        public static LoginStatusScreen Instance;
        //        public string Text
        //        {
        //            get { return (Controls["_contents"] as Label).Text; }
        //            set
        //            {
        //                _threadSafety.EnforceThreadSafety();
        //                (Controls["_contents"] as Label).Text = value;
        //            }
        //        }
        //        public override void Init()
        //        {
        //            BackgroundColor = Color.Wheat;
        //            Label cointents = new Label("_contents", "", new Point(0, 0), new Size(Window.ScreenWidth, Window.ScreenHeight), FontInfo.GetInstance(10, FontTypes.Sans));
        //            cointents.TextAlignement = TextAlignement.Center;
        //            Controls.Add(10, cointents);

        //        }

        //        public override void OnResize()
        //        {
        //            Size = Window.Instance.Size;
        //        }

        //        public override void Load()
        //        {
        //            base.Load();
        //            //This is called right before the window becomes the focused screen.
        //            //Any code to run should go here.
        //        }

        //        public override void Unload()
        //        {
        //            base.Unload();
        //            //This is called when the window is no longer the focused screen.
        //            //Any code to run should go here.
        //        }



        //    }


        Label _mainLabel;
        Button _exitButton;
        Label _testLabel;
        TextBox _testTextBox;

        public MainMenuScreen(TGraphics graphics)
            : base(graphics)
        {

        }



        public override void Initialize()
        {
            _mainLabel = new Label(_graphics, "MainLabel", Program.GameName, 10, 10, 400, 30, FontManager.GetInstance(_graphics,34, FontTypes.Sans));
            _exitButton = new Button(_graphics, "ExitButton", "Exit", 340, 340, 60, 30, FontManager.GetInstance(_graphics, 22, FontTypes.Sans));
            _testLabel = new Label(_graphics, "Test", "TEST", 0, 0, 30, 30);
            _testTextBox = new TextBox(_graphics, "TestBox", 50, 50, 400, 29)
            {
                BackgroundColor = Color.Green
            };
            _exitButton.MouseUp += _exitButton_MouseUp;

            _exitButton.Anchor = ControlAnchor.Right | ControlAnchor.Bottom;
            _testLabel.Anchor = ControlAnchor.Right;


            this.Controls.Add(_mainLabel);
            this.Controls.Add(_exitButton);
            this.Controls.Add(_testLabel);
            this.Controls.Add(_testTextBox);

            t.Start();
        }

        private Timer t = new Timer();
        private bool isdown;

        public static Action<string, string> LoginRequest { get; internal set; }

        public override void Tick(TickEventArgs e) {
            base.Tick(e);
            if (t.ElapsedMilliseconds > 100)
            {
                t.Restart();

                if (isdown)
                    _testLabel.Y -= 1;
                else
                    _testLabel.Y += 1;
                if(_testLabel.Y < 1)
                {
                    isdown = true;
                } 

                if(_testLabel.Bottom + 1 > this.Bottom)
                {
                    isdown = false;
                }
                _testLabel.Text = _testLabel.Y.ToString();
            }
        }

        void _exitButton_MouseUp(object sender, MouseEventArgs e)
        {
            //TODO: Proper application exit.
            Program.Exit();
        }


        

    }
}
