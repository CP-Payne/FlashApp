using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using FlashApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlashApp.Interfaces.Services
{
    public record UpdateFlashcardCommand(
        Guid FlashcardId,
        string Question,
        string Answer,
        string AppUserId
    );

    public record UpdateReviewScheduleCommand(
        Guid FlashcardId,
        DateTime NewLastReviewed,
        DateTime NewNextReview,
        string AppUserId
    );

    public record CreateFlashcardCommand(
        string Question,
        string Answer,
        string AppUserId
    //List<Guid>? DeckIds // TODO
    );

    public interface IFlashcardService
    {
        Task<ErrorOr<List<Models.Flashcard>>> GetFlashcardsByUserIdAsync(string userId);
        Task<ErrorOr<Models.Flashcard>> GetFlashcardByIdAndUserIdAsync(
            Guid flashcardId,
            string userId
        );
        Task<ErrorOr<Models.Flashcard>> CreateFlashcardAsync(CreateFlashcardCommand command);

        Task<ErrorOr<Models.Flashcard>> UpdateFlashcardAsync(UpdateFlashcardCommand command);
        Task<ErrorOr<Success>> DeleteFlashcardAsync(Guid flashcardId, string userId); // Success is a special value from errorOr
        Task<ErrorOr<Models.Flashcard>> UpdateFlashcardReviewScheduleAsync(
            UpdateReviewScheduleCommand command
        );
    }
}
