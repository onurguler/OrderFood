using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Dto;
using OrderFood.Domain.Models;
using OrderFood.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.Application.Managers
{
    public class CategoryManager : BaseManager
    {
        private readonly IBaseRepository<Category, long> _categoryRepository;

        public CategoryManager(IServiceProvider provider, IHostingEnvironment hostingEnvironment) : base(provider, hostingEnvironment)
        {
            _categoryRepository = UnitOfWork.GetRepository<Category, long>();
        }

        public async Task<List<CategoryDto>> GetCategoriesList()
        {
            var categories = await _categoryRepository.GetAll().Select(category => new CategoryDto()
            {
                Id = category.Id,
                Title = category.Title
            }).OrderBy(category => category.Title).ToListAsync();

            return categories;
        }
    }
}
