using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(O => O.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());
            builder.Property(O => O.Status)
                .HasConversion(
                    Status => Status.ToString(),
                    Status => (OrderStatus)Enum.Parse(typeof(OrderStatus), Status)
                    );
            builder.Property(O => O.SubTotal)
                .HasColumnType("decimal(18, 2)");
            builder.HasOne(O => O.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
