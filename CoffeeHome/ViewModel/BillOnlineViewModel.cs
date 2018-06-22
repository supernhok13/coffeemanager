﻿using CoffeeHome.Model;
using CoffeeHome.TemplateView.CRUTemplate;
using CoffeeHome.TemplateView.DeleteTemplate;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace CoffeeHome.ViewModel
{
    class BillOnlineViewModel:BaseViewModel
    {
        #region VariableBinding
        private ObservableCollection<Bill_Online> billOnlineList;
        public ObservableCollection<Bill_Online> BillOnlineList
        {
            get => billOnlineList;
            set
            {
                billOnlineList = value;
                OnPropertyChanged("billOnlineList");
            }
        }

        private BillOnlineCRUDialog CruDialog = new BillOnlineCRUDialog();
        private DeleteDialog deleteDialog = new DeleteDialog();

        private string action;
        public string Action
        {
            get => action;
            set
            {
                action = value;
                OnPropertyChanged("action");
            }
        }

        private CollectionViewSource billOnlineViewSource = new CollectionViewSource();
        public CollectionViewSource BillOnlineViewSource
        {
            get => billOnlineViewSource;
            set
            {
                billOnlineViewSource = value;
                OnPropertyChanged("billOnlineViewSource");
            }
        }
        #endregion

        #region ViewObject
        private Bill_Online billOnlineViewObject = new Bill_Online();
        public Bill_Online BillOnlineViewObject
        {
            get => billOnlineViewObject; set
            {
                billOnlineViewObject = value;
                OnPropertyChanged("billOnlineViewObject");
            }
        }
        #endregion

        #region Model
        private BillOnlineModel billOnlineModel = new BillOnlineModel();
        #endregion

        #region Command
        public ICommand OpenCruDialogCommand { get => openCruDialogCommand; set => openCruDialogCommand = value; }
        private ICommand openCruDialogCommand;

        private ICommand createCommand;
        public ICommand CreateCommand { get => createCommand; set => createCommand = value; }

        private ICommand submitCommand;
        public ICommand SubmitCommand { get => submitCommand; set => submitCommand = value; }

        private ICommand openDeleteDialogCommand;
        public ICommand OpenDeleteDialogCommand { get => openDeleteDialogCommand; set => openDeleteDialogCommand = value; }
        #endregion

        public BillOnlineViewModel()
        {
            CruDialog.DataContext = this;
            billOnlineList = new ObservableCollection<Bill_Online>(billOnlineModel.getList());
            billOnlineViewSource.Source = BillOnlineList;

            OpenCruDialogCommand = new RelayCommand<object>(p => true, OpenCRUDialogEventAsync);
            createCommand = new RelayCommand<Bill_Online>(p => true, create);
            submitCommand = new RelayCommand<Bill_Online>(p => true, submit);
            OpenDeleteDialogCommand = new RelayCommand<object>(p => true, openDeleteDialog);
        }

        private void refreshView()
        {
            billOnlineList = null;
            billOnlineList = new ObservableCollection<Bill_Online>(billOnlineModel.getList());
            billOnlineViewSource.Source = BillOnlineList;
            billOnlineViewSource.View.Refresh();
        }

        private async void openDeleteDialog(object obj)
        {
            deleteDialog.DataContext = this;
            this.Action = obj.ToString();
            var result = await DialogHost.Show(deleteDialog, "RootDialog");
        }

        private void submit(Bill_Online bill)
        {
            if (this.Action == "Thêm")
            {
                create(bill);
            }
            else if (this.Action == "Sửa")
            {
                update(bill);
            }
            else
            {
                delete();
            }
        }

        private void create(Bill_Online obj)
        {
            if (billOnlineModel.create(obj))
            {
                this.BindingMessage(true, "Đã thêm thành công");
                refreshView();
            }
            else
            {
                this.BindingMessage(false, "Không thêm được hóa đơn");
            }
            DialogHost.CloseDialogCommand.Execute(new object(), null);
        }

        private void update(Bill_Online obj)
        {
            if (billOnlineModel.update(obj, BillOnlineViewObject.id_bill_online))
            {
                this.BindingMessage(true, "Đã sửa thành công");
                refreshView();
            }
            else
            {
                this.BindingMessage(false, "Không sửa được hóa đơn");
            }
            DialogHost.CloseDialogCommand.Execute(new object(), null);
        }

        private void delete()
        {
            if (billOnlineModel.delete(int.Parse(this.Action)))
            {
                this.BindingMessage(true, "Đã xóa thành công");
                refreshView();
            }
            else
            {
                this.BindingMessage(false, "Không xóa được đồ uống");
            }
            DialogHost.CloseDialogCommand.Execute(new object(), null);
        }

        private async void OpenCRUDialogEventAsync(object obj)
        {
            if (obj == null)
            {
                this.Action = "Thêm";
                BillOnlineViewObject = new Bill_Online();
            }
            else
            {
                this.Action = "Sửa";
                BillOnlineViewObject = null;
                BillOnlineViewObject = billOnlineModel.getBillByID((int)obj);
            }
            var result = await DialogHost.Show(CruDialog, "RootDialog");
        }
    }
}
