namespace AppHub.Domain.Entities;

public class OAuthProvider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserOAuth> UserOAuths { get; set; } = new List<UserOAuth>();
}

