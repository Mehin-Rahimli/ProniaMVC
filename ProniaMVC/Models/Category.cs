using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Models
{
    public class Category:BaseEntity
    {
        [MaxLength(30,ErrorMessage ="Maximum length is 30 idiot")]
        public string Name { get; set; }


        //relational
        public List<Product>? Products { get; set; }
    }
}
