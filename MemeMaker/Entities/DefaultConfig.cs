using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class DefaultConfig
    {
        [JsonProperty("serpapi.com")] 
        public SerpApiComConfig SerpApiCom { get; set; } = new SerpApiComConfig();
        
        [JsonProperty("vk.com")] 
        public VkComConfig VkCom { get; set; } = new VkComConfig();

        [JsonProperty("template")] 
        public string Template { get; set; } = "{0}\n#mm #meme #maker";
        
        [JsonProperty("watermark")] 
        public string Watermark { get; set; } = "MemeMaker";

        [JsonProperty("tags")] 
        public IEnumerable<string> Tags { get; set; } = new List<string>()
        {
            "волк животное",
            "злой волк",
            "бандитские картинки", 
            "приора девушка",
            "mark 2 дрифт",
            "subaru девушка",
            "закат",
            "природа красивая",
            "ночной город",
            "ночные улицы"
        };

        public class SerpApiComConfig
        {
            [JsonProperty("key")]
            public string Key { get; set; }
        }

        public class VkComConfig
        {
            [JsonProperty("app id")]
            public ulong AppId { get; set; }
        
            [JsonProperty("login")]
            public string Login { get; set; }
        
            [JsonProperty("password")]
            public string Password { get; set; }
        
            [JsonProperty("owner id")]
            public long OwnerId { get; set; }
        }
    }
}