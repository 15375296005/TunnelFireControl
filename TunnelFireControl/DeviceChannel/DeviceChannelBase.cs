#region License

/**
 * Copyright (c) 2010,安徽皖通科技股份有限公司 All rights reserved.
 */

#endregion

using System;
using System.IO;
using System.Net.Sockets;
using Common.Logging;
using CommMonitor = System.Threading.Monitor;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// 全双工设备通道抽象基类。
    /// </summary>
    public abstract class DeviceChannelBase : IDeviceChannel
    {
        #region Fields

        private string _address;
        private ChannelState state;
        private object lockObj = new object();
        private ILog log;

        #endregion
         
        #region Constructor(s)

        /// <summary>
        /// 通过通道地址初始化类 <see cref="DeviceChannelBase"/> 的新实例。
        /// </summary>
        /// <param name="address">通道地址。</param>
        public DeviceChannelBase(string address)
        {
            this._address = address;
            this.state = ChannelState.Created;
            this.log = LogManager.GetLogger(GetType());
        }

        #endregion

        #region IDeviceChannel Members

        /// <summary>
        /// 打开通道。
        /// </summary>
        public void Open()
        {
            if (state == ChannelState.Opened)
            {
                return;
            }
            try
            {
                SetState(ChannelState.Opening);
                InternalOpen();
                SetState(ChannelState.Opened);
            }
            catch (Exception ex)
            {
                SetState(ChannelState.Faulted);
                log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// 关闭通道。
        /// </summary>
        public void Close()
        {
            if (state == ChannelState.Closed)
            {
                return;
            }
            try
            {
                SetState(ChannelState.Closing);
                InternalClose();
                SetState(ChannelState.Closed);
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                log.Error(string.Format("关闭IP为{0}的设备通道时发生异常", this._address), e);
            }
        }

        /// <summary>
        /// 向通道发送数据。
        /// </summary>
        /// <param name="buffer">发送数据缓冲。</param>
        /// <param name="offset">发送的起始位置。</param>
        /// <param name="count">发送的字节数。</param>
        public void Send(byte[] buffer, int offset, int count)
        {
            try
            {
                AssertChannelOpened();
                Stream stream = GetInternalStream();
                stream.Write(buffer, offset, count);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    log.Error(string.Format("向通道【{0}】时发送数据时发生异常", this._address), e);
                }
            }
        }

        /// <summary>
        /// 从通道接收数据。
        /// </summary>
        /// <param name="buffer">接收数据缓冲。</param>
        /// <param name="offset">接收的起始位置。</param>
        /// <param name="count">接收的起始位置。</param>
        /// <returns>接收到的实际字节数。</returns>
        public int Receive(byte[] buffer, int offset, int count)
        {
            try
            {
                Stream stream = GetInternalStream();
                return stream.Read(buffer, offset, count);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    log.Error(string.Format("从通道【{0}】时接收数据时发生异常", this._address), e);
                }
                return 0;
            }
        }

        /// <summary>
        /// 开始异步发送数据。
        /// </summary>
        /// <param name="buffer">发送数据缓冲。</param>
        /// <param name="offset">发送的起始位置。</param>
        /// <param name="size">发送的字节数。</param>
        /// <param name="callback">异步回调对象。</param>
        /// <param name="state">状态对象。</param>
        /// <returns>异步结果对象。</returns>
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            try
            {
                AssertChannelOpened();
                Stream stream = GetInternalStream();
                return stream.BeginWrite(buffer, offset, size, callback, state);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    SetState(ChannelState.Faulted);
                    log.Error(string.Format("从通道【{0}】时开始异步发送数据时发生异常", this._address), e);
                }

                return null;
            }
        }

        /// <summary>
        /// 结束异步发送。
        /// </summary>
        /// <param name="asyncResult">异步结果对象。</param>
        public void EndSend(IAsyncResult asyncResult)
        {
            try
            {
                Stream stream = GetInternalStream();
                stream.EndWrite(asyncResult);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    log.Error(string.Format("从通道【{0}】时结束异步发送数据时发生异常", this._address), e);
                }
            }
        }

        /// <summary>
        /// 开始异步接收数据。
        /// </summary>
        /// <param name="buffer">接收数据缓冲。</param>
        /// <param name="offset">接收的起始位置。</param>
        /// <param name="size">接收的起始位置。</param>
        /// <param name="callback">异步回调对象。</param>
        /// <param name="state">状态对象。</param>
        /// <returns>异步结果对象。</returns>
        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            try
            {
                Stream stream = GetInternalStream();
                return stream.BeginRead(buffer, offset, size, callback, state);
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    log.Error(string.Format("从通道【{0}】时开始异步接收数据时发生异常", this._address), e);
                }

                return null;
            }
        }

        /// <summary>
        /// 结束异步接收。
        /// </summary>
        /// <param name="asyncResult">异步结果对象。</param>
        /// <returns>接收到的实际字节数。</returns>
        public int EndReceive(IAsyncResult asyncResult)
        {
            try
            {
                Stream stream = GetInternalStream();
                int len = stream.EndRead(asyncResult);
                if (len == 0)
                {
                    //this.Close();
                }
                return len;
            }
            catch (Exception e)
            {
                if (e is IOException || e is SocketException)
                {
                    SetState(ChannelState.Faulted);
                }
                else
                {
                    log.Error(string.Format("从通道【{0}】时接收数据时发生异常", this._address), e);
                }
                return 0;
            }
        }

        /// <summary>
        /// 获得通道的地址。
        /// </summary>
        public string Address
        {
            get
            {
                return this._address;
            }
        }

        /// <summary>
        /// 获得通道的状态。
        /// </summary>
        public ChannelState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// 通道被关闭事件。
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// 通道正在关闭事件。
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// 通道异常事件。
        /// </summary>
        public event EventHandler Faulted;

        /// <summary>
        /// 通道被打开事件。
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// 通道正在打开事件。
        /// </summary>
        public event EventHandler Opening;

        #endregion

        #region Help Methods

        /// <summary>
        /// 设置通道状态辅助方法。
        /// </summary>
        /// <param name="state">目标状态。</param>
        protected void SetState(ChannelState state)
        {
            this.state = state;
            try
            {
                switch (state)
                {
                    case ChannelState.Opening:
                        if (this.Opening != null)
                            this.Opening(this, new EventArgs());
                        break;
                    case ChannelState.Opened:
                        if (this.Opened != null)
                            this.Opened(this, new EventArgs());
                        break;
                    case ChannelState.Closing:
                        if (this.Closing != null)
                            this.Closing(this, new EventArgs());
                        break;
                    case ChannelState.Closed:
                        if (this.Closed != null)
                            this.Closed(this, new EventArgs());
                        break;
                    default:
                        if (this.Faulted != null)
                            this.Faulted(this, new EventArgs());
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("通道【{0}】设置状态时发生异常", this._address), e);
            }
        }

        /// <summary>
        /// 确认通道处于打开状态。
        /// </summary>
        /// <remarks>确认通道处于打开状态，否则重新打开通道。</remarks>
        /// <exception cref="Exception"></exception>
        private void AssertChannelOpened()
        {
            if (State == ChannelState.Opened)
            {
                return;
            }
            if (CommMonitor.TryEnter(lockObj))
            {
                if (State != ChannelState.Opened)
                {
                    //this.Close();
                    this.Open();
                }
                CommMonitor.Exit(lockObj);
            }
            if (State != ChannelState.Opened)
            {
                throw new Exception("通道重连失败，请检查通讯链路。");
            }
        }

        /// <summary>
        /// 内部打开通道。
        /// </summary>
        protected abstract void InternalOpen();

        /// <summary>
        /// 内部关闭通道。
        /// </summary>
        protected abstract void InternalClose();

        /// <summary>
        /// 获得通道内部流对象。
        /// </summary>
        /// <returns></returns>
        protected abstract Stream GetInternalStream();

        #endregion
    }
}
