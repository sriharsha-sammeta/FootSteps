using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootSteps.DataModels {
    public class Person: INotifyPropertyChanged {

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

        private string _phoneNo;
        public string phoneNo {
            get { return _phoneNo; }
            set {
                if (value != _phoneNo) {
                    _phoneNo = value;
                    NotifyPropertyChanged("phoneNo");
                }
            }
        }

        private double _lat;
        public double latitude {
            get { return _lat; }
            set {
                if (value != _lat) {
                    _lat = value;
                    NotifyPropertyChanged("latitude");
                }
            }
        }

        private double _long;
        public double longitude {
            get { return _long; }
            set {
                if (value != _long) {
                    _long = value;
                    NotifyPropertyChanged("longitude");
                }
            }
        }

        private bool _isGloballyVisible;
        public bool isGloballyVisible {
            get { return _isGloballyVisible; }
            set {
                if (value != _isGloballyVisible) {
                    _isGloballyVisible = value;
                    NotifyPropertyChanged("isGloballyVisible");
                }
            }
        }    
    }
}
