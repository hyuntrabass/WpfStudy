using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo
{
    public class TodoItem : ViewModelBase
    {
        private string _title = string.Empty;
        private bool _isDone;
        private string _note = string.Empty;
        private DateTime? _alarmTime;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public bool IsDone
        {
            get => _isDone;
            set
            {
                _isDone = value;
                OnPropertyChanged(nameof(IsDone));
            }
        }

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public DateTime? AlarmTime
        {
            get => _alarmTime;
            set
            {
                _alarmTime = value;
                OnPropertyChanged(nameof(AlarmTime));
            }
        }
    }
}
