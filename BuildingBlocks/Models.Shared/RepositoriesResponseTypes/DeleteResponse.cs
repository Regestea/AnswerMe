using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class DeleteResponse : OneOfBase<Success, AccessDenied, NotFound>
    {
        private DeleteResponse(OneOf<Success, AccessDenied, NotFound> input)
            : base(input)
        {
        }
        
        public Success AsSuccess => AsT0;
        public AccessDenied AsAccessDenied => AsT1;
        public NotFound AsNotFound => AsT2;

        public bool IsSuccess => IsT0;
        public bool IsAccessDenied => IsT1;
        public bool IsNotFound => IsT2;
        
        public static implicit operator DeleteResponse(Success success) => new(success);
        public static implicit operator DeleteResponse(NotFound notFound) => new(notFound);
        public static implicit operator DeleteResponse(AccessDenied accessDenied) => new(accessDenied);
    }
}