using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TunnelFireControl.DeviceChannel;

namespace TunnelFireControl.PlcDriver
{
    public class LuanbeiSiemensDriver : IPlcDriver
    {
        IDeviceChannel channel;
        ILog log;
        private int plcNode = 10;
        private Thread thread;

        private int startAddr = 0;
        private int length = 12;

        private Dictionary<int, int> dictItemWord = new Dictionary<int, int>();
        private Dictionary<int, byte> dictItemByte = new Dictionary<int, byte>();
        public LuanbeiSiemensDriver()
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
                //if (ss.Length >= 5)
                //{
                //    startAddr = int.Parse(ss[3]);
                //    length = int.Parse(ss[4]);
                //}
                string address = string.Format("{0}:{1}:{2}", contype, ip, port);
                channel = DeviceChannelFactory.Instance.Create(address);
                channel.Open();
                thread = new Thread(new ThreadStart(GatherDataHandle));
                thread.Start();
            }
            catch (Exception ex)
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
            //SortData();
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
            byte[] crc = Modbus_CRC(buff, 0, 6);
            buff[6] = crc[0];
            buff[7] = crc[1];
            if (channel != null)
            {
                channel.Send(buff, 0, buff.Length);
                System.Threading.Thread.Sleep(200);
                byte[] rcvbuff = new byte[1024];
                channel.Receive(rcvbuff, 0, rcvbuff.Length);
            }
        }

        private void SortData()
        {
            byte[] dscLcBts = Read(100, 4);
            byte[] dscYwBts = Read(104, 4);
            byte[] gscLcBts = Read(108, 4);
            byte[] gscYwBts = Read(112, 4);
            byte[]ylLcBts = Read(116, 4);
            byte[] ylValBts = Read(120, 4);

            byte[] pump1Bts = Read(140, 4);
            byte[] pump2Bts = Read(144, 4);

            int dscLc = (int)(GetFloat(dscLcBts[0], dscLcBts[1], dscLcBts[2], dscLcBts[3]) * 100);
            int dscYW = (int)(GetFloat(dscYwBts[0], dscYwBts[1], dscYwBts[2], dscYwBts[3]) * 100);
            int gscLc = (int)(GetFloat(gscLcBts[0], gscLcBts[1], gscLcBts[2], gscLcBts[3]) * 100);
            int gscYW = (int)(GetFloat(gscYwBts[0], gscYwBts[1], gscYwBts[2], gscYwBts[3]) * 100);
            int ylLc = (int)(GetFloat(ylLcBts[0], ylLcBts[1], ylLcBts[2], ylLcBts[3]) * 100);
            int ylVal = (int)(GetFloat(ylValBts[0], ylValBts[1], ylValBts[2], ylValBts[3]) * 100);

            int pump1 = pump1Bts[0] * 256 + pump1Bts[1];
            int pump2 = pump2Bts[0] * 256 + pump2Bts[1];

            SaveToDicState(100, dscLc);
            SaveToDicState(104, dscYW);
            SaveToDicState(108, gscLc);
            SaveToDicState(112, gscYW);
            SaveToDicState(116, ylLc);
            SaveToDicState(120, ylVal);
            SaveToDicState(140, pump1);
            SaveToDicState(144, pump2);
        }

        private void SaveToDicState(int add,int state)
        {
            if (dictItemWord.ContainsKey(add))
            {
                dictItemWord[add] = state;
            }
            else
            {
                dictItemWord.Add(add, state);
            }
        }
        public float GetFloat(ushort P1, ushort P2)
        {
            int intSign, intSignRest, intExponent, intExponentRest;
            float faResult, faDigit;
            intSign = P1 / 32768;
            intSignRest = P1 % 32768;
            intExponent = intSignRest / 128;
            intExponentRest = intSignRest % 128;
            faDigit = (float)(intExponentRest * 65536 + P2) / 8388608;
            faResult = (float)Math.Pow(-1, intSign) * (float)Math.Pow(2, intExponent - 127) * (faDigit + 1);
            return faResult;
        }

        public float GetFloat(byte b1, byte b2, byte b3, byte b4)
        {
            float faResult = 0.0f;
            ushort P1 = (ushort)(b1 * 256 + b2);
            ushort P2 = (ushort)(b3 * 256 + b4);
            faResult = GetFloat(P1, P2);
            return faResult;
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
            byte[] buff = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x0A, 0x03, 0x00, 0x00, 0x00, 0x0C };
            //buff[0] = (byte)plcNode;
            //buff[1] = 0x03;
            //buff[2] = (byte)(startAddr / 256);
            //buff[3] = (byte)(startAddr % 256);
            //buff[4] = (byte)(length / 256);
            //buff[5] = (byte)(length % 256);
            //byte[] crc = Modbus_CRC(buff, 0, 6);
            //buff[6] = crc[0];
            //buff[7] = crc[1];
            if (channel != null)
            {
                channel.Send(buff, 0, buff.Length);
                System.Threading.Thread.Sleep(200);
                byte[] rcvbuff = new byte[1024];
                int len = channel.Receive(rcvbuff, 0, rcvbuff.Length);
                if (len > 0)
                {
                    UnPacker(rcvbuff, 0, len);
                }
            }

        }


        private void UnPacker(byte[] buff, int offfset, int len)
        {
            byte[] rcvBuff = new byte[len];
            if (buff.Length < len || len < 3 + 6) return;
            Array.Copy(buff, offfset, rcvBuff, 0, len);
            int dataLen = rcvBuff[8];
            if (len < dataLen + 9) return;
            int add1 = startAddr;
            int add2 = startAddr;
            byte val1 = 0;
            int val2 = 0;
            for (int i = 0; i < dataLen; i++)
            {
                //val1 = rcvBuff[i + 3];
                //dictItemByte.Add(add1, val1);
                //add1++;
                if (i % 2 == 0)
                {
                    val2 = rcvBuff[i + 3 + 6] * 256 + rcvBuff[i + 4 + 6];
                    if (dictItemWord.ContainsKey(add2))
                    {
                        dictItemWord[add2] = val2;
                    }
                    else
                    {
                        dictItemWord.Add(add2, val2);
                    }
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
        protected byte[] Get_CRC(byte[] b, int offset, int len)
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
