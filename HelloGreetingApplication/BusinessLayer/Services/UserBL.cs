using System;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Services;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly ILogger<UserBL> _logger;
       
        private readonly IUserRL _userRL;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        //constructor of class
        public UserBL(IUserRL userRL,ILogger<UserBL> logger, RabbitMQPublisher rabbitMQPublisher)
            {
            _logger = logger;
            _userRL = userRL;
            _rabbitMQPublisher = rabbitMQPublisher;

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
            if (user != null)
            {
                var message = new
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    RegisterAt = DateTime.UtcNow
                };
                _rabbitMQPublisher.PublishMessage("UserRegistrationQueue", message);
                _logger.LogInformation("User Registration event published to RabbitMq");
            }
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

