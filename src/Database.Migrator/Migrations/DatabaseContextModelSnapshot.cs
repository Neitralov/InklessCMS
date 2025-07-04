﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrator.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArticleCollection", b =>
                {
                    b.Property<string>("ArticlesArticleId")
                        .HasColumnType("character varying(64)");

                    b.Property<string>("CollectionId")
                        .HasColumnType("character varying(32)");

                    b.HasKey("ArticlesArticleId", "CollectionId");

                    b.HasIndex("CollectionId");

                    b.ToTable("ArticleCollection");
                });

            modelBuilder.Entity("Domain.Articles.Article", b =>
                {
                    b.Property<string>("ArticleId")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

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
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("ArticleId");

                    b.ToTable("Articles", (string)null);
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

                    b.ToTable("UserSessions", (string)null);
                });

            modelBuilder.Entity("Domain.Collections.Collection", b =>
                {
                    b.Property<string>("CollectionId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("CollectionId");

                    b.ToTable("Collections", (string)null);
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

                    b.ToTable("Users", (string)null);
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
