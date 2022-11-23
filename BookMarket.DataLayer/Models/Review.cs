using System.ComponentModel.DataAnnotations;

namespace BookMarket.DataLayer.Models
{
    public class Review
    {
        [Key]
        public int ID { get; set; }
        public int WorkID { get; set; }
        public LiteraryWork Work { get; set; }
        public string ReviewerName { get; set; }
        public string Text { get; set; }
        [Range(1, 10)]
        public int Stars { get; set; } 
    }
}
