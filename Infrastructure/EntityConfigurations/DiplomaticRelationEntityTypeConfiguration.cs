using DiscordBot.Domain.Entities.Alliances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class DiplomaticRelationEntityTypeConfiguration : IEntityTypeConfiguration<DiplomaticRelation>
    {
        public void Configure(EntityTypeBuilder<DiplomaticRelation> diplomaticRelationConfiguration)
        {
            diplomaticRelationConfiguration.ToTable("ct_diplomatic_relation");

            diplomaticRelationConfiguration.HasKey(dr => dr.Id);

            diplomaticRelationConfiguration.Property(dr => dr.Id)
                .HasDefaultValue(0)
                .ValueGeneratedNever()
                .IsRequired();

            diplomaticRelationConfiguration.Property(dr => dr.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            diplomaticRelationConfiguration.HasData(
                data: DiplomaticRelation.List()
                );
        }
    }
}
