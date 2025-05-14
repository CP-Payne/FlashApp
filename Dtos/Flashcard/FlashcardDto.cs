using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlashApp.Dtos.Flashcard
{
    public class FlashcardDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
