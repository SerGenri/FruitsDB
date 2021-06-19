namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Stock")]
    public partial class Stock
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock()
        {
            StockFruits = new HashSet<StockFruits>();
        }

        [Key]
        public int IdStock { get; set; }

        public int IdProviderCatalog { get; set; }

        public DateTime DeliveryDate { get; set; }

        public virtual ProvidersCatalog ProvidersCatalog { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockFruits> StockFruits { get; set; }
    }
}
