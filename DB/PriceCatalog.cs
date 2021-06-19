namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PriceCatalog")]
    public partial class PriceCatalog
    {
        [Key]
        public int IdPriceCatalog { get; set; }

        public int IdProviderCatalog { get; set; }

        public int IdFruitsCatalog { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Price { get; set; }

        public virtual FruitsCatalog FruitsCatalog { get; set; }

        public virtual ProvidersCatalog ProvidersCatalog { get; set; }
    }
}
