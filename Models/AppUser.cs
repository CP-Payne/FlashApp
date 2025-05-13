using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FlashApp.Models
{
    public class AppUser : IdentityUser
    {
        public List<Deck> Decks { get; set; } = new List<Deck>();
        public List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}
