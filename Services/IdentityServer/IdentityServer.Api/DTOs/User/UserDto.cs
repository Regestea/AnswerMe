namespace IdentityServer.Api.DTOs.User
{
    public class UserDto: BaseEntityDto
    {
        public string? IdName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
