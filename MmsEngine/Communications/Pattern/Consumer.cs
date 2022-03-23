using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MmsEngine.Communications.Pattern
{
    internal class Consumer
    {
        private BlockingCollection<string> _collection;
        private CancellationTokenSource _tokenSource;

        public Action Closed;
        public Action<string> ParseMessage;
        //public Action<string> StatusMessage;
        public string Name { get; set; }
        public Consumer(BlockingCollection<string> collection) => _collection = collection;
        public void Process()
        {
            if (_tokenSource == null)
                _tokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (_tokenSource.Token.IsCancellationRequested)
                                break;

                            while (_collection.TryTake(out string data) && !_tokenSource.Token.IsCancellationRequested)
                                ParseMessage?.Invoke(data);
                        }
                        catch (Exception)
                        {
                            //StatusMessage?.Invoke($"{Name} Consumer Process() Error: {ex.Message}");
                        }
                    }
                }
                catch { }
                finally
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                    Closed?.Invoke();
                }
            }, _tokenSource.Token);
        }
        public void Close()
        {
            if (_tokenSource == null)
                Closed?.Invoke();
            else
                _tokenSource.Cancel();
        }
    }
}
