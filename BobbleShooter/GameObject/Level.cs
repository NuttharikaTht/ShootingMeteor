using System;
using System.Collections.Generic;

namespace BobbleShooter
{
    public class Level
    {
        public List<Bubble> Bubbles { get; private set; }

        public Level(List<Bubble> bubbles)
        {
            Bubbles = bubbles;
        }
    }

    
}
