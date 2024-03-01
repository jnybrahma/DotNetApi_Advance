using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToAdd);
        public IEnumerable<User> GetUsers();

        public IEnumerable<UserSalary> GetUserSalary();

        public IEnumerable<UserJobInfo> GetUserJobInfo();

        public User GetSingleUser(int userId);

        public UserSalary GetSingleUserSalary(int userId);

        public UserJobInfo GetSingleUserJobInfo(int userId);

    }


}