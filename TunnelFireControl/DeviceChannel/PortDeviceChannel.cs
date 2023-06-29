#region License

/**
 * Copyright (c) 2010,安徽皖通科技股份有限公司 All rights reserved.
 */

#endregion

using System;
using System.IO;
using System.IO.Ports;

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// 串口设备通道。
    /// </summary>
    public class PortDeviceChannel : DeviceChannelBase , IDeviceChannel
    {
        #region Fields

        private SerialPort _serialPort = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// 使用通道地址初始化类 <see cref="PortDeviceChannel"/> 的新实例
        /// </summary>
        /// <param name="address">通道地址。</param>
        /// <para>地址格式： 串口设备： Port = COM1-N [; BaudRate = N ; Parity = Even/Mark/None/Odd/Space; DataBits = N; StopBits = None/One/OnePointFive/Two ]</para>     
        public PortDeviceChannel(string address)
            :base(address)
        { 
        }

        #endregion

        #region DeviceChannelBase Abstract Methods

        /// <summary>
        /// 内部打开通道。
        /// </summary>
        protected override void InternalOpen()
        {
            String portName = "COM1";
            int baudRate = 9600;
            Parity parity = Parity.None;
            int dataBits = 8;
            StopBits stopBits = StopBits.One;
            try
            {
                String[] props = Address.Split(':')[1].Split(';');
                foreach (String prop in props)
                {
                    if (prop.Contains("="))
                    {
                        String[] keyvalue = prop.Split('=');
                        if (keyvalue.Length != 2)
                        {
                            throw new Exception("串口设备通道参数配置不正确");
                        }

                        String key = keyvalue[0].Trim();
                        String value = keyvalue[1].Trim();
                        if (String.Compare(key, "Port", true) == 0)
                        {
                            portName = value;
                        }
                        else if (String.Compare(key, "BaudRate", true) == 0)
                        {
                            baudRate = Int32.Parse(value);
                        }
                        else if (String.Compare(key, "Parity", true) == 0)
                        {
                            parity = (Parity)Enum.Parse(typeof(Parity), value);
                        }
                        else if (String.Compare(key, "DataBits", true) == 0)
                        {
                            dataBits = Int32.Parse(value);
                        }
                        else if (String.Compare(key, "StopBits", true) == 0)
                        {
                            stopBits = (StopBits)Enum.Parse(typeof(StopBits), value);
                        }
                        else
                            throw new ArgumentException("ConfigFormatError", "串口设备通道参数");
                    }
                }
            }
            catch (ArgumentException ae)
            {                                
                throw ae;
            }
            catch (Exception e)
            {               
                throw new ArgumentException("设置时发生异常", "串口设备通道参数", e);
            }
            _serialPort = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
            _serialPort.Open();
        }

        /// <summary>
        /// 内部关闭通道。
        /// </summary>
        protected override void InternalClose()
        {
            if ((_serialPort != null) && (_serialPort.IsOpen))
            {
                _serialPort.BaseStream.Close();
                _serialPort.Close();
            }
        }

        /// <summary>
        /// 获得通道内部流对象。
        /// </summary>
        /// <returns></returns>
        protected override Stream GetInternalStream()
        {
            try
            {
                return _serialPort.BaseStream;
            }
            catch(Exception ex)
            {
                throw new Exception ("获得内部流对象错误",ex) ;
            }
        }

        #endregion
    }
}
