//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using GorgonLibrary.Graphics;

//namespace Tortoise.Client.Screen
//{
//    public abstract class TDesktop : IDisposable
//    {
//        public string Name { get; set; }
//        public bool Loaded { get; protected set; }
//        protected bool _inited;

//        /// <summary>
//        /// Initilise the Item, set values, etc
//        /// </summary>
//        public abstract void Init();
//        /// <summary>
//        /// Load any textures, prepare for rendering
//        /// </summary>
//        public abstract void Load();
//        /// <summary>
//        /// Unload any textures, prepare for idle
//        /// </summary>
//        public virtual void Unload()
//        {
//            Loaded = false;
//        }

//        public virtual void Dispose()
//        {
//            if (Loaded)
//                Unload();
//        }
//        /// <summary>
//        /// Ran before Render, can use to check and prepare before each rendering
//        /// </summary>
//        public abstract void Tick(FrameEventArgs e);

//        /// <summary>
//        /// Renders the object to the screen.
//        /// </summary>
//        public abstract void Render(FrameEventArgs e);
//    }
//}
