using System;
using System.IO;

namespace VirtualTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of TradeProcessor
            IProcessTrades tradeProcessor = new TradeProcessor();

            // Assuming you have a stream from a file or URL
            using (var stream = File.OpenRead("trade_data.csv"))
            {
                tradeProcessor.ProcessTrades(stream);
            }
        }
    }

    internal interface IProcessTrades
    {
        void ProcessTrades(FileStream stream);
    }
}
