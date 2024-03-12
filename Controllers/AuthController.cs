using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;


        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {

            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                     userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                if (existingUsers.Count() == 0)
                {


                    UserForLoginDto userForSetPassword = new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password

                    };
                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName +
                            "',@LastName = '" + userForRegistration.LastName +
                            "',@Email = '" + userForRegistration.Email +
                            "',@Gender = '" + userForRegistration.Gender +
                            "',@Active = 1" +
                            ",@JobTitle = '" + userForRegistration.JobTitle +
                            "',@Department = '" + userForRegistration.Department +
                            "',@Salary = '" + userForRegistration.Salary + "'";
                        // string sqlAddUser = @"
                        //     INSERT INTO TutorialAppSchema.Users (
                        //     [FirstName],
                        //     [LastName],
                        //     [Email],
                        //     [Gender],
                        //     [Active] )
                        //  VALUES (" +
                        //  "'" + userForRegistration.FirstName +
                        // "', '" + userForRegistration.LastName +
                        // "', '" + userForRegistration.Email +
                        // "', '" + userForRegistration.Gender +
                        // "', 1)";
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }

                        throw new Exception("Failed to add user!");
                    }

                    throw new Exception("Failed to register user");
                }

                throw new Exception("User with this email already exists!");
            }

            throw new Exception("Password do not match");

        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();

            }
            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]

        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @" SELECT 
            [PasswordHash],
            [PasswordSalt]  FROM TutorialAppSchema.Auth WHERE Email = '" +
            userForLogin.Email + "'";

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password !");
                }

            }

            string userIdSql = @"SELECT  [UserId] FROM TutorialAppSchema.Users
                              WHERE EMAIL = '" + userForLogin.Email + "'";


            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });

        }

        [HttpGet("RefreshToken")]

        public string RefreshToken()
        {

            string sqlGetUserId = @"SELECT  [UserId] FROM TutorialAppSchema.Users
                              WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);

            return _authHelper.CreateToken(userId);

        }

    }



}