namespace StdControllers.Models;

public class Roles
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = [];
}

public class RolesDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RolesWithUsersDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<UserDto> Users { get; set; } = [];
}