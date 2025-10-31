using System.ComponentModel.DataAnnotations;

namespace WebApi_ManyToManyRel.Models
{
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public decimal Price { get; set; }

        // Navigation to join-entities (BookOrder)
        public virtual ICollection<BookOrder> BookOrders { get; set; } = new List<BookOrder>();
    }
}
