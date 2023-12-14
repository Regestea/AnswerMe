using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class UpdateResponse : OneOfBase<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound>
    {
        private UpdateResponse(OneOf<Success, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }
        
        public Success AsSuccess => AsT0;
        public List<ValidationFailed> AsValidationFailureList => AsT1;
        public ValidationFailed AsValidationFailure => AsT2;
        public Error<string> AsError => AsT3;
        public AccessDenied AsAccessDenied => AsT4;
        public NotFound AsNotFound => AsT5;

        public bool IsSuccess => IsT0;
        public bool IsValidationFailure => IsT1;
        public bool IsSingleValidationFailure => IsT2;
        public bool IsError => IsT3;
        public bool IsAccessDenied => IsT4;
        public bool IsNotFound => IsT5;
        
        
        public static implicit operator UpdateResponse(Success success) => new(success);
        public static implicit operator UpdateResponse(NotFound notFound) => new(notFound);
        public static implicit operator UpdateResponse(Error<string> error) => new(error);
        public static implicit operator UpdateResponse(AccessDenied accessDenied) => new(accessDenied);
        public static implicit operator UpdateResponse(List<ValidationFailed> validationFailures) => new(validationFailures);
        public static implicit operator UpdateResponse(ValidationFailed validationFailure) => new(validationFailure);
    }
}