using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
            if (dbContext.ProductBrands.Count() == 0)
            {
                var brands = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                var brandsData = JsonSerializer.Deserialize<List<ProductBrand>>(brands);
                if (brandsData is not null && brandsData.Count() > 0)
                {
                    foreach (var brand in brandsData)
                    {
                        dbContext.Set<ProductBrand>().Add(brand);
                    }
                    await dbContext.SaveChangesAsync();
                } 
            }
            if (dbContext.ProductCategories.Count() == 0)
            {
                var Categories = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
                var categoriessData = JsonSerializer.Deserialize<List<ProductCategory>>(Categories);
                if (categoriessData is not null && categoriessData.Count() > 0)
                {
                    foreach (var category in categoriessData)
                    {
                        dbContext.Set<ProductCategory>().Add(category);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            if (dbContext.Products.Count() == 0)
            {
                var products = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var productsData = JsonSerializer.Deserialize<List<Product>>(products);
                if (productsData is not null && productsData.Count() > 0)
                {
                    foreach (var product in productsData)
                    {
                        dbContext.Set<Product>().Add(product);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            if(dbContext.DeliveryMethods.Count() == 0)
            {
                var methods = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                var methodsData = JsonSerializer.Deserialize<List<DeliveryMethod>>(methods);
                if(methodsData is not null && methodsData.Count() > 0)
                {
                    foreach(var method in methodsData)
                    {
                        dbContext.Set<DeliveryMethod>().Add(method);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
