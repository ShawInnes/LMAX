using Disruptor;
using Serilog;

namespace LMAX
{
    public class ConsoleLogHandler : IEventHandler<SellItem>
    {
        public void OnNext(SellItem data, long sequence, bool endOfBatch)
        {
            if (sequence % 10000 == 0)
            {
/*
                Log.Information("[{seq}] {Id}: {CustomerId}, {ProductId}, {Price}", sequence, data.Id, data.CustomerId,
                    data.ProductId, data.Price);
*/
            }
        }
    }
}