using DirectoryDash.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DirectoryDash.Helpers
{
    class ListHelper
    {
        public static void UpdateCollection<T>(
            ICollection<T> target,
            IEnumerable<T> source,
            bool fromDispatcher = false)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;
            if (fromDispatcher)
            {
                dispatcher.BeginInvoke(() =>
                {
                    target.Clear();

                }, DispatcherPriority.Background);
            }
            else
            {
                target.Clear();
            }
            foreach (var item in source)
            {
                if (fromDispatcher)
                {
                    dispatcher.BeginInvoke(() =>
                    {
                        target.Add(item);
                    }, DispatcherPriority.Background);
                }
                else
                {
                    target.Add(item);
                }

            }

        }

        public static void AddToCollection<T>(
            ICollection<T> target,
            ICollection<T> source,
            bool fromDispatcher = false)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            void apply()
            {
                foreach (var item in source)
                    target.Add(item);
            }

            if (fromDispatcher)
            {
                dispatcher.BeginInvoke(apply, DispatcherPriority.Background);
                return;
            }

            if (dispatcher.CheckAccess())
            {
                apply();
                return;
            }
        }

        public static void UpdateWrapperCollection<TWrapper, TModel>(
            ICollection<TWrapper> target,
            IEnumerable<TModel> source,
            bool fromDispatcher = false)
        {

            var dispatcher = System.Windows.Application.Current.Dispatcher;

            if (fromDispatcher)
            {
                dispatcher.BeginInvoke(() =>
                {
                    target.Clear();

                }, DispatcherPriority.Background);
            }
            else
            {
                target.Clear();
            }

            foreach (var model in source)
            {
                if (fromDispatcher)
                {

                    dispatcher.BeginInvoke(() =>
                    {
                        var created = Activator.CreateInstance(typeof(TWrapper), model);
                        if (created is TWrapper wrapper)
                        {
                            target.Add(wrapper);
                        }
                        else
                        {
                            //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                        }

                    }, DispatcherPriority.Background);
                }
                else
                {
                    var created = Activator.CreateInstance(typeof(TWrapper), model);
                    if (created is TWrapper wrapper)
                    {
                        target.Add(wrapper);
                    }
                    else
                    {
                        //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                    }
                }
            }
        }

        public static async Task UpdateWrapperCollectionAsync<TWrapper, TModel>(
            ObservableCollection<TWrapper> target,
            IEnumerable<TModel> source,
            bool fromDispatcher = false)
        {
            target.Clear();

            var dispatcher = System.Windows.Application.Current.Dispatcher;

            foreach (var model in source)
            {
                if (fromDispatcher)
                {

                    await dispatcher.InvokeAsync(() =>
                    {
                        var created = Activator.CreateInstance(typeof(TWrapper), model);
                        if (created is TWrapper wrapper)
                        {
                            target.Add(wrapper);
                        }
                        else
                        {
                            //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                        }

                    }, DispatcherPriority.Background);
                }
                else
                {
                    var created = Activator.CreateInstance(typeof(TWrapper), model);
                    if (created is TWrapper wrapper)
                    {
                        target.Add(wrapper);
                    }
                    else
                    {
                        //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                    }
                }
            }
        }

        public static void UpdateWrapperList<TWrapper, TModel>(
            List<TWrapper> target,
            IEnumerable<TModel> source,
            bool fromDispatcher = false)
        {

            target.Clear();

            var dispatcher = System.Windows.Application.Current.Dispatcher;

            void Apply()
            {

                foreach (var model in source)
                {
                    if (fromDispatcher)
                    {

                        dispatcher.BeginInvoke(() =>
                        {
                            var created = Activator.CreateInstance(typeof(TWrapper), model);
                            if (created is TWrapper wrapper)
                            {
                                target.Add(wrapper);
                            }
                            else
                            {
                                //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                            }

                        }, DispatcherPriority.Background);
                    }
                    else
                    {
                        var created = Activator.CreateInstance(typeof(TWrapper), model);
                        if (created is TWrapper wrapper)
                        {
                            target.Add(wrapper);
                        }
                        else
                        {
                            //Logger.LogError("Error", "Failed to create TWrapper instance: " + typeof(TWrapper).Name);
                        }
                    }
                }
            }
            ;

            //if( dispatcher.CheckAccess() || fromDispatcher)
            //{
            Apply();
            //    return;
            //}
            //Apply();

        }

        public static void ReplaceCollection<T>(ObservableCollection<T> target, ObservableCollection<T> source)
        {
            target = source;
        }

        internal static void ReorderCollectionById<T>(ObservableCollection<T> notifications) where T : class
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                return;

            var old = notifications.OrderByDescending(x => idProperty.GetValue(x)).ToList();
            notifications.Clear();

            foreach (var item in old)
            {
                notifications.Add(item);
            }
        }

        internal static void ReplaceItem<T>(ObservableCollection<T> presets, T preset) where T : class
        {
            var index = presets.IndexOf(preset);
            if (index < 0) return;
            presets[index] = preset;
        }

        internal static void ReplaceItemById<T>(IEnumerable<T> items, T toReplace)
        {
            var idProp = typeof(T).GetProperty("Id");
            if (idProp == null) return;

            var toReplaceId = idProp.GetValue(toReplace);

            if (items is IList<T> list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var toBeReplacedId = idProp.GetValue(list[i]);
                    if (toReplaceId.Equals(toBeReplacedId))
                    {
                        list[i] = toReplace;
                        break;
                    }
                }
            }
        }

        internal static void RebuildCollection<T>(ObservableCollection<T> collection, List<T> list)
        {
            collection.Clear();
            UpdateCollection(collection, list);
        }

        internal static void AddToCollection<T>(ObservableCollection<T> collection, List<T> list)
        {
            foreach (var item in list)
            {
                collection.Add(item);
            }
        }

        internal static void AddItemFromDispatcher<T>(ObservableCollection<T> collection, T item)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(() =>
            {
                collection.Add(item);
            }, DispatcherPriority.Background);
        }

        internal static void DataFromWrappers<TWrapper, TModel>(ObservableCollection<TWrapper> wrappers,
                                                 ref List<TModel> models)
        {
            foreach (var wrapper in wrappers)
            {
                var propExits = wrapper.GetType().GetProperty(typeof(TModel).Name);
                if (propExits != null)
                {
                    var model = (TModel)propExits.GetValue(wrapper);
                    models.Add(model);
                }
            }
        }

        internal static async Task AddInBatches<T> (
            ICollection<T> destination,
            IEnumerable<T> explorerItems,
            bool fromDispatcher = false,
            int batchSize = 10)
        {

            for (int i = 0; i < explorerItems.Count(); i += batchSize)
            {
                var batch = explorerItems.Skip(i).Take(batchSize);

                AddToCollection(destination, batch.ToList(), true);

                await Task.Yield();
            }
        }
    }
}
