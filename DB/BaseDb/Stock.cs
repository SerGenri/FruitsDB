using System.Linq;

namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Stock : Base.Base
    {
        private int GetPriceSumm()
        {
            int summ = 0;

            foreach (StockFruits item in StockFruits)
            {
                summ += item.Mass * item.PriceCatDb;
            }

            return summ;
        }
        public int PriceSumm => GetPriceSumm();

        public int MassSumm => StockFruits.Sum(x => x.Mass);


    }
}
