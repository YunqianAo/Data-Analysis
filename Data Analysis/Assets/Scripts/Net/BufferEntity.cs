using System.Net;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Net
{
    public class BufferEntity
    {
        public int recurCount = 0;
        public int protoSize;
        public int session;
        public int sn;
        public int moduleID;
        public long time;
        public int messageType;
        public int messageID;
        public byte[] proto;
        public IPEndPoint endPoint;
        public byte[] buffer;

        public BufferEntity(IPEndPoint endPoint, int session, int sn, int moduleID, int messageType, int messageID, byte[] proto)
        {
            protoSize = proto.Length;
            this.endPoint = endPoint;
            this.session = session;
            this.sn = sn;
            this.moduleID = moduleID;
            this.messageType = messageType;
            this.messageID = messageID;

        }
        public byte[] Encoder(bool isAck) {

            byte[] data = new byte[32+protoSize];
            if (isAck = true)
            {
                protoSize = 0;
            }
            byte[] _length=BitConverter.GetBytes(protoSize);
            byte[] _session=BitConverter.GetBytes(session);
            byte[] _sn=BitConverter.GetBytes(sn);
            byte[] _moduleID=BitConverter.GetBytes(moduleID);
            byte[] _time=BitConverter.GetBytes(time);
            byte[] _messageType=BitConverter.GetBytes(messageType);
            byte[] _messageID=BitConverter.GetBytes(messageID);

            Array.Copy(_length,0,data,0, 4);
            Array.Copy(_session, 0, data, 4, 4);
            Array.Copy(_sn, 0, data, 8, 4);
            Array.Copy(_moduleID, 0, data, 12, 4);
            Array.Copy(_time,0,data,16,8);
            Array.Copy(_messageType, 0, data, 24,4);
            Array.Copy(_messageID, 0, data, 28,4);
            if (isAck)
            {

            }
            else
            {
                Array.Copy(proto,0, data, 32, proto.Length);
            }

            buffer=data;
            return data;

        }

        public BufferEntity(IPEndPoint endPoint, byte[] buffer)
        {
            this.endPoint=endPoint;
            this.buffer=buffer;
            Decode();
        }
        public bool isFull=false;  
        private void Decode()
        {
            if (buffer.Length >= 4)
            {
                protoSize=BitConverter.ToInt32(buffer, 0);
                if (buffer.Length==protoSize+32)
                {
                    isFull=true;
                }
                else
                {
                    isFull=false ;
                    return;
                }
            }
            protoSize=BitConverter.ToInt32(buffer,0);
            session=BitConverter.ToInt32(buffer,4);
            sn=BitConverter.ToInt32(buffer,8);
            moduleID=BitConverter.ToInt32(buffer,12);
            time=BitConverter.ToInt64(buffer,16);
            messageType=BitConverter.ToInt32(buffer,24);
            messageID=BitConverter.ToInt32(buffer,28);

            if(messageType==0)
            {

            }
            else
            {
                proto=new byte[protoSize];
                Array.Copy(buffer,32,proto,0,protoSize);
            }

            //BitConverter.ToInt64();
        }
        public BufferEntity(BufferEntity package)
        {
            protoSize = 0;
            this.endPoint=package.endPoint; 
            this.session=package.session;
            this.sn=package.sn;
            this.moduleID=package.moduleID;
            this.time=0;
            this.messageType=0;
            this.messageID=package.messageID;
            buffer=Encoder(true);

        }
    }
}
