using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tortoise.Graphics
{
    public partial class MainForm : Form
    {

        static MainForm()
        {
            AvailableScreens = new Dictionary<string, TDesktop>();
        }

        public static MainForm Instance;

        public static bool ThreadsRunning { get; set; }

        public static Dictionary<string, TDesktop> AvailableScreens { get; private set; }

        public static TDesktop CurrentScreen { get; private set; }

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
                screen.Value.Init();

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
            //The lamba expression keeps the name in scope
            //so no worried there, i hope...
            //The InvokeMethod will run it right away if this is the correct thread.
            System.Action<object> id = (object nothing) =>
            {
                if (!AvailableScreens.ContainsKey(screenName))
                    throw new Exception(string.Format("{0} does not exist!", screenName));
                CurrentScreen.Unload();
                CurrentScreen = AvailableScreens[screenName];

                CurrentScreen.Load();
                if (ScreenChanged != null)
                    ScreenChanged(this, EventArgs.Empty);

            };
            InvokeMethod(id, null);
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
