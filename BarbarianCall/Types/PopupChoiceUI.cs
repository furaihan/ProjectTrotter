using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace BarbarianCall.Types
{
    public class PopupChoiceUI
    {
        public List<string> Choices { get; set; } = new List<string>();
        public List<ResText> ChoicesUI { get; private set; } = new List<ResText>();
        public ResRectangle Background { get; private set; }
        public Color BackgroundColor { get; set; } = Color.Black;
        public Size BackgroundSize { get; set; } = new Size(480, 110);
        public Color TextColor { get; set; } = Color.White;
        public byte Opacity { get; set; } = 255;
        public int LineHeight { get; set; } = 8;
        public string Title { get; set; }
        public ResText TitleUI { get; private set; }
        public PopupChoiceUI(List<string> choices, string title, bool shuffle = false)
        {
            if (shuffle)
            {
                choices.ShuffleSecure();
            }
            Choices = choices;
            Title = title;         
        }               
        public void Process()
        {
            Background = new(new Point(10, 20), BackgroundSize, Color.FromArgb(Opacity, BackgroundColor));
            TitleUI = new(Title, new Point(20, LineHeight), 0.30f, TextColor, Common.EFont.ChaletLondon, ResText.Alignment.Left);
            TitleUI.Outline = true;
            int initialHeight = LineHeight;
            LineHeight += initialHeight + (initialHeight / 4);
            var i = 0;
            foreach (string c in Choices)
            {
                i++;
                ResText rt = new($"{i}. {c}", new Point(25, LineHeight), 0.25f, TextColor, Common.EFont.ChaletLondon, ResText.Alignment.Left);
                rt.Outline = true;
                LineHeight += initialHeight;
                ChoicesUI.Add(rt);
            }
        }
        public void Draw()
        {         
            Background.Draw();
            TitleUI.Draw();
            ChoicesUI.ForEach(rt => rt.Draw());
        }       
    }
}
