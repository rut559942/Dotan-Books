namespace DTOs
{
    public record AuthResultDto
    {
        public string Token { get; init; } = string.Empty;
        public CustomerDto User { get; init; } = new();
    }
}