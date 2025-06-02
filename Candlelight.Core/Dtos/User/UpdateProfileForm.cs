using Microsoft.AspNetCore.Http;

namespace Candlelight.Core.Dtos.User;
public class UpdateProfileForm
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? BackgroundColour { get; set; }
    public bool? FavouritesVisible { get; set; }
    public IFormFile? Avatar { get; set; }
}
