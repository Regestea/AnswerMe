using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    /// <summary>
    /// Represents a response when deleting an item from a system.
    /// </summary>
    public class DeleteResponse : OneOfBase<Success, AccessDenied, NotFound>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteResponse"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        protected DeleteResponse(OneOf<Success, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        /// <summary>
        /// Implicitly converts an instance of Success class to an instance of DeleteResponse class.
        /// </summary>
        /// <param name="_">The Success instance to be converted.</param>
        /// <returns>A new instance of DeleteResponse.</returns>
        public static implicit operator DeleteResponse(Success _) => new(_);

        /// <summary>
        /// Converts a NotFound object into a DeleteResponse object.
        /// </summary>
        /// <param name="_">The NotFound object to convert.</param>
        /// <returns>The corresponding DeleteResponse object.</returns>
        public static implicit operator DeleteResponse(NotFound _) => new(_);

        /// Converts an AccessDenied object to a DeleteResponse object implicitly.
        /// @param _ The AccessDenied object to convert.
        /// @return A new DeleteResponse object.
        /// /
        public static implicit operator DeleteResponse(AccessDenied _) => new(_);
    }
}