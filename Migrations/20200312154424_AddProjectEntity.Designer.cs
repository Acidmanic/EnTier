﻿// <auto-generated />
using System;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace mock_server.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    [Migration("20200312154424_AddProjectEntity")]
    partial class AddProjectEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StorageModels.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<long?>("CreatorId");

                    b.Property<DateTime>("PostDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("StorageModels.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("Surname");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StorageModels.Post", b =>
                {
                    b.HasOne("StorageModels.User", "Creator")
                        .WithMany("Posts")
                        .HasForeignKey("CreatorId");
                });
#pragma warning restore 612, 618
        }
    }
}
