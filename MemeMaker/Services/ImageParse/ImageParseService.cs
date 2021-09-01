using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MemeMaker.Entities;
using Newtonsoft.Json.Linq;

namespace MemeMaker.Services.ImageParse
{
    public class ImageParseService
    {
        private readonly ImageSearchEntity[] searchEntities;

        public ImageParseService(params ImageSearchEntity[] searchEntities)
        {
            this.searchEntities = searchEntities;
        }

        public async Task<IReadOnlyList<ImageParseEntity>> ProcessAsync()
        {
            List<ImageParseEntity> list = new List<ImageParseEntity>();

            foreach (ImageSearchEntity entity in this.searchEntities)
                list.AddRange(await this.ProcessAsync(entity));

            return list;
        }

        public async Task<IReadOnlyList<ImageParseEntity>> ProcessAsync(ImageSearchEntity entity)
        {
            List<ImageParseEntity> list = new List<ImageParseEntity>();

            try
            {
                const string dir = "data/parsings";
                Directory.CreateDirectory(dir);
                const string ext = "json";
                
                JObject data = JObject.Parse(await File.ReadAllTextAsync(entity.Path));
                JArray items = data.Value<JArray>("images_results");//google
                // JArray items = data.Value<JArray>("inline_images");//yandex,duck
                foreach (JToken item in items)
                {
                    try
                    {
                        string original = item.Value<string>("original");//google
                        // string original = item.Value<string>("image");//yandex,duck
                        if (string.IsNullOrWhiteSpace(original))
                            continue;

                        string id = $"0x{original.GetHashCode().ToString()}.{Guid.NewGuid().ToString()}";
                        
                        ImageParseEntity result = new ImageParseEntity()
                        {
                            SearchId = entity.Id,
                            SearchCurrentPath = entity.CurrentPath,
                            Id = id,
                            Tags = entity.Tags,
                            Uri = original,
                            CurrentPath = $"{dir}/{id}.{ext}",
                        };
                        
                        await File.WriteAllTextAsync(result.CurrentPath, result.ToJson());
                        
                        list.Add(result);
                    }
                    catch (Exception ex)
                    {
                        await Console.Error.WriteLineAsync(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return null;
            }

            return list;
        }
    }
}