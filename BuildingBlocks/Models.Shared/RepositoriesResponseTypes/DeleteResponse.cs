using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class DeleteResponse : OneOfBase<Success, AccessDenied, NotFound>
    {
        protected DeleteResponse(OneOf<Success, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        public static implicit operator DeleteResponse(Success _) => new(_);
        public static implicit operator DeleteResponse(NotFound _) => new(_);
        public static implicit operator DeleteResponse(AccessDenied _) => new(_);
    }
}