namespace ProniaMVC.Models
{
    public class ProductSize:BaseEntity
    {

        public int Id { get; set; }
        public Product Product { get; set; }
        public Size Size { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; } 

    }
}
