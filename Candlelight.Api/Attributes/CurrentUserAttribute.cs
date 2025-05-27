using Microsoft.AspNetCore.Mvc;
using Candlelight.Api.ModelBinders;

namespace Candlelight.Api.Attributes;

public class CurrentUserAttribute() : ModelBinderAttribute(typeof(CurrentUserModelBinder));