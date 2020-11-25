﻿// <auto-generated />
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Desafio.Umbler.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20201124222217_second")]
    partial class second
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("Desafio.Umbler.Models.Domain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HostedAt");

                    b.Property<string>("Ip");

                    b.Property<string>("Name");

                    b.Property<string>("NsList");

                    b.Property<int>("Ttl");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<string>("WhoIs");

                    b.HasKey("Id");

                    b.ToTable("Domains");
                });
#pragma warning restore 612, 618
        }
    }
}
