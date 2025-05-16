using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlashApp.Dtos.Flashcard;
using FlashApp.Interfaces.Services;
using FlashApp.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashApp.Controllers
{
    [Route("api/flashcards")]
    public class FlashcardController : ApiController
    {
        private readonly IFlashcardService _flashcardService;

        public FlashcardController(IFlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
        }

        [HttpPut("{flashcardId:guid}")]
        public async Task<IActionResult> UpdateFlashcard(
            Guid flashcardId,
            [FromBody] UpdateFlashcardDto updateDto
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(
                    Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User ID not found in token."
                    )
                );
            }

            var command = new UpdateFlashcardCommand(
                flashcardId,
                updateDto.Question,
                updateDto.Answer,
                userId
            );

            var updateResult = await _flashcardService.UpdateFlashcardAsync(command);

            return updateResult.Match(
                updatedFlashcard =>
                {
                    var responseDto = updatedFlashcard.ToFlashcardDto();
                    return Ok(responseDto);
                },
                errors => Problem(errors)
            );
        }

        [HttpDelete("{flashcardId:guid}")]
        public async Task<IActionResult> DeleteFlashcard([FromRoute] Guid flashcardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(
                    Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User ID not found in token."
                    )
                );
            }

            var deleteResult = await _flashcardService.DeleteFlashcardAsync(flashcardId, userId);

            return deleteResult.Match(success => NoContent(), errors => Problem(errors));
        }

        [HttpPost("{flashcardId:guid}/review-schedule")]
        public async Task<IActionResult> UpdateReviewSchedule(
            Guid flashcardId,
            [FromBody] UpdateReviewScheduleDto reviewDto
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(
                    Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User ID not found in token."
                    )
                );
            }

            var command = new UpdateReviewScheduleCommand(
                flashcardId,
                reviewDto.NewLastReviewed,
                reviewDto.NewNextReview,
                userId
            );

            var updateResult = await _flashcardService.UpdateFlashcardReviewScheduleAsync(command);

            return updateResult.Match(
                updatedFlashcard =>
                {
                    var responseDto = updatedFlashcard.ToFlashcardDto();
                    return Ok(responseDto);
                },
                errors => Problem(errors)
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcard([FromBody] CreateFlashcardDto createDto)
        {
            // Automatically validated from annotation with [ApiController]
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // This case should be rare if [Authorize] is working and the token is valid.
                // If this case is reached then it might indicate a problem with token generation or authentication setup.
                return Unauthorized(
                    Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User ID not found in token."
                    )
                );
            }

            var command = new CreateFlashcardCommand(
                createDto.Question,
                createDto.Answer,
                userId
            // createDto.DeckIds // TODO: when I add decks
            );

            var createResult = await _flashcardService.CreateFlashcardAsync(command);

            return createResult.Match(
                flashcard =>
                {
                    var responseDto = flashcard.ToFlashcardDto();

                    return CreatedAtAction(
                        nameof(GetFlashcardById),
                        new { flashcardId = responseDto.Id },
                        responseDto
                    );
                },
                errors => Problem(errors)
            );
        }

        [HttpGet]
        [Route("{flashcardId:guid}")]
        public async Task<IActionResult> GetFlashcardById([FromRoute] Guid flashcardId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(
                    Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "User ID not found in token."
                    )
                );
            }

            var getResult = await _flashcardService.GetFlashcardByIdAndUserIdAsync(
                flashcardId,
                userId
            );

            return getResult.Match(
                flashcard =>
                {
                    var responseDto = flashcard.ToFlashcardDto();
                    return Ok(responseDto);
                },
                errors => Problem(errors)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFlashcards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(
                    new { message = "User ID claim (sub/NameIdentifier) not found in token." }
                );
            }

            var getResult = await _flashcardService.GetFlashcardsByUserIdAsync(userId);

            return getResult.Match(
                flashcards =>
                {
                    var responseDtos = flashcards.Select(f => f.ToFlashcardDto()).ToList();
                    return Ok(responseDtos);
                },
                errors => Problem(errors)
            );
        }
    }
}
