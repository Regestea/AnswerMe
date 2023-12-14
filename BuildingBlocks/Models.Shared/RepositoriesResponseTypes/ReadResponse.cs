using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class ReadResponse<TResponse> : OneOfBase<Success<TResponse>, AccessDenied, NotFound>
    {
        private ReadResponse(OneOf<Success<TResponse>, AccessDenied, NotFound> input)
            : base(input)
        {
        }
        
        public Success<TResponse> AsSuccess => AsT0;
        public AccessDenied AsAccessDenied => AsT1;
        public NotFound AsNotFound => AsT2;

        public bool IsSuccess => IsT0;
        public bool IsAccessDenied => IsT1;
        public bool IsNotFound => IsT2;
        
        public static implicit operator ReadResponse<TResponse>(Success<TResponse> success) => new(success);
        public static implicit operator ReadResponse<TResponse>(AccessDenied accessDenied) => new(accessDenied);
        public static implicit operator ReadResponse<TResponse>(NotFound notFound) => new(notFound);
    }
}