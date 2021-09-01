using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MemeMaker.Entities;
using Newtonsoft.Json.Linq;
using SerpApi;

namespace MemeMaker.Services.ImageSearch
{
    public class ImageSearchService
    {
        private readonly string key;
        private readonly int? number;
        private readonly string[] tags;
        
        public ImageSearchService(string key, int? number, params string[] tags)
        {
            this.key = key;
            this.number = number;
            this.tags = tags;
        }
        
        public ImageSearchService(string key, params string[] tags) : this(key, null , tags)
        {
        }

        public async Task<IReadOnlyList<ImageSearchEntity>> ProcessAsync(int? num = null)
        {
            List<ImageSearchEntity> list = new List<ImageSearchEntity>();
            
            foreach (string tag in this.tags)
            {
                ImageSearchEntity? entity = await this.ProcessAsync(this.GetNum(num), tag);
                if (entity is {})
                    list.Add(entity);
            }

            return list;
        }

        public async Task<ImageSearchEntity?> ProcessAsync(int num, string tag)
        {
            try
            {
                const string dir = "data/searches";
                Directory.CreateDirectory(dir);
                const string ext = "json";
                
                Hashtable request = new Hashtable
                {
                    // { "engine", "duckduckgo" },//duck
                    // { "q", tag },//duck
                    // { "engine", "yandex" },//yandex
                    // { "text", tag },//yandex
                    { "engine", "google" },//google
                    { "q", tag },//google
                    { "hl", "ru" },//google
                    { "ijn", $"{num}" },//google
                    { "tbm", "isch" },//google
                };
                GoogleSearch search = new GoogleSearch(request, this.key);
                JObject data = search.GetJson();

                string id = data["search_metadata"].Value<string>("id");

                ImageSearchEntity result = new ImageSearchEntity()
                {
                    Id = id,
                    Number = $"{num}",
                    Tags = new List<string> { tag },
                    Path = $"{dir}/{id}.response.{ext}",
                    CurrentPath = $"{dir}/{id}.{ext}",
                };
                
                await File.WriteAllTextAsync(result.Path, data.ToString());
                await File.WriteAllTextAsync(result.CurrentPath, result.ToJson());
                
                return result;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
                return null;
            }
        }

        private int GetNum(int? num)
        {
            num ??= this.number;
            if (num is {} n)
                return n;
            return 0;
        }
    }
}