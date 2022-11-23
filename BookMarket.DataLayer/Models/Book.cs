using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Models
{
    [Index(nameof(Book.ISBN), IsUnique = true)]
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ISBN { get; set; } = null!;

        [Required]
        public string BookName { get; set; } = null!;
        public string? Year { get; set; }
        
        public int PageCount { get; set; }
        
        public string BookImageGuid { get; set; } = Guid.NewGuid().ToString();

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public List<LiteraryWork> LiteraryWorks { get; set; } = new();
        public List<PageInStore> PagesInStores { get; set; } = new();
    }
}
