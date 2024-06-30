using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorWeb.Model
{
    public class Article
    {
        [Key]
        public int Artical_ID { get; set; }

        [DisplayName("Tiêu Đề")]
        [Column(TypeName = "Nvarchar")]
        [Required(ErrorMessage = "{0} Phải Nhập")]
        [StringLength(maximumLength: 255, MinimumLength = 5, ErrorMessage = "{2} <= {0} <= {1}")]
        public string Title { get; set; }
        [Required(ErrorMessage = "{0} Phải Nhập")]
        [DisplayName("Ngày Tạo")]
        public DateTime Created { get; set; }

        [Column(TypeName = "NText")]
        [DisplayName("Nội Dung")]
        public string? Content { get; set; }


    }
}
