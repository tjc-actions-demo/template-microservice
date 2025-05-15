using Microservice.WebApi.Entities;

namespace Microservice.WebApi.Services
{
    public interface IProblemsService
    {
        void Create(Problem entity, CancellationToken cancellationToken = default);

        bool Exists(string id, CancellationToken cancellationToken = default);

        IList<Problem> Get(CancellationToken cancellationToken = default);

        IList<Problem> Get(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Problem? Get(string id, CancellationToken cancellationToken = default);

        void Remove(Problem entity, CancellationToken cancellationToken = default);

        void Update(Problem entity, CancellationToken cancellationToken = default);
    }
}
