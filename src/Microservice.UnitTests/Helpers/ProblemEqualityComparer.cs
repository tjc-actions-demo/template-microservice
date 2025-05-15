using System.Diagnostics.CodeAnalysis;
using Microservice.WebApi.Entities;

namespace Microservice.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class ProblemEqualityComparer : IEqualityComparer<Problem?>, IEqualityComparer<IList<Problem>?>
{
    public bool Equals(Problem? x, Problem? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        var descriptionsMatch = x.Description == y.Description;
        var idsMatch = x.Id == y.Id;
        var isActiveMatch = x.IsActive == y.IsActive;
        var titlesMatch = x.Title == y.Title;

        return descriptionsMatch && idsMatch && isActiveMatch && titlesMatch;
    }

    public bool Equals(IList<Problem>? x, IList<Problem>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftProblem in x)
        {
            var rightProblem = y.SingleOrDefault(_ => _.Id == leftProblem.Id);

            if (!Equals(leftProblem, rightProblem)) return false;
        }

        return true;
    }

    public int GetHashCode(Problem? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }

    public int GetHashCode(IList<Problem>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
