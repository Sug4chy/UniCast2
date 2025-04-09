﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UniCast.Infrastructure.Persistence.Context;

#nullable disable

namespace UniCast.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(PostgresqlDataContext))]
    partial class PostgresqlDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UniCast.Domain.Messages.Entities.MessageFromMethodist", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<int>("SenderExtId")
                        .HasColumnType("integer")
                        .HasColumnName("sender_ext_id");

                    b.Property<string>("SenderUsername")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("sender_username");

                    b.HasKey("Id");

                    b.ToTable("message_from_methodist", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Messages.Entities.StudentsReply", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.Property<string>("ReplyText")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reply_text");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid")
                        .HasColumnName("student_id");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("StudentId");

                    b.ToTable("students_reply", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Moodle.MoodleAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CurrentToken")
                        .HasColumnType("text")
                        .HasColumnName("current_token");

                    b.Property<long>("ExtId")
                        .HasColumnType("bigint")
                        .HasColumnName("ext_id");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid")
                        .HasColumnName("student_id");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.HasIndex("ExtId")
                        .IsUnique();

                    b.HasIndex("StudentId")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("moodle_account", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Students.Entities.AcademicGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("academic_group", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Students.Entities.Student", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("student", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramChat", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("ExtId")
                        .HasColumnType("bigint")
                        .HasColumnName("ext_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("ExtId")
                        .IsUnique();

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("telegram_chat", (string)null);

                    b.HasDiscriminator<byte>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ChatId")
                        .HasColumnType("uuid")
                        .HasColumnName("chat_id");

                    b.Property<int>("ExtId")
                        .HasColumnType("integer")
                        .HasColumnName("ext_id");

                    b.Property<Guid>("SrcMessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("src_message_id");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("SrcMessageId");

                    b.HasIndex("ExtId", "ChatId")
                        .IsUnique();

                    b.ToTable("telegram_message", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramMessageReaction", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.Property<string>("Reaction")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reaction");

                    b.Property<string>("ReactorUsername")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reactor_username");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.ToTable("telegram_message_reaction", (string)null);
                });

            modelBuilder.Entity("UniCast.Infrastructure.Persistence.Entities.MessageFromMethodistStudent", b =>
                {
                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid")
                        .HasColumnName("student_id");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.HasKey("StudentId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("message_from_methodist_student", (string)null);
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.PrivateTelegramChat", b =>
                {
                    b.HasBaseType("UniCast.Domain.Telegram.Entities.TelegramChat");

                    b.Property<int?>("CurrentScenario")
                        .HasColumnType("integer")
                        .HasColumnName("current_scenario");

                    b.Property<string>("CurrentScenarioArgs")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("current_scenario_args");

                    b.Property<int?>("CurrentState")
                        .HasColumnType("integer")
                        .HasColumnName("current_state");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid")
                        .HasColumnName("student_id");

                    b.HasIndex("StudentId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue((byte)0);
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramChannel", b =>
                {
                    b.HasBaseType("UniCast.Domain.Telegram.Entities.TelegramChat");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.HasIndex("GroupId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("UniCast.Domain.Messages.Entities.StudentsReply", b =>
                {
                    b.HasOne("UniCast.Domain.Messages.Entities.MessageFromMethodist", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UniCast.Domain.Students.Entities.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("UniCast.Domain.Moodle.MoodleAccount", b =>
                {
                    b.HasOne("UniCast.Domain.Students.Entities.Student", "Student")
                        .WithOne("MoodleAccount")
                        .HasForeignKey("UniCast.Domain.Moodle.MoodleAccount", "StudentId");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("UniCast.Domain.Students.Entities.Student", b =>
                {
                    b.HasOne("UniCast.Domain.Students.Entities.AcademicGroup", "Group")
                        .WithMany("Students")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramMessage", b =>
                {
                    b.HasOne("UniCast.Domain.Telegram.Entities.TelegramChat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UniCast.Domain.Messages.Entities.MessageFromMethodist", "SrcMessage")
                        .WithMany("TelegramMessages")
                        .HasForeignKey("SrcMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("SrcMessage");
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramMessageReaction", b =>
                {
                    b.HasOne("UniCast.Domain.Telegram.Entities.TelegramMessage", "Message")
                        .WithMany("Reactions")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("UniCast.Infrastructure.Persistence.Entities.MessageFromMethodistStudent", b =>
                {
                    b.HasOne("UniCast.Domain.Messages.Entities.MessageFromMethodist", null)
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UniCast.Domain.Students.Entities.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.PrivateTelegramChat", b =>
                {
                    b.HasOne("UniCast.Domain.Students.Entities.Student", "Student")
                        .WithOne("TelegramChat")
                        .HasForeignKey("UniCast.Domain.Telegram.Entities.PrivateTelegramChat", "StudentId");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramChannel", b =>
                {
                    b.HasOne("UniCast.Domain.Students.Entities.AcademicGroup", "AcademicGroup")
                        .WithOne("TelegramChannel")
                        .HasForeignKey("UniCast.Domain.Telegram.Entities.TelegramChannel", "GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AcademicGroup");
                });

            modelBuilder.Entity("UniCast.Domain.Messages.Entities.MessageFromMethodist", b =>
                {
                    b.Navigation("TelegramMessages");
                });

            modelBuilder.Entity("UniCast.Domain.Students.Entities.AcademicGroup", b =>
                {
                    b.Navigation("Students");

                    b.Navigation("TelegramChannel");
                });

            modelBuilder.Entity("UniCast.Domain.Students.Entities.Student", b =>
                {
                    b.Navigation("MoodleAccount");

                    b.Navigation("TelegramChat");
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramChat", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("UniCast.Domain.Telegram.Entities.TelegramMessage", b =>
                {
                    b.Navigation("Reactions");
                });
#pragma warning restore 612, 618
        }
    }
}
