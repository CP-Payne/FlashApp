using ErrorOr;
using FlashApp.Common.Helpers.Validation;
using FlashApp.Interfaces.Repository;
using FlashApp.Interfaces.Services;
using FlashApp.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlashApp.Services.Flashcard
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IFlashcardRepository _flashcardRepo;
        private readonly IValidator<CreateFlashcardCommand> _createCommandValidator;
        private readonly IValidator<UpdateFlashcardCommand> _updateFlashcardCommandValidator;
        private readonly IValidator<UpdateReviewScheduleCommand> _updateReviewScheduleCommandValidator;

        public FlashcardService(
            IFlashcardRepository flashcardRepo,
            IValidator<CreateFlashcardCommand> createCommandValidator,
            IValidator<UpdateFlashcardCommand> updateFlashcardCommandValidator,
            IValidator<UpdateReviewScheduleCommand> updateReviewScheduleCommandValidator
        )
        {
            _flashcardRepo = flashcardRepo;
            _createCommandValidator = createCommandValidator;
            _updateFlashcardCommandValidator = updateFlashcardCommandValidator;
            _updateReviewScheduleCommandValidator = updateReviewScheduleCommandValidator;
        }

        public async Task<ErrorOr<Models.Flashcard>> CreateFlashcardAsync(
            CreateFlashcardCommand command
        )
        {
            var validationResult = await _createCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return validationResult.ConvertToErrorList();
            }

            var existingFlashcard = await _flashcardRepo.GetByQuestionAndUserIdAsync(
                command.Question,
                command.AppUserId
            );
            if (existingFlashcard != null)
            {
                return Error.Conflict(
                    code: "Flashcard.DuplicateQuestion",
                    description: "A flashcard with this question already exists for your account."
                );
            }
            var flashcard = new Models.Flashcard
            {
                Id = Guid.NewGuid(),
                Answer = command.Answer,
                Question = command.Question,
                AppUserId = command.AppUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastReviewed = DateTime.MinValue,
                NextReview = DateTime.UtcNow, // TODO: Calculate initial review based on SR (or other) algorithms

                // TODO: Decide if I should add card to decks here
            };

            await _flashcardRepo.AddAsync(flashcard);
            bool success = await _flashcardRepo.SaveChangesAsync();

            if (!success)
            {
                return Error.Unexpected(
                    "Flashcard.CreateFailed",
                    "Failed to save the new flashcard."
                );
            }
            return flashcard;
        }

        public async Task<ErrorOr<Success>> DeleteFlashcardAsync(Guid flashcardId, string userId)
        {
            if (!Guid.TryParse(userId, out _))
            {
                return Error.Validation("UserId.Invalid", "User ID format is invalid.");
            }

            var flashcardToDelete = await _flashcardRepo.GetByIdAndUserIdAsync(flashcardId, userId);
            if (flashcardToDelete is null)
            {
                return Error.NotFound(
                    code: "Flashcard.NotFound",
                    description: "Flashcard not found or you do not have permission to delete it."
                );
            }

            _flashcardRepo.Delete(flashcardToDelete);
            var success = await _flashcardRepo.SaveChangesAsync();
            if (!success)
            {
                return Error.Failure(
                    code: "Flashcard.DeleteFailed",
                    "Failed to delete the flashcard."
                );
            }

            return Result.Success;
        }

        public async Task<ErrorOr<Models.Flashcard>> GetFlashcardByIdAndUserIdAsync(
            Guid flashcardId,
            string userId
        )
        {
            if (!Guid.TryParse(userId, out _))
            {
                // If this point is reached then something is wrong with JWT Token
                return Error.Validation("UserId.InvalidId", "User ID format is invalid.");
            }

            var flashcard = await _flashcardRepo.GetByIdAndUserIdAsync(flashcardId, userId);
            if (flashcard == null)
            {
                return Error.NotFound(
                    code: "Flashcard.NotFound",
                    description: "Flashcard not found"
                );
            }
            return flashcard;
        }

        public async Task<ErrorOr<List<Models.Flashcard>>> GetFlashcardsByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out _))
            {
                return Error.Validation("UserId.InvalidId", "User ID format is invalid.");
            }

            var flashcards = await _flashcardRepo.GetAllByUserIdAsync(userId);
            return flashcards;
        }

        public async Task<ErrorOr<Models.Flashcard>> UpdateFlashcardAsync(
            UpdateFlashcardCommand command
        )
        {
            var validationResult = await _updateFlashcardCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return validationResult.ConvertToErrorList();
            }
            var existingFlashcard = await _flashcardRepo.GetByIdAndUserIdAsync(
                command.FlashcardId,
                command.AppUserId
            );
            if (existingFlashcard is null)
            {
                return Error.NotFound(
                    code: "Flashcard.NotFound",
                    description: "Flashcard not found or you do not have permission to modify it."
                );
            }

            if (existingFlashcard.Question != command.Question)
            {
                var conflictingFlashcard = await _flashcardRepo.GetByQuestionAndUserIdAsync(
                    command.Question,
                    command.AppUserId
                );
                if (conflictingFlashcard != null && conflictingFlashcard.Id != existingFlashcard.Id)
                {
                    return Error.Conflict(
                        code: "Flashcard.DuplicateQuestion",
                        description: "A flashcard with this question already exists for your account."
                    );
                }
            }

            existingFlashcard.Question = command.Question;
            existingFlashcard.Answer = command.Answer;
            existingFlashcard.UpdatedAt = DateTime.UtcNow;

            _flashcardRepo.Update(existingFlashcard);
            var success = await _flashcardRepo.SaveChangesAsync();

            if (!success)
            {
                return Error.Failure(
                    code: "Flashcard.UpdateFailed",
                    description: "Failed to update the flashcard."
                );
            }

            return existingFlashcard;
        }

        public async Task<ErrorOr<Models.Flashcard>> UpdateFlashcardReviewScheduleAsync(
            UpdateReviewScheduleCommand command
        )
        {
            var validationResult = await _updateReviewScheduleCommandValidator.ValidateAsync(
                command
            );
            if (!validationResult.IsValid)
            {
                return validationResult.ConvertToErrorList();
            }
            var existingFlashcard = await _flashcardRepo.GetByIdAndUserIdAsync(
                command.FlashcardId,
                command.AppUserId
            );
            if (existingFlashcard is null)
            {
                return Error.NotFound(
                    code: "Flashcard.NotFound",
                    description: "Flashcard not found or you do not have permission to modify it."
                );
            }

            // TODO: Decide if validation for dates is needed

            existingFlashcard.LastReviewed = command.NewLastReviewed;
            existingFlashcard.NextReview = command.NewNextReview;
            existingFlashcard.UpdatedAt = DateTime.UtcNow;

            _flashcardRepo.Update(existingFlashcard);
            var success = await _flashcardRepo.SaveChangesAsync();

            if (!success)
            {
                return Error.Failure(
                    code: "Flashcard.UpdateFailed",
                    description: "Failed to update flashcard review schedule."
                );
            }

            return existingFlashcard;
        }
    }
}
