using Candlelight.Application.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Candlelight.Api.ModelBinders;

public class CurrentUserModelBinder(UserContextResolver userContextResolver) : IModelBinder
{
    private readonly UserContextResolver _userContextResolver = userContextResolver;
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var user = await _userContextResolver.ResolveUserAsync(httpContext.User);
        bindingContext.Result = ModelBindingResult.Success(user);
    }
}