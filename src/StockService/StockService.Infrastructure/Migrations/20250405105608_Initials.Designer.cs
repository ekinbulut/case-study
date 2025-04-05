﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StockService.Infrastructure.Data;

#nullable disable

namespace StockService.Infrastructure.Migrations
{
    [DbContext(typeof(StockDbContext))]
    [Migration("20250405105608_Initials")]
    partial class Initials
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StockService.Domain.Entities.Stock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Stocks");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"),
                            CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Apple Watch 44mm",
                            ProductId = new Guid("a9a11000-2f6f-4a62-8d5f-d24a9eaadadf"),
                            Quantity = 100,
                            UnitPrice = 150.75m,
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"),
                            CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Nvidia RTX 5080",
                            ProductId = new Guid("b1a12000-3e6f-4b62-9d6f-e25a9ebbbade"),
                            Quantity = 50,
                            UnitPrice = 250.50m,
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"),
                            CreatedAt = new DateTime(2025, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Apple Iphone 14 Pro",
                            ProductId = new Guid("c2a13000-4d6f-4c62-0d7f-f36a9ecccadf"),
                            Quantity = 150,
                            UnitPrice = 85000m,
                            UpdatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
