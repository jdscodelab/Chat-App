using ChatApp.CustomControls;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Status Thumbs

        #region Properties
        public ObservableCollection<StatusDataModel> statusThumbsCollection { get; set; }
        #endregion
        
        #region Logics
        void LoadStatusThumbs()
        {
            //Lets bind our collection to itemscontrol
            statusThumbsCollection = new ObservableCollection<StatusDataModel>()
            {
                //Since we want to keep first status blank for the user to add own status
            new StatusDataModel
            {
                IsMeAddStatus=true
            },
            new StatusDataModel
            {
              ContactName="Mike",
               ContactPhoto=new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            new StatusDataModel
            {
              ContactName="Steve",
               ContactPhoto=new Uri("/assets/2.jpg", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            new StatusDataModel
            {
              ContactName="Will",
               ContactPhoto=new Uri("/assets/3.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },

            new StatusDataModel
            {
              ContactName="John",
               ContactPhoto=new Uri("/assets/4.png", UriKind.RelativeOrAbsolute),
                 StatusImage=new Uri("/assets/3.jpg", UriKind.RelativeOrAbsolute),
                IsMeAddStatus=false
            },
            };
            OnPropertyChanged("statusThumbsCollection");
        }
        #endregion

        #endregion

        #region Chats List
        #region Properties
        public ObservableCollection<ChatListData> Chats { get; set; }
        #endregion

        #region Logics
        void LoadChats()
        {
            Chats = new ObservableCollection<ChatListData>()
            {
                new ChatListData{
                ContactName = "Billy",
                ContactPhoto = new Uri("/assets/6.jpg", UriKind.RelativeOrAbsolute),
                Message="Hey, What's Up?",
                LastMessageTime="Tue, 12:58 PM",
                ChatIsSelected=true
                },
                new ChatListData{
                ContactName = "Mike",
                ContactPhoto = new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                Message="Check the mail.",
                LastMessageTime="Mon, 10:07 AM"
                },
                new ChatListData{
                ContactName = "Steve",
                ContactPhoto = new Uri("/assets/7.png", UriKind.RelativeOrAbsolute),
                Message="Yes, we had fun.",
                LastMessageTime="Tue, 08:10 AM"
                },
                new ChatListData{
                ContactName = "John",
                ContactPhoto = new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                Message="What about you?",
                LastMessageTime="Tue, 01:00 PM"
                }
            };
            OnPropertyChanged();
        }
        #endregion
        #endregion
        public ViewModel()
        {
            LoadStatusThumbs();
            LoadChats();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}