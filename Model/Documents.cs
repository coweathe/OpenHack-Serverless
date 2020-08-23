// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
using System.Collections.Generic;

namespace BFYOC {
    public class Document    {
        public string id { get; set; } 
        public double score { get; set; } 
    }

    public class Root    {
        public List<Document> documents { get; set; } 
        public List<object> errors { get; set; } 
    }
}