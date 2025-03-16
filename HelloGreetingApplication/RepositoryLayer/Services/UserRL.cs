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
        private readonly IEmailService _emailService;
        //Constructor to Initialize the objects
        public UserRL(HelloGreetingContext context, Password_Hash hash,ILogger<UserRL> logger,IConfiguration configuration,IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _passwordHash = hash;
            _configuration = configuration;
            _emailService = emailService;
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

        /// <summary>
		/// method to get the token on mail to reset the password
		/// </summary>
		/// <param name="email">email address of the user</param>
		/// <returns>token on email</returns>
		public bool ForgetPassword(string email)
        {
            try
            {
                _logger.LogInformation("Finding Email in database");
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogInformation("Email not found");
                    return false;
                }
                _logger.LogInformation("Generate and save reset token");
                user.ResetToken = GenerateJwtToken(user);
                user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
                _context.SaveChanges();
                string emailBody = $"Reset Token :\n {user.ResetToken}";
                _emailService.SendEmail(user.Email, "Reset Password", emailBody);
                _logger.LogInformation("Email sent");
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// method to change the password
        /// </summary>
        /// <param name="token">system generated token recieved on mail</param>
        /// <param name="newPassword"> new password </param>
        /// <returns>Success or failure response</returns>
        public bool ResetPassword(string token, string newPassword)
        {
            try
            {
                _logger.LogInformation("Finding Email in database");
                var user = _context.Users.FirstOrDefault(e => e.ResetToken == token && e.ResetTokenExpiry > DateTime.UtcNow);
                if (user == null)
                {
                    _logger.LogInformation("Invalid token");
                    //Invalid token
                    return false;
                }
                user.PasswordHash = _passwordHash.PasswordHashing(newPassword);
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
                _logger.LogInformation("Password Reset Successfull");
                return true;
            }
            catch(Exception ) {
                return false;
            }
        }


    }
}

