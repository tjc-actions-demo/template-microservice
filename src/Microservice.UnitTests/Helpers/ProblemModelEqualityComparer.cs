using System.Diagnostics.CodeAnalysis;
using Microservice.WebApi.Models;

namespace Microservice.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal class ProblemModelEqualityComparer : IEqualityComparer<ProblemModel?>, IEqualityComparer<IList<ProblemModel>?>
{
    public bool Equals(ProblemModel? x, ProblemModel? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        return
            x.Description == y.Description &&
            x.Id == y.Id &&
            x.IsActive == y.IsActive &&
            x.Title == y.Title;
    }

    public bool Equals(IList<ProblemModel>? x, IList<ProblemModel>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        foreach (var leftProblemModel in x)
        {
            var rightProblemModel = y.SingleOrDefault(_ => _.Id == leftProblemModel.Id);

            if (!Equals(leftProblemModel, rightProblemModel)) return false;
        }

        return true;
    }

    public int GetHashCode(ProblemModel obj)
    {
        return HashCode.Combine(obj.Description, obj.Id, obj.IsActive, obj.Title);
    }

    public int GetHashCode(IList<ProblemModel>? obj)
    {
        return obj == null ? 0 : obj.GetHashCode();
    }
}
