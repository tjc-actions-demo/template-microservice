using Microservice.WebApi.Entities;

namespace Microservice.WebApi.Services;

public class ProblemsService : IProblemsService
{
    private readonly List<Problem> _problems = [];

    public void Create(Problem entity, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid().ToString();

        entity.Id = id;

        _problems.Add(entity);
    }

    public bool Exists(string id, CancellationToken cancellationToken = default)
    {
        return _problems.Any(problem => problem.Id == id);
    }

    public IList<Problem> Get(CancellationToken cancellationToken = default)
    {
        return _problems;
    }

    public IList<Problem> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        return ids.Select(id => _problems.SingleOrDefault(problem => problem.Id == id)).OfType<Problem>().ToList();
    }

    public Problem? Get(string id, CancellationToken cancellationToken = default)
    {
        return _problems.SingleOrDefault(problem => problem.Id == id);
    }

    public void Remove(Problem entity, CancellationToken cancellationToken = default)
    {
        var problemToRemove = _problems.SingleOrDefault(problem => problem.Id == entity.Id);

        if (problemToRemove == null) return;

        _problems.Remove(problemToRemove);
    }

    public void Update(Problem entity, CancellationToken cancellationToken = default)
    {
        var problemToUpdate = _problems.SingleOrDefault(problem => problem.Id == entity.Id);

        if (problemToUpdate == null) return;

        problemToUpdate.Description = entity.Description;
        problemToUpdate.IsActive = entity.IsActive;
        problemToUpdate.Title = entity.Title;
    }
}
