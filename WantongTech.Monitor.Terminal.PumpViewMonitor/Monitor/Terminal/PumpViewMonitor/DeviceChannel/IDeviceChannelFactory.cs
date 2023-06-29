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
    /// 设备通道工厂接口。
    /// </summary>
    public interface IDeviceChannelFactory
    {
        /// <summary>
        /// 根据地址创建设备通道。
        /// </summary>
        /// <param name="address">设备地址。</param>
        /// <returns>设备通道实例。</returns>
        IDeviceChannel Create(string address);
    }
}
