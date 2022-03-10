using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BobbleShooter
{
    public class Particle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngleVelocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public Int16 TimeVelocity { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angleVelocity, Color color, float size, Int16 timeVelocity)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngleVelocity = angleVelocity;
            Color = color;
            Size = size;
            TimeVelocity = timeVelocity;
        }

        public void Update()
        {
            TimeVelocity--;
            Position += Velocity;
            Angle += AngleVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle _rectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, _rectangle, Color, Angle, _origin, Velocity, SpriteEffects.None, 0f);
        }
    }
}
