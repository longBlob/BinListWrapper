using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BinListWrapperApi.Models;
using BinListWrapperApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
namespace BinListWrapperApi.Controllers
 
{
    /// <summary>
    /// Controller with two end points
    /// BinList Controller
    /// </summary>

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BinListController : ControllerBase
    {
        private readonly ILogger _logger;
        private IUserService _userService;
        private IBinlistWrapperService _binlistWrapperService;

        /// <summary>
        /// ctor 
        /// BinList controller constructor
        /// </summary>
        /// <param name="logger"></param>  
        /// <param name="userService"></param>  
        ///   <param name="binlistWrapperService"></param>  
        public BinListController(ILogger<BinListController> logger, IUserService userService, IBinlistWrapperService binlistWrapperService)
        {
            _userService = userService;
            _logger = logger;
            _binlistWrapperService = binlistWrapperService;
        }
        
    
        private int GetIntegerLength(int number)
        {
            if (number < 100000)
            {
                if (number < 100)
                {
                    if (number < 10)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    if (number < 1000)
                    {
                        return 3;
                    }
                    else
                    {
                        if (number < 10000)
                        {
                            return 4;
                        }
                        else
                        {
                            return 5;
                        }
                    }
                }
            }
            else
            {
                if (number < 10000000)
                {
                    if (number < 1000000)
                    {
                        return 6;
                    }
                    else
                    {
                        return 7;
                    }
                }
                else
                {
                    if (number < 100000000)
                    {
                        return 8;
                    }
                    else
                    {
                        if (number < 1000000000)
                        {
                            return 9;
                        }
                        else
                        {
                            return 10;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// get bearer token from Authenticate endpoint
        /// Gets a card information from Binlist Api
        /// </summary>
        /// <param name="id"></param>   
        [HttpGet("GetCardInfo/{id}")]
      
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetBin(int id)
        {
      
            //create a strongly type request model..

        #region Card length check
        int cardid = GetIntegerLength(id);
            //check if first digit is zero
            if (id < 6)
            {

                _logger.LogCritical("_logger: LogCritical: Card Id entered is less than  6 digits ");
                // bad request
                return BadRequest(new ApiResponse(400, $"Card Id entered is less than  6 digits {id}"));
            } 
            #endregion
            IRestResponse<CardInfo> response ;
            
         
            response =  await _binlistWrapperService.GetCardDetails(id.ToString());
          
        
            if (response == null)
            {

                _logger.LogCritical("_logger: Empty Response from BinList Api");

                return NotFound(new ApiResponse(404, $"Card Information not found with id {id}"));
            }

            if (response != null && (int)response.StatusCode == 0)
            {

                _logger.LogCritical("_logger: Could not Connect to BinList Api");
                // return StatusCode(StatusCodes.Status500InternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, $"Could not Connect to BinList Api to retrieve info for Card with id: {id}"));

            }
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };


            CardInfo crd = JsonConvert.DeserializeObject<CardInfo>(response.Content, settings);


            HttpStatusCode statusCode = response.StatusCode;
            int numericStatusCode = (int)statusCode;
            if (numericStatusCode == 404)
            {

                _logger.LogError($"_logger: Card Information not found with id {id}");

                return NotFound(new ApiResponse(404, $"Card Information not found with id {id}"));
                //  return NotFound($"Card Information not found with id {id}");
            }


            if (numericStatusCode == 200)
            {

                // return Ok(new ApiOkResponse(crd, "Card Found"));
                _logger.LogInformation("BinListWrapper:Successfully returned Card Information");
                return Ok(crd);
            }

            else

                return BadRequest(new ApiResponse(numericStatusCode, $"Bad Request")); 
   
        }


        /// <summary>
        /// username = test
        /// password = test
        /// Authenticate and generates a Jwt token for user
        /// </summary>
        /// <param name="model"></param>   

        [AllowAnonymous]
   
       [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

    
    }
}