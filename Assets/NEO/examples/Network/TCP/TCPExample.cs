using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEO.MODULE;
using System;
using UnityEngine.UI;

public class TCPExample : MonoBehaviour
{
    TCP client;
    public Text serverText;
    public Text errorText;
    string IP = "127.0.0.1";
    int port = 555;

    void Start()
    {
        client = new TCP();
        client.Initialize();
        client.AddErrorHandler(OnError);
        client.AddReceiveDataHandler(OnReceiveData);
        client.Connect(IP, port);
    }

    void Update()
    {
        client.Update();
    }

    void OnReceiveData(byte[] message)
    {
        serverText.text = serverText.text + "\n" + System.Text.Encoding.Default.GetString(message);
    }

    void OnError(TCP.ERROR error, string errorInfo)
    {
        errorText.text = "ERROR:" + error.ToString() + "  :  " + errorInfo;
    }

    public void NormalMessage()
    {
        byte[] sendBytes = BitConverter.GetBytes((int)TCPServerExample.SendType.Normal);
        client.Send(sendBytes);
    }

    public void MultiMessage()
    {
        byte[] sendBytes = BitConverter.GetBytes((int)TCPServerExample.SendType.Multi);
        client.Send(sendBytes);
    }

    public void MultiSimulateMessage()
    {
        byte[] sendBytes = BitConverter.GetBytes((int)TCPServerExample.SendType.MultiSimulate);
        client.Send(sendBytes);
    }

    public void HugeMessage()
    {
        byte[] sendBytes = BitConverter.GetBytes((int)TCPServerExample.SendType.Huge);
        client.Send(sendBytes);
    }
}
