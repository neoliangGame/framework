using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class TCPServerExample : MonoBehaviour
{
    string IP = "127.0.0.1";
    int serverPort = 555;
    Socket serverSocket = null;
    const int listenMaxNumber = 10;
    bool keepListening;
    Queue<SendCMD> sendCMDs;

    public Text fromClientText;

    void Start()
    {
        sendCMDs = new Queue<SendCMD>();
        StartServer();
    }

    void StartServer()
    {
        fromClientText.text = "start server...";
        IPAddress serverAddress = IPAddress.Parse(IP);
        IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, serverPort);

        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(serverEndPoint);
        serverSocket.Listen(listenMaxNumber);
        Thread listenThread = new Thread(ServerListen);
        keepListening = true;
        listenThread.Start();
        fromClientText.text = "start listening...";
    }

    void ServerListen()
    {
        while (keepListening)
        {
            Socket clientSocket = serverSocket.Accept();
            Thread handleClientThread = new Thread(HandleClientConnect);
            handleClientThread.Start(clientSocket);
        }
    }

    void HandleClientConnect(object socketObject)
    {
        Socket clientSocket = socketObject as Socket;
        byte[] receiveBytes = new byte[1024 * 64];
        int length;
        int sendType = -1;
        while (true)
        {
            length = clientSocket.Receive(receiveBytes);
            if(length > 0)
            {
                sendType = BitConverter.ToInt32(receiveBytes,4);
                SendCMD sendCMD = new SendCMD();
                sendCMD.client = clientSocket;
                sendCMD.sendType = (SendType)sendType;
                lock (sendCMDs)
                {
                    sendCMDs.Enqueue(sendCMD);
                }
            }
        }
    }

    private void Update()
    {
        while(sendCMDs.Count > 0)
        {
            lock (sendCMDs)
            {
                HandleCMD(sendCMDs.Dequeue());
            }
        }
    }

    void HandleCMD(SendCMD sendCMD)
    {
        string sendMessage;
        StringBuilder hugeMessage;
        fromClientText.text = sendCMD.sendType.ToString();
        byte[] sendBytes;
        byte[] sendBytes2;
        byte[] sendBytes3;
        byte[] sendMultiBytes;
        byte[] headBytes;
        byte[] messageBytes;
        switch (sendCMD.sendType)
        {
            case SendType.Normal:
                sendMessage = "normal message AAAAA";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes, 4, messageBytes.Length);

                sendCMD.client.Send(sendBytes, SocketFlags.None);
                break;
            case SendType.Multi:
                //本意想测多个包合起来发送的情况，不过貌似依靠系统自身合包不行
                sendMessage = "1";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes, 4, messageBytes.Length);

                sendMessage = "2";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes2 = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes2, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes2, 4, messageBytes.Length);

                sendMessage = "3";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes3 = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes3, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes3, 4, messageBytes.Length);

                sendCMD.client.Send(sendBytes, SocketFlags.None);
                sendCMD.client.Send(sendBytes2, SocketFlags.None);
                sendCMD.client.Send(sendBytes3, SocketFlags.None);
                break;
            case SendType.MultiSimulate:
                //模拟合包
                sendMessage = "1";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes, 4, messageBytes.Length);
                sendMultiBytes = new byte[sendBytes.Length];
                Array.Copy(sendBytes, 0, sendMultiBytes, 0, sendBytes.Length);

                sendMessage = "2";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes = new byte[sendMultiBytes.Length + 4 + messageBytes.Length];
                Array.Copy(sendMultiBytes, 0, sendBytes, 0, sendMultiBytes.Length);
                Array.Copy(headBytes, 0, sendBytes, sendMultiBytes.Length, 4);
                Array.Copy(messageBytes, 0, sendBytes, sendMultiBytes.Length + 4, messageBytes.Length);
                sendMultiBytes = sendBytes;

                sendMessage = "3";
                headBytes = BitConverter.GetBytes(sendMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(sendMessage);
                sendBytes = new byte[sendMultiBytes.Length + messageBytes.Length + 4];
                Array.Copy(sendMultiBytes, 0, sendBytes, 0, sendMultiBytes.Length);
                Array.Copy(headBytes, 0, sendBytes, sendMultiBytes.Length, 4);
                Array.Copy(messageBytes, 0, sendBytes, sendMultiBytes.Length + 4, messageBytes.Length);

                sendCMD.client.Send(sendBytes, SocketFlags.None);
                break;
            case SendType.Huge:
                hugeMessage = new StringBuilder();
                hugeMessage.Append("start:::");
                for(int i = 0;i < 1000; i++)
                {
                    hugeMessage.Append("message ");
                }
                hugeMessage.Append(":::end");

                headBytes = BitConverter.GetBytes(hugeMessage.Length);
                messageBytes = System.Text.Encoding.Default.GetBytes(hugeMessage.ToString());
                sendBytes = new byte[messageBytes.Length + 4];
                Array.Copy(headBytes, 0, sendBytes, 0, 4);
                Array.Copy(messageBytes, 0, sendBytes, 4, messageBytes.Length);

                sendCMD.client.Send(sendBytes, SocketFlags.None);
                break;
        }
    }

    class SendCMD
    {
        public Socket client;
        public SendType sendType;
    }

    public enum SendType
    {
        Normal,
        Multi,
        MultiSimulate,
        Huge,
    }
}
