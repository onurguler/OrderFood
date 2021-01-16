using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderFood.Domain.Dto
{
    public class CategoryDto
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
