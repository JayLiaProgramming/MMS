using System.Collections.Concurrent;

namespace MmsEngine.Communications.Pattern
{
    internal class Producer
    {
        private BlockingCollection<string> _collection;
        public Producer(BlockingCollection<string> collection)
        {
            _collection = collection;
        }
        public void AddData(string data) => _collection.Add(data);
    }
}
