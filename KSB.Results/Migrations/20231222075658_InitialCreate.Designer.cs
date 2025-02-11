﻿// <auto-generated />
using System;
using KSB.Results.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KSB.Results.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231222075658_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KSB.Results.Db.Club", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("KSB.Results.Db.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("KSB.Results.Db.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClubId")
                        .HasColumnType("int");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LicenseNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpreadSheetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surnamename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("KSB.Results.Db.StartResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<double?>("Factor")
                        .HasColumnType("float");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int?>("Points")
                        .HasColumnType("int");

                    b.Property<int?>("TensCount")
                        .HasColumnType("int");

                    b.Property<double?>("Time")
                        .HasColumnType("float");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("PlayerId");

                    b.ToTable("StartResults");
                });

            modelBuilder.Entity("KSB.Results.Db.Player", b =>
                {
                    b.HasOne("KSB.Results.Db.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("KSB.Results.Db.StartResult", b =>
                {
                    b.HasOne("KSB.Results.Db.Course", "Course")
                        .WithMany("StartResults")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSB.Results.Db.Player", "Player")
                        .WithMany("StartResults")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("KSB.Results.Db.Course", b =>
                {
                    b.Navigation("StartResults");
                });

            modelBuilder.Entity("KSB.Results.Db.Player", b =>
                {
                    b.Navigation("StartResults");
                });
#pragma warning restore 612, 618
        }
    }
}
