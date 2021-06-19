using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using FruitsDB.DB;
using FruitsDB.DB.Base;

namespace FruitsDB.Catalog.ProvidersCatalog.ViewModel
{
    public class ViewModelProvidersCatalog : Base
    {

        private DB.ProvidersCatalog _listProvidersCatalogEntry;

        public ViewModelProvidersCatalog()
        {
            Load();
        }

        /// <summary>
        /// Выгрузка данных
        /// </summary>
        private void Load()
        {
            DbProvidersCatalog = new ModelDb();


            DbProvidersCatalog.ProvidersCatalog.Load();
            ListProvidersCatalog = DbProvidersCatalog.ProvidersCatalog.Local;
        }

        /// <summary>
        /// Модель базы данных
        /// </summary>
        public ModelDb DbProvidersCatalog { get; set; }

        /// <summary>
        /// Таблица поставщики
        /// </summary>
        public ObservableCollection<DB.ProvidersCatalog> ListProvidersCatalog { get; set; }
        /// <summary>
        /// Выбранная строка в таблице поставщики
        /// </summary>
        public DB.ProvidersCatalog ListProvidersCatalogEntry
        {
            get => _listProvidersCatalogEntry;
            set
            {
                _listProvidersCatalogEntry = value;

                BoolListProvidersCatalogEntry = value != null;

                Raise();
            }
        }

        /// <summary>
        /// Блокировка полей если не выбран элемент в таблице поставщики
        /// </summary>
        public bool BoolListProvidersCatalogEntry { get; set; }


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
                           ListProvidersCatalog.Insert(0, new DB.ProvidersCatalog());

                           ListProvidersCatalogEntry = ListProvidersCatalog.FirstOrDefault();


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
                           ListProvidersCatalog.Remove(ListProvidersCatalogEntry);



                       }, obj => ListProvidersCatalogEntry != null));
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
                               DbProvidersCatalog.SaveChanges();
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