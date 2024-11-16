using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualTrader;

namespace AbstractTrader
{
    class TradeProcessorVersion2 
    {
        // Implementing the LogMessage method
        protected void LogMessage(string message, params object[] args)
        {
            Console.WriteLine(message, args);
            // added for Request 408
            using (StreamWriter logfile = File.AppendText("log.xml"))
            {
                logfile.WriteLine("<log>" + string.Format(message, args) + "</log>");
            }
        }

        // The ProcessTrades method, which processes a stream of trade data
        public virtual void ProcessTrades(Stream stream)
        {
            LogMessage("INFO: Starting trade processing version 2.");

            // Step 1: Read trade data from the stream
            var lines = ReadTradeData(stream);

            // Step 2: Parse the trade data into trade records
            var trades = ParseTrades(lines);

            // Step 3: Store the parsed trades (simulated here, would usually be saved to a database)
            StoreTrades(trades);

            LogMessage("INFO: Trade processing version 2 completed.");
        }

        protected override IEnumerable<string> ReadTradeData(Stream stream)
        {
            LogMessage("INFO: Simulating ReadTradeData version 2");
            var tradeData = new List<string>();
            // Simulate reading data (could read data from the stream here)
            LogMessage("INFO: {0} lines of trade data read", tradeData.Count);
            return tradeData;
        }

        protected override IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> tradeData)
        {
            LogMessage("INFO: Simulating ParseTrades version 2");
            var trades = new List<TradeRecord>();
            // Simulate parsing trades (could iterate over tradeData and parse it here)
            LogMessage("INFO: {0} trades parsed", trades.Count);
            return trades;
        }

        protected override void StoreTrades(IEnumerable<TradeRecord> trades)
        {
            LogMessage("INFO: Simulating database connection in StoreTrades version 2");
            // Simulate storing trades (could simulate database interaction here)
            LogMessage("INFO: {0} trades processed", trades.Count());
        }

        public void ProcessTrades(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
