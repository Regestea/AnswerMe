using Models.Shared.Requests.Shared;

namespace AnswerMe.Client.Core.Extensions;

public static class PaginationExtension
{
    public static string AddPagination(this string address, PaginationRequest request)
    {
        return address + "?" + nameof(request.CurrentPage) + "=" + request.CurrentPage + "&" +
               nameof(request.PageSize) + "=" + request.CurrentPage;
    }
}