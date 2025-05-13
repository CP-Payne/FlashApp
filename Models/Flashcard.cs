using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Models
{
    [Table("Flashcards")]
    public class Flashcard
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        public DateTime LastReviewed { get; set; } = DateTime.UtcNow;
        public DateTime NextReview { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string AppUserId { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }

        public List<Deck> Decks { get; set; } = new List<Deck>();
    }
}
