using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinListWrapperApi.Models
{
    
    public class CardInfo
    {

       

        [JsonProperty("scheme")]
       
        public string Scheme { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("prepaid")]
        public bool Prepaid { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("bank")]
        public Bank Bank { get; set; }
    }

    public partial class Bank
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public partial class Country
    {
    

        [JsonProperty("name")]
        public string Name { get; set; }

 
        [JsonProperty("currency")]
        public string Currency { get; set; }

       
    }

   


   

}
