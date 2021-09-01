using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class ImageDownloadEntity
    {
        [JsonProperty("parseId")] 
        public string ParseId { get; set; }
        
        [JsonProperty("parseCurrentPath")] 
        public string ParseCurrentPath { get; set; }
        
        [JsonProperty("searchId")] 
        public string SearchId { get; set; }
        
        [JsonProperty("searchCurrentPath")] 
        public string SearchCurrentPath { get; set; }
        
        [JsonProperty("Id")] 
        public string Id { get; set; }
        
        [JsonProperty("tags")] 
        public List<string> Tags { get; set; } = new List<string>();

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("currentPath")]
        public string CurrentPath { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}