﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Mailo.Data.Enums;
using Mailo.Models;

namespace Mailo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderProduct>().ToTable("OrderProduct");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Payment>().ToTable("Payment");
            modelBuilder.Entity<Wishlist>().ToTable("Wishlist");
            modelBuilder.Entity<Contact>().ToTable("Contact");

            modelBuilder.Entity<Product>().HasKey(p => new { p.ID });
            #region M:M Tables

            modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderID, op.ProductID });
            modelBuilder.Entity<OrderProduct>()
           .HasOne(op => op.order)
           .WithMany(o => o.OrderProducts)
           .HasForeignKey(op => op.OrderID);

          

            modelBuilder.Entity<Wishlist>().HasKey(op => new { op.UserID, op.ProductID });
            modelBuilder.Entity<Wishlist>()
           .HasOne(w => w.user)
           .WithMany(u => u.wishlist)
           .HasForeignKey(w => w.UserID);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.product)
                .WithMany(p => p.wishlists)
                .HasForeignKey(w => w.ProductID);

            modelBuilder.Entity<Review>().HasKey(op => new { op.UserID, op.ProductID });
            modelBuilder.Entity<Review>()
           .HasOne(r => r.user)
           .WithMany(u => u.reviews)
           .HasForeignKey(r => r.UserID);

            //modelBuilder.Entity<Review>()
            //    .HasOne(r => r)
            //    .WithMany(p => p)
              //  .HasForeignKey(r => r.ProductID);
            modelBuilder.Entity<EmployeeContact>().HasKey(ec => new { ec.ContactID, ec.EmpID });
            modelBuilder.Entity<EmployeeContact>()
           .HasOne(ec => ec.contact)
           .WithMany(c => c.employeeContacts)
           .HasForeignKey(ec => ec.ContactID);


            #endregion

            #region Unique Attributes
            modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            modelBuilder.Entity<Product>()
  .HasIndex(u => u.Name)
  .IsUnique();
            //modelBuilder.Entity<Productss>()
            //.HasIndex(op => new { op.ID, op.Colors, op.Size })
            //.IsUnique();
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            #endregion

            #region Computed Attributes
            modelBuilder.Entity<User>().Property(u => u.ID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(x => x.TotalPrice).HasComputedColumnSql("[Price]-([Discount]/100)*[Price]");
            modelBuilder.Entity<User>()
            .Property(u => u.FullName)
            .HasComputedColumnSql("[FName] + ' ' + [LName]");
            //modelBuilder.Entity<Employee>()
            //.Property(e => e.FullName)
            //.HasComputedColumnSql("[FName] + ' ' + [LName]");
            modelBuilder.Entity<Order>()
            .Property(e => e.TotalPrice)
            .HasComputedColumnSql("[OrderPrice] + [DeliveryFee]");
            #endregion

            #region Products Data
            modelBuilder.Entity<Product>()
            .HasData(
            new Product { ID=1 , Name = "Mailo basha pants", Description = "Designed for comfort and style, the Mailo Pants offer a relaxed fit with soft, breathable fabric—your go-to for any occasion.", Price = 750, ImageUrl = "~/assets/blackpants1.jpeg",Quantity=3 }
            );
            #endregion

            foreach (var rel in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                rel.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Employee> Employees { get; set; }

        #endregion
    }
}