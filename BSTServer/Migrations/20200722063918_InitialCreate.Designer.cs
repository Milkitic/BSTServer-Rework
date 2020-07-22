﻿// <auto-generated />
using System;
using BSTServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BSTServer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200722063918_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("BSTServer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("BstServer.Models.Session", b =>
                {
                    b.Property<Guid>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ConnectTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("DisconnectTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("SteamUserId")
                        .HasColumnType("TEXT");

                    b.HasKey("SessionId");

                    b.HasIndex("SteamUserId");

                    b.ToTable("Session");
                });

            modelBuilder.Entity("BstServer.Models.SessionDamage", b =>
                {
                    b.Property<Guid>("SessionDamageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Damage")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("DamageTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsHurt")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SteamUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Weapon")
                        .HasColumnType("TEXT");

                    b.HasKey("SessionDamageId");

                    b.HasIndex("SessionId");

                    b.HasIndex("SteamUserId");

                    b.ToTable("SessionDamage");
                });

            modelBuilder.Entity("BstServer.Models.SteamUser", b =>
                {
                    b.Property<string>("SteamUserId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nickname")
                        .HasColumnType("TEXT");

                    b.HasKey("SteamUserId");

                    b.ToTable("SteamUsers");
                });

            modelBuilder.Entity("BstServer.Models.Session", b =>
                {
                    b.HasOne("BstServer.Models.SteamUser", "SteamUser")
                        .WithMany("Sessions")
                        .HasForeignKey("SteamUserId");
                });

            modelBuilder.Entity("BstServer.Models.SessionDamage", b =>
                {
                    b.HasOne("BstServer.Models.Session", "Session")
                        .WithMany("UserDamages")
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BstServer.Models.SteamUser", "SteamUser")
                        .WithMany()
                        .HasForeignKey("SteamUserId");
                });
#pragma warning restore 612, 618
        }
    }
}