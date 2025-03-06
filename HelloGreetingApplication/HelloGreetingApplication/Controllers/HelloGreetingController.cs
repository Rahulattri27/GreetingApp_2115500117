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
    public IActionResult CreateGreeting(GreetingModel greeting)
    {
        _logger.LogInformation("Creating the greeting in database");
        string message = _greetingBL.Create(greeting);
        var response = new ResponseModel<string>
        {
            Success = true,
            Message = "Greeting added to database",
            Data = message
        };
        _logger.LogInformation("Returning response: {message}", message);
        return Ok(response);

    }
    /// <summary>
    /// Method Getting all the greeting from database 
    /// </summary>
    /// <returns>return all the greeting  from database</returns>
    [HttpGet("Greetings")]
    public IActionResult GetGreetings()
    {
        _logger.LogInformation("Get the Greeting from database");
        var result = _greetingBL.GetDatabaseGreeting();
        var response = new ResponseModel<List<GreetingModel>>
        {
            Success = true,
            Message = "Get the Greetings from Database",
            Data = result
        };
        _logger.LogInformation("Returning Database Greetings");
        return Ok(response);

    }


    /// <summary>
    /// Method to find the greeting from database using id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>return the greeting if id found</returns>
    [HttpGet("find/{id}")]
    public IActionResult FindGreeting(int id)
    {
        _logger.LogInformation("find the greeting from database");
        var result = _greetingBL.FindGreeting(id);

        var response = new ResponseModel<GreetingModel>();
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
}
