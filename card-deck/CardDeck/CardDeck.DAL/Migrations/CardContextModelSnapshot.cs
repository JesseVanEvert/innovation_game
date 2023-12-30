﻿// <auto-generated />
using System;
using CardDeck.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CardDeck.DAL.Migrations
{
    [DbContext(typeof(CardContext))]
    partial class CardContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CardDeck.Model.Entity.Card", b =>
                {
                    b.Property<Guid>("CardID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BackSideText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FrontSideText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CardID");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("CardDeck.Model.Entity.CardDeck", b =>
                {
                    b.Property<Guid>("CardID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(1);

                    b.Property<Guid>("DeckID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("CardID", "DeckID");

                    b.HasIndex("DeckID");

                    b.ToTable("CardDecks");
                });

            modelBuilder.Entity("CardDeck.Model.Entity.Deck", b =>
                {
                    b.Property<Guid>("DeckID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DeckID");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("CardDeck.Model.Entity.TeamDeck", b =>
                {
                    b.Property<Guid>("DeckID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(0);

                    b.Property<Guid>("TeamID")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnOrder(1);

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("DeckID", "TeamID");

                    b.ToTable("TeamDecks");
                });

            modelBuilder.Entity("CardDeck.Model.Entity.CardDeck", b =>
                {
                    b.HasOne("CardDeck.Model.Entity.Card", null)
                        .WithMany("CardDecks")
                        .HasForeignKey("CardID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CardDeck.Model.Entity.Deck", null)
                        .WithMany("CardDecks")
                        .HasForeignKey("DeckID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CardDeck.Model.Entity.TeamDeck", b =>
                {
                    b.HasOne("CardDeck.Model.Entity.Deck", null)
                        .WithMany("TeamDecks")
                        .HasForeignKey("DeckID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CardDeck.Model.Entity.Card", b =>
                {
                    b.Navigation("CardDecks");
                });

            modelBuilder.Entity("CardDeck.Model.Entity.Deck", b =>
                {
                    b.Navigation("CardDecks");

                    b.Navigation("TeamDecks");
                });
#pragma warning restore 612, 618
        }
    }
}
