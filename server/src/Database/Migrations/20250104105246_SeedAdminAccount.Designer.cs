﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250104105246_SeedAdminAccount")]
    partial class SeedAdminAccount
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArticleCollection", b =>
                {
                    b.Property<string>("ArticlesArticleId")
                        .HasColumnType("text");

                    b.Property<string>("CollectionId")
                        .HasColumnType("text");

                    b.HasKey("ArticlesArticleId", "CollectionId");

                    b.HasIndex("CollectionId");

                    b.ToTable("ArticleCollection");
                });

            modelBuilder.Entity("Domain.Articles.Article", b =>
                {
                    b.Property<string>("ArticleId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPinned")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("ArticleId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Domain.Authorization.UserSession", b =>
                {
                    b.Property<long>("UserSessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserSessionId"));

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.ComplexProperty<Dictionary<string, object>>("RefreshToken", "Domain.Authorization.UserSession.RefreshToken#RefreshToken", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("text");
                        });

                    b.HasKey("UserSessionId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("Domain.Collections.Collection", b =>
                {
                    b.Property<string>("CollectionId")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CollectionId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("CanManageArticles")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("be5d8b69-2c71-4532-b304-5597e1c519ee"),
                            CanManageArticles = true,
                            Email = "admin@example.ru",
                            PasswordHash = new byte[] { 8, 14, 179, 106, 14, 232, 75, 244, 70, 75, 183, 34, 225, 172, 160, 91, 14, 196, 238, 125, 207, 140, 175, 58, 73, 24, 12, 136, 201, 252, 155, 98, 174, 142, 90, 113, 209, 44, 15, 204, 165, 153, 65, 215, 236, 23, 177, 182, 89, 51, 113, 97, 20, 124, 212, 166, 252, 62, 247, 255, 1, 107, 138, 80 },
                            PasswordSalt = new byte[] { 147, 104, 176, 193, 252, 142, 184, 175, 131, 159, 222, 22, 145, 71, 130, 41, 95, 238, 250, 148, 91, 24, 150, 233, 147, 177, 133, 228, 22, 88, 206, 251, 128, 203, 58, 123, 193, 77, 149, 252, 64, 206, 233, 136, 120, 180, 102, 5, 21, 33, 30, 83, 195, 47, 30, 243, 219, 184, 106, 224, 169, 72, 197, 110, 15, 47, 113, 137, 221, 163, 1, 14, 56, 7, 72, 207, 41, 109, 52, 135, 120, 237, 240, 25, 242, 29, 141, 54, 123, 228, 46, 232, 184, 49, 96, 110, 165, 29, 236, 48, 44, 22, 146, 123, 251, 222, 130, 126, 135, 168, 180, 9, 211, 234, 21, 5, 252, 148, 226, 144, 142, 109, 96, 237, 78, 154, 142, 31 }
                        });
                });

            modelBuilder.Entity("ArticleCollection", b =>
                {
                    b.HasOne("Domain.Articles.Article", null)
                        .WithMany()
                        .HasForeignKey("ArticlesArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Collections.Collection", null)
                        .WithMany()
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
