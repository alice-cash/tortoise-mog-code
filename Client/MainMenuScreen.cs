using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tortoise.Graphics;
using Tortoise.Graphics.Input;
using Tortoise.Graphics.Rendering;
using Tortoise.Graphics.Rendering.GUI;


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


        public MainMenuScreen(TGraphics graphics)
            : base(graphics)
        {

        }



        public override void Initialize()
        {
            _mainLabel = new Label(_graphics, "MainLabel", Program.GameName, 10, 10, 400, 30);
            _exitButton = new Button(_graphics, "ExitButton", "Exit", 340, 340, 60, 30, FontInfo.GetInstance(_graphics, 22, FontTypes.Sans));

            _exitButton.MouseUp += _exitButton_MouseUp;

            this.Controls.Add(_mainLabel);
            this.Controls.Add(_exitButton);
        }

        void _exitButton_MouseUp(object sender, MouseEventArgs e)
        {
            //TODO: Proper application exit.
            Program.GameLogic.Quit();
        }


        

    }
}
