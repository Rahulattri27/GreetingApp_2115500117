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
        }
    
}

