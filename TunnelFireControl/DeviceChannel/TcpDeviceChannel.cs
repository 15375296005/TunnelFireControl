#region License

/**
 * Copyright (c) 2010,安徽皖通科技股份有限公司 All rights reserved.
 */

#endregion

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// Tcp设备通道。
    /// </summary>
    public class TcpDeviceChannel : DeviceChannelBase, IDeviceChannel
    {
        #region Fields

        private TcpClient _tcpClient = null;
        private IPAddress ipAddress;
        private int port;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// 使用通道地址初始化类 <see cref="TcpDeviceChannel"/> 的新实例。
        /// </summary>
        /// <param name="address">通道地址。</param>
        /// <remarks>地址格式:2:IPAddress:Port。</remarks>
        public TcpDeviceChannel(string address)
            : base(address)
        {
            String[] props = Address.Split(':');
            if (props.Length != 3)
            {
                throw new Exception("Tcp设备通道参数配置不正确");
            }
            ipAddress = IPAddress.Parse(props[1].Trim());
            port = Int32.Parse(props[2].Trim());
        }

        #endregion

        #region DeviceChannelBase Abstract Methods

        /// <summary>
        /// 内部打开通道。
        /// </summary>
        protected override void InternalOpen()
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(ipAddress, port);
            }
        }

        /// <summary>
        /// 内部关闭通道。
        /// </summary>
        protected override void InternalClose()
        {
            if (_tcpClient == null)
            {
                return;
            }
            if (_tcpClient.Connected)
            {
                _tcpClient.GetStream().Close();
            }
            _tcpClient.Close();
            _tcpClient = null;
        }

        /// <summary>
        /// 获得通道内部流对象。
        /// </summary>
        /// <returns>通道内部流对象。</returns>
        protected override Stream GetInternalStream()
        {
            try
            {
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(ipAddress, port);
                    //throw new Exception("TcpClient实例为空");
                }
                else if (!_tcpClient.Connected)
                {
                    _tcpClient.Close();
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(ipAddress, port);
                }
                return _tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                throw new Exception("获得内部流对象错误", ex);
            }
        }

        #endregion
    }
}
