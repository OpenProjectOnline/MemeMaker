using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MemeMaker.Entities;
using MemeMaker.Extensions;

namespace MemeMaker.Services.ImageDownload
{
    public class ImageDownloadService
    {
        private readonly ImageParseEntity[] parseEntities;

        public ImageDownloadService(params ImageParseEntity[] parseEntities)
        {
            this.parseEntities = parseEntities;
        }

        public async Task<IReadOnlyList<ImageDownloadEntity>> ProcessAsync()
        {
            List<ImageDownloadEntity> list = new List<ImageDownloadEntity>();

            foreach (IEnumerable<ImageParseEntity> group in this.parseEntities.Chunk(50))
            {
                List<Task<ImageDownloadEntity?>> tasks = new List<Task<ImageDownloadEntity?>>();
                
                foreach (ImageParseEntity entity in group)
                    tasks.Add(ProcessAsync(entity));
                
                list.AddRange(await Task.WhenAll(tasks));
            }

            return list;
        }
        
        public async Task<ImageDownloadEntity?> ProcessAsync(ImageParseEntity entity)
        {
            try
            {
                const string dir = "data/images";
                Directory.CreateDirectory(dir);
                string id = $"{entity.Id}.{Guid.NewGuid().ToString()}";
                string ext = Path.GetExtension(entity.Uri);

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.90 Safari/537.36");
                client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                client.DefaultRequestHeaders.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

                ImageDownloadEntity result = new ImageDownloadEntity()
                {
                    Id = id,
                    ParseId = entity.Id,
                    ParseCurrentPath = entity.CurrentPath,
                    SearchId = entity.SearchId,
                    SearchCurrentPath = entity.SearchCurrentPath,
                    Tags = entity.Tags,
                    Path = $"{dir}/{id}{(!string.IsNullOrEmpty(ext) ? $"{ext}" : ".UNDEFINED")}",
                    CurrentPath = $"{dir}/{id}.json",
                };
                
                await using Stream stream = await client.GetStreamAsync(entity.Uri);
                await using FileStream fileStream = new FileStream(result.Path, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(fileStream);
                
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