using System.Text;
using Disruptor;

namespace LMAX
{
    public class PriceHandler : IEventHandler<SellItem>
    {
        private double _totalPrice;

        public void OnNext(SellItem data, long sequence, bool endOfBatch)
        {
            _totalPrice += data.Price;
        }

        public string GetSummary()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Total price: {0}", _totalPrice);
            return builder.ToString();
        }
    }
}