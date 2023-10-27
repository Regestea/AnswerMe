using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class UpdateResponse : OneOfBase<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound>
    {
        protected UpdateResponse(OneOf<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        public static implicit operator UpdateResponse(Success _) => new(_);

        public static implicit operator UpdateResponse(NotFound _) => new(_);

        public static implicit operator UpdateResponse(Error<string> _) => new(_);

        public static implicit operator UpdateResponse(AccessDenied _) => new(_);

        public static implicit operator UpdateResponse(List<ValidationFailed> _) => new(_);

        public static implicit operator UpdateResponse(ValidationFailed _) => new(_);
    }
}