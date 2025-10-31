using System.ComponentModel.DataAnnotations;

namespace WebApi_ManyToManyRel.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        // Navigation to join-entities (BookOrder)
        public virtual ICollection<BookOrder> BookOrders { get; set; } = new List<BookOrder>();
    }
}
