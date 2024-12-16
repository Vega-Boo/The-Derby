using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Derby
{
    internal class FinishLine
    {
        Vector2 _pos;
        Texture2D _tex, _pixel;
        public Rectangle _hitBox;
        float _playerDistance;


        public FinishLine(Texture2D tex, Vector2 pos, Texture2D pixel)
        {
            _tex = tex;
            _pixel = pixel;
            _pos = pos;
            _hitBox = new Rectangle((int)_pos.X, (int)_pos.Y + (_tex.Height - (_tex.Height / 7)), _tex.Width, _tex.Height);
        }

        public void UpdateMe(float playerDistance)
        {
            _playerDistance = playerDistance;
            _hitBox = new Rectangle(((int)_pos.X + (int)_playerDistance), (int)_pos.Y + _tex.Height - 3, _tex.Width - 63, _tex.Height + 400);
        }

        public void DrawMe(SpriteBatch SB)
        {
            SB.Draw(_pixel, _hitBox, Color.White * 1f);
            SB.Draw(_tex, new Vector2(_pos.X + _playerDistance, _pos.Y), Color.White);
        }
    }
}
