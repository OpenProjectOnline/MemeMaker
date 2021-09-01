using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class ImageSearchEntity
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        
        [JsonProperty("tags")] 
        public List<string> Tags { get; set; } = new List<string>();

        [JsonProperty("number")] 
        public string Number { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("currentPath")]
        public string CurrentPath { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}