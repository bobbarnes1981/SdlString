using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArt
{
    internal class Circle
    {
        private int _x;
        private int _y;
        private int _radius;

        private Point[] _points;

        public Circle(int x, int y, int radius)
        {
            _x = x;
            _y = y;
            _radius = radius;

            int count = 5;

            _points = new Point[360 / count];
            for (int i = 0; i < 360 / count; i++)
            {
                var coord = calcArcPoint(radius, i * count);
                _points[i] = new Point((int)(_x + coord.Item1), (int)(_y + coord.Item2));
            }
        }

        public void Render(IntPtr renderer)
        {
            drawArc(renderer, _x, _y, _radius + 5, 0, 90, 0xFF, 0x00, 0x00);
            drawArc(renderer, _x, _y, _radius + 5, 90, 180, 0x00, 0xFF, 0x00);
            drawArc(renderer, _x, _y, _radius + 5, 180, 270, 0x00, 0x00, 0xFF);
            drawArc(renderer, _x, _y, _radius + 5, 270, 360, 0xFF, 0xFF, 0xFF);

            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);

            int offset = 20;
            for (int i = 0; i < _points.Length; i++)
            {
                var l1 = _points[i];
                var j = i + offset;
                if (j >= _points.Length)
                {
                    j -= _points.Length;
                }
                var l2 = _points[j];

                SDL.SDL_RenderDrawLine(renderer, l1.X, l1.Y, l2.X, l2.Y);
            }
        }

        private void drawArc(IntPtr renderer, int x, int y, int radius, int start, int end, byte r, byte g, byte b)
        {
            for (int angle = start; angle < end; angle += 10)
            {
                var coord = calcArcPoint(radius, angle);
                SDL.SDL_SetRenderDrawColor(renderer, r, g, b, 0xFF);
                SDL.SDL_RenderDrawPoint(renderer, (int)(x + coord.Item1), (int)(y + coord.Item2));
            }
        }

        private (double, double) calcArcPoint(int radius, int angle)
        {
            // SOH
            // sin(angle)*h=o
            double opposite = Math.Sin(degToRad(angle)) * radius;

            // a2+b2=c2
            // a2=c2-b2
            // a = sqrt(c2-b2)
            double a = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(opposite, 2));

            // hacky
            if (angle >= 0 && angle < 90)
            {
                a = a * -1;
            }
            if (angle >= 270 && angle < 360)
            {
                a = a * -1;
            }

            return (a, opposite);
        }

        private double degToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private double radToDeg(double rad)
        {
            return rad / (Math.PI / 180);
        }
    }
}
