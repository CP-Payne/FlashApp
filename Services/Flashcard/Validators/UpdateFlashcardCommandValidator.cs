using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Common.Helpers.Validation;
using FlashApp.Interfaces.Services;
using FluentValidation;

namespace FlashApp.Services.Flashcard.Validators
{
    public class UpdateFlashcardCommandValidator : AbstractValidator<UpdateFlashcardCommand>
    {
        public UpdateFlashcardCommandValidator()
        {
            RuleFor(command => command.FlashcardId)
                .NotEmpty()
                .WithMessage("Flashcard ID is required to identify the flashcard to update.");

            RuleFor(command => command.AppUserId)
                .NotEmpty()
                .WithMessage("User ID is required to verify ownership.")
                .Must(ValidationHelper.BeAValidGuid)
                .WithMessage("User ID must be a valid GUID format.");

            RuleFor(command => command.Question)
                .NotEmpty()
                .WithMessage("Question is required.")
                .Length(3, 500)
                .WithMessage("Question must be between 3 and 500 characters.");

            RuleFor(command => command.Answer)
                .NotEmpty()
                .WithMessage("Answer is required.")
                .Length(1, 1000)
                .WithMessage("Answer must be between 1 and 1000 characters.");
        }
    }
}
