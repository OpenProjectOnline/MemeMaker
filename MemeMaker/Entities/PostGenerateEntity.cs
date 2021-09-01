using System.Collections.Generic;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class PostGenerateEntity
    {
        [JsonProperty("imageId")] 
        public string ImageId { get; set; }
        
        [JsonProperty("imageCurrentPath")] 
        public string ImageCurrentPath { get; set; }
        
        [JsonProperty("quoteText")] 
        public string QuoteText { get; set; }
        
        [JsonProperty("quoteId")] 
        public string QuoteId { get; set; }
        
        [JsonProperty("quoteCurrentPath")] 
        public string QuoteCurrentPath { get; set; }
        
        [JsonProperty("Id")] 
        public string Id { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("currentPath")]
        public string CurrentPath { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}