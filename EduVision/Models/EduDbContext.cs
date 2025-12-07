using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EduVision.Models;

public partial class EduDbContext : DbContext
{
    public EduDbContext()
    {
    }

    public EduDbContext(DbContextOptions<EduDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DailyStat> DailyStats { get; set; }

    public virtual DbSet<ExamType> ExamTypes { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionBank> QuestionBanks { get; set; }

    public virtual DbSet<QuestionLog> QuestionLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwQuestionDetail> VwQuestionDetails { get; set; }

    public virtual DbSet<Topic> Topics { get; set; } = null!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=VERA\\SQLEXPRESS;Database=EduDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyStat>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("PK__DailySta__3A162D3E6885DA9A");
        });

        modelBuilder.Entity<ExamType>(entity =>
        {
            entity.HasKey(e => e.ExamTypeId).HasName("PK__ExamType__087D50F08CDE1609");

            entity.Property(e => e.ExamName).HasMaxLength(50);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonId).HasName("PK__Lessons__B084ACD078E7CFD5");

            entity.Property(e => e.LessonName).HasMaxLength(50);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FACED23073B");

            entity.ToTable(tb => tb.HasTrigger("trg_AfterDeleteQuestion"));

            entity.HasIndex(e => new { e.ExamTypeId, e.LessonId }, "IX_Questions_Exam_Lesson");

            entity.HasIndex(e => e.IsActive, "IX_Questions_IsActive");

            entity.HasIndex(e => e.QuestionBankId, "IX_Questions_QuestionBank");

            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DifficultyLevel).HasDefaultValue((byte)1);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.OptionA).HasMaxLength(255);
            entity.Property(e => e.OptionB).HasMaxLength(255);
            entity.Property(e => e.OptionC).HasMaxLength(255);
            entity.Property(e => e.OptionD).HasMaxLength(255);
            entity.Property(e => e.OptionE).HasMaxLength(255);

            entity.HasOne(d => d.ExamType).WithMany(p => p.Questions)
                .HasForeignKey(d => d.ExamTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__ExamT__5629CD9C");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Questions)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__Lesso__571DF1D5");

            entity.HasOne(d => d.QuestionBank).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuestionBankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__Quest__5535A963");
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.HasKey(e => e.QuestionBankId).HasName("PK__Question__2A49BCFE202060EE");

            entity.HasIndex(e => new { e.ExamTypeId, e.LessonId }, "UQ_QuestionBanks_Exam_Lesson").IsUnique();

            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(300);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Publisher).HasMaxLength(100);

            entity.HasOne(d => d.ExamType).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.ExamTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionBanks_ExamTypes");

            entity.HasOne(d => d.Lesson).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionBanks_Lessons");
        });

        modelBuilder.Entity<QuestionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Question__5E548648DA8E9DAC");

            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07233AC813");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344735062E").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(128);
        });

        modelBuilder.Entity<VwQuestionDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_QuestionDetails");

            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CorrectAnswer)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CoverImageUrl).HasMaxLength(300);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DifficultyText)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.ExamName).HasMaxLength(50);
            entity.Property(e => e.LessonName).HasMaxLength(50);
            entity.Property(e => e.Publisher).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
