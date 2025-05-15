using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using Microservice.UnitTests.Data;
using Microservice.UnitTests.Helpers;
using Microservice.WebApi.Controllers;
using Microservice.WebApi.Entities;
using Microservice.WebApi.Models;
using Microservice.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class ProblemsControllerTests
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var methodInfos = problemsControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        Assert.NotEmpty(methodInfos);

        foreach (var methodInfo in methodInfos)
        {
            // Needs to be nullable so the compiler sees it's initialized
            // The Assert.Fail doesn't tell it that the line it's being used
            // will only ever be hit if it's initialized
            Type? attributeType = null;

            switch (methodInfo.Name.ToLower())
            {
                case "delete":
                    attributeType = typeof(HttpDeleteAttribute);
                    break;
                case "get":
                    attributeType = typeof(HttpGetAttribute);
                    break;
                case "head":
                    attributeType = typeof(HttpHeadAttribute);
                    break;
                case "options":
                    attributeType = typeof(HttpOptionsAttribute);
                    break;
                case "patch":
                    attributeType = typeof(HttpPatchAttribute);
                    break;
                case "post":
                    attributeType = typeof(HttpPostAttribute);
                    break;
                case "put":
                    attributeType = typeof(HttpPutAttribute);
                    break;
                default:
                    Assert.Fail("Unsupported public HTTP operation");
                    break;
            }

            var attributes = methodInfo.GetCustomAttributes(attributeType, false);

            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            Assert.Single(attributes);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Produces_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(ProducesAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var producesAttribute = (ProducesAttribute)attributes[0];

        Assert.Contains("application/json", producesAttribute.ContentTypes);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Route_Attribute()
    {
        var problemsControllerType = typeof(ProblemsController);

        var attributes = problemsControllerType.GetCustomAttributes(typeof(RouteAttribute), false);

        Assert.NotNull(attributes);
        Assert.NotEmpty(attributes);
        Assert.Single(attributes);

        var routeAttribute = (RouteAttribute)attributes[0];

        Assert.Equal("api/[controller]", routeAttribute.Template);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Delete_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(It.IsAny<string>(), default))
            .Returns(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Delete("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Delete_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Delete("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_By_Id_Should_Return_Ok()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(It.IsAny<string>(), default))
            .Returns(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Get("64639f6fcdde06187b09ecae");

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(problem!.ToModel(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var expectedProblemModel = problem.ToModel();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.Get(It.Is(problemId, new StringEqualityComparer())!, default))
            .Returns(problem);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = problemsController.Get(problemId!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemModel, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_By_Id_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var problemId = problem!.Id;

        var expectedProblemModel = problem.ToModel();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService
            .Setup(problemsService => problemsService.Get(It.Is(problemId, new StringEqualityComparer())!, default))
            .Returns(problem);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = problemsController.Get(problemId!);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Equal(expectedProblemModel, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var emptyProblemsList = new List<Problem>();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(default))
            .Returns(emptyProblemsList);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.Empty(actionResult.Value!);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_Should_Return_Ok_When_Not_Empty()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(default))
            .Returns(problemsList);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Get();

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(problemsList.Count, actionResult.Value!.Count);
        Assert.Equal(problemsList.ToModels(), actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Judge()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var expectedProblemModels = problemsList.ToModels();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(default))
            .Returns(problemsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(false);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemModels.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemModels, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Get_Should_Return_Ok_With_Test_Sets_Expanded_For_Participant()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problemsList = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var expectedProblemModels = problemsList.ToModels();

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(default))
            .Returns(problemsList);

        var identityMock = new Mock<IIdentity>();
        identityMock.Setup(i => i.Name).Returns("0000-000");

        var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
        claimsPrincipalMock.Setup(cp => cp.Identity).Returns(identityMock.Object);
        claimsPrincipalMock.Setup(cp => cp.IsInRole(It.IsAny<string>())).Returns(true);

        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipalMock.Object
        };

        var problemsController = new ProblemsController(mockedProblemsService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        var actionResult = problemsController.Get(true);

        // Assert
        Assert.NotNull(actionResult);
        Assert.NotNull(actionResult.Value);
        Assert.NotEmpty(actionResult.Value!);
        Assert.Equal(expectedProblemModels.Count, actionResult.Value!.Count);
        Assert.Equal(expectedProblemModels, actionResult.Value, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Post_Should_Return_Created()
    {
        // Arrange
        var newProblem = new ProblemModel
        {
            Description = "This is the description",
            Id = "12345",
            IsActive = true,
            Title = "This is the title"
        };

        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var createdAtActionResult = problemsController.Post(newProblem);

        // Assert
        Assert.NotNull(createdAtActionResult);

        Assert.IsType<ProblemModel>(createdAtActionResult.Value);

        mockedProblemsService.Verify(problemsService => problemsService.Create(It.Is(newProblem.ToEntity(), new ProblemEqualityComparer()), default),
            Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Put_Should_Return_No_Content()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var updatedProblem = new ProblemModel
        {
            Description = "This is the description",
            Id = "12345",
            IsActive = true,
            Title = "This is the title"
        };

        var mockedProblemsService = new Mock<IProblemsService>();
        mockedProblemsService.Setup(problemsService => problemsService.Get(It.Is(problem!.Id, new StringEqualityComparer())!, default))
            .Returns(problem);

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Put(problem!.Id!, updatedProblem);

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NoContentResult>(actionResult);

        mockedProblemsService.Verify(
            problemsService => problemsService.Update(It.Is(updatedProblem.ToEntity(), new ProblemEqualityComparer()), default), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Put_Should_Return_Not_Found()
    {
        // Arrange
        var mockedProblemsService = new Mock<IProblemsService>();

        var problemsController = new ProblemsController(mockedProblemsService.Object);

        // Act
        var actionResult = problemsController.Put("64639f6fcdde06187b09ecae", new ProblemModel());

        // Assert
        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
