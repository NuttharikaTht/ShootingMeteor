using BobbleShooter.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BobbleShooter
{
    public class bubbleShooter : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int bubbleSize = 60;

        public static int limitRight = 1070, limitLeft = 530, limitUp = 40, limitDown = 460;

        public Vector2 positionBubbleLaunch;
        public static List<Bubble> bubbleStuck { get; private set; }
        public static List<Bubble> bubbleFall { get; private set; }
        public static List<Color> colors;

        private Color _Color;
        private int alpha = 255;
        private bool fadeFinish = false;
        private float timerPerUpdate = 0.05f;
        private float _timer = 0f;
        private Vector2 fontSize;
        
        private Vector2 locationPointer;

        Texture2D launcher, bubble, Box, BG, Overlay;
        SpriteFont Orbitron, Orbitron_24B;
        private List<Component> _gameComponents;
        private Component _restartButton;

        Bubble bubbleLaunched, nextBubble;

        float elapsed;
        float rotation;

        ParticlesEngine particles;
        Texture2D textParticle;

        Texture2D textRoof;
        int TimeRoofLower = 0;
        public static int Rooflower = 0;
        float Timecount =0;
        
        private float Time = 0, StartLevelTime = 0;
        private int _previousScore = 0;

        Rectangle leftLine, topLine, rightLine, downLine;

        // game state :: create game state
        GameState state;

        //load level :: The variable that will store the current level and the Level Manager is created
        AdministratorLevels levelManager;

        public bubbleShooter()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int)Singleton.Instance.Diemensions.X;
            _graphics.PreferredBackBufferHeight = (int)Singleton.Instance.Diemensions.Y;
            _graphics.ApplyChanges();
            Window.Title = "ShootingMeteor";

            _Color = new Color(255, 255, 255, alpha);
            
            locationPointer = new Vector2(800, 520);
            rotation = 0f;
            elapsed = 0f;
           
            //bubble สำหรับยิง
            positionBubbleLaunch = new Vector2(locationPointer.X- 30, locationPointer.Y - 30);
            
            bubbleStuck = new List<Bubble>();
            bubbleFall = new List<Bubble>();

            // load level :: uncomment for generate bubble each level
            colors = new List<Color>();
            colors.Add(Color.Purple);
            colors.Add(Color.BlueViolet);
            colors.Add(Color.Blue);
            colors.Add(Color.Green);
            colors.Add(Color.Yellow);
            colors.Add(Color.Orange);
            colors.Add(Color.Red);

            //line
            leftLine = new Rectangle(limitLeft, limitUp, 1, limitDown - limitUp + 100);
            topLine = new Rectangle(limitLeft, limitUp, limitRight - limitLeft, 1);
            rightLine = new Rectangle(limitRight, limitUp, 1, limitDown - limitUp + 100);
            downLine = new Rectangle(limitLeft, limitDown + 100, limitRight - limitLeft, 1);
            
            state = GameState.play;

            // load level :: Initialize method, the name of the file is sent, first the namespace and then the name of the file
            levelManager = new AdministratorLevels();

            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            BG = this.Content.Load<Texture2D>("Screen/BG");
            Overlay = this.Content.Load<Texture2D>("Screen/Black");
            Box = this.Content.Load<Texture2D>("Interface/box");

            var quitButton = new Button(Content.Load<Texture2D>("Interface/quit_button"), Content.Load<SpriteFont>("Fonts/Orbitron_Button"))
            {
                Position = new Vector2(20, 520),
                Text = "",
            };

            quitButton.Click += QuitButton_Click;

            var resetButton = new Button(Content.Load<Texture2D>("Interface/button"), Content.Load<SpriteFont>("Fonts/Orbitron_Button"))
            {
                Position = new Vector2(20, 440),
                Text = "Reset",
            };

            resetButton.Click += ResetButton_Click;


            var restartButton = new Button(Content.Load<Texture2D>("Interface/restart_button"), Content.Load<SpriteFont>("Fonts/Orbitron_Button"))
            {
                Position = Singleton.Instance.Diemensions / 2 - new Vector2(200, 100) /2 ,
                Text = "",
            };

            restartButton.Click += RestartButton_Click;

            _gameComponents = new List<Component>()
              {
                resetButton,
                quitButton,
              };
            _restartButton = restartButton;

            launcher = this.Content.Load<Texture2D>("GameObject/spaceship");
            bubble = this.Content.Load<Texture2D>("GameObject/bubble");
            

            textParticle = this.Content.Load<Texture2D>("GameObject/explosion");
            particles = new ParticlesEngine();
            particles.addTexture(textParticle);
            particles.RandomParticle = true;
            // load level
            if (levelManager != null)
            {
                
                loadLevel();
                
            }
            Orbitron = Content.Load<SpriteFont>("Fonts/Orbitron");
            Orbitron_24B = Content.Load<SpriteFont>("Fonts/Orbitron_24_Bold");

            textRoof = Content.Load<Texture2D>("GameObject/Roof");
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (fadeFinish) {
                elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                Time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //ยิงauto เมื่อไม่่ขยับ
                Timecount += (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            foreach (var component in _gameComponents)
                component.Update(gameTime);
            _restartButton.Update(gameTime);

            if (state == GameState.play) // game state :: check is play state?
            {
               
                bubbleLaunched.Mover();
                
                    if (bubbleLaunched.bubbleRect.Intersects(rightLine) || bubbleLaunched.bubbleRect.Intersects(leftLine))
                    {
                        bubbleLaunched.Direction.X *= -1;
                    }

                    if (bubbleLaunched.bubbleRect.Intersects(downLine))
                    {
                        bubbleLaunched.Direction.Y *= -1;
                        bubbleLaunched.Direction.X *= -1;
                    }


                        if (bubbleLaunched.Position.Y <= limitUp + (Rooflower * bubbleSize))
                        {

                        bubbleLaunched.findNearest();
                        pasteBubble(bubbleLaunched);
                        bubbleLaunched = null;


                    }

                else
                {
                    for (int i = 0; i < bubbleStuck.Count; i++)
                    {
                        if (bubbleStuck[i] != bubbleLaunched)
                        {
                            if (crashBubble(bubbleLaunched, bubbleStuck[i]))
                            {
                                bubbleLaunched.findNearest();
                                pasteBubble(bubbleLaunched);
                                bubbleLaunched = null;
                                break;
                            }
                        }
                    }
                }


                if (elapsed > 0.05f)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        rotation -= 0.1f;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        rotation += 0.1f;
                    }
                    elapsed = 0f;
                }

                //รายการชั่วคราวถูกสร้างขึ้นเพื่อให้สามารถกำจัดฟองอากาศที่จะตกลงมา
                List<Bubble> destroyBubbles = new List<Bubble>();

                foreach (Bubble bubble1 in bubbleFall)
                {
                    //เริ่มระเบิด
                    particles.pos_Transmitter = bubble1.Position;

                    particles.startParticle(10, 1.5f, 40, bubble1.Color);
                    destroyBubbles.Add(bubble1);
                }
                Singleton.Instance.Score += destroyBubbles.Count * 100;
                foreach (Bubble bubble1 in destroyBubbles)
                {
                    //ฟองอากาศจะถูกลบออก
                    bubbleFall.Remove(bubble1);

                }

                if (bubbleLaunched == null)
                {
                    generateBubble();
                }                
                
                if (Timecount > 20)
                {
                    //แสดงtext
                    shootBubble();
                   
                }
                else
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        Timecount = 0;
                    }
                }
                
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    bubbleLaunched.moving = true;

                    Matrix matrix = Matrix.CreateRotationZ(rotation);
                    bubbleLaunched.Direction.X += matrix.M12 * 1.5f;
                    bubbleLaunched.Direction.Y -= matrix.M11 * 1.5f;
                    
                    if (TimeRoofLower == 10)
                    {
                        Rooflower++;
                        //LimitUp += bubblesize;
                        TimeRoofLower = 0;
                        foreach (Bubble bb in bubbleStuck)
                        {
                            bb.pos_bubbleBox = new Vector2(bb.pos_bubbleBox.X, bb.pos_bubbleBox.Y + 1);
                            bb.Position = bubbleBox((Int32)bb.pos_bubbleBox.X, (Int32)bb.pos_bubbleBox.Y);
                        }
                    }

                }

                particles.Update();
                
                isGameOver(); // game state :: check is game over?
                
            }
            else if (state == GameState.win) // load level :: load next level
            {
                if (Singleton.Instance.LevelActual < (levelManager.Levels.Count - 1))
                {
                    state = GameState.play;

                    fadeFinish = false;
                    alpha = 255;
                    StartLevelTime = Time;
                    _previousScore = Singleton.Instance.Score;

                    Singleton.Instance.LevelActual++;

                    loadLevel();
                }
            }

            if (!fadeFinish)
            {
                _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (_timer >= timerPerUpdate)
                {
                    alpha -= 5;
                    _timer -= timerPerUpdate;
                    if (alpha <= 5)
                    {
                        fadeFinish = true;
                    }
                    _Color.A = (byte)alpha;
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);
            _spriteBatch.Begin();

            _spriteBatch.Draw(BG, new Vector2(0, 0), Color.White);

            //playable area bg
            _spriteBatch.Draw(Box, new Vector2(limitLeft, limitUp - 20), Color.White);

            _spriteBatch.DrawString(Orbitron, "Level " + (Singleton.Instance.LevelActual + 1).ToString(), new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(Orbitron, "Score: " + Singleton.Instance.Score.ToString(), new Vector2(20, 40), Color.White);

            _spriteBatch.DrawString(Orbitron, "Best Score: " + Singleton.Instance.BestScore, new Vector2(20, 80), Color.White);
            _spriteBatch.DrawString(Orbitron, "Best Time: " + Singleton.Instance.BestTime.ToString("00:00"), new Vector2(20, 100), Color.White);
            _spriteBatch.DrawString(Orbitron, "Time " + Time.ToString("00:00"), new Vector2(limitLeft - 100, limitUp), Color.White);

            foreach (var component in _gameComponents)
                component.Draw(gameTime, _spriteBatch);


            for (int y = 0; y < 8; y++)
            {
                for(int x = 0; x<10; x++)
                {
                    //draw rect
                    Rectangle dRect = new Rectangle((x * bubbleSize) + limitLeft + (((y+Rooflower) % 2) * (bubbleSize / 2)), (y * bubbleSize) + limitUp, bubbleSize, bubbleSize);
                    _spriteBatch.Draw(textRoof, new Vector2(limitLeft, (limitUp + (Rooflower * bubbleSize)) - 20), null, Color.White);
                   
                }
            }
           
            _spriteBatch.Draw(launcher, locationPointer, null, Color.White, rotation, new Vector2(launcher.Width / 2, launcher.Height / 2), 1f, SpriteEffects.None, 1);

            if(bubbleLaunched != null)
            {
                _spriteBatch.Draw(bubble, bubbleLaunched.Position, bubbleLaunched.Color);
            }

            foreach (Bubble bb in bubbleStuck)
            {
                _spriteBatch.Draw(bubble, bb.Position, null, bb.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }

            particles.Draw(_spriteBatch);

            if (nextBubble != null)
            {
                _spriteBatch.Draw(bubble, nextBubble.Position, nextBubble.Color);
            }

            if (state == GameState.lose) {
                _spriteBatch.Draw(Overlay, Vector2.Zero, new Color(255, 255, 255, 210));
                fontSize = Orbitron_24B.MeasureString("GameOver !!");
                _spriteBatch.DrawString(Orbitron_24B, "GameOver !!", new Vector2(Singleton.Instance.Diemensions.X, Singleton.Instance.Diemensions.Y - 200) / 2 - fontSize / 2, Color.White);
                _restartButton.Draw(gameTime, _spriteBatch);
            }
            if (!fadeFinish)
            {
                _spriteBatch.Draw(Overlay, Vector2.Zero, _Color);
            }

            _spriteBatch.End();
         
            base.Draw(gameTime);
        }


        private void QuitButton_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            RestartGame(false);
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            RestartGame(true);
        }


        private void RestartGame(bool isRestart)
        {
            if (isRestart)
            {
                if (int.Parse(Singleton.Instance.BestScore) < Singleton.Instance.Score)
                {
                    Singleton.Instance.BestScore = Singleton.Instance.Score.ToString();
                }
                if (Singleton.Instance.BestTime < Time)
                {
                    Singleton.Instance.BestTime = Time;
                }
                Singleton.Instance.LevelActual = 0;
                Singleton.Instance.Score = 0;
                state = GameState.play;
                elapsed = 0;
                Time = 0f;
                Timecount = 0f;
                
            }
            else
            {
                Singleton.Instance.Score = _previousScore;
                elapsed = 0;
                Time = StartLevelTime;
                Timecount = 0f;
            }
            levelManager.loadNewGame();
            loadLevel();

            fadeFinish = false;
            alpha = 255;
        }

        public void pasteBubble(Bubble bubble)
        {
            
                List<Bubble> groupEquals = bubble.findEquals();
                TimeRoofLower++;

                if (groupEquals.Count < 3)
                {

                    bubble.Position = bubbleBox((Int32)bubble.pos_bubbleBox.X, (Int32)bubble.pos_bubbleBox.Y);
                    bubbleStuck.Add(bubble);

                }

                else
                {

                    //ฟองสีเดียวกันลงกลุ่มที่กำลังระเบิด
                    bubbleFall = groupEquals;

                    //ลบออกจากรายการฟองที่ติดอยู่กับฟองที่สร้างกลุ่ม
                    //โดยใช้วิธี Except:
                    bubbleStuck = new List<Bubble>(bubbleStuck.Except(groupEquals));

                    //สองรายการชั่วคราวถูกสร้างขึ้นเพื่อเก็บฟองอากาศที่เชื่อมต่อ
                    List<Bubble> floating = new List<Bubble>();
                    List<Bubble> connected = new List<Bubble>();

                    // เรารีเซ็ตธงที่ระบุว่าฟองกำลังจะตกลงมาจากฟองที่ติดอยู่
                    foreach (Bubble bubb in bubbleStuck)
                    {
                        bubb.isDestroy = false;
                    }

                    bool _root = false;
                    foreach (Bubble bubb in bubbleStuck)
                    {
                        _root = false;
                        //ถ้าฟองนั้นไม่ใช่อันที่ติดกับเพดานอยู่แล้ว
                        if (bubb.pos_bubbleBox.Y > Rooflower)
                        {
                            //ค้นหาฟองที่เชื่อมต่อ
                            connected = bubb.findConnectedBubbles();
                            foreach (Bubble bb in connected)
                            {
                                //ตรวจสอบว่ามีอันหนึ่งติดอยู่กับเพดาน
                                if (bb.pos_bubbleBox.Y == Rooflower)
                                {
                                    _root = true;
                                    break;

                                }

                            }


                            // ถ้าไม่ติดเพดาน ฟองก็ตกลงมา
                            if (!_root)
                            {
                                bubb.isDestroy = true;
                                floating.Add(bubb);
                            }

                        }


                    }


                    // เรากำจัดฟองที่ติดอยู่ที่จะไม่ตกหล่น
                    //เดินติดกาวไม่ติดคนอื่น
                    bubbleStuck = new List<Bubble>(bubbleStuck.Except(floating));

                    foreach (Bubble bubble1 in floating)
                    {
                        bubbleFall.Add(bubble1);
                    }


                }
            if (TimeRoofLower == 10)
            {
                    Rooflower++;
                    TimeRoofLower = 0;
                
                foreach (Bubble bb in bubbleStuck)
                {
                    bb.pos_bubbleBox = new Vector2(bb.pos_bubbleBox.X, bb.pos_bubbleBox.Y + 1);
                    bb.Position = bubbleBox((Int32)bb.pos_bubbleBox.X, (Int32)bb.pos_bubbleBox.Y);
                }
                
                
            }
            
        }


        private Vector2 bubbleBox(int cx, int cy)
        {
            Rectangle dRect = new Rectangle((cx * bubbleSize) + limitLeft +
                (((cy+Rooflower) % 2) * (bubbleSize / 2)),
                (int)cy * bubbleSize + limitUp, bubbleSize, bubbleSize);

            return new Vector2(dRect.X, dRect.Y);
        }

        public bool crashBubble(Bubble bb1, Bubble bb2) // to check that bubbles crash or not
        {
            float dx = bb2.Position.X - bb1.Position.X;
            float dy = bb2.Position.Y - bb1.Position.Y;
            Int32 rad = bubble.Width / 2;
            Int32 radTotal = rad * 2; // radius of 2 bubbles
            double diff = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            double radPow = Math.Pow(radTotal, 2);

            //use this => (x2 – x1)2 + (y2 – y1)2 <= (R1+ R2)2

            if (diff <= radPow)
            {
                return true; // if diffPow between bubble is less than radPow then BUBBLE CRASHED !
            }
            return false; 
        }

        public void generateBubble()
        {
            if (nextBubble != null)
            {
                bubbleLaunched = nextBubble;
                bubbleLaunched.Position = positionBubbleLaunch;
            }

            Color bubbleColor ;

            if (bubbleStuck.Count > 0)
            {
                List<Color> missingColors = new List<Color>((from c in bubbleStuck select c.Color).Distinct());
                Int32 _color = new Random().Next(missingColors.Count);
                
                if(missingColors.Count <= 0)
                {
                    bubbleColor = colors[new Random().Next(7)];
                }
                else
                {
                    bubbleColor = missingColors[_color];
                }
            }
            else
            {
                bubbleColor = colors[new Random().Next(7)];
            }
            nextBubble = new Bubble(positionBubbleLaunch - new Vector2(bubbleSize * 3, 0), bubbleColor);
        }


        // game state :: Method for check is over?
        private void isGameOver()
        {
            if (bubbleStuck.Count == 0) // no bubble left :: win
            {
                state = GameState.win;
            }
            else
            {
                foreach (Bubble b in bubbleStuck)
                {
                    if (b.pos_bubbleBox.Y > 7) // check bubble over limit :: lose
                    {
                        state = GameState.lose;
                        return;
                    }
                }
            }
        }

        // load level :: Method for loading levels
        private void loadLevel()
        {
            
            // load level :: clear all before start level
            bubbleStuck.Clear();
            bubbleFall.Clear();
            rotation = 0f;
            

            bubbleLaunched = new Bubble(positionBubbleLaunch, colors[new Random().Next(0, 7)]);
            generateBubble();
            
            Level level = levelManager.Levels[Singleton.Instance.LevelActual];
            
            
            // load level :: add bubble
            foreach (Bubble bub in level.Bubbles)
            {
                Rooflower = 0;
                TimeRoofLower = 0;
                Vector2 PositionBubble = bubbleBox((int)bub.pos_bubbleBox.X, (int)bub.pos_bubbleBox.Y);
                Console.WriteLine(bub.isDestroy);
                //bub.isDestroy = false;
                bub.Position = PositionBubble;
                pasteBubble(bub);
                //bubbleStuck.Add(b);
            }
            
        }

        private void shootBubble()
        {
            if (bubbleLaunched != null && !bubbleLaunched.moving)
            {
                bubbleLaunched.moving = true;

                Matrix matrix = Matrix.CreateRotationZ(rotation);
                bubbleLaunched.Direction.X += matrix.M12 * 1.5f;
                bubbleLaunched.Direction.Y -= matrix.M11 * 1.5f;
                Timecount = 0;
            }

        }

    }

}
