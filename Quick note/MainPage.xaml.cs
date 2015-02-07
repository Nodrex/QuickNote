using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Quick_note.Resources;
using Windows.UI;
using System.Data.Linq;

//saxelis shecvla WMAppManifest.xml fileshi

namespace Quick_note
{
    public partial class QuickNote : PhoneApplicationPage
    {

        public QuickNote()
        {
            InitializeComponent();
            DataContext = App.ViewModel;

            using (DB db = new DB(Constants.ConnectionString))
            {
                if (!db.DatabaseExists())
                {
                    try
                    {
                        db.CreateDatabase();
                        //db.DeleteDatabase();
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                string page = NavigationContext.QueryString["page"];
                this.MainPivot.SelectedIndex = 1;//navigate to list
            }catch(Exception ex)
            {
                //page change request was not send
            }

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void ChangePivotItem(object sender, SelectionChangedEventArgs e)
        {
            if (this.MainPivot.SelectedIndex == 1)
            {
                IList<Note> NoteList = Tools.SelectAllNotes();
                if (NoteList == null)
                {
                    Note_List.ItemsSource = new List<string>();
                    return;
                }
                List<string> Notes = new List<string>();
                for (int i = NoteList.Count - 1; i >= 0; i--)
                {
                    Notes.Add(NoteList[i].Text);
                }
                Note_List.ItemsSource = Notes;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DB db = new DB(Constants.ConnectionString))
                {
                    if (!db.DatabaseExists())
                    {
                        return;
                    }
                    string text = MainTextBox.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        return;
                    }
                    Note note = new Note();
                    note.Text = text;
                    note.NoteID = text;
                    db.Notes.InsertOnSubmit(note);
                    db.SubmitChanges();
                    MainTextBox.Text = "";
                    this.MainPivot.SelectedIndex = 1;//navigate to list
                }
            }
            catch (Exception ex)
            {
                //how to see exception message
                //ex.InnerException;
            }
        }


        private void SelectNote(object sender, RoutedEventArgs e)
        {
            String item = ((String)((LongListSelector)sender).SelectedItem);
            //int indexOfItem = ((LongListSelector)sender).ItemsSource.IndexOf(item);
            NavigationService.Navigate(new Uri("/SecondPage.xaml?text=" + item, UriKind.Relative));
        }

        private void DeleteAllNotes_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult MessageBoxClickResult = MessageBox.Show("Delete all Notes?", Constants.Warning, MessageBoxButton.OKCancel);
            if (MessageBoxClickResult == MessageBoxResult.Cancel)
            {
                return;
            }
            IList<Note> Notes = Tools.SelectAllNotes();
            foreach (Note item in Notes)
            {
                Tools.DeleteNote(item.Text);
            }
            this.MainPivot.SelectedIndex = 0;
        }

        private void SendAllToMail_Click(object sender, RoutedEventArgs e)
        {
            IList<Note> Notes = Tools.SelectAllNotes();
            string AllNotes = "";
            for (int i = Notes.Count - 1; i >= 0; i--)
            {
                AllNotes += Notes[i].Text + Environment.NewLine + "__________________________" + Environment.NewLine;
            }
            Tools.SendMail(AllNotes);
        }
    }
}