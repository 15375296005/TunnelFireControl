#region Notice
/**
 * 
 * Copyright (c) 2008,安徽皖通科技股份有限公司 All rights reserved.
 * 
 * History:
 *			-- 2008-03-10 创建文件 by hjcao
 *  
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// 设备通道工厂实现。
    /// </summary>
    public class DeviceChannelFactory : IDeviceChannelFactory
    {
        #region Fields

        private static readonly DeviceChannelFactory _instance;

        #endregion

        #region Constructor(s)

        static DeviceChannelFactory()
        {
            _instance = new DeviceChannelFactory();
        }

        private DeviceChannelFactory()
        { 
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获得设备通道工厂实例。
        /// </summary>
        public static IDeviceChannelFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region IDeviceChannelFactory Members

        /// <summary>
        /// 根据地址创建设备通道。
        /// </summary>
        /// <param name="address">设备地址。</param>
        /// <para>1、全双工串口设备： 1:Port://COM1-N[: BaudRate = N : Parity = Even/Mark/None/Odd/Space: DataBits = N: StopBits = None/One/OnePointFive/Two ]；</para>
        /// <para>2、全双工通过Tcp方式的网络设备：2:Tcp://IPAddress:Port；</para>
        /// <para>3、半双工串口设备： 3:Port://COM1-N[: BaudRate = N : Parity = Even/Mark/None/Odd/Space: DataBits = N: StopBits = None/One/OnePointFive/Two ]；</para>
        /// <para>4、半双工通过Tcp方式的网络设备：4:Tcp://IPAddress:Port；</para>
        /// <para>5、AutoScope车辆检测器设备：5:CS2通讯服务登陆（服务器IP|连接用户名|连接密码|采集时间秒数）:站检测器信息（检测器IP地址&端口|车道1流量站检测器&车道2流量站检测器&...|车道1速度站检测器&车道2速度站检测器&...）:cs2相关配置参数（是否需要重启cs2服务[0:不需要,1:需要]|cs2端口|发送重启命令的最大次数）；</para>
        /// <returns>设备通道实例。</returns>
        public IDeviceChannel Create(string address)
        {
            IDeviceChannel deviceChannel = null;
            if (address == null || address == "")
            {
                return deviceChannel;
            }
            int type = int.Parse(address.Substring(0, address.IndexOf(':')));
            switch (type)
            {
                case 1:                    
                    deviceChannel = new PortDeviceChannel(address);
                    break;
                case 2:
                    deviceChannel = new TcpDeviceChannel(address);
                    break;
                case 3:
                    deviceChannel = new UdpDeviceChannel(address);
                    break;
            }
            return deviceChannel;
        }

        #endregion

    }
}
