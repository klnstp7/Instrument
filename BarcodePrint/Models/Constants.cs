using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarcodePrint.Models
{
    public class Constants
    {
        public enum InstrumentForm
        {
            仪器 = 0,
            固定资产 = 1
        }
        /// <summary>
        /// 条码二维码打印布局
        /// </summary>
        public enum BarCodeLayout
        {
            条码上 = 0,
            条码下 = 1,
            二维码左 = 2,
            二维码右 = 3,
            混合条码上 = 4,
            混合条码下 = 5
        }
    }
}
