﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using EHentai.uwp.Model;
using Uwp.Http;

namespace EHentai.uwp
{
    public abstract class BasePage : Page
    {
        public MainPage Main = MainPage.Main;
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
            MessageDialog messageDialog = new MessageDialog(cotent);
            await messageDialog.ShowAsync();
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
