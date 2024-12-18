using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendAPI.Entities {
    [Keyless]
    public class CommentReaction {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public required string ReactionType { get; set; }
        public required DateTime TimeStamp { get; set; }
        public Comment? Comment { get; set; } = null!;
        public User? User { get; set; } = null!;
    }

    public class CommentReactionTableConfig : IEntityTypeConfiguration<CommentReaction> {
        public void Configure(EntityTypeBuilder<CommentReaction> builder) {
            builder.ToTable("comment_reaction");
            builder.Property(reaction => reaction.ReactionType).HasColumnType("varchar(32)").IsRequired();
            builder.Property(reaction => reaction.TimeStamp).IsRequired();
        }
    }

    public class CommentReactionDTO {
        public required string ReactionType { get; set; }
        public required DateTime TimeStamp { get; set; }
    }
}
