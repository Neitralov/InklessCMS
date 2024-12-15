using Microsoft.AspNetCore.Mvc;

namespace Domain.PagedList;

public record PageOptions(int Page = 1, int Size = 10);