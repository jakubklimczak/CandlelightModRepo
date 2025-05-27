using Candlelight.Api.Attributes;
using Candlelight.Application.Services;
using Candlelight.Core.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Candlelight.Api.ModelBinders;

public class CurrentUserModelBinderProvider(IServiceProvider serviceProvider) : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(AppUser) &&
            context.Metadata is DefaultModelMetadata metadata &&
            metadata.Attributes.ParameterAttributes?.Any(attr => attr is CurrentUserAttribute) == true)
        {
            return new BinderTypeModelBinder(typeof(CurrentUserModelBinder));
        }
        return null;
    }
}