using System.ComponentModel.DataAnnotations;

namespace Elysium.WebFinger.Services
{
    public class WebFingerQuery
    {
        [Required]
        public required string Resource { get; set; }
        public List<string>? Rel { get; set; }
    }
}
