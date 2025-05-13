using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Repository
{
    public interface IFlashcardRepository
    {
        Task<List<Flashcard>> GetAllAsync();
        Task<Flashcard?> GetByIdAsync(Guid id);
    }
}
