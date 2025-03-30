using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpToolkit.Models
{
    public class ModelParam
    {
        [Key]
        [Required(ErrorMessage = "Required Id")]
        public object Id { get; set; }
    }
}
