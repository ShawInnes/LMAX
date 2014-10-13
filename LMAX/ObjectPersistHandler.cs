using System;
using System.IO;
using Disruptor;

namespace LMAX
{
    public class ObjectPersistHandler : IEventHandler<SellItem>, IDisposable
    {
        private readonly StreamWriter _writer;

        public ObjectPersistHandler(string path)
        {
            _writer = new StreamWriter(path, true);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void OnNext(SellItem data, long sequence, bool endOfBatch)
        {
            AppendData(data);
            if (endOfBatch)
            {
                _writer.Flush();
            }
        }

        private void AppendData(SellItem data)
        {
            _writer.Write(data.Id);
            _writer.Write(' ');
            _writer.Write(data.CustomerId);
            _writer.Write(' ');
            _writer.Write(data.ProductId);
            _writer.Write(' ');
            _writer.Write(data.Price);
            _writer.Write(Environment.NewLine);
        }
    }
}