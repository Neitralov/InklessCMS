namespace WebAPI.Contracts.User;

public record LoginUserResponse(
    string AccessToken,
    string RefreshToken)
{
    /// <example>eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRmZDI2ZjRjLWEyYjAtNGVlYS1hMjcyLWI1YjI5YTU5Yjk0ZCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJhZG1pbkBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImV4cCI6MTcwNjA5NjU4M30.Hu6kDy4OIcO23DF544M8jmb3vIqNguAf7G9eCOXANYqkzxCEvD3PXiUrQ6kYL7dACvxz61EZEjfPBHO1ilNVuw</example>
    public string AccessToken { get; init; } = AccessToken;

    /// <example>4a367fa2-a4c0-4376-88b5-9635e1ea99df</example>
    public string RefreshToken { get; init; } = RefreshToken;
}