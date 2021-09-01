using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class Config
    {
        [JsonProperty("serpapi.com", Required = Required.Always)] 
        public SerpApiComConfig SerpApiCom { get; set; }
        
        [JsonProperty("vk.com", Required = Required.Always)] 
        public VkComConfig VkCom { get; set; }

        [JsonProperty("template", Required = Required.Always)] 
        public string Template { get; set; }
        
        [JsonProperty("watermark", Required = Required.Always)] 
        public string Watermark { get; set; }

        [JsonProperty("tags", Required = Required.Always)] 
        public IEnumerable<string> Tags { get; set; }
        
        public class SerpApiComConfig
        {
            [JsonProperty("key", Required = Required.Always)]
            public string Key { get; set; }
        }

        public class VkComConfig
        {
            [JsonProperty("app id", Required = Required.Always)]
            public ulong AppId { get; set; }
        
            [JsonProperty("login", Required = Required.Always)]
            public string Login { get; set; }
        
            [JsonProperty("password", Required = Required.Always)]
            public string Password { get; set; }
        
            [JsonProperty("owner id", Required = Required.Always)]
            public long OwnerId { get; set; }
        }
    }
}