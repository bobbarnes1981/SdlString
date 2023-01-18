using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StringArt
{
    internal class Renderer
    {
        private int _width;
        private int _height;
        private bool _running;

        private IntPtr _renderer;
        private IntPtr _window;

        private Point[] _points;

        private void initVideo(int videoWidth, int videoHeight)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"Init error: {SDL.SDL_GetError()}");
            }

            _window = SDL.SDL_CreateWindow("StringArt", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, videoWidth, videoHeight, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (_window == IntPtr.Zero)
            {
                Console.WriteLine($"Window error: {SDL.SDL_GetError()}");
            }

            _renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (_renderer == IntPtr.Zero)
            {
                Console.WriteLine($"Renderer error: {SDL.SDL_GetError()}");
            }
        }

        private void setWindowTitle()
        {
            SDL.SDL_SetWindowTitle(_window, $"StringArt");
        }

        private void checkEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        _running = false;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        checkKey(e.key);
                        break;
                }
            }
        }

        private void checkKey(SDL.SDL_KeyboardEvent e)
        {
            switch (e.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_ESCAPE:
                    _running = false;
                    break;
            }
            setWindowTitle();
        }

        private void clearScreen()
        {
            if (SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255) < 0)
            {
                Console.WriteLine($"Colour error: {SDL.SDL_GetError()}");
            }

            if (SDL.SDL_RenderClear(_renderer) < 0)
            {
                Console.WriteLine($"Clear error: {SDL.SDL_GetError()}");
            }
        }

        private double degToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private double radToDeg(double rad)
        {
            return rad / (Math.PI / 180);
        }

        private void render()
        {
            drawArc(200, 0, 90, 0xFF, 0x00, 0x00);
            drawArc(200, 90, 180, 0x00, 0xFF, 0x00);
            drawArc(200, 180, 270, 0x00, 0x00, 0xFF);
            drawArc(200, 270, 360, 0xFF, 0xFF, 0xFF);

            SDL.SDL_SetRenderDrawColor(_renderer, 0xFF, 0xFF, 0xFF, 0xFF);

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

                SDL.SDL_RenderDrawLine(_renderer, l1.X, l1.Y, l2.X, l2.Y);
            }
        }

        private void drawArc(int radius, int start, int end, byte r, byte g, byte b)
        {
            int x = 300;
            int y = 300;

            for (int angle = start; angle < end; angle += 10)
            {
                var coord = calcArcPoint(radius, angle);
                SDL.SDL_SetRenderDrawColor(_renderer, r, g, b, 0xFF);
                SDL.SDL_RenderDrawPoint(_renderer, (int)(x + coord.Item1), (int)(y + coord.Item2));
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

        public void Run(int videoWidth, int videoHeight)
        {
            int count = 5;
            int radius = 195;
            int x = 300;
            int y = 300;
            _points = new Point[360 / count];
            for (int i = 0; i < 360/count; i++)
            {
                var coord = calcArcPoint(radius, i * count);
                _points[i] = new Point((int)(x + coord.Item1), (int)(y + coord.Item2));
            }


            _width = videoWidth;
            _height = videoHeight;

            _running = true;

            initVideo(videoWidth, videoHeight);

            setWindowTitle();

            while (_running)
            {
                checkEvents();

                clearScreen();

                render();

                SDL.SDL_RenderPresent(_renderer);
            }

            SDL.SDL_DestroyRenderer(_renderer);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }
}
