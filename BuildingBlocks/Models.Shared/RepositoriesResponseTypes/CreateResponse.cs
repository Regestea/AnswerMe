using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;
using System.Collections.Generic;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class CreateResponse<TResponse> : OneOfBase<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound>
    {
        private CreateResponse(OneOf<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        public Success<TResponse> AsSuccess => AsT0;
        public List<ValidationFailed> AsValidationFailureList => AsT1;
        public ValidationFailed AsValidationFailure => AsT2;
        public Error<string> AsError => AsT3;
        public AccessDenied AsAccessDenied => AsT4;
        public NotFound AsNotFound => AsT5;

        public bool IsSuccess => IsT0;
        public bool IsValidationFailureList => IsT1;
        public bool IsValidationFailure => IsT2;
        public bool IsError => IsT3;
        public bool IsAccessDenied => IsT4;
        public bool IsNotFound => IsT5;

        public static implicit operator CreateResponse<TResponse>(Success<TResponse> success) => new(success);
        public static implicit operator CreateResponse<TResponse>(NotFound notFound) => new(notFound);
        public static implicit operator CreateResponse<TResponse>(Error<string> error) => new(error);
        public static implicit operator CreateResponse<TResponse>(AccessDenied accessDenied) => new(accessDenied);
        public static implicit operator CreateResponse<TResponse>(List<ValidationFailed> validationFailureList) => new(validationFailureList);
        public static implicit operator CreateResponse<TResponse>(ValidationFailed validationFailure) => new(validationFailure);
    }
}
