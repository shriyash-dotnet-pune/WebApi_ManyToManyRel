namespace WebApi_ManyToManyRel.Models
{
    public class BookOrder
    {
        public int BookId { get; set; }
        public virtual Book? Book { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public int Quantity { get; set; }
    }
}
