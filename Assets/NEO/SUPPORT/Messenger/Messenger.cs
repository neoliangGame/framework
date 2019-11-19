using System;
using System.Collections.Generic;

namespace NEO.SUPPORT
{
    /// <summary>
    /// 事件系统基础类
    /// 负责最基础的事件侦听和分发数据
    /// 每个事件侦听做了判重，相同的只能注册一次
    /// </summary>
    public partial class Messenger
    {
        /// <summary>
        /// 所有事件侦听者
        /// </summary>
        Dictionary<Type, List<Action<Message>>> messengers;

        /// <summary>
        /// 缓存事件
        /// </summary>
        List<Message> messages;

        #region 初始化
        /// <summary>
        /// 无参构造函数初始化必要的属性
        /// </summary>
        public Messenger()
        {
            messengers = new Dictionary<Type, List<Action<Message>>>();
            messages = new List<Message>();
        }
        #endregion

        #region 监听者
        /// <summary>
        /// 注册事件，订阅事件
        /// </summary>
        /// <param name="T">订阅类型</param>
        /// <param name="receiver">接收事件者</param>
        /// <returns></returns>
        public void Subscribe<T>(Action<Message> receiver) where T : Message
        {
            Type t = typeof(T);
            if (!messengers.TryGetValue(t, out List<Action<Message>> messenger))
            {
                messenger = new List<Action<Message>>();
                messengers[t] = messenger;
            }

            Delegate existReceiver = messenger.Find(er => er.Equals(receiver));
            if (existReceiver == null)
            {
                messenger.Add(receiver);
            }
            else
            {
                //warning：reSubscribe the same message receiver !
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="T">订阅类型</param>
        /// <param name="receiver">接收事件者</param>
        public void UnSubscribe<T>(Action<Message> receiver) where T : Message
        {
            if (!messengers.TryGetValue(typeof(T), out List<Action<Message>> messenger))
            {
                return;
            }
            messenger.Remove(receiver);
        }
        #endregion

        #region 发消息
        /// <summary>
        /// 发消息
        /// 类型由消息类型决定
        /// </summary>
        /// <param name="message">消息体</param>
        public void Send(Message message)
        {
            if (messengers.TryGetValue(message.GetType(), out List<Action<Message>> messenger))
            {
                for (int i = 0; i < messenger.Count; i++)
                {
                    messenger[i].Invoke(message);
                }
            }
        }
        #endregion

        /// <summary>
        /// 删除所有状态数据
        /// 1。事件侦听者
        /// 2。缓存事件
        /// </summary>
        public void Clear()
        {
            messengers.Clear();
        }
    }
}

