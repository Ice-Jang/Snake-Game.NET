using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameV2
{
    internal class Particle
    {
        public float X;
        public float Y;
        public float VX;
        public float VY;
        public int Life;
        public Color Color;

        public Particle(float x, float y, float vx, float vy, int life, Color color)
        {
            X = x; Y = y;
            VX = vx; VY = vy;
            Life = life;
            Color = color;
        }
    }
}
