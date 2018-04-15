namespace Lands.Domain.Soccer.RelationsMap
{
    using System.Data.Entity.ModelConfiguration;

    public class MatchesMap : EntityTypeConfiguration<Match>
    {
        public MatchesMap()
        {
            //  CHEJ - Relacion de la tabla Match, la propiedad virtual Local le extrae la propiedad virtual 
            //  De la tabla Team Locals (ICollection) y luego establece la relacion con la propiedad de la tabla
            //  Match en el campo LocalId
            HasRequired(o => o.Local)
                .WithMany(m => m.Locals)
                .HasForeignKey(m => m.LocalId);

            HasRequired(o => o.Visitor)
                .WithMany(m => m.Visitors)
                .HasForeignKey(m => m.VisitorId);
        }
    }
}