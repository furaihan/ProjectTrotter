using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Rage;

namespace BarbarianCall.Types
{
    public class PopupChoice
    {
        public List<string> Choices { get; set; } = new List<string>();
        public Rectangle MainRectangle { get; private set; }
        public Rectangle Border { get; private set; }
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color TextColor { get; set; } = Color.White;
        public byte Opacity { get; set; } = 255;
        public byte LineHeight { get; set; } = 2;
        public string Title { get; set; }
        public bool Displayed { get; private set; }
        public Size GameResolution => Game.Resolution;
        public PopupChoice(IEnumerable<string> choices, string title)
        {
            Choices = choices.ToList();
            Title = title;
        }
        public void Show()
        {
            MainRectangle = new Rectangle(GameResolution.Width / 4, GameResolution.Height / 7, 750, 200);
            Border = new Rectangle(GameResolution.Width / 4 - 5, GameResolution.Height / 7 - 5, 760, 210);
            Game.RawFrameRender += Draw;
        }
        public void Delete()
        {
            Game.RawFrameRender -= Draw;
        }
        public string GetResult()
        {
            string ret = string.Empty;
            Keys[] keys = { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6 };
            while (Displayed)
            {
                GameFiber.Yield();
                if (Game.IsKeyDown(Keys.D1)) ret = Choices[0];
                if (Game.IsKeyDown(Keys.D2)) ret = Choices[1];
                if (Game.IsKeyDown(Keys.D3)) ret = Choices[2];
                if (Game.IsKeyDown(Keys.D4)) ret = Choices[3];
                if (Game.IsKeyDown(Keys.D5)) ret = Choices[4];
                if (Game.IsKeyDown(Keys.D6)) ret = Choices[5];
                if (keys.Any(Game.IsKeyDown))
                {
                    Delete();
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Must be called in ticks
        /// </summary>
        public void Draw(object sender, GraphicsEventArgs e)
        {
            if (Displayed)
            {
                e.Graphics.DrawRectangle(Border, Color.FromArgb(90, BackgroundColor));
                e.Graphics.DrawRectangle(MainRectangle, Color.FromArgb(Opacity, BackgroundColor));
                e.Graphics.DrawText(Title, "Aharoni Bold", 18f, new PointF(Border.X + 5, Border.Y + 5), TextColor);
                int i = 0;
                foreach (var choice in Choices)
                {
                    i++;
                    e.Graphics.DrawText($"{i}. {choice}", "Verdana Bold", 15f, new PointF(MainRectangle.X, MainRectangle.Y + 35), TextColor);
                }
            }
        }
    }
}
