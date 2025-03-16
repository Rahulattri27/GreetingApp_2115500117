using System;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Hashing;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using Microsoft.Extensions.Logging;

namespace RepositoryLayer.Services
{
	public class UserRL : IUserRL
    {
        private readonly HelloGreetingContext _context;
        private readonly Password_Hash _passwordHash;
        private readonly ILogger<UserRL> _logger;
        //Constructor to Initialize the objects
        public UserRL(HelloGreetingContext context, Password_Hash hash,ILogger<UserRL> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHash = hash;
        }


        /// <summary>
        /// Method to register the user in database
        /// </summary>
        /// <param name="userRegisterDTO">User data </param>
        /// <returns>User(info) or null </returns>
        public User RegisterUser(RegisterDTO userRegisterDTO)
        {
            _logger.LogInformation("checking if user exist or not");
            var existingUser = _context.Users.FirstOrDefault(e => e.Email == userRegisterDTO.Email);
            if (existingUser == null)
            {
                _logger.LogInformation("User does not exist, So saving user");
                var newUser = new User
                {
                    FirstName = userRegisterDTO.FirstName,
                    LastName = userRegisterDTO.LastName,
                    Email = userRegisterDTO.Email,
                    PasswordHash = _passwordHash.PasswordHashing(userRegisterDTO.Password)
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                _logger.LogInformation("User Details saved in database");
                return newUser;
            }
            _logger.LogInformation("User exist in database");
            return null;
        }


        /// <summary>
        /// method to login the user
        /// </summary>
        /// <param name="loginDTO">email and password from user</param>
        /// <returns>return user info with token  or null</returns>
        public User LoginUser(LoginDTO loginDTO)
        {
            _logger.LogInformation("checking if user exist or not");
            var validUser = _context.Users.FirstOrDefault(e => e.Email == loginDTO.Email);
            if (validUser != null && _passwordHash.VerifyPassword(loginDTO.Password, validUser.PasswordHash))
            {
                _logger.LogInformation("User details Found in Database");
                return validUser;

            }
            _logger.LogInformation("User details not found in Database");
            return null;
        }



    }
}

