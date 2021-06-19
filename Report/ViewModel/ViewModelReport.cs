using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Data;
using FruitsDB.DB;
using FruitsDB.DB.Base;

namespace FruitsDB.Report.ViewModel
{
    public class ViewModelReport : Base
    {
        public ViewModelReport()
        {
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;

        }

        /// <summary>
        /// Модель базы данных
        /// </summary>
        public ModelDb DbReport { get; set; }


        /// <summary>
        /// Дата начала отчетного периода
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Дата конца отчетного периода
        /// </summary>
        public DateTime EndDate { get; set; }


        private CollectionView ListReportView { get; set; }

        /// <summary>
        /// Таблица для отчета
        /// </summary>
        public ObservableCollection<Model.Report> ListReport { get; set; }


        /// <summary>
        /// Формируем отчет
        /// </summary>
        private void GetReport()
        {
            DbReport = new ModelDb();
            ListReport = new ObservableCollection<Model.Report>();

            // Группировка
            ListReportView = (CollectionView)CollectionViewSource.GetDefaultView(ListReport);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Provider");
            ListReportView.GroupDescriptions.Add(groupDescription);

                              
            DbReport.ProvidersCatalog.Load();

            //Проходим по таблицам от поставщика до его заказов
            ObservableCollection <ProvidersCatalog> objProvidersCatalog = DbReport.ProvidersCatalog.Local;
            foreach (ProvidersCatalog itemProvidersCatalog in objProvidersCatalog)
            {
                ICollection<Stock> objStock = itemProvidersCatalog.Stock;
                foreach (Stock itemStock in objStock)
                {
                    //проверяем диапазон дат
                    if (itemStock.DeliveryDate >= StartDate && itemStock.DeliveryDate <= EndDate)
                    {
                        ICollection<StockFruits> objStockFruits = itemStock.StockFruits;
                        foreach (StockFruits itemStockFruit in objStockFruits)
                        {
                            //Проверяем наличие записи в листе, если нет добавляем, если есть плюсуем к существующей
                            if (ListReport.Any(x=>x.Provider == itemProvidersCatalog.NameProvider && x.Fruit == itemStockFruit.FullName))
                            {
                                Model.Report objReport = ListReport.First(x => x.Provider == itemProvidersCatalog.NameProvider && x.Fruit == itemStockFruit.FullName);

                                objReport.MassSumm += itemStockFruit.Mass;
                                objReport.PriceSumm += itemStockFruit.Mass * itemStockFruit.PriceCatDb;
                            }
                            else
                            {
                                ListReport.Add(new Model.Report
                                {
                                    Provider = itemProvidersCatalog.NameProvider,
                                    Fruit = itemStockFruit.FullName,
                                    MassSumm = itemStockFruit.Mass,
                                    PriceSumm = itemStockFruit.Mass * itemStockFruit.PriceCatDb
                                });
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Запуск выгрузки отчета
        /// </summary>
        private RelayCommand _reportCommand;
        public RelayCommand ReportCommand
        {
            get
            {
                return _reportCommand ??
                       (_reportCommand = new RelayCommand(obj =>
                       {
                           GetReport();

                       }));
            }
        }
    }
}