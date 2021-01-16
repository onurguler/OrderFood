using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Dto;
using OrderFood.Domain.Models;
using OrderFood.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFood.Application.Managers
{
    public class ProductManager : BaseManager
    {
        private readonly IBaseRepository<Product, long> _productRepository;

        public ProductManager(IServiceProvider provider) : base(provider)
        {
            _productRepository = UnitOfWork.GetRepository<Product, long>();
        }

        public async Task<List<ProductDto>> GetProductList()
        {
            var products = await _productRepository.GetAll().Include(product => product.ProductCategories).ThenInclude(productCategory => productCategory.Category).Select(product => new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Categories = product.ProductCategories.Select(productCategory => new CategoryDto()
                {
                    Id = productCategory.CategoryId,
                    Title = productCategory.Category.Title
                }).ToList()
            }).OrderBy(product => product.Title).ToListAsync();

            return products;
        }

        public async Task<List<ProductDto>> SearchProducts(string searchString)
        {
            var query = _productRepository.GetAll().Include(product => product.ProductCategories).ThenInclude(productCategory => productCategory.Category).Select(product => new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Categories = product.ProductCategories.Select(productCategory => new CategoryDto()
                {
                    Id = productCategory.CategoryId,
                    Title = productCategory.Category.Title
                }).ToList()
            }).OrderBy(product => product.Title).Where(product => product.Title.ToLower().Contains(searchString.ToLower()) || product.Subtitle.ToLower().Contains(searchString.ToLower()) || product.Description.ToLower().Contains(searchString.ToLower()));

            return await query.ToListAsync();
        }
    }
}
