using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Derby
{
    internal class Transition
    {
        Texture2D _tex;
        public float _transparentsy;
        bool _goingUp;
        public Transition(Texture2D tex)
        {
            _tex = tex;
            _transparentsy = 0.005f;
            _goingUp = true;
        }
        public void UpdateMe(GameTime gt)
        {
            if (_goingUp == true)
                _transparentsy += (float)gt.ElapsedGameTime.TotalSeconds * 3;
            else if (_goingUp == false)
                _transparentsy -= (float)gt.ElapsedGameTime.TotalSeconds * 2;
            if (_transparentsy >= 2)
                _goingUp = false;
        }
        public void DrawMe(SpriteBatch sb)
        {
            sb.Draw(_tex, new Rectangle(0, 0, 1920, 1080), Color.Black * _transparentsy);
        }
    }
}
