using AnswerMe.Application.DTOs.Error;
using OneOf;
using OneOf.Types;

namespace AnswerMe.Application.RepositoriesResponseTypes
{
    public class UpdateResponse : OneOfBase<Success, Success<Guid>, List<ValidationFailedDto>, Error<string>>
    {
        protected UpdateResponse(OneOf<Success, Success<Guid>, List<ValidationFailedDto>, Error<string>> input)
            : base(input)
        {
        }

        public static implicit operator UpdateResponse(Success _) => new(_);

        public static implicit operator UpdateResponse(Success<Guid> _) => new(_);

        public static implicit operator UpdateResponse(Error<string> _) => new(_);

        public static implicit operator UpdateResponse(List<ValidationFailedDto> _) => new(_);
    }
}