using System.Windows.Forms;

namespace FFmpeg_Utilizer.Data
{
    public class Tab
    {
        public Panel tab;
        public Panel indicator;
        public Label text;
        public Panel mainPanel;
        public bool active = true;

        public Tab(Panel tab, Panel indicator, Label text, Panel mainPanel, bool active = true)
        {
            this.tab = tab;
            this.indicator = indicator;
            this.text = text;
            this.mainPanel = mainPanel;
            this.active = active;
        }
    }
}