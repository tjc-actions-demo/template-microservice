using Microservice.WebApi.Entities;
using Microservice.WebApi.Models;
using Microservice.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemsService _problemsService;

    public ProblemsController(IProblemsService problemsService)
    {
        _problemsService = problemsService;
    }

    /// <summary>
    ///     Deletes a problem from database
    /// </summary>
    /// <param name="id">The ID of the problem to be removed</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledges the problem was successfully removed</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    /// <response code="404">The problem to remove does not exist in the database</response>
    [HttpDelete("{id:length(37)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(string id, CancellationToken cancellationToken = default)
    {
        var problem = _problemsService.Get(id, cancellationToken);

        if (problem == null) return NotFound();

        _problemsService.Remove(problem, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Fetches all the problems from the database
    /// </summary>
    /// <param name="expandTestSets">If true, the test sets are returned with the problems, otherwise null is returned</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">All available problems returned</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProblemModel>))]
    public ActionResult<IList<ProblemModel>> Get(bool expandTestSets = false, CancellationToken cancellationToken = default)
    {
        var problems = _problemsService.Get(cancellationToken);

        return problems.ToModels();
    }

    /// <summary>
    ///     Fetches a problem from the database
    /// </summary>
    /// <param name="id">The ID of the problem to get</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="200">Returns the requested problem</response>
    /// <response code="404">The problem does not exist in the database</response>
    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProblemModel))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProblemModel> Get(string id, CancellationToken cancellationToken = default)
    {
        var problem = _problemsService.Get(id, cancellationToken);

        if (problem == null) return NotFound();

        return problem.ToModel();
    }

    /// <summary>
    ///     Creates a new problem
    /// </summary>
    /// <param name="problemModel">The problem to be created</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="201">Returns the requested problem</response>
    /// <response code="400">The problem is not in a valid state and cannot be created</response>
    /// <response code="403">You do not have permission to use this endpoint</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public CreatedAtActionResult Post(ProblemModel problemModel, CancellationToken cancellationToken = default)
    {
        var problem = problemModel.ToEntity();

        _problemsService.Create(problem, cancellationToken);

        problemModel.Id = problem.Id;

        return CreatedAtAction(nameof(Get), new { id = problem.Id }, problemModel);
    }

    /// <summary>
    ///     Updates the specified problem
    /// </summary>
    /// <param name="id">The ID of the problem to update</param>
    /// <param name="updatedProblemModel">The problem that should replace the one in the database</param>
    /// <param name="cancellationToken">The .NET cancellation token</param>
    /// <response code="204">Acknowledgement that the problem was updated</response>
    /// <response code="400">The problem is not in a valid state and cannot be updated</response>
    /// <response code="404">The problem requested to be updated could not be found</response>
    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Put(string id, ProblemModel updatedProblemModel, CancellationToken cancellationToken = default)
    {
        var problem = _problemsService.Get(id, cancellationToken);

        if (problem == null) return NotFound();

        updatedProblemModel.Id = problem.Id;

        _problemsService.Update(updatedProblemModel.ToEntity(), cancellationToken);

        return NoContent();
    }
}
