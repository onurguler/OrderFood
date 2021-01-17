using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrderFood.Domain.Dto;
using OrderFood.Domain.Models;
using OrderFood.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFood.Application.Managers
{
    public class ProductManager : BaseManager
    {
        private readonly IBaseRepository<Product, long> _productRepository;

        public ProductManager(IServiceProvider provider, IHostingEnvironment hostingEnvironment) : base(provider, hostingEnvironment)
        {
            _productRepository = UnitOfWork.GetRepository<Product, long>();
        }

        public async Task<List<ProductDto>> GetProductList()
        {
            var products = await _productRepository.GetAll().Select(product => new ProductDto()
            {
                Id = product.Id,
                Title = product.Title,
                Subtitle = product.Subtitle,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price
            }).OrderBy(product => product.Title).ToListAsync();

            return products;
        }

        public async Task<ProductDto> GetProduct(long id)
        {
            var model = await _productRepository.Find(product => product.Id == id).Include(product => product.ProductCategories).ThenInclude(productCategory => productCategory.Category).Select(product => new ProductDto()
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
            }).FirstOrDefaultAsync();

            return model;
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

        public async Task<ProductDto> CreateProduct(ProductDto model, IFormFile image)
        {
            var entity = new Product()
            {
                Title = model.Title,
                Subtitle = model.Subtitle,
                Description = model.Description,
                Price = model.Price ?? 0
            };
           
            if (image != null)
            {
                var extension = Path.GetExtension(image.FileName);
                var fileName = $"{Guid.NewGuid()}.{extension.Replace(".", "")}";
                var path = Path.Combine(HostingEnvironment.WebRootPath, "uploads", "product_images", fileName);
                using var fileStream = File.Create(path);
                await image.CopyToAsync(fileStream);
                entity.ImageUrl = "/uploads/product_images/" + fileName;
            }

            _productRepository.Add(entity);

            await UnitOfWork.SaveAsync();

            model.Id = entity.Id;
            
            return model;
        }

        public async Task<ProductDto> EditProduct(ProductDto model, IFormFile image, long[] categoryIds = null)
        {
            var entity = await _productRepository.Find(product => product.Id == model.Id).Include(product => product.ProductCategories).FirstOrDefaultAsync();

            if (entity == null)
            {
                return null;
            }

            entity.Title = model.Title;
            entity.Subtitle = model.Subtitle;
            entity.Description = model.Description;
            entity.Price = model.Price ?? 0;

            if (image != null)
            {
                if (!string.IsNullOrEmpty(entity.ImageUrl) && entity.ImageUrl.StartsWith("/"))
                {
                    var fileNameToDelete = entity.ImageUrl.Substring(1);
                    var deletePath = Path.Combine(HostingEnvironment.WebRootPath, fileNameToDelete);

                    if (File.Exists(deletePath))
                    {
                        File.Delete(deletePath);
                    }
                }

                var extension = Path.GetExtension(image.FileName);
                var fileName = $"{Guid.NewGuid()}.{extension.Replace(".", "")}";
                var path = Path.Combine(HostingEnvironment.WebRootPath, "uploads", "product_images", fileName);
                using var fileStream = File.Create(path);
                await image.CopyToAsync(fileStream);
                entity.ImageUrl = "/uploads/product_images/" + fileName;

                model.ImageUrl = entity.ImageUrl;
            }

            _productRepository.Update(entity);

            await UnitOfWork.SaveAsync();

            return model;
        }

        public async Task<bool> RemoveImage(long id)
        {
            var entity = await _productRepository.GetAsync(id);

            if (entity == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(entity.ImageUrl) && entity.ImageUrl.StartsWith("/"))
            {
                var fileNameToDelete = entity.ImageUrl.Substring(1);
                var deletePath = Path.Combine(HostingEnvironment.WebRootPath, fileNameToDelete);

                if (File.Exists(deletePath))
                {
                    File.Delete(deletePath);
                }
            }

            entity.ImageUrl = null;

            _productRepository.Update(entity);

            await UnitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteProduct(long id)
        {
            var entity = await _productRepository.GetAsync(id);

            if (entity == null)
            {
                return false;
            }

            _productRepository.Remove(entity);

            await UnitOfWork.SaveAsync();

            return true;
        }
    }
}
