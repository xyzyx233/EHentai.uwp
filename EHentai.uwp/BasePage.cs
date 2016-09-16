using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using EHentai.uwp.Model;
using Uwp.Control;
using Uwp.Control.Model;
using Uwp.Http;

namespace EHentai.uwp
{
    public abstract class BasePage : Page
    {
        public static MainPage Main;
        public static PivotViewModel PivotView;
        public List<Task> Tasks = new List<Task>();
        public static Site Site = new ExHentaiSite();
        public static Http Http = Site.Http;

        protected BasePage()
        {
            Unloaded += BasePage_Unloaded;
        }

        private void BasePage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (Task task in Tasks)
            {
                task.Wait();
            }

            Tasks.RemoveAll(x => true);
        }

        public async void ShowMessage(string cotent)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                MessageDialog messageDialog = new MessageDialog(cotent);
                await messageDialog.ShowAsync();
            });


        }

        public void CreateTask(Action action)
        {
            Tasks.Add(Task.Run(action));
        }

        public void CreateTask(Action action, CancellationToken cancellationToken)
        {
            Tasks.Add(Task.Run(action, cancellationToken));
        }
    }
}
