using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Dtos.Flashcard;
using FlashApp.Models;

namespace FlashApp.Mapper
{
    public static class FlashcardMapper
    {
        public static FlashcardDto ToFlashcardDto(this Flashcard flashcard)
        {
            return new FlashcardDto
            {
                Id = flashcard.Id,
                Answer = flashcard.Answer,
                Question = flashcard.Question,
                CreatedAt = flashcard.CreatedAt,
                LastReviewed = flashcard.LastReviewed,
                NextReview = flashcard.NextReview,
                UpdatedAt = flashcard.UpdatedAt,
            };
        }
    }
}
