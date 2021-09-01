using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class ImageParseEntity
    {
        [JsonProperty("searchId")] 
        public string SearchId { get; set; }
        
        [JsonProperty("searchCurrentPath")] 
        public string SearchCurrentPath { get; set; }
        
        [JsonProperty("id")] 
        public string Id { get; set; }
        
        [JsonProperty("tags")] 
        public List<string> Tags { get; set; } = new List<string>();
        
        [JsonProperty("uri")] 
        public string Uri { get; set; }

        [JsonProperty("currentPath")]
        public string CurrentPath { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}