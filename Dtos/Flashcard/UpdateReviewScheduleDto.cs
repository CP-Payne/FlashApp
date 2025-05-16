using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Dtos.Flashcard
{
    public class UpdateReviewScheduleDto : IValidatableObject
    {
        [Required]
        public DateTime NewLastReviewed { get; set; }

        [Required]
        public DateTime NewNextReview { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewNextReview <= NewLastReviewed)
            {
                yield return new ValidationResult(
                    "NewNextReview must be later than NewLastReviewed",
                    new[] { nameof(NewNextReview) }
                );
            }

            if (NewNextReview < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "NewNextReview must be later than the current date (UTC).",
                    new[] { nameof(NewNextReview) }
                );
            }
        }
    }
}
