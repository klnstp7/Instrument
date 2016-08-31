using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Upgrade
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public int ProgressValue 
        {
            get { return this.prcBar.Value; }
            set { prcBar.Value = value; }
        }
    }
}
