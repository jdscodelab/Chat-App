using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatApp.Models
{
    public class ChatListData: INotifyPropertyChanged
    {
        public string ContactName { get; set; }
        public byte[] ContactPhoto { get; set; }
        protected string message { get; set; }
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        protected string lastMessageTime { get; set; }
        public string LastMessageTime
        {
            get
            {
                return lastMessageTime;
            }
            set
            {
                lastMessageTime = value;
                OnPropertyChanged("LastMessageTime");
            }
        }
        public bool ChatIsSelected { get; set; }
        public bool ChatIsPinned { get; set; }
        public bool ChatIsArchived { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}