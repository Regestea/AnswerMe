using Models.Shared.Requests.Shared;

namespace AnswerMe.Client.Core.Extensions;

public static class UrlExtension
{
    public static string AddPagination(this string address, PaginationRequest request)
    {
        return address +"&" + nameof(request.CurrentPage) + "=" + request.CurrentPage + "&" +
               nameof(request.PageSize) + "=" + request.CurrentPage;
    }

    public static string AddQuery(this string address,string fieldName,string value)
    {
        return address + "&" + fieldName + "=" + value;
    }
}