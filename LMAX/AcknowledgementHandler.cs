using Disruptor;
using Serilog;

namespace LMAX
{
    public class AcknowledgementHandler : IEventHandler<SellItem>
    {

        private readonly Simulator _simulator;

        public AcknowledgementHandler(Simulator simulator)
        {
            _simulator = simulator;
        }

        public void OnNext(SellItem data, long sequence, bool endOfBatch)
        {
            _simulator.Ack(data.Id);
        }
    }
}