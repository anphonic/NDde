namespace NDde.Test
{
    using System;
    using System.Collections.Generic;
    using NDde;
    using NDde.Advanced;
    using NDde.Client;
    using NDde.Server;
    using NUnit.Framework;

    [TestFixture]
    public sealed class Test_DdeContext
    {
        private const string ServiceName = "test";
        private const int Timeout = 1000;

        [Test]
        public void Test_Ctor_Overload_1()
        {
            DdeContext context = new DdeContext();
        }

        [Test]
        public void Test_Ctor_Overload_2()
        {
            DdeContext context = new DdeContext(new DdeContext());
        }

        [Test]
        public void Test_Dispose()
        {
            using (DdeContext context = new DdeContext())
            {
            }
        }

        [Test]
        public void Test_Initialize()
        {
            using (DdeContext context = new DdeContext())
            {
                context.Initialize();
            }
        }

        [Test]
        public void Test_Initialize_After_Dispose()
        {
            using (DdeContext context = new DdeContext())
            {
                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Initialize());
            }
        }

        [Test]
        public void Test_Initialize_After_Initialize()
        {
            using (DdeContext context = new DdeContext())
            {
                context.Initialize();
                Assert.Throws<InvalidOperationException>(() => context.Initialize());
            }
        }

        [Test]
        public void Test_IsInitialized_Variation_1()
        {
            using (DdeContext context = new DdeContext())
            {
                Assert.That(context.IsInitialized, Is.False);
            }
        }

        [Test]
        public void Test_IsInitialized_Variation_2()
        {
            using (DdeContext context = new DdeContext())
            {
                context.Initialize();
                Assert.That(context.IsInitialized, Is.True);
            }
        }

        [Test]
        public void Test_AddTransactionFilter()
        {
            using (DdeContext context = new DdeContext())
            {
                IDdeTransactionFilter filter = new TransactionFilter();
                context.AddTransactionFilter(filter);
            }
        }

        [Test]
        public void Test_AddTransactionFilter_After_Dispose()
        {
            using (DdeContext context = new DdeContext())
            {
                IDdeTransactionFilter filter = new TransactionFilter();
                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.AddTransactionFilter(filter));
            }
        }

        [Test]
        public void Test_RemoveTransactionFilter()
        {
            using (DdeContext context = new DdeContext())
            {
                TransactionFilter filter = new TransactionFilter();
                context.AddTransactionFilter(filter);
                context.RemoveTransactionFilter(filter);
            }
        }

        [Test]
        public void Test_RemoveTransactionFilter_After_Dispose()
        {
            using (DdeContext context = new DdeContext())
            {
                TransactionFilter filter = new TransactionFilter();
                context.AddTransactionFilter(filter);
                context.Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.RemoveTransactionFilter(filter));
            }
        }

        [Test]
        public void Test_TransactionFilter()
        {
            using (DdeContext context = new DdeContext())
            {
                TransactionFilter filter = new TransactionFilter();
                context.AddTransactionFilter(filter);
                context.Initialize();
                using (DdeServer server = new TestServer(ServiceName))
                {
                    server.Register();
                }
                Assert.That(filter.Received.WaitOne(Timeout), Is.True);
            }
        }

        [Test]
        public void Test_Register()
        {
            using (DdeContext context = new DdeContext())
            {
                EventListener listener = new EventListener();
                context.Register += listener.OnEvent;
                context.Initialize();
                using (DdeServer server = new TestServer(ServiceName))
                {
                    server.Register();
                }
                Assert.That(listener.Received.WaitOne(Timeout), Is.True);
            }
        }

        [Test]
        public void Test_Unregister()
        {
            using (DdeContext context = new DdeContext())
            {
                EventListener listener = new EventListener();
                context.Unregister += listener.OnEvent;
                context.Initialize();
                using (DdeServer server = new TestServer(ServiceName))
                {
                    server.Register();
                    server.Unregister();
                }
                Assert.That(listener.Received.WaitOne(Timeout), Is.True);
            }
        }

        #region TransactionFilter
        private sealed class TransactionFilter : IDdeTransactionFilter
        {
            private System.Threading.ManualResetEvent _Received     = new System.Threading.ManualResetEvent(false);
            private List<DdeTransaction>               _Transactions = new List<DdeTransaction>();

            public IReadOnlyList<DdeTransaction> Transactions
            {
                get { return _Transactions.AsReadOnly(); }
            }

            public System.Threading.WaitHandle Received
            {
                get { return _Received; }
            }

            public bool PreFilterTransaction(DdeTransaction t)
            {
                _Transactions.Add(t);
                _Received.Set();
                return false;
            }
        }
        #endregion

    } // class

} // namespace