using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using FruitsDB.DB;
using FruitsDB.DB.Base;

namespace FruitsDB.Catalog.FruitsCatalog.ViewModel
{
    public class ViewModelFruitsCatalog : Base
    {
        private DB.FruitsCatalog _listFruitsCatalogEntry;

        public ViewModelFruitsCatalog()
        {
            Load();
        }

        /// <summary>
        /// Выгрузка данных
        /// </summary>
        private void Load()
        {
            DbFruitsCatalog = new ModelDb();

            DbFruitsCatalog.FruitsCatalog.Load();
            ListFruitsCatalog = DbFruitsCatalog.FruitsCatalog.Local;
        }

        /// <summary>
        /// Модель базы данных
        /// </summary>
        public ModelDb DbFruitsCatalog { get; set; }


        /// <summary>
        /// Таблица фрукты
        /// </summary>
        public ObservableCollection<DB.FruitsCatalog> ListFruitsCatalog { get; set; }
        /// <summary>
        /// Выбранная строка в таблице фрукты
        /// </summary>
        public DB.FruitsCatalog ListFruitsCatalogEntry
        {
            get => _listFruitsCatalogEntry;
            set
            {
                _listFruitsCatalogEntry = value;

                BoolListFruitsCatalogEntry = value != null;

                Raise();
            }
        }


        /// <summary>
        /// Блокировка полей если не выбран элемент в таблице фрукты
        /// </summary>
        public bool BoolListFruitsCatalogEntry { get; set; }


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
                           ListFruitsCatalog.Insert(0, new DB.FruitsCatalog());
                           
                           ListFruitsCatalogEntry = ListFruitsCatalog.FirstOrDefault();


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
                           ListFruitsCatalog.Remove(ListFruitsCatalogEntry);


                       }, obj => ListFruitsCatalogEntry != null));
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
                               DbFruitsCatalog.SaveChanges();
                           }
                           catch (DbUpdateException)
                           {
                               MessageBox.Show("Нельзя удалить используемый элемент!");
                           }
                           catch (Exception e)
                           {
                               MessageBox.Show(e.Message);
                           }

                           Load();


                       }));
            }
        }

    }
}