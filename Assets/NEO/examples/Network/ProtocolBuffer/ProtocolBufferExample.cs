using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEO.ProtoTest;
using System.IO;
using Google.Protobuf;

public class ProtocolBufferExample : MonoBehaviour
{
    void Start()
    {
        TestProto message = new TestProto();
        message.Id = 111;
        message.Name = "abcd";
        message.Number = 222;
        message.Content = "just content";
        MemoryStream memory = new MemoryStream();
        byte[] messageBytes;
        message.WriteTo(memory);
        messageBytes = memory.ToArray();
        memory.Close();
        TestProto copy = TestProto.Parser.ParseFrom(messageBytes);
        Debug.Log(copy.Id);
        Debug.Log(copy.Name);
        Debug.Log(copy.Number);
        Debug.Log(copy.Content);
    }

    void Update()
    {
        
    }
}
