using System;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly ILogger<UserBL> _logger;
       
            private readonly IUserRL _userRL;

            //constructor of class
            public UserBL(IUserRL userRL,ILogger<UserBL> logger)
            {
            _logger = logger;
            _userRL = userRL;

            }

            /// <summary>
            /// method to register the user
            /// </summary>
            /// <param name="userRegisterDTO">User Details to register</param>
            /// <returns>Returns user details if save else null</returns>

            public User RegisterUser(RegisterDTO userRegisterDTO)
            {
            _logger.LogInformation("User Details in Business Layer");
            var user = _userRL.RegisterUser(userRegisterDTO);
            _logger.LogInformation("Returning register method from repository layer");
            return user;
            }
            /// <summary>
            /// method to login the user
            /// </summary>
            /// <param name="loginDTO">login credentials</param>
            /// <returns>Success or failure response</returns>
            public UserResponseDTO LoginUser(LoginDTO loginDTO)
            {
            _logger.LogInformation("Login in Business Layer");
            return _userRL.LoginUser(loginDTO);
            }
        /// <summary>
        /// method to get the token on mail for forget password
        /// </summary>
        /// <param name="email">email of user </param>
        /// <returns>true or false id email exist or not</returns>
        public bool ForgetPassword(string email)
        {
            _logger.LogInformation("Forget PAssword in Business Layer");
            return _userRL.ForgetPassword(email);
        }
        /// <summary>
        /// method to reset the password 
        /// </summary>
        /// <param name="token">reset token from mail</param>
        /// <param name="newPassword">new password from user</param>
        /// <returns>true or false</returns>
        public bool ResetPassword(string token, string newPassword)
        {
            _logger.LogInformation("ResetPassword in BusinessLayer");
            return _userRL.ResetPassword(token, newPassword);
        }
    }
    
}

