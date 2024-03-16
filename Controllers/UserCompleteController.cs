using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;

    public UserCompleteController(IConfiguration config)
    {
        //  Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (!isActive)
        {
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1); //, parameters.Length);
        }

        // sql += parameters.Substring(1);//, parameters.Length);

        Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
        return users;

    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {

        // string sql = @"EXEC TutorialAppSchema.spUser_Upsert
        //             @FirstName = @FirstNameParameter,
        //             @LastName = @LastNameParameter,
        //             @Email =  @EmailParameter,
        //             @Gender = @GenderParameter,
        //             @Active =  @ActiveParameter,
        //             @JobTitle = @JobTitleParameter, 
        //             @Department = @DepartmentParameter,
        //             @Salary = @SalaryParameter,
        //             @UserId = @UserIdParameter";

        // DynamicParameters sqlParameters = new DynamicParameters();

        // sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
        // sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
        // sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
        // sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
        // sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
        // sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
        // sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
        // sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
        // sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);


        if (_reusableSql.UpsertUser(user))
        {
            return Ok();
        }

        throw new Exception("Failed to Update user");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC  TutorialAppSchema.spUser_Delete
                @UserId = @UserIdParameter";

        Console.WriteLine(sql);

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete user");
    }


}


