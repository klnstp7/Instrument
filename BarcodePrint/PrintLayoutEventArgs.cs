using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BarcodePrint.Models;

namespace BarcodePrint
{
    public class PrintLayoutEventArgs : EventArgs
    {
        public Constants.BarCodeLayout Layout { get; set; }
    }
}
