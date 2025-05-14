using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Repository
{
    public interface IFlashcardRepository
    {
        Task<List<Flashcard>> GetAllByUserIdAsync(Guid userId);
        Task<Flashcard?> GetByIdAsync(Guid userId, Guid flashcardId);
        Task<Flashcard> AddAsync(Flashcard flashcard);
        Task Update(Flashcard flashcard);
        Task<bool> SaveChangesAsync();
    }
}
