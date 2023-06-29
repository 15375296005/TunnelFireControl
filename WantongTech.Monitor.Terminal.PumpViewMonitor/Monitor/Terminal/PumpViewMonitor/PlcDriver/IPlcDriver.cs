using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TunnelFireControl
{
    public interface IPlcDriver
    {
        void SetPlcNode(int plcNode);
        void Init(string communiction);

        void InitItem(string item);
        void Write(int addr, int value);

        int Read(int addr);

        byte[] Read(int addr, int length);

        void UnInit();
    }
}
