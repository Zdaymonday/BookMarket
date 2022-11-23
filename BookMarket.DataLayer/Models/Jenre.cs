using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookMarket.DataLayer.Models
{
    [Index(nameof(JenreName), IsUnique = true)]
    public class Jenre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string JenreName { get; set; } = null!;
    }
}
