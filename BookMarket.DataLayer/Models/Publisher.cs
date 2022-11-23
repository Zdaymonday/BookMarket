using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Models
{
    [Index(nameof(Publisher.Name), IsUnique = true)]
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } 
        public string? FullName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Year { get; set; }
    }
}
