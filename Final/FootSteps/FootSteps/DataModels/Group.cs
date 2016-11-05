using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps.DataModels {
    public class Group : INotifyPropertyChanged {

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

        private string _name;
        public string name {
            get { return _name; }
            set {
                if (value != _name) {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }

        private string _theme;
        public string theme {
            get { return _theme; }
            set {
                if (value != _theme) {
                    _theme = value;
                    NotifyPropertyChanged("theme");
                }
            }
        }
    }
}
