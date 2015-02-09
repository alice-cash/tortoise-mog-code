using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedUnitTests.Threading
{
    [TestClass]
    public class InvokeTest
    {

        #region "Invoker.InvokeRequired()"
        [TestMethod]
        public void Invoker_InvokeRequired__ThreadSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            System.Threading.Thread testThread;

            UnitTestAssertException exception = null;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsTrue(InvokerObject.InvokeRequired(), "Invoker.InvokeRequired()");
                }
                catch (UnitTestAssertException e)
                {
                    exception = e;
                }
            });

            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();


            if (exception != null) throw exception;
        }

        [TestMethod]
        public void Invoker_InvokeRequired__ParentSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");
            Assert.IsFalse(InvokerObject.InvokeRequired(), "Invoker.InvokeRequired()");
        }
        #endregion

        #region "Invoker.PollInvokes()"
        [TestMethod]
        public void Invoker_PollInvokes__ThreadSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            System.Threading.Thread testThread;

            UnitTestAssertException exception = null;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    try
                    {
                        InvokerObject.PollInvokes();
                        Assert.Fail("Invoker.PollInvokes()");
                    }
                    catch (InvalidOperationException)
                    {
                        //success
                    }
                }
                catch (UnitTestAssertException e)
                {
                    exception = e;
                }
            });

            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();


            if (exception != null) throw exception;
        }

        [TestMethod]
        public void Invoker_PollInvokes__ParentSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");
            try
            {
                InvokerObject.PollInvokes();
                //Success
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("Invoker.PollInvokes()");
            }
        }
        #endregion

        #region "Invoker.InvokeCount()"
        [TestMethod]
        public void Invoker_InvokeCount__ThreadSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            System.Threading.Thread testThread;

            UnitTestAssertException exception = null;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    try
                    {
                        InvokerObject.InvokeCount();
                        Assert.Fail("Invoker.InvokeCount()");
                    }
                    catch (InvalidOperationException)
                    {
                        //success
                    }
                }
                catch (UnitTestAssertException e)
                {
                    exception = e;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();


            if (exception != null) throw exception;
        }

        [TestMethod]
        public void Invoker_InvokeCount__ParentSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");
            try
            {
                InvokerObject.InvokeCount();
                //Success
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("Invoker.InvokeCount()");
            }
        }
        #endregion


        #region "Invoker.InvokeMethod()"
        [TestMethod]
        public void Invoker_InvokeMethod__ThreadSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            System.Threading.Thread testThread;

            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            bool testSet = false;

            testThread = new System.Threading.Thread(() =>
            {
                InvokerObject.InvokeMethod((object nothing) =>
                {
                    testSet = true;
                }, null);
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            Assert.IsFalse(testSet);

            InvokerObject.PollInvokes();

            Assert.IsTrue(testSet);

        }

        [TestMethod]
        public void Invoker_InvokeMethod__ParentSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            bool testSet = false;

            InvokerObject.InvokeMethod((object nothing) =>
            {
                testSet = true;
            }, null);

            Assert.IsFalse(testSet);

            InvokerObject.PollInvokes();

            Assert.IsTrue(testSet);

        }
        #endregion

        #region "Invoker.SynchronousInvokeMethod()"
        [TestMethod]
        public void Invoker_SynchronousInvokeMethod__ThreadSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            System.Threading.Thread testThread;

            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            bool testSet = false;
            bool threadDone = false;

            bool Waiting = false;

            testThread = new System.Threading.Thread(() =>
            {
                //We need a way for this to pause until we are listening for the invoke.
                while (!Waiting)
                    System.Threading.Thread.Yield();

                InvokerObject.SynchronousInvokeMethod((object nothing) =>
                {
                    testSet = true;
                }, null);
                threadDone = true;
            });
            testThread.Start();

            DateTime Timeout = DateTime.Now.AddSeconds(1);

            while (InvokerObject.InvokeCount() == 0)
            {
                if (!Waiting) Waiting = true;
                if (Timeout <= DateTime.Now)
                    Assert.Fail("Invoke Timeout! No invokable items arrived! If workstation is overloaded try again.");
            }


            //sleep and force previous thread to run... Hopefully
            System.Threading.Thread.Yield();
            System.Threading.Thread.Sleep(10);
            System.Threading.Thread.Yield();

            Assert.IsFalse(testSet);
            Assert.IsFalse(threadDone);

            InvokerObject.PollInvokes();

            Assert.IsTrue(testSet);

            Timeout = DateTime.Now.AddSeconds(1);

            while (testThread.IsAlive)
            {
                System.Threading.Thread.Yield();
                if (Timeout <= DateTime.Now)
                    Assert.Fail("Thread Timeout! Thread has not finished resuming! If workstation is overloaded try again.");
            }

        }

        [TestMethod, Timeout(1000)]
        public void Invoker_SynchronousInvokeMethod__ParentSideTest()
        {
            Tortoise.Shared.Threading.Invoker InvokerObject;
            InvokerObject = new Tortoise.Shared.Threading.Invoker("InvokerTest Object");

            bool testSet = false;

            InvokerObject.SynchronousInvokeMethod((object nothing) =>
            {
                testSet = true;
            }, null);

            Assert.IsTrue(testSet);

            //In reality if this were to fail this would never run. The above timeout will catch this.
            Assert.AreEqual(InvokerObject.InvokeCount(), 0);
        }
        #endregion

    }
}
