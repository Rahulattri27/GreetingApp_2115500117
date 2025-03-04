using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using BusinessLayer.Interface;

namespace HelloGreetingApplication.Controllers;
    ///<summary>
    /// class providing Api for HelloGreeting
    ///<summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        //create the instance 
        private readonly ILogger<HelloGreetingController> _logger;
        private readonly IGreetingBL _greetingBL;
        public HelloGreetingController(ILogger<HelloGreetingController> logger, IGreetingBL greetingBL)
        {
            _logger = logger;
            _greetingBL = greetingBL;
        }
        ///<summary>
        /// Get method to get the Greeting Message
        ///<summary>
        ///<returns>"Hello, World!"</returns>
        [HttpGet]
        public IActionResult GetMethod()
        {
            _logger.LogInformation("Executing Get Method");
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Hello to Greeting app API Endpoint";
            responseModel.Data = "Hello, World!";

            _logger.LogInformation("Get request successful");
            return Ok(responseModel);
        }
        /// <summary>
        /// Adds new Data
        /// </summary>
        /// <param name="requestBody">the request body containing data</param>
        /// <returns>Success message with data created</returns>
        [HttpPost]
        public IActionResult Post(RequestBody requestBody) {
            if (requestBody == null)
            {
                _logger.LogWarning("Post request failed.request body was empty");
                return BadRequest("Invalid data");
            }
            _logger.LogInformation("Executing Post method");
            ResponseModel<RequestBody> responseModel = new ResponseModel<RequestBody>();
            responseModel.Success = true;
            responseModel.Message = "Data Added Successfully";
            responseModel.Data = requestBody;

            _logger.LogInformation("Post request successful");
            return Ok(responseModel);

        }

        /// <summary>
        /// Updates the data at a particular Id
        /// </summary>
        /// <param name="id">The id of the data to update</param>
        /// <param name="updatedData">The updated data</param>
        /// <returns>Success or failure response</returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RequestBody updatedData) {
            if (updatedData == null)
            {
                _logger.LogWarning("Put request failed. updatedData was empty");
                return BadRequest("Invalid data provided");
            }
            _logger.LogInformation("Executing Put method");
            ResponseModel<RequestBody> responseModel = new ResponseModel<RequestBody>();
            responseModel.Success = true;
            responseModel.Message = $"Data with {id} updated successfully";
            responseModel.Data = updatedData;

            _logger.LogInformation("Put request successful");
            return Ok(responseModel);
        }


        /// <summary>
        /// Partially update the data for a particular id
        /// </summary>
        /// <param name="id"> the id of the data to update</param>
        /// <param name="patchedData">the particular data to be updated</param>
        /// <returns>Success or failure response</returns>
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] string patchedData)
        {
            if (patchedData == null)
            {
                _logger.LogWarning("Patch request failed. patchedData was empty");
                return BadRequest("Invalid data provided");
            }
            _logger.LogInformation("Executing Patch method");
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = $"Data with {id} patched successfully";
            responseModel.Data = patchedData;

            _logger.LogInformation("Patch request successful");
            return Ok(responseModel);
        }

        /// <summary>
        /// Deletes the data for a given ID.
        /// </summary>
        /// <param name="id">The ID of the data to delete.</param>
        /// <returns>Success or failure response.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Execution of Delete Request");
            var response = new ResponseModel<string>
            {
                Success = true,
                Message = $"Data with ID {id} deleted successfully.",
                Data = null
            };

            _logger.LogInformation("Delete was successful");
            return Ok(response);
        }

        [HttpGet("SimpleGreeting")]
        public IActionResult GetSimpleGreeting()
        {
            _logger.LogInformation("Executing Simple Greeting ");
            string result = _greetingBL.SimpleGreeting();
            var response = new ResponseModel<string>
            {
                Success = true,
                Message = $"Got the Simple Greeting{result}",
                Data = result
            };
            _logger.LogInformation("GetSimpleGreeting executed successfully");
            return Ok(response);

        }
        [HttpGet("PersonalizedGreeting")]
        public IActionResult GetGreeting(string? FirstName, string? LastName)
    {
        _logger.LogInformation("Executing Get Greeting");
        string message = _greetingBL.GreetingMessage(FirstName, LastName);
        var response = new ResponseModel<string>
        {
            Success = true,
            Message = $"Got the Personalized Greeting: {message}",
            Data = message
        };
        _logger.LogInformation("Returning response: {message}", message);
        return Ok(new { response });

    }
    
 }
