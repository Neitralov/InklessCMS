global using System.Data.Common;
global using System.Net;
global using System.Security.Claims;
global using System.Text.Encodings.Web;

global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;

global using Xunit;
global using Respawn;
global using FluentAssertions;

global using Database;
global using Domain.Users;
global using WebAPI.Contracts.Article;
global using WebAPI.Contracts.Collection;
global using WebAPI.Contracts.User;
