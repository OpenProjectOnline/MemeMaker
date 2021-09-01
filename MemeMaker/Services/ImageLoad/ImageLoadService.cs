using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MemeMaker.Entities;
using Newtonsoft.Json;

namespace MemeMaker.Services.ImageLoad
{
    public class ImageLoadService
    {
        public async Task<IReadOnlyList<ImageLoadEntity>> ProcessAsync()
        {
            List<ImageLoadEntity> list = new List<ImageLoadEntity>();

            string[] files = Directory.GetFiles("data/images", "*.json");
            foreach (string file in files)
            {
                try
                {
                    ImageLoadEntity response = JsonConvert.DeserializeObject<ImageLoadEntity>(await File.ReadAllTextAsync(file));
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