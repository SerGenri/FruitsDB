namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FruitsCatalog")]
    public partial class FruitsCatalog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FruitsCatalog()
        {
            PriceCatalog = new HashSet<PriceCatalog>();
            StockFruits = new HashSet<StockFruits>();
        }

        [Key]
        public int IdFruitsCatalog { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Class { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Sort { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceCatalog> PriceCatalog { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockFruits> StockFruits { get; set; }
    }
}
