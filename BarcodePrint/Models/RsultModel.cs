using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarcodePrint.Models
{
    public class RsultModel
    {
        public string Msg { get; set; }
        public IList<InstrumentModel> Data { get; set; }
    }
}
