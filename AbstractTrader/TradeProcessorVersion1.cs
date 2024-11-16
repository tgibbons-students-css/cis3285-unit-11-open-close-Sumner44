using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualTrader;

namespace AbstractTrader
{
    public class TradeProcessorVersion1 
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
            LogMessage("INFO: Starting trade processing.");

            // Step 1: Read trade data from the stream
            var lines = ReadTradeData(stream);

            // Step 2: Parse the trade data into trade records
            var trades = ParseTrades(lines);

            // Step 3: Store the parsed trades (simulated here, would usually be saved to a database)
            StoreTrades(trades);

            LogMessage("INFO: Trade processing completed.");
        }

        protected override IEnumerable<string> ReadTradeData(Stream stream)
        {
            LogMessage("INFO: ReadTradeData version 1");
            var tradeData = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tradeData.Add(line);
                }
            }
            LogMessage("INFO: {0} lines of trade data read", tradeData.Count);
            return tradeData;
        }

        protected override IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> tradeData)
        {
            LogMessage("INFO: ParseTrades version 1");
            var trades = new List<TradeRecord>();
            var lineCount = 1;
            foreach (var line in tradeData)
            {
                var fields = line.Split(new char[] { ',' });

                var trade = MapTradeDataToTradeRecord(fields);

                trades.Add(trade);

                lineCount++;
            }
            LogMessage("INFO: {0} trades parsed", trades.Count);
            return trades;
        }

        protected TradeRecord MapTradeDataToTradeRecord(string[] fields)
        {
            var sourceCurrencyCode = fields[0].Substring(0, 3);
            var destinationCurrencyCode = fields[0].Substring(3, 3);
            var tradeAmount = int.Parse(fields[1]);
            var tradePrice = decimal.Parse(fields[2]);
            const float LotSize = 100000f;
            var trade = new TradeRecord
            {
                SourceCurrency = sourceCurrencyCode,
                DestinationCurrency = destinationCurrencyCode,
                Lots = tradeAmount / LotSize,
                Price = tradePrice
            };

            return trade;
        }

        protected override void StoreTrades(IEnumerable<TradeRecord> trades)
        {
            LogMessage("INFO: Simulating database connection in StoreTrades");
            // Not really connecting to database in this sample
            LogMessage("INFO: {0} trades processed", trades.Count());
        }

        public void ProcessTrades(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
