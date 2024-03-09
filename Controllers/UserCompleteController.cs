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

        sql += parameters.Substring(1);//, parameters.Length);

        Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;

    }


    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
            SELECT  [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
            FROM  TutorialAppSchema.Users 
            WHERE UserId = " + userId.ToString();

        User user = _dapper.LoadDataSingle<User>(sql);

        return user;

    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
             UPDATE TutorialAppSchema.Users 
                SET [FirstName] = '" + user.FirstName +
                    "',[LastName] = '" + user.LastName +
                    "',[Email] = '" + user.Email +
                    "',[Gender] = '" + user.Gender +
                    "',[Active] = '" + user.Active +
                "' WHERE UserId = " + user.UserId;

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update user");

    }

    [HttpPost("AddUser")]
    public IActionResult Adduser(UserToAddDto user)
    {

        string sql = @"
             INSERT INTO TutorialAppSchema.Users (
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] )
            VALUES (" +
                "'" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +
            "')";
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Insert user");
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
    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(
            @"
            SELECT UserSalary.UserId
            , UserSalary.Salary
            FROM   TutorialAppSchema.UserSalary
            WHERE UserId =  " + userId);
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary(
                UserId,
                Salary
            ) VALUES ("
                + userSalaryForInsert.UserId
                + " , " + userSalaryForInsert.Salary
                + ")";
        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForInsert);
        }
        throw new Exception("Adding UserSalary failed to save");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary="
        + userSalaryForUpdate.Salary
        + " WHERE UserId = " + userSalaryForUpdate.UserId;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForUpdate);
        }
        throw new Exception("Updating UserSalary failed to update");
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


    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(
            @"
            SELECT UserJobInfo.UserId
            , UserJobInfo.JobTitle
            , UserJobInfo.Department
            FROM   TutorialAppSchema.UserJobInfo
            WHERE UserId =  " + userId);
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo(
                [UserId],
                [JobTitle],
                [Department]
            ) VALUES ("
                + userJobInfoInsert.UserId +
                  " , '" + userJobInfoInsert.JobTitle +
                  "', '" + userJobInfoInsert.Department
                + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoInsert);
        }
        throw new Exception("Adding User Job Info failed to save");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoUpdate)
    {
        string sql = @"
        UPDATE TutorialAppSchema.UserJobInfo 
        SET [JobTitle] = '" + userJobInfoUpdate.JobTitle +
        "', [Department] = '" + userJobInfoUpdate.Department +
        "' WHERE UserId = " + userJobInfoUpdate.UserId;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoUpdate);
        }
        throw new Exception("Updating User Job Info failed to update");
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


