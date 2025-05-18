using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Repository
{
    public interface IFlashcardRepository
    {
        Task<List<Flashcard>> GetAllByUserIdAsync(string userId);
        Task<Flashcard?> GetByIdAndUserIdAsync(Guid flashcardId, string userId);
        Task<Flashcard?> GetByQuestionAndUserIdAsync(string question, string userId);
        Task AddAsync(Flashcard flashcard);
        void Update(Flashcard flashcard);
        void Delete(Flashcard flashcard);
        Task<bool> SaveChangesAsync();
    }
}
