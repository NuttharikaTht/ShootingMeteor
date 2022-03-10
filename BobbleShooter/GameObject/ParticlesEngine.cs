using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BobbleShooter
{
    public class ParticlesEngine
    {
        private Random _random;
        public Vector2 pos_Transmitter { get; set; }
        public List<Particle> particles;
        private List<Texture2D> Textures;
        public bool RandomParticle = false;

        public ParticlesEngine(List<Texture2D> textures, Vector2 position)
        {
            pos_Transmitter = position;
            this.Textures = textures;
            this.particles = new List<Particle>();
            _random = new Random();
        }

        public ParticlesEngine()
        {
            this.particles = new List<Particle>();
            this.Textures = new List<Texture2D>();
            _random = new Random();
        }

        private Particle GenerateRandomParticles(float maxSpeed, Int32 maxTimeVida, Color color)
        {
            Texture2D texture = Textures[_random.Next(Textures.Count)];
            Vector2 position = pos_Transmitter;
            Vector2 speed = new Vector2(1f * (float)(_random.NextDouble() * 2 - 1), 1f * (float)(_random.NextDouble() * 2 - 1));
            float angle = 0;
            float angleSpeed = 0.1f * (float)(_random.NextDouble() * 2 - 1);
            float size = (float)_random.NextDouble() * maxSpeed;
            Int16 TimeVelocity = Convert.ToInt16(_random.Next(maxTimeVida));
            return new Particle(texture, position, speed, angle, angleSpeed, color, size, TimeVelocity);
        }

        public void addParticle(Texture2D texture, Vector2 position, Vector2 speed, float angle, float angleSpeed, Color color, float size, Int16 TimeVelocity)
        {
            particles.Add(new Particle(texture, position, speed, angle, angleSpeed, color, size, TimeVelocity));
        }

        public void startParticle(Int32 quantitytexture, float maxSpeed, Int32 maxTimeVida, Color color)
        {
            for(int i = 0; i < quantitytexture; i++)
            {
                particles.Add(GenerateRandomParticles(maxSpeed, maxTimeVida, color));
            }
        }

        public void addTexture(Texture2D texture)
        {
            Textures.Add(texture);
        }

        public void Update()
        {
            for(Int32 particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TimeVelocity <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
                
                
            }
            
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            for(Int32 index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(_spriteBatch);
            }
        }
    }
}
