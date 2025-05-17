using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Dtos.Flashcard
{
    public class UpdateReviewScheduleDto
    {
        [Required]
        public DateTime NewLastReviewed { get; set; }

        [Required]
        public DateTime NewNextReview { get; set; }
    }
}
