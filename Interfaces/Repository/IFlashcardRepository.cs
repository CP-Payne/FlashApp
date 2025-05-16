using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Repository
{
    public interface IFlashcardRepository
    {
        // Completed
        Task<List<Flashcard>> GetAllByUserIdAsync(string userId);

        // Completed
        Task<Flashcard?> GetByIdAndUserIdAsync(Guid flashcardId, string userId);

        // Completed
        Task<Flashcard?> GetByQuestionAndUserIdAsync(string question, string userId);

        // Completed
        Task AddAsync(Flashcard flashcard);

        // Completed
        void Update(Flashcard flashcard);
        void Delete(Flashcard flashcard);

        // SaveChangesAsync();
        Task<bool> SaveChangesAsync();
    }
}
