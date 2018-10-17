using KeepIT.DataAccessLayer.EntityFramework;
using KeepIT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepIT.BusinessLayer
{
    public class NoteManager
    {
        private Repository<Note> repo_note = new Repository<Note>();
        
        public List<Note> GetNotes()
        {
            return repo_note.List();
        }

    }
}
