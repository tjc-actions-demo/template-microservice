using Swashbuckle.AspNetCore.Annotations;

namespace Microservice.WebApi.Entities;

public class Problem
{
    public string? Description { get; set; }

    [SwaggerSchema(ReadOnly = true)]
    public string? Id { get; set; }

    public bool IsActive { get; set; }

    public string? Title { get; set; }
}
