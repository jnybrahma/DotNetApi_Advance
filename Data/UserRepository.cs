using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);

        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        //public bool AddEntity<T>(T entityToAdd)
        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
                // return true;
            }
            //  return false;

        }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Remove(entityToAdd);

            }

        }
        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;

        }
        public IEnumerable<UserSalary> GetUserSalary()
        {
            IEnumerable<UserSalary> userSalary = _entityFramework.UserSalary.ToList<UserSalary>();

            return userSalary;

        }
        public IEnumerable<UserJobInfo> GetUserJobInfo()
        {
            IEnumerable<UserJobInfo> userJobInfo = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
            return userJobInfo;

        }


        public User GetSingleUser(int userId)
        {

            User? user = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to get User");

        }

        public UserSalary GetSingleUserSalary(int userId)
        {

            UserSalary? userSalary = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to get User salary");

        }

        public UserJobInfo GetSingleUserJobInfo(int userId)
        {

            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Failed to get User Job Info");

        }


    }

}