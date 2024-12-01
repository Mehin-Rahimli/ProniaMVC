namespace ProniaMVC.Models
{
    public class Tag:BaseEntity
    {
        public int Id {  get; set; }
        public string Name {  get; set; }
      

        //relational
        public List<ProductTag> ProductTags { get; set; }
    }
}
