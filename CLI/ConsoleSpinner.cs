using System;
using System.Threading;
using System.Threading.Tasks;

namespace CLI
{
    public class ConsoleSpiner
    {
        private int _counter = 0;
        private CancellationTokenSource _source;

        public void Turn()
        {
            switch (_counter++ % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            try
            {
                Console.SetCursorPosition(Math.Max(Console.CursorLeft - 1, 0), Console.CursorTop);
            }
            catch (Exception e)
            {
                // it seems console doesn't support animation
                _source.Cancel();
            }
        }

        public async Task Start()
        {
            _source = new CancellationTokenSource();
            while (true)
            {
                if (_source.IsCancellationRequested)
                {
                    return;
                }
                Turn();
                await Task.Delay(50, _source.Token);
            }
        }

        public void Stop()
        {
            _source.Cancel();
        }
    }
}