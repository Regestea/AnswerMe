namespace Models.Shared.OneOfTypes
{
    public class ValidationFailed
    {
        public string Field { get; set; } = null!;
        public string Error { get; set; } = null!;
    }
}