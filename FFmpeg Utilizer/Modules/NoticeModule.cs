#pragma warning disable IDE0044 // Stop nagging about main readonly. Not possible.

using System.Collections.Generic;
using System.Drawing;
using System.Media;

namespace FFmpeg_Utilizer.Modules
{
    public class NoticeModule
    {
        private Main main;

        public enum TypeNotice
        {
            Warning = 0,
            Info = 2,
            Error = 4,
            Success = 6,
        }

        private readonly List<Color> colors = new List<Color>() {
            Color.FromArgb(230, 165, 109),
            Color.FromArgb(255, 184, 120),
            Color.FromArgb(15, 188, 249),
            Color.FromArgb(75, 207, 250),
            Color.FromArgb(235, 59, 90),
            Color.FromArgb(252, 92, 101),
            Color.FromArgb(32, 191, 107),
            Color.FromArgb(38, 222, 129) };

        public NoticeModule(Main main) => this.main = main;

        public void SetNotice(string noticeText, TypeNotice nType)
        {
            main.PixelTopNotice.BackColor = colors[(int)nType];
            main.NoticePanel.BackColor = colors[(int)nType + 1];
            main.NoticeTextLabel.BackColor = colors[(int)nType + 1];
            main.NoticeCloseButton.BackColor = colors[(int)nType + 1];

            main.NoticeTextLabel.Text = noticeText;
            main.NoticePanel.Enabled = true;
            main.NoticePanel.Visible = true;

            switch (nType)
            {
                case TypeNotice.Warning:
                    SystemSounds.Hand.Play();
                    break;

                case TypeNotice.Error:
                    SystemSounds.Hand.Play();
                    break;
            }
        }

        public void CloseNotice()
        {
            main.NoticePanel.Visible = false;
            main.NoticePanel.Enabled = false;
        }
    }
}