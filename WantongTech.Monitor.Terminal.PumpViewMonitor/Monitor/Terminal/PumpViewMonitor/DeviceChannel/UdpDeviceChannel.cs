#region License

/**
 * Copyright (c) 2010,安徽皖通科技股份有限公司 All rights reserved.
 */

#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// Udp设备通道
    /// </summary>
    public class UdpDeviceChannel : IDeviceChannel
    {
        #region Fields

        private string _address;
        private string ipAddr;
        private int port;
        private ChannelState _state;
        private UdpClient _udpClient;
        private bool _bSending = false;
        private bool _bReceiving = false;

        #endregion

        /// <summary>
        /// 通过通道地址初始化类。
        /// </summary>
        /// <param name="address">通道地址。</param>
        public UdpDeviceChannel(string address)
        {
            this._address = address;
            this._state = ChannelState.Created;
            String[] props = _address.Split(':');
            if ((props.Length != 3))
            {
                throw new ArgumentException("Udp设备通道参数配置不正确");
            }
            ipAddr = props[1].Trim();
            port = Int32.Parse(props[2].Trim());
        }

        #region IDeviceChannel 成员

        /// <summary>
        /// 打开通道。
        /// </summary>
        public void Open()
        {
            try
            {
                SetState(ChannelState.Opening);
                this._udpClient = new UdpClient();
                this._udpClient.Connect(ipAddr, port);
                SetState(ChannelState.Opened);
            }
            catch
            {
                SetState(ChannelState.Faulted);
                throw;
            }
        }

        /// <summary>
        /// 关闭通道。
        /// </summary>
        public void Close()
        {
            try
            {
                SetState(ChannelState.Closing);
                if (this._udpClient != null)
                {
                    this._udpClient.Close();
                    this._udpClient = null;
                }
                SetState(ChannelState.Closed);
            }
            catch (Exception ex)
            {
                SetState(ChannelState.Faulted);
                throw ex;
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
            while (_bSending)
            {
                Thread.Sleep(1);
            }
            _bSending = true;
            try
            {
                if ((offset == 0) && (count == buffer.Length))
                {
                    this._udpClient.Send(buffer, count);
                }
                else
                {
                    byte[] byts = new byte[count];
                    Array.Copy(buffer, offset, byts, 0, count);
                    this._udpClient.Send(byts, count);
                }
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("向通道【{0}】时发送数据时发生异常", this._address), e);
            }
            finally
            {
                _bSending = false;
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
            while (_bReceiving)
            {
                Thread.Sleep(1);
            }
            _bReceiving = true;
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] rbyts = this._udpClient.Receive(ref RemoteIpEndPoint);
                Array.Copy(rbyts, 0, buffer, offset, rbyts.Length);
                return rbyts.Length;
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("从通道【{0}】时接收数据时发生异常", this._address), e);
            }
            finally
            {
                _bReceiving = false;
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
            while (_bSending)
            {
                Thread.Sleep(1);
            }
            _bSending = true;
            try
            {
                if ((offset == 0) && (size == buffer.Length))
                {
                    return this._udpClient.BeginSend(buffer, size, callback, state);
                }
                else
                {
                    byte[] byts = new byte[size];
                    Array.Copy(buffer, offset, byts, 0, size);
                    return this._udpClient.BeginSend(byts, size, callback, state);
                }

            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("从通道【{0}】时开始异步发送数据时发生异常", this._address), e);
            }
            finally
            {
                _bSending = false;
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
                this._udpClient.EndSend(asyncResult);
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("从通道【{0}】时结束异步发送数据时发生异常", this._address), e);
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
            while (_bReceiving)
            {
                Thread.Sleep(1);
            }
            _bReceiving = true;
            try
            {
                return this._udpClient.BeginReceive(callback, new object[] { buffer, offset, size });
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("从通道【{0}】时开始异步接收数据时发生异常", this._address), e);
            }
            finally
            {
                _bReceiving = false;
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
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                object[] asyObj = asyncResult.AsyncState as object[];

                byte[] rbyts = this._udpClient.EndReceive(asyncResult, ref remoteIpEndPoint);
                Array.Copy(rbyts, 0, asyObj[0] as byte[], (int)(asyObj[1]), rbyts.Length);
                return rbyts.Length;
            }
            catch (Exception e)
            {
                SetState(ChannelState.Faulted);
                throw new Exception(string.Format("从通道【{0}】时结束异步接收数据时发生异常", this._address), e);
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
                return this._state;
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
            this._state = state;
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
        #endregion
    }
}
