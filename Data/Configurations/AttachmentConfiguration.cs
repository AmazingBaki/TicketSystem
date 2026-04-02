using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSupportSystem.Data.Entities;

namespace TicketSupportSystem.Data.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.Property(attach => attach.StoredFileName).IsRequired();
            builder.Property(attach => attach.OriginalFileName).IsRequired();
            builder.Property(attach => attach.ContentType).IsRequired();
            builder.Property(attach => attach.Size).IsRequired();
            builder.Property(attach => attach.Path)
                    .IsRequired();

            builder.HasOne(a => a.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
