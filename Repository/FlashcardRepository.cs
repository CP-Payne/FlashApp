using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Data;
using FlashApp.Interfaces.Repository;
using FlashApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashApp.Repository
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly ApplicationDBContext _context;

        public FlashcardRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Flashcard flashcard)
        {
            await _context.Flashcards.AddAsync(flashcard);
        }

        public async Task<List<Flashcard>> GetAllByUserIdAsync(string userId)
        {
            return await _context
                .Flashcards.Where(f => f.AppUserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Flashcard?> GetByIdAndUserIdAsync(Guid flashcardId, string userId)
        {
            return await _context.Flashcards.FirstOrDefaultAsync(f =>
                f.AppUserId == userId && f.Id == flashcardId
            );
        }

        public async Task<Flashcard?> GetByQuestionAndUserIdAsync(string question, string userId)
        {
            return await _context.Flashcards.FirstOrDefaultAsync(f =>
                f.Question == question && f.AppUserId == userId
            );
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Return true if any entities were saved
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Flashcard flashcard)
        {
            _context.Flashcards.Update(flashcard);
        }

        public void Delete(Flashcard flashcard)
        {
            _context.Flashcards.Remove(flashcard);
        }
    }
}
