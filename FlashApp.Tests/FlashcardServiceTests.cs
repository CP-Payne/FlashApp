using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Interfaces.Repository;
using FlashApp.Interfaces.Services;
using FlashApp.Models;
using FlashApp.Services.Flashcard;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace FlashApp.Tests
{
    public class FlashcardServiceTests
    {
        private readonly Mock<IFlashcardRepository> _mockFlashcardRepo;
        private readonly Mock<IValidator<CreateFlashcardCommand>> _mockCreateCommandValidator;
        private readonly Mock<IValidator<UpdateFlashcardCommand>> _mockUpdateCommandValidator;
        private readonly Mock<
            IValidator<UpdateReviewScheduleCommand>
        > _mockUpdateReviewScheduleCommandValidator;

        private readonly FlashcardService _flashcardService;

        public FlashcardServiceTests()
        {
            _mockFlashcardRepo = new Mock<IFlashcardRepository>();
            _mockCreateCommandValidator = new Mock<IValidator<CreateFlashcardCommand>>();
            _mockUpdateCommandValidator = new Mock<IValidator<UpdateFlashcardCommand>>();
            _mockUpdateReviewScheduleCommandValidator =
                new Mock<IValidator<UpdateReviewScheduleCommand>>();

            _flashcardService = new FlashcardService(
                _mockFlashcardRepo.Object,
                _mockCreateCommandValidator.Object,
                _mockUpdateCommandValidator.Object,
                _mockUpdateReviewScheduleCommandValidator.Object
            );
        }

        [Fact]
        public async Task CreateFlashcardAsync_WithValidCommand_ShouldReturnFlashcard()
        {
            var command = new CreateFlashcardCommand(
                "What is C#",
                "A programming language",
                Guid.NewGuid().ToString()
            );

            var validationResult = new ValidationResult();

            _mockCreateCommandValidator
                .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            Flashcard capturedFlashcard = null;
            _mockFlashcardRepo
                .Setup(repo => repo.AddAsync(It.IsAny<Flashcard>()))
                .Callback<Flashcard>(fc => capturedFlashcard = fc)
                .Returns(Task.CompletedTask);

            _mockFlashcardRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _flashcardService.CreateFlashcardAsync(command);

            result.IsError.Should().BeFalse();
            result.Value.Question.Should().Be(command.Question);
            result.Value.Answer.Should().Be(command.Answer);
            result.Value.AppUserId.Should().Be(command.AppUserId);
            result.Value.Id.Should().NotBe(Guid.Empty);
            result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            result.Value.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            result.Value.LastReviewed.Should().Be(DateTime.MinValue);
            result.Value.NextReview.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            // Verify that repository methods were called as expected
            _mockFlashcardRepo.Verify(repo => repo.AddAsync(It.IsAny<Flashcard>()), Times.Once);
            _mockFlashcardRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);

            // Assert on the captured flashcard
            capturedFlashcard.Should().NotBeNull();
            capturedFlashcard.Question.Should().Be(command.Question);
        }
    }
}
