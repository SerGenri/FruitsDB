using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using FruitsDB.DB;
using FruitsDB.DB.Base;

namespace FruitsDB.Catalog.PriceCatalog.ViewModel
{
    public class ViewModelPriceCatalog : Base
    {
        private DB.ProvidersCatalog _comboBoxListProvidersCatalogEntry;
        private DB.FruitsCatalog _comboBoxListFruitsCatalogEntry;
        private DB.PriceCatalog _listPriceCatalogEntry;

        public ViewModelPriceCatalog()
        {
            Load();
        }


        /// <summary>
        /// Выгрузка данных
        /// </summary>
        private void Load()
        {
            DbPriceCatalog = new ModelDb();


            DbPriceCatalog.PriceCatalog.Load();
            ListPriceCatalog = DbPriceCatalog.PriceCatalog.Local;


            //==================== ComboBox
            DbPriceCatalog.ProvidersCatalog.Load();
            ComboBoxListProvidersCatalog = new ObservableCollection<DB.ProvidersCatalog>(DbPriceCatalog.ProvidersCatalog.Local);
            ComboBoxListProvidersCatalog = new ObservableCollection<DB.ProvidersCatalog>(ComboBoxListProvidersCatalog.OrderBy(x => x.NameProvider));

            DbPriceCatalog.FruitsCatalog.Load();
            ComboBoxListFruitsCatalog = new ObservableCollection<DB.FruitsCatalog>(DbPriceCatalog.FruitsCatalog.Local);
            ComboBoxListFruitsCatalog = new ObservableCollection<DB.FruitsCatalog>(ComboBoxListFruitsCatalog.OrderBy(x => x.Class).ThenBy(x => x.Sort));

        }

        /// <summary>
        /// Модель базы данных
        /// </summary>
        public ModelDb DbPriceCatalog { get; set; }

        /// <summary>
        /// Блокировка болей если не выбран элемент в таблице план поставок
        /// </summary>
        public bool BoolListPriceCatalogEntry { get; set; }


        /// <summary>
        /// Таблица план поставок
        /// </summary>
        public ObservableCollection<DB.PriceCatalog> ListPriceCatalog { get; set; }
        /// <summary>
        /// Выбранная строка в таблице план поставок
        /// </summary>
        public DB.PriceCatalog ListPriceCatalogEntry
        {
            get => _listPriceCatalogEntry;
            set
            {
                _listPriceCatalogEntry = value;

                ComboBoxListFruitsCatalogEntry = value?.FruitsCatalog ?? ComboBoxListFruitsCatalog.First();
                ComboBoxListProvidersCatalogEntry = value?.ProvidersCatalog ?? ComboBoxListProvidersCatalog.First();

                BoolListPriceCatalogEntry = value != null;

                Raise();
            }
        }



        /// <summary>
        /// Список Поставщиков
        /// </summary>
        public ObservableCollection<DB.ProvidersCatalog> ComboBoxListProvidersCatalog { get; set; }
        /// <summary>
        /// Выбранный поставщик в комбобоксе поставщики
        /// </summary>
        public DB.ProvidersCatalog ComboBoxListProvidersCatalogEntry
        {
            get => _comboBoxListProvidersCatalogEntry;
            set
            {
                _comboBoxListProvidersCatalogEntry = value;

                if (ListPriceCatalogEntry != null)
                {
                    ListPriceCatalogEntry.ProvidersCatalog = value;
                }

                GetFormLock();

                Raise();
            }
        }

        /// <summary>
        /// Список Фруктов
        /// </summary>
        public ObservableCollection<DB.FruitsCatalog> ComboBoxListFruitsCatalog { get; set; }
        /// <summary>
        /// Выбранный поставщик в комбобоксе фрукты
        /// </summary>
        public DB.FruitsCatalog ComboBoxListFruitsCatalogEntry
        {
            get => _comboBoxListFruitsCatalogEntry;
            set
            {
                _comboBoxListFruitsCatalogEntry = value;

                if (ListPriceCatalogEntry != null)
                {
                    ListPriceCatalogEntry.FruitsCatalog = value;
                }

                GetFormLock();

                Raise();
            }
        }



        /// <summary>
        /// Блокировка кнопки сохранить если не выбран поставщик или вид фруктов
        /// </summary>
        public bool SaveDbLock { get; set; }
        private void GetFormLock()
        {
            if (ListPriceCatalog != null)
            {
                SaveDbLock = !ListPriceCatalog.Any(x => x.ProvidersCatalog != null && x.ProvidersCatalog.NameProvider.Contains("--- Выбери ---")
                                                        || (x.FruitsCatalog != null && x.FruitsCatalog.FullName.Contains("--- Выбери ---")));
            }
        }


        /// <summary>
        /// Добавить строку
        /// </summary>
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                       {
                           DB.PriceCatalog obPriceCatalogj = new DB.PriceCatalog
                           {
                               StartDate = DateTime.Now.Date,
                               EndDate = DateTime.Now.Date,
                               FruitsCatalog = ComboBoxListFruitsCatalog.First(),
                               ProvidersCatalog = ComboBoxListProvidersCatalog.First()
                           };

                           ListPriceCatalog.Insert(0, obPriceCatalogj);

                           ListPriceCatalogEntry = obPriceCatalogj;
                                
                           GetFormLock();

                       }));
            }
        }

        /// <summary>
        /// Удалить строку
        /// </summary>
        private RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand = new RelayCommand(obj =>
                       {
                           ListPriceCatalog.Remove(ListPriceCatalogEntry);

                           GetFormLock();

                       }, obj => ListPriceCatalogEntry != null));
            }
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                       (_saveCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               DbPriceCatalog.SaveChanges();
                           }
                           catch (Exception e)
                           {
                               MessageBox.Show(e.Message);
                           }

                           Load();

                       }, obj=> SaveDbLock));
            }
        }



    }

}