using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Game.Net
{
    public class USocket
    {
        UdpClient udpClient;
        string ip="192.168.0.144";
        int port=8899;
        public static IPEndPoint server;
        public static UClient local;

        public USocket(Action<BufferEntity> dispatchNetEvent)
        {
            udpClient = new UdpClient(0);
            server=new IPEndPoint(IPAddress.Parse(ip), port);
            local=new UClient(this, server,0,0,0,dispatchNetEvent);
        }
        ConcurrentQueue<UdpReceiveResult> awaitHandle = new ConcurrentQueue<UdpReceiveResult>();
        public async void ReceiveTask()
        {
            while (udpClient!=null)
            {
                try
                {
                    UdpReceiveResult result= await udpClient.ReceiveAsync();
                    awaitHandle.Enqueue(result);
                }
                catch(Exception e) {
                    Debug.LogError(e.Message);
                }
            }
        }
        public async void Send(byte[] data,IPEndPoint endPoint) {
            if (udpClient!=null)
            {
                try
                {
                    await udpClient.SendAsync(data,data.Length,ip,port);
                }
                catch( Exception e ) 
                {
                    Debug.LogError($"Send exception{e.Message}");
                }
            }
        }
        public void SendACK(BufferEntity bufferEntity)
        {
            Send(bufferEntity.buffer, server);
        }
        public void Handle()
        {
            if(awaitHandle.Count > 0)
            {
                UdpReceiveResult data;
                if(awaitHandle.TryDequeue(out data)){
                    BufferEntity bufferEntity = new BufferEntity(data.RemoteEndPoint, data.Buffer);
                    if(bufferEntity.isFull) {
                        Debug.Log($"process message id{bufferEntity.messageID} ");
                        local.Handle(bufferEntity);
                    }
                     
                }
            }
        }
        public void Close()
        {
            if (local != null)
            {
                local = null;
            }
            if ( udpClient != null )
            {
                udpClient.Close();
                udpClient = null;
            }
            
            
        }
    }
}
