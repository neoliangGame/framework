using UnityEngine;
using System.Threading;

using NEO.SUPPORT;

namespace NEO.example
{
    public class MessengerExample : MonoBehaviour
    {
        Messenger messenger;

        void Awake()
        {
            messenger = new Messenger();
        }

        void Start()
        {
            //注册监听事件
            messenger.Subscribe<Message1>(Receiver1);
            messenger.Subscribe<Message2>(Receiver2);
            messenger.Subscribe<Message2>(Receiver2);

            //分发事件
            messenger.Send(new Message1());
            messenger.Send(new Message2());

            //子线程调用
            Thread thread = new Thread(ThreadTest);
            thread.Start();
        }

        void Update()
        {
            messenger.Update();
        }

        #region 事件侦听者
        public void Receiver1(Message message)
        {
            Debug.Log("Receiver1 : " + ((Message1)message).content);

            //只是为了调用保证子线程调用Unity接口报错
            GameObject go = new GameObject();
        }

        public void Receiver2(Message message)
        {
            Debug.Log("Receiver2 : " + ((Message2)message).content);

            //只是为了调用保证子线程调用Unity接口报错
            GameObject go = new GameObject();
        }
        #endregion

        void ThreadTest()
        {
            messenger.SaveSend(new Message1() { content = "thread message 1" });
            messenger.SaveSend(new Message1() { content = "thread message 1 2 1" });
            messenger.SaveSend(new Message2() { content = "thread message 2" });
        }

        #region 消息类型
        public class Message1 : Message
        {
            public string content = "Message 1";
        }

        public class Message2 : Message
        {
            public string content = "Message 2";
        }
        #endregion
    }
}


