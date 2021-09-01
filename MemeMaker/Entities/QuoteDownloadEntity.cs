using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MemeMaker.Entities
{
    public class QuoteDownloadEntity
    {
        [JsonProperty("quoteText")]
        public string QuoteText { get; set; }
        [JsonProperty("quoteAuthor")]
        public string QuoteAuthor { get; set; }
        [JsonProperty("senderName")]
        public string SenderName { get; set; }
        [JsonProperty("senderLink")]
        public string SenderLink { get; set; }
        [JsonProperty("quoteLink")]
        public string QuoteLink { get; set; }

        [JsonProperty("currentPath")]
        public string CurrentPath { get; set; }

        public string GetUniqueHash()
        {
            try
            {
                return Regex.Match(this.QuoteLink, @"http:\/\/forismatic\.com\/ru\/(?<hash>\S+)\/").Groups["hash"].Value;
            }
            catch (Exception)
            {
                return $"0x{this.QuoteText.GetHashCode().ToString()}";
            }
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}