using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Derby
{
    internal class Player
    {
        Texture2D _horseTex, _pixel;
        Song _horseGalopp;
        SoundEffect _jumpSFX;
        int _ground;
        public float _speed, _distance, _maxSpeed, _minSpeed, _speedDecay, _gravity, _Ylevel, _upSpeed, _accelaration, _slowedMaxSpeed;
        Vector2 _centre, _pos;
        public Rectangle _hitBox;
        public float distance;
         
        bool _isAirborne;
        public bool _isPlayerSlowed, _isPlayerFinished;


        public Player(Texture2D horseTex, Texture2D pixel, int Ylevel, Song horseGalopp, SoundEffect jump)
        {
            _horseTex = horseTex;
            _horseGalopp = horseGalopp;
            _jumpSFX = jump;
            _pixel = pixel;
            _ground = Ylevel - 110;
            _Ylevel = Ylevel;
            _pos = new Vector2(0, _ground);
            _centre = new Vector2(_pos.X + (_horseTex.Width / 2), _pos.Y + (_horseTex.Height / 2));
            _maxSpeed = -11f;
            _minSpeed = -0f;
            _slowedMaxSpeed = -8f;
            _speedDecay = 0.96f;
            _hitBox = new Rectangle((int)_pos.X + 65, (int)_Ylevel, _horseTex.Width - 90, _horseTex.Height);
            _accelaration = 1.07f;
            _gravity = 0.25f;
        }



        public void UpdateMe(KeyboardState currKey, bool isSpacePressed, bool isEnterPressed, MouseState currMouse, bool isLeftMousePressed, bool isRightPressed)
        {
            if (_isPlayerFinished == false)
            {
                if ((isSpacePressed == false && currKey.IsKeyDown(Keys.Space) == true) || (isLeftMousePressed == false && currMouse.LeftButton == ButtonState.Pressed))
                {
                    _speed += -3;
                }

                if (((isEnterPressed == false && currKey.IsKeyDown(Keys.LeftAlt) == true) || (isRightPressed == false && currMouse.RightButton == ButtonState.Pressed)) && _isAirborne == false)
                {
                    _upSpeed = -7;
                    _isAirborne = true;
                    _jumpSFX.Play();
                }
            }
            


            distance += _speed;
            _Ylevel += _upSpeed;



            if (_isPlayerSlowed == false)
                if (_speed < _maxSpeed)
                    _speed = _maxSpeed;
            if (_isPlayerSlowed == true)
                if (_speed < _slowedMaxSpeed)
                    _speed = _slowedMaxSpeed;
            if (_speed > 0)
                _speed = 0;


            if (_Ylevel > _ground)    
            {
                _Ylevel = _ground;
                _isAirborne = false;    
            }


            if (MediaPlayer.State == MediaState.Stopped && _speed < -2)
            {
                MediaPlayer.Play(_horseGalopp);
            }
            if (_speed > -2)
            {
                MediaPlayer.Stop();
            }



            _speed *= _speedDecay;
            _upSpeed += _gravity;



            _hitBox = new Rectangle((int)_pos.X + 65, (int)_Ylevel, _horseTex.Width - 90, _horseTex.Height);
        }

        public void DrawMe(SpriteBatch SB)
        {
            SB.Draw(_horseTex, new Vector2(0, _Ylevel), Color.White);
            SB.Draw(_pixel, _hitBox, Color.Blue * 0f);
        }

        public void DrawStick(SpriteBatch SB)
        {
            SB.Draw(_pixel, new Rectangle((int)_centre.X - 2, (int)_Ylevel + (_horseTex.Height / 2), 4, 500), Color.BurlyWood);
        }
    }
}
