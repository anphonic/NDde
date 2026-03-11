namespace NDde.Test
{
    using System;
    using System.Text;
    using NDde;
    using NDde.Advanced;
    using NDde.Client;
    using NDde.Server;
    using NUnit.Framework;

    [TestFixture]
    public sealed class Test_DdeClient
    {
        private const string ServiceName = "myservice";
        private const string TopicName = "mytopic";
        private const string ItemName = "myitem";
        private const string CommandText = "mycommand";
        private const string TestData = "Hello World";
        private const int Timeout = 1000;

        [Test]
        public void Test_Ctor_Overload_1()
        {
            DdeClient client = new DdeClient(ServiceName, TopicName);
        }

        [Test]
        public void Test_Ctor_Overload_2()
        {
            using (DdeContext context = new DdeContext())
            {
                DdeClient client = new DdeClient(ServiceName, TopicName, context);
            }
        }

        [Test]
        public void Test_Dispose()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName))
            {
            }
        }

        [Test]
        public void Test_Service()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName)) 
            {
                Assert.That(client.Service, Is.EqualTo(ServiceName));
            }
        }

        [Test]
        public void Test_Topic()
        {
            using (DdeClient client = new DdeClient(ServiceName, TopicName)) 
            {
                Assert.That(client.Topic, Is.EqualTo(TopicName));
            }
        }

        [Test]
        public void Test_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                }
            }
        }

        [Test]
        public void Test_Connect_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Connect());
                }
            }
        }

        [Test]
        public void Test_Connect_After_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.Throws<InvalidOperationException>(() => client.Connect());
                }
            }
        }

        [Test]
        public void Test_Disconnect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                }
            }
        }

        [Test]
        public void Test_Disconnect_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Disconnect());
                }
            }
        }

        [Test]
        public void Test_Disconnect_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.Disconnect());
                }
            }
        }


        [Test]
        public void Test_Handle_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.That(client.Handle, Is.EqualTo(IntPtr.Zero));
                }
            }
        }

        [Test]
        public void Test_Handle_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.That(client.Handle, Is.Not.EqualTo(IntPtr.Zero));
                }
            }
        }

        [Test]
        public void Test_Handle_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                    Assert.That(client.Handle, Is.EqualTo(IntPtr.Zero));
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.That(client.IsConnected, Is.False);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.That(client.IsConnected, Is.True);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Disconnect();
                    Assert.That(client.IsConnected, Is.False);
                }
            }
        }

        [Test]
        public void Test_IsConnected_Variation_4()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    server.Disconnect();
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    Assert.That(client.IsConnected, Is.False);
                }
            }
        }

        [Test]
        public void Test_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.False);
                }
            }
        }

        [Test]
        public void Test_Pause_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Pause());
                }
            }
        }

        [Test]
        public void Test_Pause_After_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    Assert.Throws<InvalidOperationException>(() => client.Pause());
                }
            }
        }

        [Test]
        public void Test_Resume()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.False);
                    client.Resume();
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                }
            }
        }

        [Test]
        public void Test_Resume_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Resume());
                }
            }
        }

        [Test]
        public void Test_Resume_Before_Pause()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.Throws<InvalidOperationException>(() => client.Resume());
                }
            }
        }

        [Test]
        public void Test_Abandon()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.False);
                    client.Abandon(ar);
                    client.Resume();
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.False);
                }
            }
        }

        [Test]
        public void Test_Abandon_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    IAsyncResult ar = client.BeginExecute(CommandText, null, null);
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Abandon(ar));
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.That(client.IsPaused, Is.False);
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    Assert.That(client.IsPaused, Is.True);
                }
            }
        }

        [Test]
        public void Test_IsPaused_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Pause();
                    client.Resume();
                    Assert.That(client.IsPaused, Is.False);
                }
            }
        }
        
        [Test]
        public void Test_Poke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.That(Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_TryPoke_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    int result = client.TryPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.That(result, Is.Not.EqualTo(0));
                }
            }
        }

        [Test]
        public void Test_TryPoke_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    int result = client.TryPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout);
                    Assert.That(result, Is.EqualTo(0));
                    Assert.That(Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_Poke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout));
                }
            }
        }

        [Test]
        public void Test_Poke_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.Poke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, Timeout));
                }
            }
        }

        [Test]
        public void Test_BeginPoke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                }
            }
        }

        [Test]
        public void Test_BeginPoke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null));
                }
            }
        }

        [Test]
        public void Test_BeginPoke_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null));
                }
            }
        }

        [Test]
        public void Test_EndPoke()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.EndPoke(ar);
                    Assert.That(Encoding.ASCII.GetString(server.GetData(TopicName, ItemName, 1)), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_EndPoke_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginPoke(ItemName, Encoding.ASCII.GetBytes(TestData), 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.EndPoke(ar));
                }
            }
        }

        [Test]
        public void Test_Request_Overload_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    byte[] data = client.Request(ItemName, 1, Timeout);
                    Assert.That(Encoding.ASCII.GetString(data), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_Request_Overload_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    string data = client.Request(ItemName, Timeout);
                    Assert.That(data, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_TryRequest_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    byte[] data;
                    int result = client.TryRequest(ItemName, 1, Timeout, out data);
                    Assert.That(result, Is.Not.EqualTo(0));
                }
            }
        }

        [Test]
        public void Test_TryRequest_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    byte[] data;
                    int result = client.TryRequest(ItemName, 1, Timeout, out data);
                    Assert.That(result, Is.EqualTo(0));
                    Assert.That(Encoding.ASCII.GetString(data), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_Request_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Request(ItemName, 1, Timeout));
                }
            }
        }

        [Test]
        public void Test_Request_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.Request(ItemName, 1, Timeout));
                }
            }
        }

        [Test]
        public void Test_BeginRequest()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                }
            }
        }

        [Test]
        public void Test_BeginRequest_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.BeginRequest(ItemName, 1, null, null));
                }
            }
        }

        [Test]
        public void Test_BeginRequest_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.BeginRequest(ItemName, 1, null, null));
                }
            }
        }

        [Test]
        public void Test_EndRequest()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    byte[] data = client.EndRequest(ar);
                    Assert.That(Encoding.ASCII.GetString(data), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_EndRequest_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginRequest(ItemName, 1, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.EndRequest(ar));
                }
            }
        }

        [Test]
        public void Test_Execute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Execute(TestData, Timeout);
                    Assert.That(server.Command, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_TryExecute_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    int result = client.TryExecute(TestData, Timeout);
                    Assert.That(result, Is.Not.EqualTo(0));
                }
            }
        }

        [Test]
        public void Test_TryExecute_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    int result = client.TryExecute(TestData, Timeout);
                    Assert.That(result, Is.EqualTo(0));
                    Assert.That(server.Command, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_Execute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.Execute(TestData, Timeout));
                }
            }
        }

        [Test]
        public void Test_Execute_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.Execute(TestData, Timeout));
                }
            }
        }

        [Test]
        public void Test_BeginExecute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                }
            }
        }

        [Test]
        public void Test_BeginExecute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.BeginExecute(TestData, null, null));
                }
            }
        }

        [Test]
        public void Test_BeginExecute_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.BeginExecute(TestData, null, null));
                }
            }
        }

        [Test]
        public void Test_EndExecute()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.EndExecute(ar);
                    Assert.That(server.Command, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_EndExecute_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginExecute(TestData, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.EndExecute(ar));
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    client.Disconnect();
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.That(args.IsServerInitiated, Is.False);
                    Assert.That(args.IsDisposed, Is.False);
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    server.Disconnect();
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.That(args.IsServerInitiated, Is.True);
                    Assert.That(args.IsDisposed, Is.False);
                }
            }
        }

        [Test]
        public void Test_Disconnected_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Disconnected += listener.OnEvent;
                    client.Connect();
                    client.Dispose();
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeDisconnectedEventArgs args = (DdeDisconnectedEventArgs)listener.Events[0];
                    Assert.That(args.IsServerInitiated, Is.False);
                    Assert.That(args.IsDisposed, Is.True);
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, Timeout);
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(Encoding.ASCII.GetString(args.Data), Is.EqualTo(TestData));
                    Assert.That(args.Text, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, false, Timeout);
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(args.Data, Is.Null);
                    Assert.That(args.Text, Is.Null);
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, true, Timeout, "MyStateObject");
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(args.State, Is.EqualTo("MyStateObject"));
                    Assert.That(Encoding.ASCII.GetString(args.Data), Is.EqualTo(TestData));
                    Assert.That(args.Text, Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_StartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.StartAdvise(ItemName, 1, false, Timeout));
                }
            }
        }

        [Test]
        public void Test_StartAdvise_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.StartAdvise(ItemName, 1, false, Timeout));
                }
            }
        }

        [Test]
        public void Test_StartAdvise_After_StartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.StartAdvise(ItemName, 1, false, Timeout);
                    Assert.Throws<InvalidOperationException>(() => client.StartAdvise(ItemName, 1, false, Timeout));
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_1()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(Encoding.ASCII.GetString(args.Data), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_2()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, false, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(args.Data, Is.Null);
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Variation_3()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    EventListener listener = new EventListener();
                    client.Advise += listener.OnEvent;
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, true, null, null, "MyStateObject");
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    server.Advise(TopicName, ItemName);
                    Assert.That(listener.Received.WaitOne(Timeout), Is.True);
                    DdeAdviseEventArgs args = (DdeAdviseEventArgs)listener.Events[0];
                    Assert.That(args.Item, Is.EqualTo(ItemName));
                    Assert.That(args.Format, Is.EqualTo(1));
                    Assert.That(args.State, Is.EqualTo("MyStateObject"));
                    Assert.That(Encoding.ASCII.GetString(args.Data), Is.EqualTo(TestData));
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.BeginStartAdvise(ItemName, 1, false, null, null));
                }
            }
        }

        [Test]
        public void Test_BeginStartAdvise_Before_Connect()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    Assert.Throws<InvalidOperationException>(() => client.BeginStartAdvise(ItemName, 1, false, null, null));
                }
            }
        }

        [Test]
        public void Test_EndStartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.EndStartAdvise(ar);
                }
            }
        }

        [Test]
        public void Test_EndStartAdvise_After_Dispose()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    IAsyncResult ar = client.BeginStartAdvise(ItemName, 1, true, null, null);
                    Assert.That(ar.AsyncWaitHandle.WaitOne(Timeout), Is.True);
                    client.Dispose();
                    Assert.Throws<ObjectDisposedException>(() => client.EndStartAdvise(ar));
                }
            }
        }

        [Test]
        public void Test_StopAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    client.StartAdvise(ItemName, 1, true, Timeout);
                    client.StopAdvise(ItemName, Timeout);
                }
            }
        }

        [Test]
        public void Test_StopAdvise_Before_StartAdvise()
        {
            using (TestServer server = new TestServer(ServiceName))
            {
                server.Register();
                server.SetData(TopicName, ItemName, 1, Encoding.ASCII.GetBytes(TestData));
                using (DdeClient client = new DdeClient(ServiceName, TopicName))
                {
                    client.Connect();
                    Assert.Throws<InvalidOperationException>(() => client.StopAdvise(ItemName, Timeout));
                }
            }
        }

    } // class

} // namespace