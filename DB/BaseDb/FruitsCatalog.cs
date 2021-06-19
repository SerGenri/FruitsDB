namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FruitsCatalog : Base.Base
    {
        public string FullName => $"{Class} - {Sort}".Replace("--- Выбери - ---", "--- Выбери ---");


    }
}
