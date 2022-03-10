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

        public static Int32 bubbleSize = 60;

        public static Int32 limitRight = 1070;
        public static Int32 limitLeft = 530;
        public static Int32 limitUp = 40;
        public static Int32 limitDown = 460;

        public Vector2 positionBubbleLaunch;
        public static List<Bubble> bubbleStuck { get; private set; }
        public static List<Bubble> bubbleFall { get; private set; }
        public static List<Color> colors; 
       
        
        Vector2 locationPointer;

        Texture2D launcher;
        Texture2D bubble;
        Texture2D pointText;
        Texture2D BG;
        Texture2D box;
        //Texture2D rightLineTxtr;
        //Texture2D leftLineTxtr;

        Bubble bubbleLaunched;
        Bubble nextBubble;

        int x, y;
        Vector2 dimensions;
        float elapsed;
        float rotation;

        ParticlesEngine particles;
        Texture2D textParticle;

        Texture2D textRoof;
        Int32 TimeRooflower = 0;
        public static Int32 Rooflower = 0;
        float Timecount =0;
        


        float Time = 0;
        float TimeShow = 0;
        SpriteFont font;
        Texture2D LoseT;



        Rectangle leftLine, topLine, rightLine, downLine;

        // game state :: initialize
        public enum gameState
        {
            play, // play #initial state
            lose, // lose #if bubble near limit
            win, // win #if no bubble 
        }

        // game state :: create game state
        gameState state;

        //load level :: The variable that will store the current level and the Level Manager is created
        AdministratorLevels levelManager;
        Int32 levelActual;


        public bubbleShooter()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            locationPointer = new Vector2(800, 520);
            rotation = 0f;
            elapsed = 0f;
           
            //bubble สำหรับยิง
            positionBubbleLaunch = new Vector2(800-30, 520-30);
            dimensions = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            
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

            // load level :: move there to loadLevel already so it safe to remove
            //bubbleLaunched = new Bubble(positionBubbleLaunch, randomColor());
            //generateBubble();

            //line

            leftLine = new Rectangle(limitLeft, limitUp, 1, limitDown - limitUp + 100);
            topLine = new Rectangle(limitLeft, limitUp, limitRight - limitLeft, 1);
            rightLine = new Rectangle(limitRight, limitUp, 1, limitDown - limitUp + 100);
            downLine = new Rectangle(limitLeft, limitDown + 100, limitRight - limitLeft, 1);
            
            state = gameState.play;

            // load level :: Initialize method, the name of the file is sent, first the namespace and then the name of the file
            levelManager = new AdministratorLevels("BobbleShooter.gameLevels.txt");
            levelActual = 0;
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            launcher = this.Content.Load<Texture2D>("spaceship");
            bubble = this.Content.Load<Texture2D>("bubble");
            pointText = this.Content.Load<Texture2D>("pointText");
            BG = this.Content.Load<Texture2D>("BG");

            //leftLineTxtr = this.Content.Load<Texture2D>("leftLine");
            //rightLineTxtr = this.Content.Load<Texture2D>("rightLine");

            box = this.Content.Load<Texture2D>("box");

            textParticle = this.Content.Load<Texture2D>("explosion");
            particles = new ParticlesEngine();
            particles.addTexture(textParticle);
            particles.RandomParticle = true;
            // load level
            if (levelManager != null)
            {
                
                loadLevel();
                
            }
            font = Content.Load<SpriteFont>("File");
            textRoof = Content.Load<Texture2D>("Roof");
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (state == gameState.play) // game state :: check is play state?
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

                    //if (bubbleLaunched.bubbleRect.Intersects(topLine))
                    //{

                        if (bubbleLaunched.Position.Y <= limitUp + (Rooflower * bubbleSize))
                        {

                        //bubbleLaunched.Direction = Vector2.Zero;
                        bubbleLaunched.findNearest();
                        pasteBubble(bubbleLaunched);
                        //bubbleStuck.Add(bubbleLaunched);
                        bubbleLaunched = null;


                    }
                //}

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

                foreach (Bubble bubble1 in destroyBubbles)
                {
                    //ฟองอากาศจะถูกลบออก
                    bubbleFall.Remove(bubble1);

                }

                if (bubbleLaunched == null)
                {
                    generateBubble();
                }

                //ยิงauto เมื่อไม่่ขยับ
                Timecount += (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                if (Timecount > 20)
                {
                    //แสดงtext
                    shootBubble();
                   
                }
                else
                {
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Left ))
                    {
                        Timecount = 0;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        Timecount = 0;
                    }
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
                    
                    if (TimeRooflower == 10)
                    {
                        Rooflower++;
                        //LimitUp += bubblesize;
                        TimeRooflower = 0;
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
            else if (state == gameState.win) // load level :: load next level
            {
                if (levelActual < (levelManager.Levels.Count - 1))
                {
                    
                    levelActual++;
                    
                    loadLevel();
                    
                    state = gameState.play;
                    
                }
            }
            else if(state == gameState.lose)
            {
                Time = 0;
            }
          
                base.Update(gameTime);
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


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Beige);
            _spriteBatch.Begin();

            _spriteBatch.Draw(BG, new Vector2(0, 0), Color.White);

            //playable area bg
            _spriteBatch.Draw(box, new Vector2(limitLeft , limitUp - 20), Color.White);

            //time
            _spriteBatch.DrawString(font, "Time " + Time.ToString("00:00"), new Vector2(limitLeft-100, limitUp), Color.White);
            _spriteBatch.DrawString(font, "Best Time " + Time.ToString("00:00"), new Vector2(limitLeft - 130, limitUp +30), Color.White);

            for (int y = 0; y < 8; y++)
            {
                for(int x = 0; x<10; x++)
                {
                    //draw roof rect and roof textr
                    Rectangle dRect = new Rectangle((x * bubbleSize) + limitLeft + (((y+Rooflower) % 2) * (bubbleSize / 2)) , (y * bubbleSize) + limitUp, bubbleSize, bubbleSize);
                    _spriteBatch.Draw(textRoof, new Vector2(limitLeft, (limitUp + (Rooflower * bubbleSize)) - 20), null, Color.White);
  
                }
            }
            
            //  shooter
            _spriteBatch.Draw(launcher, locationPointer, null, Color.White, rotation, new Vector2(launcher.Width / 2, launcher.Height / 2), 1f, SpriteEffects.None, 1);

            //bubble for shoot
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

            

            _spriteBatch.End();
         
            base.Draw(gameTime);
        }

        
        public void pasteBubble(Bubble bubble)
        {
            
                List<Bubble> groupEquals = bubble.findEquals();
                TimeRooflower++;

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
            if (TimeRooflower == 10)
            {
                    Rooflower++;
                    TimeRooflower = 0;
                
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
                state = gameState.win;
            }
            else
            {
                foreach (Bubble b in bubbleStuck)
                {
                    if (b.pos_bubbleBox.Y > 7) // check bubble over limit :: lose
                    {
                        state = gameState.lose;
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
            
            bubbleLaunched = new Bubble(positionBubbleLaunch, colors[new Random().Next(0, 7)]);
            generateBubble();
            
            Level level = levelManager.Levels[levelActual];
            
            
            // load level :: add bubble
            foreach (Bubble bub in level.Bubbles)
            {
              Rooflower = 0;
            TimeRooflower = 0;
                Vector2 posicionBurbuja = bubbleBox((int)bub.pos_bubbleBox.X, (int)bub.pos_bubbleBox.Y);
                bub.Position = posicionBurbuja;
                pasteBubble(bub);
                //bubbleStuck.Add(b);
            }
            
        }

      

    }

}
