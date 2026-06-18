namespace DTOs
{
    public record SemanticBookResultDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public double Score { get; set; }
    }
}
