using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ShortName { get; set; }
        public string? Country { get; set; } = null!;
        public string? BirthDate { get; set; } = null!;
        public string? DeathDate { get; set; }
        public string? FantlabPage { get; set; } = null!;
        public string Biography { get; set; } = "";
        public string AuthorImageGuid { get; set; } = Guid.NewGuid().ToString();
    }
}
