using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Derby
{
    internal class Jump
    {
        Vector2 _pos;
        Texture2D _tex, _pixel;
        public Rectangle _hitBox, _jumpPoint;
        float _playerDistance;


        public Jump(Texture2D tex, Vector2 pos, Texture2D pixel)
        {
            _tex = tex;
            _pixel = pixel;
            _pos = pos;
            _hitBox = new Rectangle((int)_pos.X, (int)_pos.Y + (_tex.Height - (_tex.Height / 7)), _tex.Width, _tex.Height);
        }

        public void UpdateMe(float playerDistance)
        {
            _playerDistance = playerDistance;      
            _hitBox = new Rectangle(((int)_pos.X + (int)_playerDistance) + 35, 300, _tex.Width - 110 , _tex.Height);

            _jumpPoint = new Rectangle(((int)_pos.X + (int)_playerDistance) - 125, 0, _tex.Width - 80, _tex.Height + 300);

        }

        public void DrawMe(SpriteBatch SB)
        {
            SB.Draw(_tex, new Vector2(_pos.X + _playerDistance, _pos.Y), Color.White);
            SB.Draw(_pixel, _hitBox, Color.Green * 0f);
            SB.Draw(_pixel, _jumpPoint, Color.Orange * 0f);
        }
    }
}
