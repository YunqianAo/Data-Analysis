using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Net
{
    public class UClient
    {
        IPEndPoint endPoint;
        USocket uSocket;
        public int sessionID;
        public int sendSN = 0;
        public int handleSN = 0;
        

        Action<BufferEntity> handleAction;
        public UClient(USocket uSocket,IPEndPoint endPoint,int sendSN, int handleSN, int sessionID, Action<BufferEntity> dispatchNetEvent)
        {
            this.endPoint = endPoint;
            this.uSocket = uSocket;
            this.sessionID = sessionID;
            this.handleSN = handleSN;
            this.sendSN=sendSN;
            handleAction = dispatchNetEvent;
            CheckOutTime();
        }
        public void Handle(BufferEntity buffer)
        {
            if(this.sessionID==0&&buffer.session!=0) {
            this.sessionID = buffer.session;
            }
            switch (buffer.messageType)
            {
                case 0:
                    BufferEntity bufferEntity;
                    if (sendPackage.TryRemove(buffer.sn, out bufferEntity))
                    {
                        Debug.Log($"received ACK,sn{buffer.sn}");
                    }
                    break;
                    case 1:
                    BufferEntity ackPackage = new BufferEntity(buffer);
                    uSocket.SendACK(ackPackage);
                    HandleLoginPackage(ackPackage);
                    break;
                    default:
                    break;
            }
        }
        ConcurrentDictionary<int,BufferEntity> waitHandle = new ConcurrentDictionary<int,BufferEntity>();
        void HandleLoginPackage(BufferEntity buffer)
        {
            if (buffer.sn <= handleSN)
            {
                return;
            }
            if(buffer.sn > handleSN+1)
            {
                if (waitHandle.TryAdd(buffer.sn, buffer))
                {
                    Debug.Log($"received incorrect buffer{buffer.sn}");
                    return;
                }
                
            }
            handleSN = buffer.sn;
            if (handleAction!=null)
            {
                handleAction(buffer);
            }
            BufferEntity nextBuffer;
            if(waitHandle.TryRemove(handleSN+1, out nextBuffer))
            {
                HandleLoginPackage(nextBuffer);
            }
        }
        ConcurrentDictionary<int,BufferEntity> sendPackage= new ConcurrentDictionary<int,BufferEntity>();
       public void Send(BufferEntity package)
        {
            package.time = TimeHelper.Now();
            sendSN += 1;
            package.sn = sendSN;
            package.Encoder(false);
            if(sessionID == 0)
            {
                sendPackage.TryAdd(sendSN, package);
            }
            else
            {

            }
            uSocket.Send(package.buffer, endPoint);
        }
        int overtime = 150;
        public async void CheckOutTime()
        {
            await Task.Delay(overtime);
            foreach (var package in sendPackage.Values)
            {
                if (TimeHelper.Now() - package.time >= overtime * 10)
                {
                    OnDisconnect();
                    return;
                }
                if (TimeHelper.Now() - package.time >=(package.recurCount+1)* overtime)
                {
                    package.recurCount += 1;
                    uSocket.Send(package.buffer, endPoint);
                }
            }
            CheckOutTime();
        }

        public void OnDisconnect() {
        handleAction= null;
            uSocket.Close();
        }
    }
}
