global using System.Data.Common;
global using System.Security.Claims;
global using System.Text.Encodings.Web;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;

global using Xunit;
global using Respawn;
global using Shouldly;
global using Testcontainers.PostgreSql;
global using HotChocolate;
global using GraphQL.Client.Http;
global using GraphQL.Client.Serializer.SystemTextJson;
global using GraphQL.Client.Abstractions;

global using Database;
global using Domain.Articles;
global using Domain.Collections;
global using Domain.Users;
global using Domain.PagedList;
global using WebAPI.GraphQL.InputTypes;
global using WebAPI.GraphQL.OutputTypes;
global using WebAPI.IntegrationTests.GraphQL.Mutations.ArticleMutations;
global using WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;
global using WebAPI.IntegrationTests.GraphQL.Mutations.UserMutations;
global using WebAPI.IntegrationTests.GraphQL.Queries.ArticleQueries;
global using WebAPI.IntegrationTests.GraphQL.Queries.CollectionQueries;
