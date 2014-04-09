﻿using System;
using System.Net;
using Helios.Net;
using Helios.Net.Bootstrap;
using Helios.Ops;
using Helios.Ops.Executors;
using Helios.Topology;

namespace Helios.Reactor.Bootstrap
{
    public class ServerBootstrap : AbstractBootstrap
    {
        public ServerBootstrap()
            : base()
        {
            UseProxies = true;
            BufferBytes = NetworkConstants.DEFAULT_BUFFER_SIZE;
            Workers = 2;
            InternalExecutor = new BasicExecutor();
        }

        public ServerBootstrap(ServerBootstrap other)
            : base(other)
        {
            UseProxies = other.UseProxies;
            BufferBytes = other.BufferBytes;
            Workers = other.Workers;
            InternalExecutor = other.InternalExecutor;
        }

        protected IExecutor InternalExecutor { get; set; }

        protected IEventLoop EventLoop
        {
            get
            {
                return EventLoopFactory.CreateThreadedEventLoop(Workers, InternalExecutor);
            }
        }

        protected int Workers { get; set; }

        protected int BufferBytes { get; set; }

        protected bool UseProxies { get; set; }

        public TransportType Type { get; private set; }

        public ServerBootstrap SetTransport(TransportType type)
        {
            Type = type;
            return this;
        }

        public ServerBootstrap WorkerThreads(int workerThreadCount)
        {
            if (workerThreadCount < 1) throw new ArgumentException("Can't be below 1", "workerThreadCount");
            Workers = workerThreadCount;
            return this;
        }

        public ServerBootstrap BufferSize(int bufferSize)
        {
            if (bufferSize < 1024) throw new ArgumentException("Can't be below 1024", "bufferSize");
            BufferBytes = bufferSize;
            return this;
        }

        public ServerBootstrap WorkersAreProxies(bool useProxies)
        {
            UseProxies = useProxies;
            return this;
        }

        public ServerBootstrap Executor(IExecutor executor)
        {
            if (executor == null) throw new ArgumentNullException("executor");
            InternalExecutor = executor;
            return this;
        }

        public new ServerBootstrap LocalAddress(INode node)
        {
            base.LocalAddress(node);
            return this;
        }

        public new ServerBootstrap OnConnect(ConnectionEstablishedCallback connectionEstablishedCallback)
        {
            base.OnConnect(connectionEstablishedCallback);
            return this;
        }

        public new ServerBootstrap OnDisconnect(ConnectionTerminatedCallback connectionTerminatedCallback)
        {
            base.OnDisconnect(connectionTerminatedCallback);
            return this;
        }

        public new ServerBootstrap OnReceive(ReceivedDataCallback receivedDataCallback)
        {
            base.OnReceive(receivedDataCallback);
            return this;
        }

        public new ServerBootstrap SetOption(string optionKey, object optionValue)
        {
            base.SetOption(optionKey, optionValue);
            return this;
        }

        public override void Validate()
        {
            if (LocalNode == null) throw new NullReferenceException("LocalNode must be set");
            if (Type == TransportType.All) throw new ArgumentException("Type must be set");
            if (Workers < 1) throw new ArgumentException("Workers must be at least 1");
            if (BufferBytes < 1024) throw new ArgumentException("BufferSize must be at least 1024");
        }

        protected override IConnectionFactory BuildInternal()
        {
            switch (Type)
            {
                case TransportType.Tcp:
                    return new TcpServerFactory(this);
                case TransportType.Udp:
                    return new UdpServerFactory(this);
                default:
                    throw new InvalidOperationException("This shouldn't happen");
            }
        }

        public new IServerFactory Build()
        {
            return (IServerFactory) BuildInternal();
        }
    }
}