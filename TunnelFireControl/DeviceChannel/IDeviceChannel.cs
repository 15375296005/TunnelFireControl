#region License

/**
 * Copyright (c) 2010,安徽皖通科技股份有限公司 All rights reserved.
 */

#endregion

using System;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// 设备通道接口。
    /// </summary>
    public interface IDeviceChannel
    {
        /// <summary>
        /// 打开通道。
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭通道。
        /// </summary>
        void Close();

        /// <summary>
        /// 向通道发送数据。
        /// </summary>
        /// <param name="buffer">发送数据缓冲。</param>
        /// <param name="offset">发送的起始位置。</param>
        /// <param name="count">发送的字节数。</param>
        void Send(byte[] buffer, int offset,int count);

        /// <summary>
        /// 从通道接收数据。
        /// </summary>
        /// <param name="buffer">接收数据缓冲。</param>
        /// <param name="offset">接收的起始位置。</param>
        /// <param name="count">接收的起始位置。</param>
        /// <returns>接收到的实际字节数。</returns>
        int Receive(byte[] buffer, int offset, int count);

        /// <summary>
        /// 开始异步发送数据。
        /// </summary>
        /// <param name="buffer">发送数据缓冲。</param>
        /// <param name="offset">发送的起始位置。</param>
        /// <param name="size">发送的字节数。</param>
        /// <param name="callback">异步回调对象。</param>
        /// <param name="state">状态对象。</param>
        /// <returns>异步结果对象。</returns>
        IAsyncResult BeginSend( byte[] buffer,int offset,int size,AsyncCallback callback,Object state);

        /// <summary>
        /// 结束异步发送。
        /// </summary>
        /// <param name="asyncResult">异步结果对象。</param>
        void EndSend( IAsyncResult asyncResult);

        /// <summary>
        /// 开始异步接收数据。
        /// </summary>
        /// <param name="buffer">接收数据缓冲。</param>
        /// <param name="offset">接收的起始位置。</param>
        /// <param name="size">接收的起始位置。</param>
        /// <param name="callback">异步回调对象。</param>
        /// <param name="state">状态对象。</param>
        /// <returns>异步结果对象。</returns>
        IAsyncResult BeginReceive( byte[] buffer,int offset,int size,AsyncCallback callback,Object state);

        /// <summary>
        /// 结束异步接收。
        /// </summary>
        /// <param name="asyncResult">异步结果对象。</param>
        /// <returns>接收到的实际字节数。</returns>
        int EndReceive( IAsyncResult asyncResult );

        /// <summary>
        /// 获得通道的地址。
        /// </summary>
        string Address { get; }

        /// <summary>
        /// 获得通道的状态。
        /// </summary>
        ChannelState State { get; }

        /// <summary>
        /// 通道被关闭事件。
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// 通道正在关闭事件。
        /// </summary>
        event EventHandler Closing;

        /// <summary>
        /// 通道异常事件。
        /// </summary>
        event EventHandler Faulted;

        /// <summary>
        /// 通道被打开事件。
        /// </summary>
        event EventHandler Opened;

        /// <summary>
        /// 通道正在打开事件。
        /// </summary>
        event EventHandler Opening;

    }
}
