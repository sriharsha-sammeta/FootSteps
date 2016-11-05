using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps.DataModels {
    class Connect : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _id;
        public string Id {
            get { return _id; }
            set {
                if (value != _id) {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _groupMapperId;
        public string groupMapperId {
            get { return _groupMapperId; }
            set {
                if (value != _groupMapperId) {
                    _groupMapperId = value;
                    NotifyPropertyChanged("groupMapperId");
                }
            }
        }

        private bool _status;
        public bool status {
            get { return _status; }
            set {
                if (value != _status) {
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }
        /*status
         * true->accepted
         * false->pending
         * tuple is not there then rejected or didnt even add
         */
    }
}
