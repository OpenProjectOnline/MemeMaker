using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MemeMaker.Entities;
using MemeMaker.Extensions;
using Newtonsoft.Json;

namespace MemeMaker.Services.QuoteDownload
{
    public class QuoteDownloadService
    {
        private readonly ushort count;
        private readonly ushort chunk;
        private readonly ushort sleep;

        public QuoteDownloadService(ushort count = 100, ushort chunk = 100, ushort sleep = 2000)
        {
            this.count = count;
            this.chunk = chunk;
            this.sleep = sleep;
        }

        public async Task<IReadOnlyList<QuoteDownloadEntity>> ProcessAsync()
        {
            List<QuoteDownloadEntity> list = new List<QuoteDownloadEntity>();

            ushort[] numbers = new ushort[this.count];
            for (ushort i = 0; i < this.count; i++)
                numbers[i] = i;
            
            foreach (IEnumerable<ushort> group in numbers.Chunk(this.chunk))
            {
                List<Task<QuoteDownloadEntity?>> tasks = new List<Task<QuoteDownloadEntity?>>();
                
                foreach (ushort number in group)
                    tasks.Add(ProcessAsync(number));
                await Task.Delay(this.sleep);
                
                list.AddRange(await Task.WhenAll(tasks));
            }

            return list;
        }

        public async Task<QuoteDownloadEntity?> ProcessAsync(ushort index)
        {
            try
            {
                const string dir = "data/quotes";
                Directory.CreateDirectory(dir);
                const string ext = "json";

                string uri = $"https://api.forismatic.com/api/1.0/?method=getQuote&format=json&key={index}";

                using HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                string json = await client.GetStringAsync(uri);
                
                QuoteDownloadEntity result = JsonConvert.DeserializeObject<QuoteDownloadEntity>(json);

                string id = result.GetUniqueHash();
                result.CurrentPath = $"{dir}/{id}.{ext}";

                await File.WriteAllTextAsync(result.CurrentPath, result.ToJson());
                return result;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return null;
            }
        }
    }
}