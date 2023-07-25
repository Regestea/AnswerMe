using OneOf.Types;
using OneOf;

namespace AnswerMe.Application.RepositoriesResponseTypes
{
    public class DeleteResponse : OneOfBase<Success>
    {
        protected DeleteResponse(OneOf<Success> input)
            : base(input)
        {
        }

        public static implicit operator DeleteResponse(Success _) => new(_);
    }
}