using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    /// <summary>
    /// Represents a response type that can be used for reading operations.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response data.</typeparam>
    public class ReadResponse<TResponse> : OneOfBase<Success<TResponse>, AccessDenied, NotFound>
    {
        /// <summary>
        /// Creates a new instance of the ReadResponse class with the given input.
        /// </summary>
        /// <param name="input">The input of type OneOf&lt;Success&lt;TResponse&gt;, AccessDenied, NotFound&gt;.</param>
        protected ReadResponse(OneOf<Success<TResponse>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        /// <summary>
        /// Implicitly converts a Success instance to a ReadResponse instance.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="_">The Success instance.</param>
        /// <returns>A new ReadResponse instance.</returns>
        public static implicit operator ReadResponse<TResponse>(Success<TResponse> _) => new(_);

        /// <summary>
        /// Implicitly converts an instance of <see cref="AccessDenied"/> to an instance of <see cref
        /// ="ReadResponse{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="_">The instance of <see cref="AccessDenied"/> to be converted.</param>
        /// <returns>An instance of <see cref="ReadResponse{TResponse}"/> initialized with the converted <see cref="AccessDenied"/> instance.</returns>
        public static implicit operator ReadResponse<TResponse>(AccessDenied _) => new(_);

        /// <summary>
        /// Implicitly converts a NotFound object to a ReadResponse object. </summary> <typeparam name="TResponse">The type of the response object.</typeparam> <param name="_">The NotFound object to convert.</param> <returns>A new ReadResponse object.</returns>
        /// /
        public static implicit operator ReadResponse<TResponse>(NotFound _) => new(_);
    }
}