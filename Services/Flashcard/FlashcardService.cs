using FlashApp.Interfaces.Repository;
using FlashApp.Interfaces.Services;
using FlashApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlashApp.Services.Flashcard
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepo;

        public FlashcardService(IFlashcardRepository flashcardRepo)
        {
            _flashcardRepo = flashcardRepo;
        }

        public async Task<FlashcardResult> CreateFlashcard(FlashcardCreateCommand command)
        {
            // Will validate the command for business rules at a later time
            // Will create mappers at a later time.
            Models.Flashcard flashcardModel = new Models.Flashcard
            {
                Id = Guid.NewGuid(),
                Answer = command.Answer,
                Question = command.Question,
                AppUserId = command.AppUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastReviewed = DateTime.UtcNow,
                NextReview = DateTime.UtcNow,
            };

            flashcardModel = await _flashcardRepo.AddAsync(flashcardModel);

            return new FlashcardResult(
                flashcardModel.Id,
                flashcardModel.Question,
                flashcardModel.Answer
            );
        }

        public async Task<List<FlashcardResult>> GetAllByUserIdAsync(Guid userId)
        {
            var flashcards = await _flashcardRepo.GetAllByUserIdAsync(userId);

            return flashcards
                .Select(f => new FlashcardResult(Id: f.Id, Question: f.Question, Answer: f.Answer))
                .ToList();
        }
    }
}
