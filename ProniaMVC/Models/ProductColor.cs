namespace ProniaMVC.Models
{
    public class ProductColor:BaseEntity
    {

        public int Id   { get; set; }
        public Product Product { get; set; }
        public Color Color { get; set; }
        public int ColorId { get; set; }
        public int ProductId { get; set; }
    }
}
