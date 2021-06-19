using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using FruitsDB.Catalog.FruitsCatalog;
using FruitsDB.Catalog.PriceCatalog;
using FruitsDB.Catalog.ProvidersCatalog;
using FruitsDB.DB;
using FruitsDB.DB.Base;
using FruitsDB.Report;

namespace FruitsDB.Main.ViewModel
{
    public class ViewModelMain : Base
    {
        private Stock _listStockEntry;
        private ProvidersCatalog _comboBoxListProvidersCatalogEntry;
        private FruitsCatalog _comboBoxListFruitsCatalogEntry;
        private StockFruits _listStockFruitsEntry;

        /// <summary>
        /// Ловим все необработанные ошибки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("HResult : " + e.HResult + Environment.NewLine + Environment.NewLine +
                            "InnerException : " + e.InnerException + Environment.NewLine + Environment.NewLine +
                            "Data : " + e.Data + Environment.NewLine + Environment.NewLine +
                            "Source : " + e.Source + Environment.NewLine + Environment.NewLine +
                            "TargetSite : " + e.TargetSite + Environment.NewLine + Environment.NewLine +
                            "Message : " + e.Message);
        }


        public ViewModelMain()
        {
            //Глобальные исключений в приложении
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MainHandler;

            //для установки формата даты и времени по системный параметрам
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));

            Load();
        }

        /// <summary>
        /// Выгрузка информации из базы
        /// </summary>
        private void Load()
        {
            DbMain = new ModelDb();

            DbMain.Stock.Load();
            ListStock = DbMain.Stock.Local;

            //==================== ComboBox
            DbMain.ProvidersCatalog.Load();
            ComboBoxListProvidersCatalog = new ObservableCollection<ProvidersCatalog>(DbMain.ProvidersCatalog.Local);
            ComboBoxListProvidersCatalog = new ObservableCollection<ProvidersCatalog>(ComboBoxListProvidersCatalog.OrderBy(x => x.NameProvider));

            DbMain.FruitsCatalog.Load();
            ComboBoxListFruitsCatalog = new ObservableCollection<FruitsCatalog>(DbMain.FruitsCatalog.Local);
            ComboBoxListFruitsCatalog = new ObservableCollection<FruitsCatalog>(ComboBoxListFruitsCatalog.OrderBy(x => x.Class).ThenBy(x => x.Sort));

            FormStockLock = true;
        }


        /// <summary>
        /// Модель базы данных
        /// </summary>
        public ModelDb DbMain { get; set; }


        #region ======================== Stock ======================== 

        /// <summary>
        /// Блокировка полей для редактирования таблицы Поставщики
        /// </summary>
        public bool FormStockPropLock { get; set; }


        /// <summary>
        /// Таблица Поставок
        /// </summary>
        public ObservableCollection <Stock> ListStock { get; set; }
        /// <summary>
        /// Выбранный пункт из таблицы поставок
        /// </summary>
        public Stock ListStockEntry
        {
            get => _listStockEntry;
            set
            {
                _listStockEntry = value;

                ListStockFruits = value?.StockFruits;
                ListStockFruitsView = (CollectionView)CollectionViewSource.GetDefaultView(ListStockFruits);

                ComboBoxListProvidersCatalogEntry = value?.ProvidersCatalog ?? ComboBoxListProvidersCatalog.First();

                FormStockPropLock = value != null;

                Raise();
            }
        }


        /// <summary>
        /// Список Поставщиков
        /// </summary>
        public ObservableCollection<ProvidersCatalog> ComboBoxListProvidersCatalog { get; set; }
        /// <summary>
        /// Выбранный поставщик в комбобоксе поставщики
        /// </summary>
        public ProvidersCatalog ComboBoxListProvidersCatalogEntry
        {
            get => _comboBoxListProvidersCatalogEntry;
            set
            {
                _comboBoxListProvidersCatalogEntry = value;

                if (ListStockEntry != null)
                {
                    ListStockEntry.ProvidersCatalog = value;
                }

                Raise();
            }
        }


        /// <summary>
        /// Добавить пункт в таблицу поставок
        /// </summary>
        private RelayCommand _addStockCommand;
        public RelayCommand AddStockCommand
        {
            get
            {
                return _addStockCommand ??
                       (_addStockCommand = new RelayCommand(obj =>
                       {
                           Stock objStock = new Stock
                           {
                               DeliveryDate = DateTime.Now.Date,
                               ProvidersCatalog = ComboBoxListProvidersCatalog.First()
                           };

                           ListStock.Insert(0, objStock);

                           ListStockEntry = objStock;
                       }));
            }
        }

        /// <summary>
        /// Удалить выбранный пункт из таблицы поставок
        /// </summary>
        private RelayCommand _removeStockCommand;
        public RelayCommand RemoveStockCommand
        {
            get
            {
                return _removeStockCommand ??
                       (_removeStockCommand = new RelayCommand(obj =>
                       {
                           foreach (StockFruits item in ListStockEntry.StockFruits.Reverse())
                           {
                               DbMain.StockFruits.Remove(item);
                           }

                           ListStock.Remove(ListStockEntry);

                       }, obj => ListStockEntry != null));
            }
        }

        #endregion ======================== Stock ========================

        #region ======================== StockFruits ========================


        /// <summary>
        /// Блокировка полей для редактирования таблицы фрукты
        /// </summary>
        public bool FormStockFruitsPropLock { get; set; }

        /// <summary>
        /// Блокировка толя Сумма
        /// </summary>
        public bool PriceCatalogPropLock { get; set; }


        /// <summary>
        /// Блокировка таблицы поставщики
        /// </summary>
        public bool FormStockLock { get; set; }
        private void GetFormStockLock()
        {
            if (ListStockFruits != null)
            {
                FormStockLock = !ListStockFruits.Any(x => x.FullName.Contains("--- Выбери"));
            }
        }



        private CollectionView ListStockFruitsView { get; set; }
        /// <summary>
        /// Таблица с фруктами, комплект  поставки 
        /// </summary>
        public ICollection<StockFruits> ListStockFruits { get; set; }
        /// <summary>
        /// Выбранный в данный момент пункт таблицы фрукты
        /// </summary>
        public StockFruits ListStockFruitsEntry
        {
            get => _listStockFruitsEntry;
            set
            {
                _listStockFruitsEntry = value;

                ComboBoxListFruitsCatalogEntry = value?.FruitsCatalog ?? ComboBoxListFruitsCatalog.First();

                FormStockFruitsPropLock = value != null;

                Raise();
            }
        }



        /// <summary>
        /// Список фруктов
        /// </summary>
        public ObservableCollection<FruitsCatalog> ComboBoxListFruitsCatalog { get; set; }
        /// <summary>
        /// Выбранный фрукт в комбобоксе фрукты
        /// </summary>
        public FruitsCatalog ComboBoxListFruitsCatalogEntry
        {
            get => _comboBoxListFruitsCatalogEntry;
            set
            {
                _comboBoxListFruitsCatalogEntry = value;

                if (ListStockFruitsEntry != null)
                {
                    ListStockFruitsEntry.FruitsCatalog = value;

                    PriceCatalogPropLock = ListStockFruitsEntry.PriceLbl == Visibility.Visible;
                }

                GetFormStockLock();
                                     
                ListStockFruitsView?.Refresh();

                Raise();
            }
        }




        /// <summary>
        /// Добавить пункт в таблицу фрукты
        /// </summary>
        private RelayCommand _addStockFruitsCommand;
        public RelayCommand AddStockFruitsCommand
        {
            get
            {
                return _addStockFruitsCommand ??
                       (_addStockFruitsCommand = new RelayCommand(obj =>
                       {
                           StockFruits objStockFruits = new StockFruits
                           {
                               FruitsCatalog = ComboBoxListFruitsCatalog.First(),
                               Stock = ListStockEntry
                           };

                           ListStockFruits.Add(objStockFruits);

                           GetFormStockLock();

                           ListStockFruitsEntry = objStockFruits;

                           ListStockFruitsView?.Refresh();


                       }, obj=> ListStockEntry != null));
            }
        }


        /// <summary>
        /// Удалить выбранный пункт из таблицы фрукты 
        /// </summary>
        private RelayCommand _removeStockFruitsCommand;
        public RelayCommand RemoveStockFruitsCommand
        {
            get
            {
                return _removeStockFruitsCommand ??
                       (_removeStockFruitsCommand = new RelayCommand(obj =>
                       {
                           DbMain.StockFruits.Remove(ListStockFruitsEntry);

                           ListStockFruits.Remove(ListStockFruitsEntry);

                           GetFormStockLock();

                           ListStockFruitsView.Refresh();

                       }, obj => ListStockFruitsEntry != null));
            }
        }


        #endregion ======================== StockFruits ========================


        /// <summary>
        /// Выгрузить данные, без сохранения изменений
        /// </summary>
        private RelayCommand _loadStockCommand;
        public RelayCommand LoadStockCommand
        {
            get
            {
                return _loadStockCommand ??
                       (_loadStockCommand = new RelayCommand(obj =>
                       {
                           Load();

                       }));
            }
        }


        /// <summary>
        /// Сохранить сделанные изменения в базу
        /// </summary>
        private RelayCommand _saveStockCommand;
        public RelayCommand SaveStockCommand
        {
            get
            {
                return _saveStockCommand ??
                       (_saveStockCommand = new RelayCommand(obj =>
                       {
                           DbMain.SaveChanges();

                           Load();

                       }));
            }
        }


        //======================== Справочники


        /// <summary>
        /// Открыть справочник Фрукты
        /// </summary>
        private RelayCommand _fruitsCatalogCommand;
        public RelayCommand FruitsCatalogCommand
        {
            get
            {
                return _fruitsCatalogCommand ??
                       (_fruitsCatalogCommand = new RelayCommand(obj =>
                       {
                           WindowFruitsCatalog form = new WindowFruitsCatalog();
                           form.ShowDialog();
                       }));
            }
        }

        /// <summary>
        /// Открыть справочник Поставщики
        /// </summary>
        private RelayCommand _providersCatalogCommand;
        public RelayCommand ProvidersCatalogCommand
        {
            get
            {
                return _providersCatalogCommand ??
                       (_providersCatalogCommand = new RelayCommand(obj =>
                       {
                           WindowProvidersCatalog form = new WindowProvidersCatalog();
                           form.ShowDialog();
                       }));
            }
        }

        /// <summary>
        /// Открыть справочник График поставок
        /// </summary>
        private RelayCommand _priceCatalogCommand;
        public RelayCommand PriceCatalogCommand
        {
            get
            {
                return _priceCatalogCommand ??
                       (_priceCatalogCommand = new RelayCommand(obj =>
                       {
                           WindowPriceCatalog form = new WindowPriceCatalog();
                           form.ShowDialog();
                       }));
            }
        }






        /// <summary>
        /// Открыть отчет
        /// </summary>
        private RelayCommand _reportCommand;
        public RelayCommand ReportCommand
        {
            get
            {
                return _reportCommand ??
                       (_reportCommand = new RelayCommand(obj =>
                       {
                           WindowReport form = new WindowReport();
                           form.ShowDialog();
                       }));
            }
        }
    }
}