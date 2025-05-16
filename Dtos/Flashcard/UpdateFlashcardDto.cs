using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Dtos.Flashcard
{
    public class UpdateFlashcardDto
    {
        [Required(ErrorMessage = "Question is required.")]
        [StringLength(
            500,
            MinimumLength = 3,
            ErrorMessage = "Question must be between 3 and 500 characters."
        )]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Answer is required.")]
        [StringLength(
            1000,
            MinimumLength = 1,
            ErrorMessage = "Answer must be between 1 and 1000 characters."
        )]
        public string Answer { get; set; } = string.Empty;
    }
}
