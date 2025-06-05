using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        bool isGameOver;

        Texture2D ballTexture;
        Vector2 ballPosition;
        Vector2 ballSpeedVector;
        float ballSpeed;

        Vector2 pl1BatPosition;
        Vector2 pl2BatPosition;

        Texture2D _dummyTexture;

        const int paddleWidth = 20;
        const int paddleHeight = 70;

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        private void checkPaddleCollision()
        {
            Rectangle ballRect = new Rectangle(
                (int)(ballPosition.X - ballTexture.Width / 2),
                (int)(ballPosition.Y - ballTexture.Height / 2),
                ballTexture.Width,
                ballTexture.Height
            );

            Rectangle paddle1Rect = new Rectangle((int)pl1BatPosition.X, (int)pl1BatPosition.Y, paddleWidth, paddleHeight);
            Rectangle paddle2Rect = new Rectangle((int)pl2BatPosition.X, (int)pl2BatPosition.Y, paddleWidth, paddleHeight);

            if (ballRect.Intersects(paddle1Rect) || ballRect.Intersects(paddle2Rect))
            {
                ballSpeedVector.X *= -1;
            }
        }


        protected override void Initialize()
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                       _graphics.PreferredBackBufferHeight / 2);
            ballSpeed = 250f;

            ballSpeedVector = new Vector2(1, -1);


            pl1BatPosition = new Vector2(30, _graphics.PreferredBackBufferHeight / 2 - paddleHeight / 2);
            pl2BatPosition = new Vector2(_graphics.PreferredBackBufferWidth - 30 - paddleWidth, _graphics.PreferredBackBufferHeight / 2 - paddleHeight / 2);

            isGameOver = false;

            base.Initialize();
        }

        SpriteFont gameOverFont;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ballTexture = Content.Load<Texture2D>("ball");

            _dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            _dummyTexture.SetData(new Color[] { Color.White });


        }


        private void checkBallCollision()
        {
            float halfBallWidth = ballTexture.Width / 2f;
            float halfBallHeight = ballTexture.Height / 2f;


            if (ballPosition.X - halfBallWidth <= 0 || ballPosition.X + halfBallWidth >= _graphics.PreferredBackBufferWidth)
            {
                isGameOver = true;
            }

            if (ballPosition.Y - halfBallHeight <= 0)
            {
                ballPosition.Y = halfBallHeight;
                ballSpeedVector.Y *= -1;
            }
            else if (ballPosition.Y + halfBallHeight >= _graphics.PreferredBackBufferHeight)
            {
                ballPosition.Y = _graphics.PreferredBackBufferHeight - halfBallHeight;
                ballSpeedVector.Y *= -1;
            }
        }



        private void updateBallPosition(float updatedBallSpeed)
        {
            float ratio = this.ballSpeedVector.X / this.ballSpeedVector.Y;
            float deltaY = updatedBallSpeed / (float)Math.Sqrt(1 + ratio * ratio);
            float deltaX = Math.Abs(ratio * deltaY);

            if (this.ballSpeedVector.X > 0)
            {
                this.ballPosition.X += deltaX;
            }
            else
            {
                this.ballPosition.X -= deltaX;
            }

            if (this.ballSpeedVector.Y > 0)
            {
                this.ballPosition.Y += deltaY;
            }
            else
            {
                this.ballPosition.Y -= deltaY;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isGameOver)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {

                    Initialize();
                    LoadContent();
                    isGameOver = false;
                }
                return;
            }

            float updatedBallSpeed = ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            updateBatsPositions();
            checkBallCollision();
            checkPaddleCollision();
            updateBallPosition(updatedBallSpeed);

            base.Update(gameTime);
        }


        private void updateBatsPositions()
        {
            var kstate = Keyboard.GetState();


            if (kstate.IsKeyDown(Keys.Up) && pl2BatPosition.Y > 0)
            {
                pl2BatPosition.Y -= 5f;
            }
            else if (kstate.IsKeyDown(Keys.Down) && pl2BatPosition.Y < _graphics.PreferredBackBufferHeight - paddleHeight)
            {
                pl2BatPosition.Y += 5f;
            }


            if (kstate.IsKeyDown(Keys.W) && pl1BatPosition.Y > 0)
            {
                pl1BatPosition.Y -= 5f;
            }
            else if (kstate.IsKeyDown(Keys.S) && pl1BatPosition.Y < _graphics.PreferredBackBufferHeight - paddleHeight)
            {
                pl1BatPosition.Y += 5f;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (isGameOver)
            {

                string gameOverMessage = "Game Over!";
                Vector2 messageSize = gameOverFont.MeasureString(gameOverMessage);
                Vector2 position = new Vector2(
                    (_graphics.PreferredBackBufferWidth - messageSize.X) / 2,
                    (_graphics.PreferredBackBufferHeight - messageSize.Y) / 2
                );

                _spriteBatch.DrawString(gameOverFont, gameOverMessage, position, Color.White);
            }
            else
            {

                _spriteBatch.Draw(
                    ballTexture,
                    ballPosition,
                    null,
                    Color.White,
                    0f,
                    new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );

                _spriteBatch.Draw(_dummyTexture, new Rectangle((int)pl1BatPosition.X, (int)pl1BatPosition.Y, paddleWidth, paddleHeight), Color.Red);
                _spriteBatch.Draw(_dummyTexture, new Rectangle((int)pl2BatPosition.X, (int)pl2BatPosition.Y, paddleWidth, paddleHeight), Color.Blue);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}