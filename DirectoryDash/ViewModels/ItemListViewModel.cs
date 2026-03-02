using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DirectoryDash.ViewModels
{
    internal partial class ItemListViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ICollectionView itemsView;

        [ObservableProperty]
        private string searchText;

        internal void UpdateCollection(ObservableCollection<ExplorerItem> items)
        {
            ItemsView = CollectionViewSource.GetDefaultView(items);
            ItemsView.Filter = Filter;
        }

        public ItemListViewModel()
        {
            PropertyChanged += ItemListViewModel_PropertyChanged;
        }

        private void ItemListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchText) && ItemsView != null)
                ItemsView.Refresh();
        }

        private bool Filter(object obj)
        {
            if (obj is ExplorerItem item)
            {
                if (string.IsNullOrEmpty(SearchText))
                    return true;
                return item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        internal void Refresh() => ItemsView?.Refresh();
    }
}
