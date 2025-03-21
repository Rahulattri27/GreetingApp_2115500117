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
        public IActionResult Login([FromBody] LoginDTO login)
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
            var response2 = new ResponseModel<UserResponseDTO>();
            response2.Success = true;
            response2.Message = "User Login Successfully";
            response2.Data = user;
            return Ok(response2);
        }


        /// <summary>
		/// Sends a password reset token to users email
		/// </summary>
		/// <param name="email">the email address of the user</param>
		/// <returns>Returns success message if email exists</returns>
		[HttpPost("forget-password")]
        public IActionResult ForgetPassword([FromBody] string email)
        {
            _logger.LogInformation("Forget Password Controller");
            var response = new ResponseModel<string>();
            bool success = _userBL.ForgetPassword(email);
            if (success)
            {
                _logger.LogInformation("Forget password Executed Successfully");
                response.Success = true;
                response.Message = "Reset Link Sent to your Email";
                return Ok(response);
            }
            _logger.LogInformation("Forget Password does not  found email");
            response.Message = "Email not found.";
            return NotFound(response);
        }

        /// <summary>
        /// Resets the password for user
        /// </summary>
        /// <param name="resetPassword">the reset token and new password</param>
        /// <returns>success message if password reset successfully</returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            _logger.LogInformation("Executing ResetPassword");
            var response = new ResponseModel<string>();
            bool success = _userBL.ResetPassword(resetPassword.ResetToken, resetPassword.NewPassword);
            if (success)
            {

                response.Success = true;
                response.Message = "Password reset Successfully";
                _logger.LogInformation("Password reset successful");
                return Ok(response);
            }
            response.Message = "Invalid or Expired token";
            _logger.LogInformation("PAssword reset fail");
            return NotFound(response);
        }



    }
}
