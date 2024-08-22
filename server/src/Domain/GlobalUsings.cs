global using System.Security.Cryptography;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text.RegularExpressions;
global using System.ComponentModel.DataAnnotations;

global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;

global using ErrorOr;

global using Domain.DomainErrors;
global using Domain.Interfaces;
global using Domain.Entities;
global using Domain.Utils;
global using Domain.Options;

global using TokensPair = (string AccessToken, string RefreshToken);