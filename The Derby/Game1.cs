using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace The_Derby
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 _screenSize = new Vector2(620, 360);
        Rectangle _screen = new Rectangle(0, 0, 620, 360);

        state _state = state.title;
        state _targetState = state.title;

        KeyboardState _currKey;
        MouseState _currMouse;

        bool _isSpacePressed, _isAltPressed, _isGameRunning, _startToggleLock, _countdownToggleLock, _transitioning, _finishingToggleLock, _isLeftMousePressed, _isRightMousePressed, _hintNavigationToggleLock, _resetToggleLock, _playerMadeTheCut, _winSFXToggleLock;

        float _countdownTimerStart, _currentGameTime, _displayCountdownTimer, _backgroundSoundStart, _backgroundSoundCurrentTime, _timeOfStun, _stunDuration = 2, _raceFinishBlurbTimerStart, _StarRotation;

        int _numPassedFinish, _playerPlacing = 999, _raceNumber = 1, _hintSceneSelection, _targetSection;



        bool[] _finishingBotToggleLock = {
            false,
            false,
            false,
            false,
            false,
            false,
            false};
            


        Player player;
        AI[] ai;
        AIMisc[] aiMisc;
        Jump[] jump;
        FinishLine finishLine;
        Transition transition;



        SpriteFont _font, _largerFont;
        Texture2D _crtEffect, _groundTxr, _fenceTxr, _hintSpeed, _hintJump, _hintSlow, _hintFinish, _trophyTxr, _starTxr;
        Song _titleBackgroundSounds, _horseGalloping;
        SoundEffect _countdown, _finishedSFX, _winSFX;



        Vector2[] _layerPos = { 
        new Vector2(0,337),
        new Vector2(0,314),
        new Vector2(0,291),
        new Vector2(0,268),
        new Vector2(0,245),
        new Vector2(0,222),
        new Vector2(0,199),
        new Vector2(0,176),};

        Vector2[] _layerPosMisc = {
        new Vector2(-700,337),
        new Vector2(-420,314),
        new Vector2(-310,291),
        new Vector2(-670,268),
        new Vector2(-460,245),
        new Vector2(-300,222),
        new Vector2(-410,199),
        new Vector2(-570,176),};


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = (int)_screenSize.X;
            _graphics.PreferredBackBufferHeight = (int)_screenSize.Y;
            _graphics.ApplyChanges();



            ai = new AI[7];
            aiMisc = new AIMisc[7];
            jump = new Jump[3];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            _font = Content.Load<SpriteFont>("Font");
            _largerFont = Content.Load<SpriteFont>("TitleFont");


            _titleBackgroundSounds = Content.Load<Song>("BackgroudSounds");
            _horseGalloping = Content.Load<Song>("HorseGalopp");
            _countdown = Content.Load<SoundEffect>("countdown");
            _finishedSFX = Content.Load<SoundEffect>("Finished");
            _winSFX = Content.Load<SoundEffect>("Win");


            for (int i = 0; i < ai.GetLength(0); i++)
            {
                ai[i] = new AI(
                    Content.Load<Texture2D>("Horse" + (i + 2)),
                    Content.Load<Texture2D>("pixel"),
                    (int)_layerPos[i].Y,
                    Content.Load<Song>("HorseGalopp"));
            }

            for (int i = 0; i < ai.GetLength(0); i++)
            {
                aiMisc[i] = new AIMisc(
                    Content.Load<Texture2D>("Horse" + (i + 2)),
                    Content.Load<Texture2D>("pixel"),
                    (int)_layerPosMisc[i].Y,
                    (int)_layerPosMisc[i].X,
                    Content.Load<Song>("HorseGalopp"));
            }

            for (int i = 0; i < jump.GetLength(0); i++)
            {
                jump[i] = new Jump(
                    Content.Load<Texture2D>("Jump"),
                    new Vector2(2040 + (2040 * i), 100),
                    Content.Load<Texture2D>("pixel"));
            }


            player = new Player(
                Content.Load<Texture2D>("Horse1"),
                Content.Load<Texture2D>("pixel"),
                (int)_layerPos[0].Y,
                Content.Load<Song>("HorseGalopp"),
                Content.Load<SoundEffect>("JumpSound"));


            finishLine = new FinishLine(
                    Content.Load<Texture2D>("Flag"),
                    new Vector2(7140, 80),
                    Content.Load<Texture2D>("pixel"));


            _groundTxr = Content.Load<Texture2D>("Ground");
            _fenceTxr = Content.Load<Texture2D>("Fence");
            _crtEffect = Content.Load<Texture2D>("Bulge");
            _hintSpeed = Content.Load<Texture2D>("TipGoFaster");
            _hintJump = Content.Load<Texture2D>("TipJump");
            _hintSlow = Content.Load<Texture2D>("TipSlow");
            _hintFinish = Content.Load<Texture2D>("TipFinish");
            _trophyTxr = Content.Load<Texture2D>("Trophy");
            _starTxr = Content.Load<Texture2D>("Star");
        }

        void Transition()
        {
            transition = new Transition(Content.Load<Texture2D>("pixel"));
            _transitioning = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currKey = Keyboard.GetState();
            _currMouse = Mouse.GetState();

            switch (_state)
            {
                case state.title:
                    UpdateTitle(
                        gameTime);
                    break;

                case state.game:
                    UpdateGame(
                        gameTime);
                    break;

                case state.help:
                    UpdateHelp(
                        gameTime);
                    break;
            }



            if (_currKey.IsKeyDown(Keys.Space) == true)
            {
                _isSpacePressed = true;
            }
            if (_currKey.IsKeyUp(Keys.Space) == true)
            {
                _isSpacePressed = false;
            }



            if (_currKey.IsKeyDown(Keys.LeftAlt) == true)
            {
                _isAltPressed = true;
            }
            if (_currKey.IsKeyUp(Keys.LeftAlt) == true)
            {
                _isAltPressed = false;
            }



            if (_currMouse.LeftButton == ButtonState.Pressed)
            {
                _isLeftMousePressed = true;
            }
            if (_currMouse.LeftButton == ButtonState.Released)
            {
                _isLeftMousePressed = false;
            }



            if (_currMouse.RightButton == ButtonState.Pressed)
            {
                _isRightMousePressed = true;
            }
            if (_currMouse.RightButton == ButtonState.Released)
            {
                _isRightMousePressed = false;
            }



            if (_transitioning == true)
            {
                transition.UpdateMe(gameTime);

                if (transition._transparentsy < 0.005)
                    _transitioning = false;
            }

            base.Update(gameTime);
        }





        void UpdateTitle(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(_titleBackgroundSounds);
                _backgroundSoundStart = _backgroundSoundCurrentTime;
            }

            for (int i = 0; i < ai.GetLength(0); i++)
            {
                aiMisc[i].UpdateMeTitle(_isGameRunning);
            }

            if (_backgroundSoundCurrentTime - _backgroundSoundStart <= 4)
            {
                for (int i = 0; i < ai.GetLength(0); i++)
                {
                    aiMisc[i] = new AIMisc(
                        Content.Load<Texture2D>("Horse" + (i + 2)),
                        Content.Load<Texture2D>("pixel"),
                        (int)_layerPosMisc[i].Y,
                        (int)_layerPosMisc[i].X,
                        Content.Load<Song>("HorseGalopp"));
                }
            }




            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _targetState = state.game;
                Transition();
            }
            if (_transitioning == true)
            {
                if (transition._transparentsy >= 1)
                {
                    _state = _targetState;
                    ResetRace();
                    _raceNumber = 1;
                }
            }




            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                _targetState = state.help;
                Transition();
            }
            if (_transitioning == true)
            {
                if (transition._transparentsy >= 1)
                {
                    _state = _targetState;
                }
            }


            _backgroundSoundCurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        }



        void UpdateHelp(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(_titleBackgroundSounds);
                _backgroundSoundStart = _backgroundSoundCurrentTime;
            }

            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.LeftAlt)) && _hintNavigationToggleLock == false)
            {
                _targetSection += 1;

                if (_targetSection == 4)
                {
                    _targetState = state.title;
                }

                Transition();
                _hintNavigationToggleLock = true;
            }
            if (_transitioning == true)
            {
                if (transition._transparentsy >= 1)
                {
                    _hintSceneSelection = _targetSection;
                    _hintNavigationToggleLock = false;


                    if (_hintSceneSelection == 4)
                    {
                        _state = _targetState;
                        _hintSceneSelection = 0;
                        _targetSection = 0;
                    }
                }
            }
        }




        void UpdateGame(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();


            if (_startToggleLock == false)
            {
                _startToggleLock = true;
                _countdownTimerStart = (float)gameTime.ElapsedGameTime.TotalSeconds;

                player.UpdateMe(_currKey, _isSpacePressed, _isAltPressed, _currMouse, _isLeftMousePressed, _isRightMousePressed);

                for (int i = 0; i < ai.GetLength(0); i++)
                {
                    ai[i].UpdateMeGame(player.distance, _isGameRunning, jump[0]._jumpPoint, jump[1]._jumpPoint, jump[2]._jumpPoint, _raceNumber, _currentGameTime);
                }

                for (int i = 0; i < jump.GetLength(0); i++)
                {
                    jump[i].UpdateMe(player.distance);
                }

                finishLine.UpdateMe(player.distance);


                if (_currKey.IsKeyDown(Keys.Space))
                {
                    _isSpacePressed = true;
                }
            }
















            _currentGameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _displayCountdownTimer = (_countdownTimerStart + 6) - _currentGameTime;


            if (_currentGameTime - _countdownTimerStart >= 5)
            {
                _isGameRunning = true;
            }


            if ((int)_displayCountdownTimer < 4 && _countdownToggleLock == false)
            {
                _countdown.Play();
                _countdownToggleLock = true;
            }


            if (_isGameRunning)
            {
                _StarRotation += 0.01f;

                player.UpdateMe(_currKey, _isSpacePressed, _isAltPressed, _currMouse, _isLeftMousePressed, _isRightMousePressed);

                for (int i = 0; i < ai.GetLength(0); i++)
                {
                    ai[i].UpdateMeGame(player.distance, _isGameRunning, jump[0]._jumpPoint, jump[1]._jumpPoint, jump[2]._jumpPoint, _raceNumber, _currentGameTime);
                }

                for (int i = 0; i < jump.GetLength(0); i++)
                {
                    jump[i].UpdateMe(player.distance);
                }

                finishLine.UpdateMe(player.distance);

                if (_currKey.IsKeyDown(Keys.Space))
                {
                    _isSpacePressed = true;
                } 




                // < Stun > -------------------------------------
                for (int i = 0; i < jump.GetLength(0); i++)
                {
                    if (player._hitBox.Intersects(jump[i]._hitBox))
                    {
                        player._isPlayerSlowed = true;
                        _timeOfStun = _currentGameTime;
                    }
                }
                if (_currentGameTime - _timeOfStun > _stunDuration)
                {
                    player._isPlayerSlowed = false;
                }






                // < Finish Line > --------------------------------------------------------------------
                if (player._hitBox.Intersects(finishLine._hitBox) && _finishingToggleLock == false)
                {
                    player._isPlayerFinished = true;
                    _finishedSFX.Play();
                    _finishingToggleLock = true;
                    _numPassedFinish += 1;
                    _playerPlacing = _numPassedFinish;
                    _raceFinishBlurbTimerStart = _currentGameTime;
                }
                for (int i = 0; i < ai.GetLength(0); i++)
                {
                    if (ai[i]._hitBox.Intersects(finishLine._hitBox) && _finishingBotToggleLock[i] == false)
                    {
                        ai[i]._isBotFinished = true;
                        _finishingBotToggleLock[i] = true;
                        _numPassedFinish += 1;
                    }
                }

                if (_raceNumber == 1 && _playerPlacing <= 3)
                {
                     _playerMadeTheCut = true;
                }
                else if (_raceNumber == 2 && _playerPlacing <= 2)
                {
                    _playerMadeTheCut = true;
                }
                else if (_raceNumber == 3 && _playerPlacing == 1)
                {
                    _playerMadeTheCut = true;
                }
                else
                {
                    _playerMadeTheCut = false;
                }

                if (_raceNumber < 3)
                {
                    if (player._isPlayerFinished == true && _currentGameTime - _raceFinishBlurbTimerStart > 8 && _resetToggleLock == false)
                    {
                        _targetState = state.title;
                        Transition();
                        _resetToggleLock = true;
                    }
                }
                if (_raceNumber == 3)
                {
                    if (player._isPlayerFinished == true && _currentGameTime - _raceFinishBlurbTimerStart > 14 && _resetToggleLock == false)
                    {
                        _targetState = state.title;
                        Transition();
                        _resetToggleLock = true;
                    }
                }
                if (_transitioning == true)
                {
                    if (transition._transparentsy >= 1)
                    {
                        if (_playerMadeTheCut == false || _raceNumber == 3)
                        {
                            ResetRace();
                            _state = _targetState;
                        }
                        if (_playerMadeTheCut == true)
                        {
                            ResetRace();
                        }
                    }
                }
                if (player._isPlayerFinished == true && _currentGameTime - _raceFinishBlurbTimerStart > 4 && _winSFXToggleLock == false)
                {
                    _winSFX.Play();
                    _winSFXToggleLock = true;
                }
            }
        }


        void ResetRace()
        {

            for (int i = 0; i < ai.GetLength(0); i++)
            {
                ai[i] = new AI(
                    Content.Load<Texture2D>("Horse" + (i + 2)),
                    Content.Load<Texture2D>("pixel"),
                    (int)_layerPos[i].Y,
                    Content.Load<Song>("HorseGalopp"));
            }

            for (int i = 0; i < ai.GetLength(0); i++)
            {
                aiMisc[i] = new AIMisc(
                    Content.Load<Texture2D>("Horse" + (i + 2)),
                    Content.Load<Texture2D>("pixel"),
                    (int)_layerPosMisc[i].Y,
                    (int)_layerPosMisc[i].X,
                    Content.Load<Song>("HorseGalopp"));
            }

            for (int i = 0; i < jump.GetLength(0); i++)
            {
                jump[i] = new Jump(
                    Content.Load<Texture2D>("Jump"),
                    new Vector2(2040 + (2040 * i), 100),
                    Content.Load<Texture2D>("pixel"));
            }


            player = new Player(
                Content.Load<Texture2D>("Horse1"),
                Content.Load<Texture2D>("pixel"),
                (int)_layerPos[0].Y,
                Content.Load<Song>("HorseGalopp"),
                Content.Load<SoundEffect>("JumpSound"));


            finishLine = new FinishLine(
                    Content.Load<Texture2D>("Flag"),
                    new Vector2(7140, 80),
                    Content.Load<Texture2D>("pixel"));

            _playerPlacing = 999;
            _numPassedFinish = 0;
            for (int i = 0; i < ai.GetLength(0); i++)
            {
                _finishingBotToggleLock[i] = false;
            }
            _finishingToggleLock = false;
            _countdownToggleLock = false;
            _resetToggleLock = false;
            _winSFXToggleLock = false;
            _isGameRunning = false;

            _raceNumber += 1;

            if (_raceNumber == 4)
                _raceNumber = 1;

            player.UpdateMe(_currKey, _isSpacePressed, _isAltPressed, _currMouse, _isLeftMousePressed, _isRightMousePressed);


            _StarRotation = 0;

            _countdownTimerStart = _currentGameTime;
            
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (_state)
            {
                case state.title:
                    DrawTitle(
                        gameTime,
                        _spriteBatch);
                    break;

                case state.game:
                    DrawGame(
                        gameTime,
                        _spriteBatch);
                    break;

                case state.help:
                    DrawHelp(
                        gameTime,
                        _spriteBatch);
                    break;
            }
            _spriteBatch.Begin();

            if (_transitioning == true)
                transition.DrawMe(_spriteBatch);

            _spriteBatch.Draw(_crtEffect, _screen, Color.White * 0.15f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawTitle(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin();



            _spriteBatch.Draw(_fenceTxr, new Vector2(player.distance, 120), Color.White);
            aiMisc[6].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[7].Y), Color.White);
            aiMisc[5].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[6].Y), Color.White);
            aiMisc[4].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[5].Y), Color.White);
            aiMisc[3].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[4].Y), Color.White);
            aiMisc[2].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[3].Y), Color.White);
            aiMisc[1].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[2].Y), Color.White);
            aiMisc[0].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[1].Y), Color.White);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[0].Y), Color.White);



            aiMisc[6].DrawMeTitle(_spriteBatch);
            aiMisc[5].DrawMeTitle(_spriteBatch);
            aiMisc[4].DrawMeTitle(_spriteBatch);
            aiMisc[3].DrawMeTitle(_spriteBatch);
            aiMisc[2].DrawMeTitle(_spriteBatch);
            aiMisc[1].DrawMeTitle(_spriteBatch);
            aiMisc[0].DrawMeTitle(_spriteBatch);



            Vector2 titleSize = _largerFont.MeasureString("The Derby");
            _spriteBatch.DrawString(_largerFont, "The Derby", new Vector2(_screenSize.X / 2 - (titleSize.X / 2) + 4, 50 + 4), Color.Black);
            _spriteBatch.DrawString(_largerFont, "The Derby", new Vector2(_screenSize.X / 2 - (titleSize.X / 2), 50), Color.White);


            Vector2 learnSize = _font.MeasureString("press LEFT ALT for how to play");
            _spriteBatch.DrawString(_font, "press LEFT ALT for how to play", new Vector2(_screenSize.X / 2 - (learnSize.X / 2) + 3, 290 + 3), Color.Black);
            _spriteBatch.DrawString(_font, "press LEFT ALT for how to play", new Vector2(_screenSize.X / 2 - (learnSize.X / 2), 290), Color.White);


            Vector2 startSize = _font.MeasureString("press SPACE to PLAY");
            _spriteBatch.DrawString(_font, "press SPACE to PLAY", new Vector2(_screenSize.X / 2 - (startSize.X / 2) + 3, 230 + 3), Color.Black);
            _spriteBatch.DrawString(_font, "press SPACE to PLAY", new Vector2(_screenSize.X / 2 - (startSize.X / 2), 230), Color.White);



            _spriteBatch.End();
        }






        void DrawHelp(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin();

            if (_hintSceneSelection == 0)
                _spriteBatch.Draw(_hintSpeed, _screen, Color.White);
            if (_hintSceneSelection == 1)
                _spriteBatch.Draw(_hintJump, _screen, Color.White);
            if (_hintSceneSelection == 2)
                _spriteBatch.Draw(_hintSlow, _screen, Color.White);
            if (_hintSceneSelection == 3)
                _spriteBatch.Draw(_hintFinish, _screen, Color.White);


            Vector2 learnSize = _font.MeasureString("press LEFT ALT to continue");
            _spriteBatch.DrawString(_font, "press LEFT ALT to continue", new Vector2(_screenSize.X / 2 - (learnSize.X / 2) + 3, 290 + 3), Color.Black);
            _spriteBatch.DrawString(_font, "press LEFT ALT to continue", new Vector2(_screenSize.X / 2 - (learnSize.X / 2), 290), Color.White);

            _spriteBatch.End();
        }





        void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin();




            _spriteBatch.Draw(_fenceTxr, new Vector2(player.distance, 120), Color.White);
            ai[6].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[7].Y), Color.White);
            ai[5].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[6].Y), Color.White);
            ai[4].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[5].Y), Color.White);
            ai[3].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[4].Y), Color.White);
            ai[2].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[3].Y), Color.White);
            ai[1].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[2].Y), Color.White);
            ai[0].DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[1].Y), Color.White);
            player.DrawStick(_spriteBatch);
            _spriteBatch.Draw(_groundTxr, new Vector2(player.distance, _layerPos[0].Y), Color.White);


            for (int i = 0; i < jump.GetLength(0); i++)
            {
                jump[i].DrawMe(_spriteBatch);
            }

            finishLine.DrawMe(_spriteBatch);    

            ai[6].DrawMeGame(_spriteBatch);
            ai[5].DrawMeGame(_spriteBatch);
            ai[4].DrawMeGame(_spriteBatch);
            ai[3].DrawMeGame(_spriteBatch);
            ai[2].DrawMeGame(_spriteBatch);
            ai[1].DrawMeGame(_spriteBatch);
            ai[0].DrawMeGame(_spriteBatch);


            player.DrawMe(_spriteBatch);




            Vector2 TimerSize = _largerFont.MeasureString("" + (int)_displayCountdownTimer);

            if (_isGameRunning == false && (int)_displayCountdownTimer < 4)
            {
                _spriteBatch.DrawString(_largerFont, "" + (int)_displayCountdownTimer, new Vector2(_screenSize.X / 2 - (TimerSize.X / 2) + 4, 25 + 4), Color.Black);
                _spriteBatch.DrawString(_largerFont, "" + (int)_displayCountdownTimer, new Vector2(_screenSize.X / 2 - (TimerSize.X / 2), 25), Color.White);
            }



            Vector2 GoSize = _largerFont.MeasureString("GO!");

            if (_isGameRunning == true && ((int)_displayCountdownTimer < 1 && (int)_displayCountdownTimer > -1))
            {
                _spriteBatch.DrawString(_largerFont, "GO!", new Vector2(_screenSize.X / 2 - (GoSize.X / 2), 25), Color.Black);
                _spriteBatch.DrawString(_largerFont, "GO!", new Vector2(_screenSize.X / 2 - (GoSize.X / 2) + 4, 25 + 4), Color.LawnGreen);
            }



            Vector2 SlowSize = _largerFont.MeasureString("Slowed!");

            if (player._isPlayerSlowed == true && player._isPlayerFinished == false)
            {
                _spriteBatch.DrawString(_largerFont, "Slowed!", new Vector2(_screenSize.X / 2 - (SlowSize.X / 2), 25), Color.Black);
                _spriteBatch.DrawString(_largerFont, "Slowed!", new Vector2(_screenSize.X / 2 - (SlowSize.X / 2) + 4, 25 + 4), Color.Crimson);
            }




            Vector2 FinishSize = _largerFont.MeasureString("Finished!");

            if (player._isPlayerFinished == true && _currentGameTime - _raceFinishBlurbTimerStart < 3)
            {
                _spriteBatch.DrawString(_largerFont, "Finished!", new Vector2(_screenSize.X / 2 - (FinishSize.X / 2), 25), Color.Black);
                _spriteBatch.DrawString(_largerFont, "Finished!", new Vector2(_screenSize.X / 2 - (FinishSize.X / 2) + 4, 25 + 4), Color.LawnGreen);
            }




            Vector2 FirstSize = _largerFont.MeasureString(_playerPlacing + "st Place");
            Vector2 SecondSize = _largerFont.MeasureString(_playerPlacing + "nd Place");
            Vector2 ThirdSize = _largerFont.MeasureString(_playerPlacing + "rd Place");
            Vector2 OtherSize = _largerFont.MeasureString(_playerPlacing + "th Place");
            Vector2 Outcome1 = _font.MeasureString("You made the cut!");
            Vector2 Outcome2 = _font.MeasureString("You lost");
            Vector2 Outcome3 = _largerFont.MeasureString("You Won");






            if (player._isPlayerFinished == true && _currentGameTime - _raceFinishBlurbTimerStart > 4)
            {
                if (_playerPlacing == 1 && _raceNumber != 3)
                {
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "st Place", new Vector2(_screenSize.X / 2 - (FirstSize.X / 2), 25), Color.Black);
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "st Place", new Vector2(_screenSize.X / 2 - (FirstSize.X / 2) + 3, 25 + 3), Color.Gold);
                }
                if (_playerPlacing == 2)
                {
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "nd Place", new Vector2(_screenSize.X / 2 - (SecondSize.X / 2), 25), Color.Black);
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "nd Place", new Vector2(_screenSize.X / 2 - (SecondSize.X / 2) + 3, 25 + 3), Color.Silver);
                }
                if (_playerPlacing == 3)
                {
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "rd Place", new Vector2(_screenSize.X / 2 - (ThirdSize.X / 2), 25), Color.Black);
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "rd Place", new Vector2(_screenSize.X / 2 - (ThirdSize.X / 2) + 3, 25 + 3), Color.Brown);
                }
                if (_playerPlacing > 3)
                {
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "th Place", new Vector2(_screenSize.X / 2 - (OtherSize.X / 2), 25), Color.Black);
                    _spriteBatch.DrawString(_largerFont, _playerPlacing + "th Place", new Vector2(_screenSize.X / 2 - (OtherSize.X / 2) + 3, 25 + 3), Color.White);
                }
                if (_playerMadeTheCut == true)
                {
                    if (_raceNumber < 3)
                    {
                        _spriteBatch.DrawString(_font, "You made the cut!", new Vector2(_screenSize.X / 2 - (Outcome1.X / 2), 225), Color.Black);
                        _spriteBatch.DrawString(_font, "You made the cut!", new Vector2(_screenSize.X / 2 - (Outcome1.X / 2) + 2, 225 + 2), Color.White);
                    }
                    if (_raceNumber == 3)
                    {
                        _spriteBatch.Draw(_starTxr, new Vector2((_screenSize.X / 2), (_screenSize.Y / 2)), null, Color.White, _StarRotation, new Vector2(_starTxr.Width / 2, _starTxr.Height / 2), 1, SpriteEffects.None, 1);
                        _spriteBatch.Draw(_trophyTxr, new Vector2((_screenSize.X / 2) - (_trophyTxr.Width / 2), (_screenSize.Y / 2) - _trophyTxr.Height / 2), Color.White);

                        _spriteBatch.DrawString(_largerFont, "You Won!", new Vector2(_screenSize.X / 2 - (Outcome3.X / 2), 215), Color.Black);
                        _spriteBatch.DrawString(_largerFont, "You Won!", new Vector2(_screenSize.X / 2 - (Outcome3.X / 2) + 4, 215 + 4), Color.Gold);
                    }
                }
                if (_playerMadeTheCut == false)
                {
                    _spriteBatch.DrawString(_font, "You Lost!", new Vector2(_screenSize.X / 2 - (Outcome2.X / 2), 225), Color.Black);
                    _spriteBatch.DrawString(_font, "You Lost!", new Vector2(_screenSize.X / 2 - (Outcome2.X / 2) + 2, 225 + 2), Color.White);
                }
            }




            _spriteBatch.End();
        }     
    }

    enum state
    {
        title,
        game,
        help
    }
}