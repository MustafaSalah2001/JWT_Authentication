namespace WebApplication1.DTO
{
    public class TokenResponeDto
    { 
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
