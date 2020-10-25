using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Data
{
    public class Tab
    {
        public Panel tab;
        public Panel indicator;
        public Label text;
        public Panel mainPanel;

        public Tab(Panel tab, Panel indicator, Label text, Panel mainPanel)
        {
            this.tab = tab;
            this.indicator = indicator;
            this.text = text;
            this.mainPanel = mainPanel;
        }
    }
}
