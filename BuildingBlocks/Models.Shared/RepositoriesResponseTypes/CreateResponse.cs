using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    /// Represents a response for
    /// creating an entity.
    /// @typeparam TResponse The type
    /// of the response data.
    /// /
    public class CreateResponse<TResponse> : OneOfBase<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>,AccessDenied,NotFound>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CreateResponse{TResponse
        /// }"/> class.
        /// </summary>
        /// <param name="input">An <see cref
        /// ="OneOf"/> object representing the
        /// result of the create operation.</param>
        protected CreateResponse(OneOf<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        /// <summary>
        /// Implicitly converts a <see cref="Success{TResponse}"/> object to a <see cref="Create
        /// Response{TResponse}"/> object.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="_">The <see cref="Success{TResponse}"/> object to convert.</param>
        /// <returns>A new <see cref="CreateResponse{TResponse}"/> object.</returns>
        public static implicit operator CreateResponse<TResponse>(Success<TResponse> _) => new(_);

        /// <summary>
        /// Implicitly converts a NotFound object to a CreateResponse of type TResponse.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="_">The NotFound object.</param>
        /// <returns>A new CreateResponse object of type TResponse.</returns>
        public static implicit operator CreateResponse<TResponse>(NotFound _) => new(_);

        /// <summary>
        /// Implicitly converts an <see cref="Error{string}"/> instance to a <see cref="CreateResponse
        /// {TResponse}"/> instance.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="_">The instance of <see cref="Error{string}"/> to convert.</param>
        /// <returns>A new instance of <see cref="CreateResponse{TResponse}"/> with the converted error.</returns>
        public static implicit operator CreateResponse<TResponse>(Error<string> _) => new(_);

        /// <summary>
        /// This code represents an implicit conversion operator in C#.
        /// It allows implicitly converting an instance of the AccessDenied class to an instance of
        /// the CreateResponse class,
        /// where TResponse is the type of the response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <param name="_">The instance of the AccessDenied class to be converted</param>
        /// <returns>An instance of the CreateResponse class</returns>
        public static implicit operator CreateResponse<TResponse>(AccessDenied _) => new(_);

        /// <summary>
        /// Implicitly converts a <see cref="List{ValidationFailed}"/> to a <see cref="
        /// CreateResponse{TResponse}"/>.
        /// </summary>
        /// <typeparam name="TResponse">The type of response.</typeparam>
        /// <param name="_">The <see cref="List{ValidationFailed}"/> instance to convert.</param>
        /// <returns>A new instance of <see cref="CreateResponse{TResponse}"/> with the converted value.</returns>
        public static implicit operator CreateResponse<TResponse>(List<ValidationFailed> _) => new(_);

        /// Implicitly converts a ValidationFailed object to a CreateResponse of type TResponse
        /// .
        /// @param _ The ValidationFailed object to convert.
        /// @return A CreateResponse of type TResponse.
        /// /
        public static implicit operator CreateResponse<TResponse>(ValidationFailed _) => new(_);
    }
}