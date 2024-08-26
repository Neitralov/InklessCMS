global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;

global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;

global using ErrorOr;
global using Mapster;
global using Swashbuckle.AspNetCore.Filters;

global using Database;
global using Database.Repositories;
global using Domain.Services;
global using Domain.Entities;
global using Domain.Interfaces;
global using Domain.DomainErrors;
global using Domain.Utils;
global using Domain.Options;
global using WebAPI;
global using WebAPI.Contracts.Article;
global using WebAPI.Contracts.Collection;
global using WebAPI.Contracts.User;

global using TokensPair = (string AccessToken, string RefreshToken);
