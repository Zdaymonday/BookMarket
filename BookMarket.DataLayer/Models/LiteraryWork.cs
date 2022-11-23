using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookMarket.DataLayer.Models
{
    public class LiteraryWork
    {
        public int Id { get; set; } 
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Raiting { get; set; }
        public string? Year { get; set; }
        public string? Annotation { get; set; }
        public string? WorkType { get; set; }
        public string? FantlabLink { get; set; }
        public int? JenreId { get; set; }
        public bool? isForKids { get; set; } = false;
        public bool isExistToSale { get; set; } = true;
        public Jenre? Jenre { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}
