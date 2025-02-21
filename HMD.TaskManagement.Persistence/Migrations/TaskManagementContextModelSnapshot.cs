﻿// <auto-generated />
using HMD.TaskManagement.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HMD.TaskManagement.Persistence.Migrations
{
    [DbContext(typeof(TaskManagementContext))]
    partial class TaskManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Defination")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("AppRoles");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppTasks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PriorityId")
                        .HasColumnType("int");

                    b.Property<bool>("State")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("PriorityId");

                    b.ToTable("AppTasks");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AppRoleId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("AppRoleId");

                    b.HasIndex("Surname")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AppUserID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("State")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AppUserID");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.Priority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Defination")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Priorities");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.TaskReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AppTaskId")
                        .HasColumnType("int");

                    b.Property<string>("Defination")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppTaskId");

                    b.ToTable("TaskReports");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppTasks", b =>
                {
                    b.HasOne("HMD.TaskManagement.Domain.Entities.AppUser", "AppUser")
                        .WithMany("Tasks")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HMD.TaskManagement.Domain.Entities.Priority", "Priority")
                        .WithMany("Tasks")
                        .HasForeignKey("PriorityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("Priority");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppUser", b =>
                {
                    b.HasOne("HMD.TaskManagement.Domain.Entities.AppRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("AppRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.Notification", b =>
                {
                    b.HasOne("HMD.TaskManagement.Domain.Entities.AppUser", "AppUser")
                        .WithMany("Notifications")
                        .HasForeignKey("AppUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.TaskReport", b =>
                {
                    b.HasOne("HMD.TaskManagement.Domain.Entities.AppTasks", "AppTask")
                        .WithMany("TaskReports")
                        .HasForeignKey("AppTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppTask");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppRole", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppTasks", b =>
                {
                    b.Navigation("TaskReports");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.AppUser", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("HMD.TaskManagement.Domain.Entities.Priority", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
