using System;

namespace Entities
{
    public class Rating
    {
        public int CallId { get; set; }
        public DateTime RequestDateTime { get; set; }
        public int? UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}