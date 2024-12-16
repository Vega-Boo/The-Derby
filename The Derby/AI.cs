using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Derby
{
    internal class AI
    {
        static readonly Random RNG = new Random();
        Texture2D _horseTex, _pixel;
        Song _horseGalopp;
        int _ground;
        float _speed, _distance, _maxSpeed, _minSpeed, _speedDecay, _playerDistance, _accelaration, _targetSpeed, _float,_slowedMaxSpeed, _gravity, _Ylevel, _upSpeed, _timeOfSpeedChange;
        Vector2 _centre, _pos;
        public Rectangle _hitBox;
        bool _isOnScreen, _toggleLock, _isAirborne;
        public bool _isBotFinished;



        public AI(Texture2D horseTex, Texture2D pixel, int Ylevel, Song horseGalopp)
        {
            _horseTex = horseTex;
            _horseGalopp = horseGalopp;
            _pixel = pixel;
            _Ylevel = Ylevel;
            _Ylevel = Ylevel - 110;
            _ground = (int)_Ylevel;
            _pos = new Vector2(0, _Ylevel);
            _centre = new Vector2(_pos.X + (_horseTex.Width / 2), _pos.Y + (_horseTex.Height / 2));
            _speed = 0.1f;
            _maxSpeed = 10;
            _slowedMaxSpeed = 5.5f;
            _minSpeed = 1f;
            _targetSpeed = (RNG.Next(5, 10)) + (float)RNG.NextDouble();
            _speedDecay = 0.97f;
            _hitBox = new Rectangle((int)_pos.X, (int)_pos.Y, _horseTex.Width, _horseTex.Height);
            _pos = new Vector2(0, _ground);
            _centre = new Vector2(_pos.X + (_horseTex.Width / 2), _pos.Y + (_horseTex.Height / 2));
            _gravity = 0.25f;
            _accelaration = 1.3f;
        }



        public void UpdateMeGame(float playerDistance, bool isGamePlaying, Rectangle jumpPoint1, Rectangle jumpPoint2, Rectangle jumpPoint3, int level, float currentTime)
        {
            _playerDistance = playerDistance;
            _distance += _speed;
            _Ylevel += _upSpeed;


            if (currentTime - _timeOfSpeedChange > 4)
            {
                if (level == 1)
                {
                    _targetSpeed = (RNG.Next(6, 8)) + (float)RNG.NextDouble();
                }
                if (level == 2)
                {
                    _targetSpeed = (RNG.Next(7, 9)) + (float)RNG.NextDouble();
                }
                if (level == 3)
                {
                    _targetSpeed = (RNG.Next(8, 10)) + (float)RNG.NextDouble();
                }

                _timeOfSpeedChange = currentTime;
            }


            if (_isBotFinished == false && _speed < _targetSpeed)
            {
                _speed *= _accelaration;
            }

            if (_hitBox.Intersects(jumpPoint1))
            {
                _upSpeed = -3;
            }
            if (_hitBox.Intersects(jumpPoint2))
            {
                _upSpeed = -3;
            }
            if (_hitBox.Intersects(jumpPoint3))
            {
                _upSpeed = -3;
            }




            if (_targetSpeed > _maxSpeed)
                _targetSpeed = _maxSpeed;
            if (_targetSpeed < _minSpeed)
                _targetSpeed = _minSpeed;
            
            if (_speed > _maxSpeed)
                _speed = _maxSpeed;
            if (_speed < 0)
                _speed = 0;

            if (_Ylevel > _ground)
            {
                _Ylevel = _ground;
                _isAirborne = false;
            }


            if (_distance > (0 - _horseTex.Width) && _distance < 620)
            {
                _isOnScreen = true;
            }
            else
            {
                _isOnScreen = false;
            }



            _speed *= _speedDecay;
            _upSpeed += _gravity;

            _centre = new Vector2((_pos.X + (_horseTex.Width / 2)) + _distance, _pos.Y + (_horseTex.Height / 2));
            _hitBox = new Rectangle((int)_distance + (int)_playerDistance, (int)_pos.Y, _horseTex.Width, _horseTex.Height);
        }


        public void DrawMeGame(SpriteBatch SB)
        {
            SB.Draw(_horseTex, new Vector2(_distance + _playerDistance, _Ylevel), Color.White);
            SB.Draw(_pixel, _hitBox, Color.Red * 0f);
        }


        public void DrawStick (SpriteBatch SB)
        {
            SB.Draw(_pixel, new Rectangle(((int)_centre.X - 2) + (int)_playerDistance, (int)_Ylevel + (_horseTex.Height / 2), 4, 500), Color.BurlyWood);
        }
    }















    internal class AIMisc
    {
        Texture2D _horseTex, _pixel;
        Song _horseGalopp;
        int _Ylevel, _startPoint;
        float _speed, _distance, _maxSpeed, _minSpeed, _speedDecay, _accelaration, _targetSpeed, _slowedMaxSpeed;
        Vector2 _centre, _pos;
        public Rectangle _hitBox;
        bool _isOnScreen, _toggleLock;

        public AIMisc(Texture2D horseTex, Texture2D pixel, int Ylevel, int Xlevel,Song horseGalopp)
        {
            _horseTex = horseTex;
            _horseGalopp = horseGalopp;
            _pixel = pixel;
            _distance = Xlevel;
            _Ylevel = Ylevel - 110;
            _pos = new Vector2(_startPoint, _Ylevel);
            _centre = new Vector2(_pos.X + (_horseTex.Width / 2), _pos.Y + (_horseTex.Height / 2));
            _speed = 7f;
            _maxSpeed = 7;
            _slowedMaxSpeed = 5.5f;
            _minSpeed = 1f;
            _targetSpeed = 7f;
            _speedDecay = 0.97f;
            _hitBox = new Rectangle((int)_pos.X, (int)_pos.Y, _horseTex.Width, _horseTex.Height);
            _accelaration = 1.05f;
        }



        public void UpdateMeTitle(bool isGamePlaying)
        {
            _distance += _speed;



            if (_targetSpeed > _maxSpeed)
                _targetSpeed = _maxSpeed;
            if (_targetSpeed < _minSpeed)
                _targetSpeed = _minSpeed;

            if (_speed > _maxSpeed)
                _speed = _maxSpeed;
            if (_speed < 0)
                _speed = 0;



            if (_distance > (0 - _horseTex.Width) && _distance < 620)
            {
                _isOnScreen = true;
            }
            else
            {
                _isOnScreen = false;
            }

            



            _centre = new Vector2((_pos.X + (_horseTex.Width / 2)) + _distance, _Ylevel + (_horseTex.Height / 2));
            _hitBox = new Rectangle((int)_distance, (int)_pos.Y, _horseTex.Width, _horseTex.Height);
        }


        public void DrawMeTitle(SpriteBatch SB)
        {
            SB.Draw(_horseTex, new Vector2(_distance, _Ylevel), Color.White);
            SB.Draw(_pixel, _hitBox, Color.Red * 0f);
        }


        public void DrawStick(SpriteBatch SB)
        {
            SB.Draw(_pixel, new Rectangle((int)_centre.X - 2, (int)_Ylevel, 4, 500), Color.BurlyWood * 0f);
        }





    }
}
