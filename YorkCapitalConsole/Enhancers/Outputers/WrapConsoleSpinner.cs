using System;
using System.Threading;

namespace Wrappers.Outputers
{
    public class WrapConsoleSpinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter = 0;
        private readonly int left;
        private readonly int top;
        private readonly int delay;
        private readonly Thread thread;

        public WrapConsoleSpinner(int delay = 100)
        {          
            this.left = Console.CursorLeft;
            this.top = Console.CursorTop;
            this.delay = delay;
            thread = new Thread(Spin);
        }

        public bool IsActive { get; private set; }

        public void Start()
        {
            IsActive = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            IsActive = false;
            thread.Abort();
            Draw(' ');
            Console.WriteLine();
        }

       

        private void Spin()
        {
            while (IsActive)
            {
                Turn();
                Thread.Sleep(delay);                
            }
        }

        private void Draw(char c)
        {
            if (IsActive)
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(c);
            }
        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
