using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_note
{
    public class Tools
    {
        public static void DeleteNote(string Note)
        {
            using (DB db = new DB(Constants.ConnectionString))
            {
                if (!db.DatabaseExists())
                {
                    //exception handling
                    return;
                }
                IQueryable<Note> NoteQuery = from NOTE in db.Notes where NOTE.Text == Note select NOTE;
                Note note = NoteQuery.FirstOrDefault();
                if (note == null)
                {
                    //exception handling: note should not be null
                    return;
                }
                db.Notes.DeleteOnSubmit(note);
                db.SubmitChanges();
            }
        }

        public static IList<Note> SelectAllNotes()
        {
            IList<Note> NoteList = null;
            using (DB db = new DB(Constants.ConnectionString))
            {
                if (!db.DatabaseExists())
                {
                    //error, here should be message to restart app and send me feedbeck
                    return null;
                }
                IQueryable<Note> NoteQuery = from NOTE in db.Notes select NOTE;
                //need to check if NoteQuery will be nul if in base will not be record
                NoteList = NoteQuery.ToList();
            }
            return NoteList;
        }

        public static void SendMail(string note)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = "Notes";
            emailComposeTask.Body = note;
            emailComposeTask.To = "";
            emailComposeTask.Show();
        }
    }
}