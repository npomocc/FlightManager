namespace Flight.Contracts;

public class LoginView
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class UserView
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}

public class CreateUserView
{
    public string UserName { get; set; }
    public string Password { get; set; }
}