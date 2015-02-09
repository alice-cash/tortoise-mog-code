using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedUnitTests.Threading
{
    [TestClass]
    public class ThreadingTest
    {

        #region Enforcing Safety
        #region "ThreadSafetyEnforcer.EnforcingThreadSafety"
        [TestMethod]
        public void ThreadSafetyEnforcer_EnforcingThreadSafety__ThreadSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsTrue(ThreadSafetyEnforcerObject.EnforcingThreadSafety, "ThreadSafetyEnforcer.EnforcingThreadSafety");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;


        }

        [TestMethod]
        public void ThreadSafetyEnforcer_EnforcingThreadSafety__ParentSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);

            Assert.IsTrue(ThreadSafetyEnforcerObject.EnforcingThreadSafety, "ThreadSafetyEnforcer.EnforcingThreadSafety");

        }
        #endregion

        #region "ThreadSafetyEnforcer.CheckThreadSafety()"
        [TestMethod]
        public void ThreadSafetyEnforcer_CheckThreadSafety__ThreadSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsFalse(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;
        }

        [TestMethod]
        public void ThreadSafetyEnforcer_CheckThreadSafety__ParentSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);

            Assert.IsTrue(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");

        }
        #endregion

        #region "ThreadSafetyEnforcer.IsSameThread()"
        [TestMethod]
        public void ThreadSafetyEnforcer_IsSameThread__ThreadSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsFalse(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;
        }

        [TestMethod]
        public void ThreadSafetyEnforcer_IsSameThread__ParentSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);

            Assert.IsTrue(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");

        }
        #endregion

        #region "ThreadSafetyEnforcer.IsSameThread()"
        [TestMethod]
        public void ThreadSafetyEnforcer_EnforceThreadSafety__ThreadSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    ThreadSafetyEnforcerObject.EnforceThreadSafety();
                    Assert.Fail("ThreadSafetyEnforcer.EnforceThreadSafety()");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
                catch (InvalidOperationException)
                {
                    //Success
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;
        }

        [TestMethod]
        public void ThreadSafetyEnforcer_EnforceThreadSafety__ParentSide_EnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", true);

            try
            {
                ThreadSafetyEnforcerObject.EnforceThreadSafety();
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("ThreadSafetyEnforcer.EnforceThreadSafety()");
            }

        }
        #endregion
        #endregion 


        #region Not Enforcing Safety
        #region "ThreadSafetyEnforcer.EnforcingThreadSafety"
        [TestMethod]
        
        public void ThreadSafetyEnforcer_EnforcingThreadSafety__ThreadSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsFalse(ThreadSafetyEnforcerObject.EnforcingThreadSafety, "ThreadSafetyEnforcer.EnforcingThreadSafety");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;


        }

        [TestMethod]
        public void ThreadSafetyEnforcer_EnforcingThreadSafety__ParentSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);

            Assert.IsFalse(ThreadSafetyEnforcerObject.EnforcingThreadSafety, "ThreadSafetyEnforcer.EnforcingThreadSafety");

        }
        #endregion
        
        #region "ThreadSafetyEnforcer.IsSameThread()"
        [TestMethod]
        public void ThreadSafetyEnforcer_IsSameThread__ThreadSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    Assert.IsFalse(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;
        }

        [TestMethod]
        public void ThreadSafetyEnforcer_IsSameThread__ParentSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);

            Assert.IsTrue(ThreadSafetyEnforcerObject.IsSameThread(), "ThreadSafetyEnforcer.CheckThreadSafety()");

        }
        #endregion

        #region "ThreadSafetyEnforcer.IsSameThread()"
        [TestMethod]
        public void ThreadSafetyEnforcer_EnforceThreadSafety__ThreadSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;
            System.Threading.Thread testThread;

            AssertFailedException threadException = null;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);
            testThread = new System.Threading.Thread(() =>
            {
                try
                {
                    try
                    {
                        ThreadSafetyEnforcerObject.EnforceThreadSafety();
                    }
                    catch
                    {
                        Assert.Fail("ThreadSafetyEnforcerObject.EnforceThreadSafety()");
                    }
                }
                catch (AssertFailedException ex)
                {
                    threadException = ex;
                }
            });
            testThread.Start();

            while (testThread.IsAlive)
                System.Threading.Thread.Yield();

            if (threadException != null)
                throw threadException;
        }

        [TestMethod]
        public void ThreadSafetyEnforcer_EnforceThreadSafety__ParentSide_NonEnforcedTest()
        {
            Tortoise.Shared.Threading.ThreadSafetyEnforcer ThreadSafetyEnforcerObject;

            ThreadSafetyEnforcerObject = new Tortoise.Shared.Threading.ThreadSafetyEnforcer("Unit Test Object", false);

            try
            {
                ThreadSafetyEnforcerObject.EnforceThreadSafety();
            }
            catch (InvalidOperationException)
            {
                Assert.Fail("ThreadSafetyEnforcer.EnforceThreadSafety()");
            }

        }
        #endregion
        #endregion 
    }
}
