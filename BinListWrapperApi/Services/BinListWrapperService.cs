using BinListWrapperApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinListWrapperApi.Services
{

    public interface IBinlistWrapperService
    {
     
        Task<IRestResponse<CardInfo>> GetCardDetails(string cardIin);
    }
    public class BinListWrapperService : IBinlistWrapperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BinListWrapperService> _logger;
        private readonly IMemoryCache _memoryCache;
        public BinListWrapperService(IConfiguration configuration, ILogger<BinListWrapperService> logger, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _logger = logger;
            _memoryCache = memoryCache;

        }
        private async Task<IRestResponse<CardInfo>> GetBinListDetails(string CardIin)
        {
           
      

      

             IRestResponse<CardInfo> binlistresponse;
            var BinlistServiceUrl = _configuration["BinlistServiceUrl"];
            var client = new RestClient($"{BinlistServiceUrl}");
       
            var request = new RestRequest(CardIin, Method.GET,DataFormat.Json);
            _logger.LogWarning("_logger: Connecting to Binlist Api");
            binlistresponse = await client.ExecuteAsync<CardInfo>(request);
            _logger.LogInformation($"[BinlistService][GetBinlist][ApiResponse] {binlistresponse.Content}");
            return binlistresponse;


            
        }

        public   async Task<IRestResponse<CardInfo>> GetCardDetails(string cardIin)
        {

            // Check cache for iin key
            var cacheKey = cardIin;

            if (!_memoryCache.TryGetValue(cacheKey, out IRestResponse<CardInfo> result))
            {
                result = await GetBinListDetails(cardIin);

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(6),
                    Priority = CacheItemPriority.Normal,
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);

            }
            return result;
        }

    }
}
