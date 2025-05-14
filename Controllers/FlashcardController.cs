using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlashApp.Dtos.Flashcard;
using FlashApp.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashApp.Controllers
{
    [Route("flashcard")]
    public class FlashcardController : ApiController
    {
        private readonly IFlashcardService _flashcardService;

        public FlashcardController(IFlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFlashcardDto flashcardDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // This case should be rare if [Authorize] is working and the token is valid.
                // If this case is reached then it might indicate a problem with token generation or authentication setup.
                return Unauthorized(
                    new { message = "User ID claim (sub/NameIdentifier) not found in token." }
                );
            }

            var flashcardResult = await _flashcardService.CreateFlashcard(
                new FlashcardCreateCommand
                {
                    Question = flashcardDto.Question,
                    Answer = flashcardDto.Answer,
                    AppUserId = userId,
                }
            );

            return Ok(
                new FlashcardDto
                {
                    Answer = flashcardResult.Answer,
                    Question = flashcardResult.Question,
                    Id = flashcardResult.Id,
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFlashcards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // This case should be rare if [Authorize] is working and the token is valid.
                // If this case is reached then it might indicate a problem with token generation or authentication setup.
                return Unauthorized(
                    new { message = "User ID claim (sub/NameIdentifier) not found in token." }
                );
            }
            Console.WriteLine(
                "===================================================USERID:" + userId
            );
            var flashcardsResults = await _flashcardService.GetAllByUserIdAsync(Guid.Parse(userId));

            return Ok(
                flashcardsResults
                    .Select(fr => new FlashcardDto
                    {
                        Id = fr.Id,
                        Question = fr.Question,
                        Answer = fr.Answer,
                    })
                    .ToList()
            );
        }
    }
}
