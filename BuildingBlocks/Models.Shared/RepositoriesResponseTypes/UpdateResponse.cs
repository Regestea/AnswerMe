using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class UpdateResponse : OneOfBase<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound>
    {
        /// <summary>
        /// Initializes a new instance of the UpdateResponse
        /// class.
        /// </summary>
        /// <param name="input">The result of the update operation.</param>
        protected UpdateResponse(OneOf<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        /// <summary>
        /// Implicitly converts a <see cref="Success"/> object to an <see cref="UpdateResponse"/> object.
        /// </summary>
        /// <param name="_">The <see cref="Success"/> object to convert.</param>
        /// <returns>An <see cref="UpdateResponse"/> object initialized with the given <see cref="Success"/> object.</returns>
        public static implicit operator UpdateResponse(Success _) => new(_);

        /// <summary>
        /// Implicitly converts a NotFound object to an UpdateResponse object.
        /// </summary>
        /// <param name="_">The NotFound object to convert.</param>
        /// <returns>The converted UpdateResponse object.</returns>
        public static implicit operator UpdateResponse(NotFound _) => new(_);

        /// <summary>
        /// Implicitly converts an <see cref="Error{T}"/> to an <see cref="UpdateResponse"/>.
        /// </summary>
        /// <param name="_">The <see cref="Error{T}"/> object to be converted.</param>
        /// <returns>The converted <see cref="UpdateResponse"/> object.</returns>
        public static implicit operator UpdateResponse(Error<string> _) => new(_);

        /// <summary>
        /// Implicitly converts an <see cref="AccessDenied"/> object to an <see cref="UpdateResponse"/> object
        /// .
        /// </summary>
        /// <param name="_">The <see cref="AccessDenied"/> object to be converted.</param>
        /// <returns>The converted <see cref="UpdateResponse"/> object.</returns>
        public static implicit operator UpdateResponse(AccessDenied _) => new(_);

        /// <summary>
        /// Implicitly converts a List of ValidationFailed objects to an UpdateResponse object.
        /// </summary>
        /// <param name="_">The List of ValidationFailed objects to be converted.</param>
        /// <returns>The converted UpdateResponse object.</returns>
        public static implicit operator UpdateResponse(List<ValidationFailed> _) => new(_);

        /// <summary>
        /// Implicitly converts a <see cref="ValidationFailed"/> instance to an <see cref="UpdateResponse
        /// "/> instance.
        /// </summary>
        /// <param name="_">The <see cref="ValidationFailed"/> instance to be converted.</param>
        /// <returns>An <see cref="UpdateResponse"/> instance.</returns>
        public static implicit operator UpdateResponse(ValidationFailed _) => new(_);
    }
}