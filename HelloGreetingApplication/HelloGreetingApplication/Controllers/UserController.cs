using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Interface;
using ModelLayer.Model;
using ModelLayer.DTO;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelloGreetingApplication.Controllers
{
    [Route("auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserBL _userBL;
        /// <summary>
        /// Constructor to Initialize the instance
        /// </summary>
        /// <param name="userBL">The Business layer service for user</param>
        public UserController(IUserBL userBL,ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="registerDTO">User Details</param>
        /// <returns> Success or Failure Response</returns>
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Starting the registeration");
            var response = new ResponseModel<User>();
            var user = _userBL.RegisterUser(registerDTO);
            if (user == null)
            {
                _logger.LogInformation("User is Null in output in controller");
                response.Message = "User Already Exist";
                response.Data = user;

                return BadRequest(response);
            }
            _logger.LogInformation("output user from controller");
            response.Success = true;
            response.Message = "User Registered Successfully";
            response.Data = user;
            return Ok(response);
        }

        /// <summary>
        /// Logs in an existing user
        /// </summary>
        /// <param name="login">User credentials for login</param>
        /// <returns>Success or Failure Message</returns>
        [HttpPost("login")]
        public IActionResult Login(LoginDTO login)
        {
            _logger.LogInformation("User Login Started");

            var user = _userBL.LoginUser(login);
            if (user == null)
            {
                _logger.LogInformation("Not able to Login (User Controller)");
                var response = new ResponseModel<LoginDTO>();
                response.Message = "Invalid Credentials";
                response.Data = login;
                return Unauthorized(response);
            }
            _logger.LogInformation("User Login Successfull");
            var response2 = new ResponseModel<User>();
            response2.Success = true;
            response2.Message = "User Login Successfully";
            response2.Data = user;
            return Ok(response2);
        }

    }
}
