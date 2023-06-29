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

namespace TunnelFireControl.DeviceChannel
{
    /// <summary>
    /// 通道状态。
    /// </summary>
    public enum ChannelState
    {
        /// <summary>
        /// 已经创建，还没有打开。
        /// </summary>
        Created,

        /// <summary>
        /// 正在打开。
        /// </summary>
        Opening,

        /// <summary>
        /// 已经打开。
        /// </summary>
        Opened,

        /// <summary>
        /// 正在关闭。
        /// </summary>
        Closing,

        /// <summary>
        /// 已经关闭。
        /// </summary>
        Closed,

        /// <summary>
        /// 异常。
        /// </summary>
        Faulted
    }
}
