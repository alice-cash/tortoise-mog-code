using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Form =  System.Windows.Forms.Form;
using FormClosingEventArgs = System.Windows.Forms.FormClosingEventArgs;

using Tortoise.Graphics.Rendering;

namespace Tortoise.Graphics
{
    public partial class MainForm : Form
    {

        static MainForm()
        {
            AvailableScreens = new Dictionary<string, Screen>();
        }

        public static MainForm Instance;

        public static bool ThreadsRunning { get; set; }

        public static Dictionary<string, Screen> AvailableScreens { get; private set; }

        public static Screen CurrentScreen { get; private set; }

        //public GUISkin GUISkin { get; protected set; }
        /*   private Invoker _invoker;
           private ThreadSafetyEnforcer _threadSafety;*/

        public event EventHandler ScreenChanged;


        public MainForm(string ScreenTitle)
        {
            Instance = this;
            InitializeComponent();
            this.Text = ScreenTitle;
        }

        public MainForm()
        {
            Instance = this;
            InitializeComponent();
        }

        protected void Initialize()
        {
            /*  _threadSafety = new ThreadSafetyEnforcer("Main Window Class");
              _invoker = new Invoker(_threadSafety); */

            if (AvailableScreens == null)
                throw new Exception("AvailableScreens not set!");

            foreach (var screen in AvailableScreens)
                screen.Value.Initialize();

            if (!AvailableScreens.ContainsKey("MainMenu"))
                throw new Exception("MainMenu not set!");

            this.FormClosing += MainForm_FormClosing;

            CurrentScreen = AvailableScreens["MainMenu"];
            CurrentScreen.Load();
            if (ScreenChanged != null)
                ScreenChanged(this, EventArgs.Empty);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThreadsRunning = false;
        }


        /// <summary>
        /// Change to the screen name listed. The screenName should be an item in the AvailableScreens.
        /// </summary>
        public void ChangeToScreen(string screenName)
        {

            //The InvokeMethod will run it right away if this is the correct thread.
            
            InvokeMethod((object nothing) =>
            {
                if (!AvailableScreens.ContainsKey(screenName))
                    throw new Exception(string.Format("{0} does not exist!", screenName));
                CurrentScreen.Unload();
                CurrentScreen = AvailableScreens[screenName];

                CurrentScreen.Load();
                if (ScreenChanged != null)
                    ScreenChanged(this, EventArgs.Empty);

            },null);
        }

        /// <summary>
        /// Either adds the specified thread to the invoke list, or calls it now if its in the parent thread.
        /// </summary>
        /// <param name="methodToInvoke">A method or delegate to call.</param>
        /// <param name="userData">An object with information sent to the method.</param>
        public void InvokeMethod(System.Action<object> methodToInvoke, object userData)
        {
            /*  _invoker.InvokeMethod(methodToInvoke, userData);*/
            if (InvokeRequired)
            {
                this.Invoke(methodToInvoke, userData);
            }
            else
            {
                methodToInvoke(userData);
            }
        }

        /// <summary>
        /// Returns true if we need to invoke a method.
        /// </summary>
        /*  public bool InvokeRequired()
          {
              return _invoker.InvokeRequired();
          }*/

    }
}
