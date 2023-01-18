using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArt
{
    internal class Chart
    {
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        private int count = 10;

        private Point[] _points;

        public Chart(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;

            _points = new Point[((_width) + (_height)) / count];

            int counter = 0;
            for (int y2 = 0; y2 < _height / count; y2++)
            {
                _points[counter] = new Point(_x, _y + (y2 * count));
                counter++;
            }

            for (int x2 = 0; x2 < _width / count; x2++)
            {
                _points[counter] = new Point(_x + (x2 * count), _y);
                counter++;
            }

        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);

            int offset = 30;
            for (int i = 0; i < _height / count; i++)
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

    }
}
