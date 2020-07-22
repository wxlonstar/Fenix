﻿using DotNetty.Buffers; 
using DotNetty.TCP;
using DotNetty.Transport.Channels;
using Fenix.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fenix
{
    public class TcpContainerClient: ITcpListener
    {
        public event Action<IChannel> Close;

        public event Action<IChannel> Connect;

        public event Action<IChannel> Disconnect;

        public event Action<IChannel, Exception> Exception;

        public event Action<IChannel, IByteBuffer> Receive;

        public TcpSocketClient client;

        public bool IsActive => this.client.IsActive; 
        
        public void OnConnect(IChannel channel)
        { 
            Connect?.Invoke(channel); 
        }

        public void OnDisconnect(IChannel channel)
        {
            Disconnect?.Invoke(channel);
        }

        void ITcpListener.OnReceive(IChannel channel, IByteBuffer buffer)
        { 
            Receive?.Invoke(channel, buffer); 
        }

        void ITcpListener.OnClose(IChannel channel)
        { 
            Close?.Invoke(channel); 
        }

        void ITcpListener.OnException(IChannel channel, Exception ex)
        { 
            Exception?.Invoke(channel, ex); 
        }

        public static TcpContainerClient Create(IPEndPoint addr)
        {
            var channelConfig = new TcpChannelConfig();
            channelConfig.Address = addr.Address.ToString();
            channelConfig.Port = addr.Port;
            channelConfig.UseLibuv = true;

            var listener = new TcpContainerClient();
            listener.client = new TcpSocketClient();
            if (!listener.client.Start(channelConfig, listener))
                return null;
            return listener;
        }

        public void Send(byte[] bytes)
        {
            this.client.SendAsync(bytes);
                /*
            var task = Task.Run(()=>this.client.SendAsync(bytes));
            task.Wait();
                */
        }

        public void Stop()
        {
            this.client?.Shutdown();
        }
    }
}
