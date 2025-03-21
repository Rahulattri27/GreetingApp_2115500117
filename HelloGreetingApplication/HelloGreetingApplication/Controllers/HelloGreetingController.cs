using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HelloGreetingApplication.Controllers;
///<summary>
/// class providing Api for HelloGreeting
///<summary>

[ApiController]
[Route("[controller]")]
public class HelloGreetingController : ControllerBase
{
    
    private readonly ILogger<HelloGreetingController> _logger;
    private readonly IGreetingBL _greetingBL;

    /// <summary>
    /// create instances of objects
    /// </summary>
    public HelloGreetingController(ILogger<HelloGreetingController> logger, IGreetingBL greetingBL)
    {
        _logger = logger;
        _greetingBL = greetingBL;
    }

    ///<summary>
    /// Get method to get the Greeting Message
    ///</summary>
    ///<returns>"Hello, World!"</returns>
    [HttpGet]
    [Authorize]
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
    public IActionResult Post(RequestBody requestBody)
    {
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
    public IActionResult Put(int id, [FromBody] RequestBody updatedData)
    {
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
    /// <summary>
    /// Get the Hello World as Greting
    /// </summary>
    /// <returns>Hello World</returns>

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
    /// <summary>
    /// Method to greeting with name or default Hello World
    /// </summary>
    /// <param name="FirstName">first name of user</param>
    /// <param name="LastName">Last name of user</param>
    /// <returns>Greeting message with name</returns>
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
        return Ok(response);

    }
    /// <summary>
    /// Method to create the greeting in database
    /// </summary>
    /// <param name="greeting">Greeting message from user</param>
    /// <returns>Created Successfully Message</returns>
    [HttpPost("AddNew")]
    [Authorize]
    public IActionResult CreateGreeting(GreetingModel greeting)
    {
        try
        {
            var response = new ResponseModel<string>();
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                _logger.LogInformation("Unauthorized user");
                response.Success = false;
                response.Message = "Unauthorized user";
                return Unauthorized(response);
            }
            _logger.LogInformation("Creating the greeting in database");
            greeting.UserId = int.Parse(userId);
            greeting.User = null;
            string message = _greetingBL.Create(greeting);
            
            if(message== "Successfully added Greeting"){
                response.Success = true;
                response.Message = "Greeting added to database";
                response.Data = message;

                _logger.LogInformation("Returning response: {message}", message);
                return Ok(response);
            }
            response.Success = false;
            response.Message = "Not Found in Database";
            _logger.LogInformation("Returning response: {message}", message);
            return NotFound(response);
        }
            
        catch(Exception ex)
        {
            _logger.LogError("Exception catched in Controller CreateGreeting().");
            var response = new ResponseModel<String>
            {
                Success = false,
                Message = $"Exception Occured. {ex.Message}"
            };
            return StatusCode(500, response);

            
        }

    }
    /// <summary>
    /// Method Getting all the greeting from database 
    /// </summary>
    /// <returns>return all the greeting  from database</returns>
    [HttpGet("Greetings")]
    [Authorize]
    public async Task<IActionResult> GetGreetings()
    {
        var response = new ResponseModel<List<GreetingModel>>();
        try
        {
            _logger.LogInformation("Get the Greeting from database");
            var result = await _greetingBL.GetDatabaseGreeting();

            response.Success = true;
            response.Message = "Get the Greetings from Database";
            response.Data = result;
            _logger.LogInformation("Returning Database Greetings");
            return Ok(response);
        }
        catch(Exception ex){
            response.Success = false;
            response.Message = $"Exception occured in GetGreetings() Business Layer: {ex.Message}";
            return StatusCode(500, response);
        }
    }


    /// <summary>
    /// Method to find the greeting from database using id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>return the greeting if id found</returns>
    [HttpGet("find/{id}")]
    [Authorize]
    public async Task<IActionResult> FindGreeting(int id)
    {
        var response = new ResponseModel<List<GreetingModel>>();
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim==null || int.Parse(userIdClaim) != id)
            {
                response.Success = false;
                response.Message = "Unauthorized user";
                return Unauthorized(response);
            }

            int userId = int.Parse(userIdClaim);
            _logger.LogInformation("find the greeting from database");
            var result = await _greetingBL.FindGreeting(id);


            if (result == null)
            {
                response.Success = false;
                response.Message = $"{id} not found";
                response.Data = null;
                return NotFound(response);
            }
            response.Success = true;
            response.Message = $"{id} found";
            response.Data = result;
            return Ok(response);
        }
        catch(Exception ex)
        {
            response.Success = false;
            response.Message = $"Exception : {ex.Message}";
            return StatusCode(500, response);

        }


    }
    /// <summary>
    /// method to update the greeting by id
    /// </summary>
    /// <param name="id">id of greeting from user</param>
    /// <param name="message">Updated greeting</param>
    /// <returns>Success or Failure response</returns>
    [HttpPatch("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateGreetingById(int id,string message)
    {
        var response = new ResponseModel<string>();
        try
        {

            _logger.LogInformation("Executing the UpdateMessagebyId");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null) 
            { 
                response.Success = false;
                response.Message = "Unauthorized user";
                return Unauthorized(response);
            }
            int _id = int.Parse(userIdClaim);

            var existingGreeting = _greetingBL.FindGreetingById(id);
            if (existingGreeting==null || existingGreeting.UserId!= _id)
            {
                response.Success = false;
                response.Message = "Cannot modify that greeting.";
                return BadRequest(response);

            }
            var result =await  _greetingBL.UpdateGreeting(id, message);

            if (result)
            {
                response.Success = true;
                response.Message = "updated successfully";
                response.Data = message;
                return Ok(response);
            }
            response.Success = false;
            response.Message = "updated unsuccessfull";
            response.Data = null;
            return NotFound(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Exception : {ex.Message}";
            return StatusCode(500, response);

        }


    }
    /// <summary>
    /// Method to delete the greeting on id
    /// </summary>
    /// <param name="id">id of greeting to delete</param>
    /// <returns>Success or failure response</returns>
    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteGreeting(int id)
    {
        var response = new ResponseModel<string>();
        try
        {
            _logger.LogInformation("Executing DeleteGreeting in Controller");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
            {
                response.Success = false;
                response.Message = "Unauthorized user";
                return Unauthorized(response);
            }
            int _id = int.Parse(userIdClaim);

            var existingGreeting = _greetingBL.FindGreetingById(id);
            if (existingGreeting == null || existingGreeting.UserId != _id)
            {
                response.Success = false;
                response.Message = "Cannot delete that greeting.";
                return BadRequest(response);

            }
            bool result = await _greetingBL.DeleteGreeting(id);
           

            if (result)
            {
                response.Success = true;
                response.Message = $"{id} successfully deleted";
                response.Data = null;
                return Ok(response);
            }
            response.Success = false;
            response.Message = $"{id} does not exist";
            response.Data = null;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Exception : {ex.Message}";
            return StatusCode(500, response);

        }


    }

}
