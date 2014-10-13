using System;
using System.Collections.Generic;
using Serilog;

namespace LMAX
{
    public class Simulator
    {
        private readonly Action<SellItem> publish;
        private readonly Dictionary<Guid, SellItem> objects = new Dictionary<Guid, SellItem>();
        private readonly Random random = new Random((int)DateTime.Now.Ticks);
        private long ackCount;
        private bool isRunning;

        public Simulator(int count, Action<SellItem> publish)
        {
            this.publish = publish;
            if (count <= 0)
            {
                throw new ArgumentException("count must be greater than 0", "count");
            }

            Log.Information("Creating {count} simulated items", count);

            for (int i = 0; i < count; i++)
            {
                var item = new SellItem
                {
                    Id = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    ProductId = DateTimeOffset.UtcNow.Millisecond,
                    Price = random.NextDouble() * 100.0
                };

                objects.Add(item.Id, item);
            }
        }

        public long AckCount
        {
            get { return ackCount; }
        }

        public void Start()
        {
            isRunning = true;
            foreach (var item in objects.Values)
            {
                publish(item);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void Ack(Guid id)
        {
            var obj = objects[id];

            ackCount++;

            if (isRunning)
            {
                publish(obj);
            }
        }
    }
}