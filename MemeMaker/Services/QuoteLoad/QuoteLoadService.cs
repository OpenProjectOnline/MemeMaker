using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MemeMaker.Entities;
using Newtonsoft.Json;

namespace MemeMaker.Services.QuoteLoad
{
    public class QuoteLoadService
    {
        public async Task<IReadOnlyList<QuoteLoadEntity>> ProcessAsync()
        {
            List<QuoteLoadEntity> list = new List<QuoteLoadEntity>();

            string[] files = Directory.GetFiles("data/quotes", "*.json");
            foreach (string file in files)
            {
                try
                {
                    QuoteLoadEntity response = JsonConvert.DeserializeObject<QuoteLoadEntity>(await File.ReadAllTextAsync(file));
                    list.Add(response);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync(ex.Message);
                    throw;
                }
            }

            return list;
        }
    }
}