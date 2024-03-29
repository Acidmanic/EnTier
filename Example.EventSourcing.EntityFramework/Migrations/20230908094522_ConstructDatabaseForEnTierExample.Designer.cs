﻿// <auto-generated />
using Example.EventSourcing.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Example.EventSourcing.EntityFramework.Migrations
{
    [DbContext(typeof(ExampleContext))]
    [Migration("20230908094522_ConstructDatabaseForEnTierExample")]
    partial class ConstructDatabaseForEnTierExample
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("EnTier.DataAccess.EntityFramework.EfObjectEntry<long, long>", b =>
                {
                    b.Property<long>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerializedValue")
                        .HasColumnType("TEXT");

                    b.Property<long>("StreamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TypeName")
                        .HasColumnType("TEXT");

                    b.HasKey("EventId");

                    b.ToTable("PostsEvents");
                });

            modelBuilder.Entity("Example.EventSourcing.EntityFramework.StoragesModels.PostStg", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<long>("PostStgId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
