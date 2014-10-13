using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;
using Serilog;

namespace LMAX
{
    public class SellItem
    {
        public Guid CustomerId;
        public Guid Id;
        public double Price;
        public int ProductId;
    }

    internal class Program
    {
        private const int RingBufferSize = 1 << 16; // 2048
        private const int ObjectsCount = RingBufferSize / 2;

        private static EventPublisher<SellItem> publisher;

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole(outputTemplate: "{Level} - {Message}{NewLine}")
                .CreateLogger();

            string storageLocation = Path.Combine(Environment.CurrentDirectory, "data.log");

            DateTime start = DateTime.Now;

            var disruptor = new Disruptor<SellItem>(() => new SellItem(), RingBufferSize, TaskScheduler.Default);

            var simulator = new Simulator(ObjectsCount, item => publisher.PublishEvent((entry, sequenceNo) =>
            {
                entry.Id = item.Id;
                entry.CustomerId = item.CustomerId;
                entry.ProductId = item.ProductId;
                entry.Price = item.Price;
                return entry;
            }));

            var priceHandler = new PriceHandler();
            var acknowledgementHandler = new AcknowledgementHandler(simulator);
            var consoleLogHandler = new ConsoleLogHandler();
            using (var persistHandler = new ObjectPersistHandler(storageLocation))
            {
                disruptor.HandleEventsWith(persistHandler)
                    .Then(consoleLogHandler, priceHandler)
                    .Then(acknowledgementHandler);

                RingBuffer<SellItem> ringBuffer = disruptor.Start();

                publisher = new EventPublisher<SellItem>(ringBuffer);

                simulator.Start();
                
                Log.Information("sleeping");

                Thread.Sleep(10000);

                simulator.Stop();

                disruptor.Shutdown();
            }

            Log.Information("LMAX Disruptor Done");

            Log.Information(priceHandler.GetSummary());
            Log.Information("Processed {0} events. Speed: {1:#.##} events per second.", simulator.AckCount, simulator.AckCount / (DateTime.Now - start).TotalSeconds);

            Console.ReadLine();
        }
    }
}