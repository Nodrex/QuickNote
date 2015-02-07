using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_note
{
    public class DB : DataContext
    {
        public DB(string connectionString)
            : base(connectionString)
        {
        }

        public Table<Note> Notes
        {
            get
            {
                return this.GetTable<Note>();
            }
        }
    }

}