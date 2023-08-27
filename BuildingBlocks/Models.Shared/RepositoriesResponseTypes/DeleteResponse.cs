using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
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