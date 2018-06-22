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
    class BillViewModel:BaseViewModel
    {
        #region BindingVariable
        private ObservableCollection<Bill> billList;
        public ObservableCollection<Bill> BillList
        {
            get => billList;
            set
            {
                billList = value;
                OnPropertyChanged("billList");
            }
        }

        private ObservableCollection<Customer> customerList;
        public ObservableCollection<Customer> CustomerList { get => customerList;
            set
            {
                customerList = value;
                OnPropertyChanged("customerList");
            }
        }

        private ObservableCollection<Table> tableList;
        public ObservableCollection<Table> TableList
        {
            get => tableList;
            set
            {
                tableList = value;
                OnPropertyChanged("tableList");
            }
        }

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

        private BillCRUDialog CruDialog = new BillCRUDialog();
        private DeleteDialog deleteDialog = new DeleteDialog();

        private CollectionViewSource billViewSource = new CollectionViewSource();
        public CollectionViewSource BillViewSource
        {
            get => billViewSource;
            set
            {
                billViewSource = value;
                OnPropertyChanged("billViewSource");
            }
        }

        private bool isDisableCombobox;
        public bool IsDisableCombobox
        {
            get => isDisableCombobox;
            set
            {
                isDisableCombobox = value;
                OnPropertyChanged("isDisableCombobox");
            }
        }
        #endregion

        #region ViewObject
        private Bill billViewObject = new Bill();
        public Bill BillViewObject
        {
            get => billViewObject; set
            {
                billViewObject = value;
                OnPropertyChanged("billViewObject");
            }
        }
        #endregion

        #region Model
        private BillModel billModel = new BillModel();
        private CustomerModel customerModel = new CustomerModel();
        private TableModel tableModel = new TableModel();
        #endregion

        #region Command
        public ICommand OpenCruDialogCommand { get => openCruDialogCommand; set => openCruDialogCommand = value; }
        private ICommand openCruDialogCommand;

        private ICommand submitCommand;
        public ICommand SubmitCommand { get => submitCommand; set => submitCommand = value; }

        private ICommand openDeleteDialogCommand;
        public ICommand OpenDeleteDialogCommand { get => openDeleteDialogCommand; set => openDeleteDialogCommand = value; }

        #endregion

        public BillViewModel()
        {
            CruDialog.DataContext = this;
            IsDisableCombobox = true;
            billList = new ObservableCollection<Bill>(billModel.getList());
            customerList = new ObservableCollection<Customer>(customerModel.getList());
            tableList = new ObservableCollection<Table>(tableModel.getList());
            billViewSource.Source = billList;

            OpenCruDialogCommand = new RelayCommand<object>(p => true, OpenCRUDialogEventAsync);
            submitCommand = new RelayCommand<Bill>(p => true, submit);
            OpenDeleteDialogCommand = new RelayCommand<object>(p => true, openDeleteDialog);
        }

        private async void openDeleteDialog(object obj)
        {
            deleteDialog.DataContext = this;
            this.Action = obj.ToString();
            var result = await DialogHost.Show(deleteDialog, "RootDialog");
        }

        private void refreshView()
        {
            billList = null;
            billList = new ObservableCollection<Bill>(billModel.getList());
            billViewSource.Source = BillList;
            billViewSource.View.Refresh();
        }

        private void submit(Bill bill)
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

        private void create(Bill obj)
        {
            if (billModel.create(obj))
            {
                this.BindingMessage(true, "Đã thêm thành công");
                refreshView();
            }
            else
            {
                this.BindingMessage(false, "Không thêm được đồ uống");
            }
            DialogHost.CloseDialogCommand.Execute(new object(), null);
        }

        private void update(Bill obj)
        {
            if (billModel.update(obj, BillViewObject.id_bill))
            {
                this.BindingMessage(true, "Đã sửa thành công");
                refreshView();
            }
            else
            {
                this.BindingMessage(false, "Không sửa được đồ uống");
            }
            DialogHost.CloseDialogCommand.Execute(new object(), null);
        }

        private void delete()
        {
            if (billModel.delete(int.Parse(this.Action)))
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
                IsDisableCombobox = true;
                BillViewObject = new Bill();
            }
            else
            {
                this.Action = "Sửa";
                IsDisableCombobox = false;
                BillViewObject = null;
                BillViewObject = billModel.getBillByID((int)obj);
            }
            var result = await DialogHost.Show(CruDialog, "RootDialog");
        }
    }
}
