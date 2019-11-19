using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace NEO.MODULE
{
    public class TCP
    {
        public delegate void OnReceiveData(byte[] bytes);
        public OnReceiveData onReceiveData = null;
        public void AddReceiveDataHandler(OnReceiveData onReceiveData)
        {
            this.onReceiveData += onReceiveData;
        }
        public void RemoveReceiveDataHandler(OnReceiveData onReceiveData)
        {
            this.onReceiveData -= onReceiveData;
        }

        public delegate void OnError(ERROR error, string errorInfo);
        public OnError onError = null;
        public void AddErrorHandler(OnError onError)
        {
            this.onError += onError;
        }
        public void RemoveErrorHandler(OnError onError)
        {
            this.onError -= onError;
        }


        //头部4字节Int型表明数据包长度
        int headSize = 4;
        Socket socket;
        
        Thread waitConnectThread = null;
        int connectTimeOutThreshold = 500;

        Thread receiveThread = null;
        bool keepReceiving = true;
        byte[] receiveBuffer;
        int receiveBufferSize = 1024 * 64;
        //这个需要变长，以适应超长数据
        //但是不能一直保留最长，需要在一定时间没有再收到超长数据时缩回去
        byte[] receiveCacheBuffer;
        //完整报文长度，用于粘包
        int messageLength;
        //当前缓存已经存储的位置，用于往后添加
        int cacheOffset;
        
        Queue<byte[]> sendMessages;
        Thread sendThread = null;
        bool keepSending = false;

        public void Initialize()
        {
            receiveBuffer = new byte[receiveBufferSize];
            receiveCacheBuffer = new byte[receiveBufferSize];

            sendMessages = new Queue<byte[]>();
        }

        public void Connect(string IP, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress adress = IPAddress.Parse(IP);
            IPEndPoint endPoint = new IPEndPoint(adress, port);

            messageLength = -1;
            cacheOffset = 0;

            try
            {
                IAsyncResult connectResult = socket.BeginConnect(endPoint, (result) => {
                    socket.EndConnect(result);
                    if (socket.Connected)
                    {
                        StartReceive();
                        StartSend();
                    }
                    else
                    {
                        if (waitConnectThread != null)
                        {
                            waitConnectThread.Abort();
                            waitConnectThread = null;
                        }
                        onError?.Invoke(ERROR.ConnectFailed, "Connect Failed");
                    }
                    if (waitConnectThread != null)
                    {
                        waitConnectThread.Abort();
                        waitConnectThread = null;
                    }
                }, null);

                waitConnectThread = new Thread(() => {
                    bool waitResult = connectResult.AsyncWaitHandle.WaitOne(connectTimeOutThreshold);
                    if (waitResult == false)
                    {
                        socket.EndConnect(connectResult);
                        onError?.Invoke(ERROR.ConnectTimeOut, "Connect Time Out");
                    }
                });
                waitConnectThread.Start();
            }
            catch(Exception e)
            {
                onError?.Invoke(ERROR.System, e.Message);
            }
        }
        
        void StartReceive()
        {
            if(receiveThread != null)
            {
                receiveThread.Abort();
            }
            keepReceiving = true;
            receiveThread = new Thread(Receiving);
            receiveThread.Start();
        }
        void Receiving()
        {
            int length = 0;
            while (keepReceiving)
            {
                Array.Clear(receiveBuffer,0, length);
                length = socket.Receive(receiveBuffer);
                if(length > 0)
                {
                    //二倍扩充
                    if(cacheOffset + length > receiveCacheBuffer.Length)
                    {
                        byte[] newCacheBuffer = new byte[receiveCacheBuffer.Length * 2];
                        Array.Copy(receiveCacheBuffer,0, newCacheBuffer, 0, cacheOffset);
                        receiveCacheBuffer = newCacheBuffer;
                    }
                    //接收数据到缓存
                    Array.Copy(receiveBuffer, 0, receiveCacheBuffer, cacheOffset, length);
                    cacheOffset += length;
                }
                else
                {
                    length = 0;
                }
            }
        }
        //主线程每帧调用，对接收到的完整数据进行分发
        public void Update()
        {
            if (cacheOffset > headSize)
            {
                if (messageLength < 0)
                {
                    messageLength = BitConverter.ToInt32(receiveCacheBuffer, 0);
                }
                if (cacheOffset >= headSize + messageLength)
                {
                    byte[] message = new byte[messageLength];
                    Array.Copy(receiveCacheBuffer, headSize, message, 0, messageLength);
                    onReceiveData?.Invoke(message);
                    for (int i = headSize + messageLength, j = 0; i < cacheOffset; ++i, ++j)
                    {
                        receiveCacheBuffer[j] = receiveCacheBuffer[i];
                    }
                    cacheOffset -= (headSize + messageLength);
                    messageLength = -1;
                }
            }
        }

        public void Send(byte[] bytes)
        {
            byte[] sendMessage = new byte[bytes.Length + headSize];
            byte[] headBytes = BitConverter.GetBytes(bytes.Length);
            Array.Copy(headBytes, 0, sendMessage, 0, headSize);
            Array.Copy(bytes, 0, sendMessage, headSize, bytes.Length);
            sendMessages.Enqueue(sendMessage);
        }
        void StartSend()
        {
            if(sendThread != null)
            {
                sendThread.Abort();
                sendThread = null;
            }
            keepSending = true;
            sendThread = new Thread(Sending);
            sendThread.Start();
        }
        void Sending()
        {
            while (keepSending)
            {
                if(sendMessages.Count > 0)
                {
                    byte[] message = sendMessages.Dequeue();
                    int messageLength = message.Length;
                    try
                    {
                        IAsyncResult sendResult = socket.BeginSend(message, 0, messageLength, SocketFlags.None,
                        (result) => {
                            int sendLength = socket.EndSend(result);
                            if (messageLength != sendLength)
                            {
                                onError?.Invoke(ERROR.SendFailed, message.ToString());
                            }
                        }, null);

                        //TODO：添加发送超时处理
                    }
                    catch(Exception e)
                    {
                        onError?.Invoke(ERROR.System, e.Message);
                    }
                    
                }
            }
        }

        public void Close()
        {
            keepReceiving = false;
            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }
            if (waitConnectThread != null)
            {
                waitConnectThread.Abort();
                waitConnectThread = null;
            }
            if (sendThread != null)
            {
                sendThread.Abort();
                sendThread = null;
            }
            if(socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }


        public enum ERROR
        {
            None,
            ConnectFailed,
            ConnectTimeOut,
            SendFailed,
            System,
        }
    }
}

