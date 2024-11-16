using AbstractTrader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace VirtualTrader
{
    public class TradeProcessor: IProcessTrades 
    {
        // Read trade data from a stream
        public IEnumerable<string> ReadTradeData(Stream stream)
        {
            var tradeData = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tradeData.Add(line);
                }
            }
            return tradeData;
        }

        // Read trade data from a URL
        public IEnumerable<string> ReadURLTradeData(string url)
        {
            var tradeData = new List<string>();
            var client = new WebClient();
            using (var stream = client.OpenRead(url))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tradeData.Add(line);
                }
            }
            return tradeData;
        }

        // Parse trade data into TradeRecord objects
        public IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> tradeData)
        {
            var trades = new List<TradeRecord>();
            var lineCount = 1;
            foreach (var line in tradeData)
            {
                var fields = line.Split(new char[] { ',' });

                if (!ValidateTradeData(fields, lineCount))
                {
                    continue;
                }

                var trade = MapTradeDataToTradeRecord(fields);

                trades.Add(trade);

                lineCount++;
            }

            return trades;
        }

        // Validate trade data for correct format
        private bool ValidateTradeData(string[] fields, int currentLine)
        {
            if (fields.Length != 3)
            {
                LogMessage("WARN: Line {0} malformed. Only {1} field(s) found.", currentLine, fields.Length);
                return false;
            }

            if (fields[0].Length != 6)
            {
                LogMessage("WARN: Trade currencies on line {0} malformed: '{1}'", currentLine, fields[0]);
                return false;
            }

            int tradeAmount;
            if (!int.TryParse(fields[1], out tradeAmount))
            {
                LogMessage("WARN: Trade amount on line {0} not a valid integer: '{1}'", currentLine, fields[1]);
                return false;
            }

            // Added boundaries for trade amount
            if (tradeAmount < 1000)
            {
                LogMessage("WARN: Trade amount on line {0} is below lower bound: '{1}'", currentLine, fields[1]);
                return false;
            }
            if (tradeAmount > 100000)
            {
                LogMessage("WARN: Trade amount on line {0} is above upper bound: '{1}'", currentLine, fields[1]);
                return false;
            }

            decimal tradePrice;
            if (!decimal.TryParse(fields[2], out tradePrice))
            {
                LogMessage("WARN: Trade price on line {0} not a valid decimal: '{1}'", currentLine, fields[2]);
                return false;
            }

            return true;
        }

        // Log messages to console and file
        public void LogMessage(string message, params object[] args)
        {
            Console.WriteLine(message, args);
            using (StreamWriter logfile = File.AppendText("log.xml"))
            {
                logfile.WriteLine("<log>" + string.Format(message, args) + "</log>");
            }
        }

        // Map raw trade data to a TradeRecord object
        public TradeRecord MapTradeDataToTradeRecord(string[] fields)
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

        // Simulate storing trades in a database
        public void StoreTrades(IEnumerable<TradeRecord> trades)
        {
            LogMessage("INFO: Simulating database connection");
            LogMessage("INFO: {0} trades processed", trades.Count());
        }

        // Process trades from the provided stream
        public void ProcessTrades(Stream stream)
        {
            var lines = ReadTradeData(stream);
            var trades = ParseTrades(lines);
            StoreTrades(trades);
        }

        public void ProcessTrades(FileStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
