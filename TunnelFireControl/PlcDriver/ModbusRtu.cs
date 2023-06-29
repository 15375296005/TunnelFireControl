using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TunnelFireControl.DeviceChannel;
using Common.Logging;
using System.Threading;

namespace TunnelFireControl.PlcDriver
{
    public class ModbusRtu : IPlcDriver
    {
        IDeviceChannel channel;
        ILog log;
        private int plcNode = 1;
        private Thread thread;

        private int startAddr = 0;
        private int length = 128;

        private Dictionary<int, int> dictItemWord = new Dictionary<int, int>();
        private Dictionary<int, byte> dictItemByte = new Dictionary<int, byte>();
        public ModbusRtu()
        {
            try
            {
                log = LogManager.GetLogger("ModbusRtu");
            }
            finally { }
        }

        public void Init(string communiction)
        {
            try
            {
                string contype = "";
                string ip = "";
                string port = "";
                string[] ss = communiction.Split(':');
                if (ss.Length < 3) return;
                contype = ss[0];
                ip = ss[1];
                port = ss[2];
                if (ss.Length >= 5)
                {
                    startAddr = int.Parse(ss[3]);
                    length = int.Parse(ss[4]);
                }
                string address = string.Format("{0}:{1}:{2}", contype, ip, port);
                channel = DeviceChannelFactory.Instance.Create(address);
                channel.Open();
                thread = new Thread(new ThreadStart(GatherDataHandle));
                thread.Start();
            }
            catch(Exception ex)
            {
                if (log != null)
                {
                    log.Error(ex);
                }
            }
        }

        public void UnInit()
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                    thread = null;
                }
                if (channel != null)
                {
                    channel.Close();
                    channel = null;
                }
            }
            catch (Exception ex)
            {
                if (log != null)
                {
                    log.Error(ex);
                }
            }
        }

        public void SetPlcNode(int plcNode)
        {
            this.plcNode = plcNode;
        }

        public void InitItem(string item)
        {
            throw new NotImplementedException();
        }

        public int Read(int addr)
        {
            int val;
            dictItemWord.TryGetValue(addr, out val);
            return val;
        }

        public byte[] Read(int addr, int length)
        {
            byte[] buff = new byte[length];
            byte val1 = 0;
            byte val2 = 0;
            byte val3 = 0;
            byte val4 = 0;
            dictItemByte.TryGetValue(addr++, out val1);
            dictItemByte.TryGetValue(addr++, out val2);
            dictItemByte.TryGetValue(addr++, out val3);
            dictItemByte.TryGetValue(addr++, out val4);
            buff[0] = val1;
            buff[1] = val2;
            buff[2] = val3;
            buff[3] = val4;
            return buff;
        }

        public void Write(int addr, int value)
        {
            byte[] buff = new byte[8];
            buff[0] = (byte)plcNode;
            buff[1] = 0x06;
            buff[2] = (byte)(addr / 256);
            buff[3] = (byte)(addr % 256);
            buff[4] = (byte)(value / 256);
            buff[5] = (byte)(value % 256);
            byte[] crc = GetCRC(buff, 0, 6);
            buff[6] = crc[0];
            buff[7] = crc[1];
            if (channel != null)
            {
                channel.Send(buff, 0, buff.Length);
                System.Threading.Thread.Sleep(200);
                byte[] rcvbuff = new byte[1024];
                channel.Receive(rcvbuff,0,rcvbuff.Length);
            }
        }

        private void GatherDataHandle()
        {
            while (true)
            {
                try
                {
                    GatherOnce();
                }
                catch { }
                Thread.Sleep(2000);
            }
        }

        private void GatherOnce()
        {
            byte[] buff = new byte[8];
            buff[0] = (byte)plcNode;
            buff[1] = 0x03;
            buff[2] = (byte)(startAddr / 256);
            buff[3] = (byte)(startAddr % 256);
            buff[4] = (byte)(length / 256);
            buff[5] = (byte)(length % 256);
            byte[] crc = GetCRC(buff, 0, 6);
            buff[6] = crc[0];
            buff[7] = crc[1];
            if (channel != null)
            {
                channel.Send(buff, 0, buff.Length);
                System.Threading.Thread.Sleep(200);
                byte[] rcvbuff = new byte[1024];
                int len = channel.Receive(rcvbuff, 0, rcvbuff.Length);
                UnPacker(rcvbuff, 0, len);
            }

        }


        private void UnPacker(byte[] buff, int offfset,int len)
        {
            byte[] rcvBuff = new byte[len];
            if (buff.Length < len || len < 3) return;
            Array.Copy(buff, offfset, rcvBuff, 0, len);
            int dataLen = rcvBuff[2];
            if (len < dataLen + 3) return;
            int add1 = startAddr;
            int add2 = startAddr;
            byte val1 = 0;
            int val2 = 0;
            for (int i = 0; i < dataLen; i++)
            {
                val1 = rcvBuff[i + 3];
                dictItemByte.Add(add1, val1);
                add1++;
                if (i % 2 == 0)
                {
                    val2 = rcvBuff[i + 3] * 256 + rcvBuff[i + 4];
                    dictItemWord.Add(add2, val2);
                    add2++;
                }
            }
        }

        private byte[] Modbus_CRC(byte[] b, int offset, int len)
        {
            byte CRC16Lo = 0;
            byte CRC16Hi = 0;
            byte bytC;
            ushort CRC_Cal = 0xFFFF; ;
            for (int j = 0; j < len; j++)
            {
                bytC = b[j + offset];
                CRC_Cal = (ushort)(CRC_Cal ^ bytC);
                for (int i = 0; i < 8; i++)
                {
                    if ((CRC_Cal & 0x0001) == 0x0001)
                    {
                        CRC_Cal = (ushort)(CRC_Cal >> 1);
                        CRC_Cal = (ushort)(CRC_Cal ^ 0xA001);
                    }
                    else
                    {
                        CRC_Cal = (ushort)(CRC_Cal >> 1);
                    }
                }
            }
            CRC16Lo = (byte)(CRC_Cal >> 8);
            CRC16Hi = (byte)(CRC_Cal & 0x00FF);
            byte[] ReturnData = new byte[2];
            ReturnData[0] = CRC16Hi;
            ReturnData[1] = CRC16Lo;
            return ReturnData;
        }

        /// <summary>
        /// 获取CRC校验
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected byte[] GetCRC(byte[] b, int offset, int len)
        {
            byte CRC16Lo = 0;
            byte CRC16Hi = 0;
            byte bytC;
            byte bytTreat;
            byte bytBcrc;
            for (int i = 0; i < len; i++)
            {
                bytC = b[i + offset];
                for (int flag = 0; flag <= 7; flag++)
                {
                    bytTreat = (byte)(bytC & 0x80);
                    bytC = (byte)((bytC * 2) % 0x100);
                    bytBcrc = (byte)(CRC16Hi & 0x80);
                    CRC16Hi = (byte)(((CRC16Hi * 2) % 0x100) + CRC16Lo / 0x80);
                    CRC16Lo = (byte)((CRC16Lo * 2) % 0x100);
                    if (bytTreat != bytBcrc)
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ 0x10);
                        CRC16Lo = (byte)(CRC16Lo ^ 0x21);
                    }
                }
            }
            byte[] ReturnData = new byte[2];
            ReturnData[0] = CRC16Hi;
            ReturnData[1] = CRC16Lo;
            return ReturnData;
        }
    }
}
