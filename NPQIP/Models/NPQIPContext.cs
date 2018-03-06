using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class NPQIPContext : DbContext
    {
        public NPQIPContext()
            : base("name=DefaultConnection")
        { 
        }

        public virtual DbSet<Checklist> Checklists { get; set; }
        public virtual DbSet<Publication> Publications { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<TrainingScore> TrainingScores { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<ReviewCompletion> ReviewCompletions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Checklist>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.Checklist)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Publication>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.Publication)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.UserProfile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Option>()
               .HasMany(e => e.Reviews);
               //.WithRequired(e => e.Optoin)
            //.WillCascadeOnDelete(false);

            modelBuilder.Entity<Checklist>()
               .HasMany(e => e.FrontPageReviews)
               .WithRequired(e => e.Checklist)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Publication>()
                .HasMany(e => e.FrontPageReviews)
                .WithRequired(e => e.Publication)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
                .HasMany(e => e.FrontPageReviews)
                .WithRequired(e => e.UserProfile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Option>()
               .HasMany(e => e.FrontPageReviews)
               .WithOptional(e => e.Optoin)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Publication>()
             .HasMany(e => e.Files)
             .WithRequired(e => e.Publications)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Publication>()
             .HasMany(e => e.TrainingScores)
             .WithRequired(e => e.Publication)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
             .HasMany(e => e.TrainingScores)
             .WithRequired(e => e.UserProfile)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingScore>()
             .HasMany(e => e.Reviews)
             .WithOptional(e => e.TrainingScore)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReviewCompletion>()
             .HasMany(e => e.Reviews)
             .WithOptional(e => e.ReviewCompletion)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
            .HasMany(e => e.Promotions)
            .WithRequired(e => e.user)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserProfile>()
            .HasMany(e => e.ReviewCompletions)
            .WithRequired(e => e.user)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<Publication>()
            .HasMany(e => e.ReviewCompletions)
            .WithRequired(e => e.publication)
            .WillCascadeOnDelete(false);
        }

    }
}