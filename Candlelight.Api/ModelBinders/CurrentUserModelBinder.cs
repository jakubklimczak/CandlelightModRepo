using Candlelight.Application.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Candlelight.Api.ModelBinders;

public class CurrentUserModelBinder(UserContextResolver resolver) : IModelBinder
{
    private readonly UserContextResolver _userContextResolver = resolver;

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var user = await _userContextResolver.ResolveUserAsync(httpContext.User);
        Console.WriteLine("[ModelBinder] Resolved user: " + (user?.UserName ?? "null"));
        if (user == null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        bindingContext.Result = ModelBindingResult.Success(user);
    }
}