using MauiApp_PracticaNotas.Models;
using System.Text.Json;

namespace MauiApp_PracticaNotas.Services
{
    public class NotesService
    {
        private const string NotesKey = "all_notes";
        private static NotesService _instance;
        private List<Note> _notes;

        public static NotesService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotesService();
                return _instance;
            }
        }

        private NotesService()
        {
            LoadNotes();
        }

        private void LoadNotes()
        {
            string json = Preferences.Get(NotesKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                _notes = new List<Note>();
            }
            else
            {
                _notes = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
            }
        }

        private void SaveNotes()
        {
            string json = JsonSerializer.Serialize(_notes);
            Preferences.Set(NotesKey, json);
        }

        public void AddOrUpdateNote(Note note)
        {
            var existingNote = _notes.FirstOrDefault(n => n.Id == note.Id);
            if (existingNote != null)
            {
                existingNote.Activity = note.Activity;
                existingNote.IsUrgent = note.IsUrgent;
            }
            else
            {
                _notes.Add(note);
            }
            SaveNotes();
        }

        public List<Note> GetActiveNotes()
        {
            return _notes.Where(n => !n.IsDeleted).OrderByDescending(n => n.CreatedDate).ToList();
        }

        public List<Note> GetDeletedNotes()
        {
            return _notes.Where(n => n.IsDeleted).OrderByDescending(n => n.CreatedDate).ToList();
        }

        public void MoveToTrash(string noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                note.IsDeleted = true;
                SaveNotes();
            }
        }

        public void RestoreNote(string noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                note.IsDeleted = false;
                SaveNotes();
            }
        }

        public void DeletePermanently(string noteId)
        {
            var note = _notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                _notes.Remove(note);
                SaveNotes();
            }
        }

        public void ClearAllDeletedNotes()
        {
            _notes.RemoveAll(n => n.IsDeleted);
            SaveNotes();
        }
    }
}
