using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WL.Host.Entities;

public class Wish
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public int WishCategoryId { get; set; }
    
    [Required]
    [MaxLength(30, ErrorMessage = "Max name length is 30 symbols")]
    public string Name { get; set; }

    [MaxLength(100, ErrorMessage = "Max name length is 100 symbols")]
    public string? Description { get; set; }
}