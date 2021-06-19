namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StockFruits
    {
        [Key]
        public int IdStockFruits { get; set; }

        public int IdStock { get; set; }

        public int IdFruitsCatalog { get; set; }

        public int Price { get; set; }

        public int Mass { get; set; }

        //public virtual FruitsCatalog FruitsCatalog { get; set; }

        public virtual Stock Stock { get; set; }
    }
}
