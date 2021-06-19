using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace FruitsDB.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StockFruits:Base.Base
    {
        private FruitsCatalog _fruitsCatalog;

        public string FullName => FruitsCatalog?.FullName;


        // ===========================  Price

        public virtual FruitsCatalog FruitsCatalog
        {
            get => _fruitsCatalog;
            set
            {
                _fruitsCatalog = value;

                GetPriceCatDb();

                Raise();
            }
        }


        [NotMapped]
        public Visibility PriceCatalogLbl { get; set; }
        [NotMapped]
        public Visibility PriceLbl { get; set; }


        private int GetPriceCatDb()
        {
            ModelDb objModelDb = new ModelDb();

            PriceCatalog objCatalogj = null;

            if (Stock != null)
            {
                objCatalogj = objModelDb.PriceCatalog.FirstOrDefault(x => x.StartDate <= Stock.DeliveryDate 
                                                   && x.EndDate >= Stock.DeliveryDate
                                                   && x.FruitsCatalog .IdFruitsCatalog == FruitsCatalog.IdFruitsCatalog
                                                   && x.ProvidersCatalog.IdProviderCatalog == Stock.ProvidersCatalog.IdProviderCatalog);
            }

            PriceLbl = objCatalogj == null ? Visibility.Visible : Visibility.Collapsed;
            PriceCatalogLbl = objCatalogj != null ? Visibility.Visible : Visibility.Collapsed;

            return objCatalogj?.Price ?? Price;
        }

        [NotMapped]
        public int PriceCatDb
        {
            get => GetPriceCatDb();
            set
            {
                Price = value;

                Raise();
            }
        }



    }
}
