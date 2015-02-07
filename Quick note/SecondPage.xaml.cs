using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Quick_note
{
    public partial class Details : PhoneApplicationPage
    {
        private string OldText = "";

        public Details()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string text = NavigationContext.QueryString["text"];
            SecondTextBox.Text = text;
            SecondTextBox.IsEnabled = false;
            base.OnNavigatedTo(e);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult MessageBoxClickResult = MessageBox.Show("Delete this note?", Constants.Warning, MessageBoxButton.OKCancel);
            if (MessageBoxClickResult == MessageBoxResult.Cancel)
            {
                return;
            }
            Tools.DeleteNote(SecondTextBox.Text);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void GoToMainPage(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void GoToList(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?page=1", UriKind.Relative));
        }

        private void SendMail(object sender, EventArgs e)
        {
            Tools.SendMail(SecondTextBox.Text);
        }

        private void SendSMS(object sender, RoutedEventArgs e)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.Body = SecondTextBox.Text;
            smsComposeTask.Show();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            if ((string)EditButton.Content == "Edit")
            {
                EditButton.Content = "Save";
                SecondTextBox.IsEnabled = true;
                this.OldText = SecondTextBox.Text;
            }
            else
            {
                using (DB db = new DB(Constants.ConnectionString))
                {
                    if (!db.DatabaseExists())
                    {
                        //exception handling
                        return;
                    }
                    IQueryable<Note> NoteQuery = from NOTE in db.Notes where NOTE.Text == this.OldText select NOTE;
                    Note note = NoteQuery.FirstOrDefault();
                    if (note == null)
                    {
                        //exception handling: note should not be null
                        return;
                    }
                    if (string.IsNullOrEmpty(SecondTextBox.Text))
                    {
                        return;
                    }
                    note.Text = SecondTextBox.Text;
                    try
                    {
                        db.SubmitChanges();
                    }catch(Exception ex)
                    {
                       Console.WriteLine(ex.Message);
                    }
                    EditButton.Content = "Edit";
                    SecondTextBox.IsEnabled = false;
                }
            }
        }

    }
}