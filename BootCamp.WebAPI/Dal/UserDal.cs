namespace BootCamp.WebAPI.Dal
{
    public class UserDal
    {
        public static Model.User Get(string username, string password)
        {
            var users = new List<Model.User>();

            // User 2:
            users.Add(new Model.User { Id = 1, UserName = "clauds@acme.com.br", Password = "678910", Role = "manager" });
            // User 1:
            users.Add(new Model.User { Id = 2, UserName = "vlamir@acme.com.br", Password = "123456", Role = "employee" });

            return users.FirstOrDefault(x => x.UserName == username && x.Password == password);
        }

    }
}
