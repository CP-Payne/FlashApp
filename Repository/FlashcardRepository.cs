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

        public async Task<Flashcard> AddAsync(Flashcard flashcard)
        {
            await _context.Flashcards.AddAsync(flashcard);
            await _context.SaveChangesAsync();
            return flashcard;
        }

        public async Task<List<Flashcard>> GetAllByUserIdAsync(Guid userId)
        {
            string userIdString = userId.ToString();
            return await _context
                .Flashcards.Include(f => f.AppUser)
                .Where(f => f.AppUserId.Equals(userIdString))
                .ToListAsync();
        }

        public Task<Flashcard?> GetByIdAsync(Guid userId, Guid flashcardId)
        {
            return _context.Flashcards.FirstOrDefaultAsync(f =>
                Guid.Parse(f.AppUserId) == userId && f.Id == flashcardId
            );
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task Update(Flashcard flashcard)
        {
            throw new NotImplementedException();
        }
    }
}
