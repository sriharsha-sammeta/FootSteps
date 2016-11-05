using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps.DataModels
{
    public class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _personInMeeting;
        public bool personInMeeting {
            get { return _personInMeeting; }
            set {
                if (value != _personInMeeting) {
                    _personInMeeting = value;
                    NotifyPropertyChanged("personInMeeting");
                }
            }
        }

        private bool _groupInMeeting;
        public bool groupInMeeting {
            get { return _groupInMeeting; }
            set {
                if (value != _groupInMeeting) {
                    _groupInMeeting = value;
                    NotifyPropertyChanged("groupInMeeting");
                }
            }
        }

        private bool _personAndGroupInSameMeeting;
        public bool personAndGroupInSameMeeting {
            get { return _personAndGroupInSameMeeting; }
            set {
                if (value != _personAndGroupInSameMeeting) {
                    _personAndGroupInSameMeeting = value;
                    NotifyPropertyChanged("personAndGroupInSameMeeting");
                }
            }
        }
    }
}
