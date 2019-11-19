using System;
using System.Collections.Generic;

namespace NEO.SUPPORT
{
    /// <summary>
    /// 事件系统扩展：线程安全
    /// 目的：可以在子线程调用这些接口而不出错
    /// 原因：Unity只允许主线程用其接口|实在要用到多线程的同时访问保护
    /// 原理：子线程只把要分发的消息告诉事件系统，事件系统再在主线程处理缓存消息
    /// </summary>
    public partial class Messenger
    {
        /// <summary>
        /// 发消息：线程安全
        /// </summary>
        /// <param name="message">消息体</param>
        public void SaveSend(Message message)
        {
            lock (messages)
            {
                messages.Add(message);
            }
        }

        /// <summary>
        /// 处理堆压的各种缓存事件
        /// 需要外部调用（默认是每帧都需要被调用）
        /// </summary>
        public void Update()
        {
            lock (messages)
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    Send(messages[i]);
                }
                messages.Clear();
            }
        }
    }
}

