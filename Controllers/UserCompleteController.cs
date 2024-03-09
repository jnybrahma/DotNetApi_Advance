using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;

    public UserCompleteController(IConfiguration config)
    {
        //  Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }
        if (isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }
        if (!isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }

        sql += parameters.Substring(1);//, parameters.Length);

        Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;

    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {

        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                    @FirstName = '" + user.FirstName +
                    "',@LastName = '" + user.LastName +
                    "',@Email = '" + user.Email +
                    "',@Gender = '" + user.Gender +
                    "',@Active = '" + user.Active +
                    "',@JobTitle = '" + user.JobTitle +
                    "',@Department = '" + user.Department +
                    "',@Salary = '" + user.Salary +
                    "',@UserId = " + user.UserId;

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update user");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.Users 
                WHERE UserId = " + userId.ToString();

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete user");
    }


    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("UserSalary failed to delete");

    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("User Job Info failed to delete");

    }






}


