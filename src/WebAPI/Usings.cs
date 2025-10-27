global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;

global using ErrorOr;
global using HotChocolate;

global using Database;
global using Database.Repositories;
global using Domain.Articles;
global using Domain.Authorization;
global using Domain.Collections;
global using Domain.Users;
global using Domain.PagedList;
global using WebAPI;
global using WebAPI.GraphQL.Queries;
global using WebAPI.GraphQL.Mutations;
global using WebAPI.GraphQL.InputTypes;
global using WebAPI.GraphQL.OutputTypes;

global using Error = ErrorOr.Error;
global using GqlAuthorize = HotChocolate.Authorization.AuthorizeAttribute;
