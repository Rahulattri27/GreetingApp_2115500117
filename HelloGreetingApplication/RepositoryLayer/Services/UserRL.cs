using System;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Hashing;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Services
{
	public class UserRL : IUserRL
    {
        private readonly HelloGreetingContext _context;
        private readonly Password_Hash _passwordHash;
        private readonly ILogger<UserRL> _logger;
        private readonly IConfiguration _configuration;
        //Constructor to Initialize the objects
        public UserRL(HelloGreetingContext context, Password_Hash hash,ILogger<UserRL> logger,IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _passwordHash = hash;
            _configuration = configuration;
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
        public UserResponseDTO LoginUser(LoginDTO loginDTO)
        {
            _logger.LogInformation("checking if user exist or not");
            var validUser = _context.Users.FirstOrDefault(e => e.Email == loginDTO.Email);
            if (validUser != null && _passwordHash.VerifyPassword(loginDTO.Password, validUser.PasswordHash))
            {
                _logger.LogInformation("User details Found in Database");
                var userresponse = new UserResponseDTO
                {
                    Email = validUser.Email,
                    Token = GenerateJwtToken(validUser)
                };
                return userresponse;

            }
            _logger.LogInformation("User details not found in Database");
            return null;
        }
        /// <summary>
		/// method to generate the jwt token
		/// </summary>
		/// <param name="user">user info from database</param>
		/// <returns>token </returns>
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




    }
}

