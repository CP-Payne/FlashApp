using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Services
{
    public record FlashcardResult(Guid Id, string Question, string Answer);

    public class FlashcardCreateCommand
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string AppUserId { get; set; } = string.Empty;
    }

    public interface IFlashcardService
    {
        Task<List<FlashcardResult>> GetAllByUserIdAsync(Guid userId);
        Task<FlashcardResult> CreateFlashcard(FlashcardCreateCommand command);
    }
}
