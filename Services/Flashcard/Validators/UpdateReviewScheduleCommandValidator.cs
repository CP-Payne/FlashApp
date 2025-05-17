using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Common.Helpers.Validation;
using FlashApp.Interfaces.Services;
using FluentValidation;

namespace FlashApp.Services.Flashcard.Validators
{
    public class UpdateReviewScheduleCommandValidator
        : AbstractValidator<UpdateReviewScheduleCommand>
    {
        public UpdateReviewScheduleCommandValidator()
        {
            RuleFor(command => command.FlashcardId)
                .NotEmpty()
                .WithMessage("Flashcard ID is required to identify the flashcard to update.");

            RuleFor(command => command.AppUserId)
                .NotEmpty()
                .WithMessage("User ID is required to verify ownership.")
                .Must(ValidationHelper.BeAValidGuid)
                .WithMessage("User ID must be a valid GUID format.");

            RuleFor(command => command.NewLastReviewed)
                .NotEmpty()
                .WithMessage("New Last Reviewed date is required.")
                .NotEqual(default(DateTime))
                .WithMessage("New Last Reviewed date must be a valid date.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Last Reviewed date cannot be in the future.");

            RuleFor(command => command.NewNextReview)
                .NotEmpty()
                .WithMessage("New Next Review date is required.")
                .NotEqual(default(DateTime))
                .WithMessage("New Next Review date must be a valid date.")
                .GreaterThan(command => command.NewLastReviewed)
                .WithMessage("Next Review date must be after the Last Reviewed date.")
                .When(command => command.NewLastReviewed != default(DateTime));

            RuleFor(command => command.NewNextReview)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Next Review date must be in the future.")
                .When(command => command.NewNextReview != default(DateTime));
        }
    }
}
