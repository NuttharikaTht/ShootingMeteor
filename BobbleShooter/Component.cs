using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BobbleShooter
{
    public abstract class Component
    {
        public abstract void Draw(GameTime gamTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
    }
}
