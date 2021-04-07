using ChatApp.Commands;
using ChatApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        //Initializing resource dictionary file
        private readonly ResourceDictionary dictionary = Application.LoadComponent(new Uri("/ChatApp;component/Assets/icons.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;

        #region MainWindow

        #region Properties
        public string ContactName { get; set; }
        public byte[] ContactPhoto { get; set; }
        public string LastSeen { get; set; }

        #region Search Chats
        protected bool _isSearchBoxOpen;
        public bool IsSearchBoxOpen
        {
            get => _isSearchBoxOpen;
            set {
                if (_isSearchBoxOpen == value)
                    return;

                _isSearchBoxOpen = value;


                if (_isSearchBoxOpen == false)
                    //Clear Search Box
                    SearchText = string.Empty;
                OnPropertyChanged("IsSearchBoxOpen");
                OnPropertyChanged("SearchText");
            }
        }
        protected string LastSearchText { get; set; }
        protected string mSearchText { get; set; }
        public string SearchText
        {
            get => mSearchText;
            set
            {

                //checked if value is different
                if (mSearchText == value)
                    return;

                //Update Value
                mSearchText = value;

                //if search text is empty restore messages
                if (string.IsNullOrEmpty(SearchText))
                    Search();
            }
        }

        //This is our list containing the Window Options..
        private ObservableCollection<MoreOptionsMenu> _windowMoreOptionsMenuList;
        public ObservableCollection<MoreOptionsMenu> WindowMoreOptionsMenuList
        {
            get
            {
                return _windowMoreOptionsMenuList;
            }
            set
            {
                _windowMoreOptionsMenuList = value;
            }
        }

        //This is our list containing the Attachment Menu Options..
        private ObservableCollection<MoreOptionsMenu> _attachmentOptionsMenuList;
        public ObservableCollection<MoreOptionsMenu> AttachmentOptionsMenuList
        {
            get
            {
                return _attachmentOptionsMenuList;
            }
            set
            {
                _attachmentOptionsMenuList = value;
            }
        }
        #endregion
        #endregion

        #region Logics

        #region Window: More options Popup
        void WindowMoreOptionsMenu()
        {
            WindowMoreOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["newgroup"],
                 MenuText="New Group"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["newbroadcast"],
                 MenuText="New Broadcast"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["starredmessages"],
                 MenuText="Starred Messages"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["settings"],
                 MenuText="Settings"
                },
            };
            OnPropertyChanged("WindowMoreOptionsMenuList");
        }
        void ConversationScreenMoreOptionsMenu()
        {
            //To populate menu items for conversation screen options list..
            WindowMoreOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["allmedia"],
                 MenuText="All Media"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["wallpaper"],
                 MenuText="Change Wallpaper"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["report"],
                 MenuText="Report"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["block"],
                 MenuText="Block"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["clearchat"],
                 MenuText="Clear Chat"
                },
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["exportchat"],
                 MenuText="Export Chat"
                },
            };
            OnPropertyChanged("WindowMoreOptionsMenuList");
        }
        void AttachmentOptionsMenu()
        {
            //To populate menu items for Attachment Menu options list..
            AttachmentOptionsMenuList = new ObservableCollection<MoreOptionsMenu>()
            {
                new MoreOptionsMenu()
                {
                 Icon = (PathGeometry)dictionary["docs"],
                 MenuText="Docs",
                 BorderStroke="#3F3990",
                 Fill="#CFCEEC"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["camera"],
                    MenuText="Camera",
                    BorderStroke="#2C5A71",
                    Fill="#C5E7F8"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["gallery"],
                    MenuText="Gallery",
                    BorderStroke="#EA2140",
                    Fill="#F3BEBE"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["audio"],
                    MenuText="Audio",
                    BorderStroke="#E67E00",
                    Fill="#F7D5AC"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["location"],
                    MenuText="Location",
                    BorderStroke="#28C58F",
                    Fill="#E3F5EF"
                },
                new MoreOptionsMenu()
                {
                    Icon=(PathGeometry)dictionary["contact"],
                    MenuText="Contact",
                    BorderStroke="#0093E0",
                    Fill="#DDF1FB"
                }
            };
            OnPropertyChanged("AttachmentOptionsMenuList");
        }
        #endregion


        public void OpenSearchBox()
        {
            IsSearchBoxOpen = true;
        }
        public void ClearSearchBox()
        {
            if (!string.IsNullOrEmpty(SearchText))
                SearchText = string.Empty;
            else
                CloseSearchBox();
        }
        public void CloseSearchBox() => IsSearchBoxOpen = false;

        public void Search()
        {
            //To avoid re searching same text again
            if ((string.IsNullOrEmpty(LastSearchText) && string.IsNullOrEmpty(SearchText)) || string.Equals(LastSearchText, SearchText))
                return;

            //If searchbox is empty or chats is null pr chat cound less than 0
            if (string.IsNullOrEmpty(SearchText) || Chats == null || Chats.Count <= 0)
            {
                FilteredChats = new ObservableCollection<ChatListData>(Chats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredChats");

                FilteredPinnedChats = new ObservableCollection<ChatListData>(PinnedChats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredPinnedChats");
                //Update Last search Text
                LastSearchText = SearchText;

                return;
            }

            //Now, to find all chats that contain the text in our search box

            //if that chat is in Normal Unpinned Chat list find there...


            FilteredChats = new ObservableCollection<ChatListData>(
                Chats.Where(
                    chat => chat.ContactName.ToLower().Contains(SearchText) //if ContactName Contains SearchText then add it in filtered chat list
                    ||
                    chat.Message != null && chat.Message.ToLower().Contains(SearchText) //if Message Contains SearchText then add it in filtered chat list
                    ));
            OnPropertyChanged("FilteredChats");

            //else if not found in Normal Unpinned Chat list, find in pinned chats list
            FilteredPinnedChats = new ObservableCollection<ChatListData>(
            PinnedChats.Where(
                pinnedchat => pinnedchat.ContactName.ToLower().Contains(SearchText) //if ContactName Contains SearchText then add it in filtered chat list
                ||
                pinnedchat.Message != null && pinnedchat.Message.ToLower().Contains(SearchText) //if Message Contains SearchText then add it in filtered chat list
                ));

            OnPropertyChanged("FilteredPinnedChats");

            //Update Last search Text
            LastSearchText = SearchText;
        }
        #endregion

        #region Commands

        protected ICommand _windowsMoreOptionsCommand;
        public ICommand WindowsMoreOptionsCommand
        {
            get
            {
                if (_windowsMoreOptionsCommand == null)
                    _windowsMoreOptionsCommand = new CommandViewModel(WindowMoreOptionsMenu);
                return _windowsMoreOptionsCommand;
            }
            set
            {
                _windowsMoreOptionsCommand = value;
            }
        }

        protected ICommand _conversationScreenMoreOptionsCommand;
        public ICommand ConversationScreenMoreOptionsCommand
        {
            get
            {
                if (_conversationScreenMoreOptionsCommand == null)
                    _conversationScreenMoreOptionsCommand = new CommandViewModel(ConversationScreenMoreOptionsMenu);
                return _conversationScreenMoreOptionsCommand;
            }
            set
            {
                _conversationScreenMoreOptionsCommand = value;
            }
        }
        protected ICommand _attachmentOptionsCommand;
        public ICommand AttachmentOptionsCommand
        {
            get
            {
                if (_attachmentOptionsCommand == null)
                    _attachmentOptionsCommand = new CommandViewModel(AttachmentOptionsMenu);
                return _attachmentOptionsCommand;
            }
            set
            {
                _attachmentOptionsCommand = value;
            }
        }
        /// <summary>
        /// Search Command
        /// </summary>
        protected ICommand _openSearchCommand;
        public ICommand OpenSearchCommand
        {
            get
            {
                if (_openSearchCommand == null)
                    _openSearchCommand = new CommandViewModel(OpenSearchBox);
                return _openSearchCommand;
            }
            set
            {
                _openSearchCommand = value;
            }
        }

        /// <summary>
        /// Clear Search Command
        /// </summary>
        protected ICommand _clearSearchCommand;
        public ICommand ClearSearchCommand
        {
            get
            {
                if (_clearSearchCommand == null)
                    _clearSearchCommand = new CommandViewModel(ClearSearchBox);
                return _clearSearchCommand;
            }
            set
            {
                _clearSearchCommand = value;
            }
        }

        /// <summary>
        /// Close Search Command
        /// </summary>
        protected ICommand _closeSearchCommand;
        public ICommand CloseSearchCommand
        {
            get
            {
                if (_closeSearchCommand == null)
                    _closeSearchCommand = new CommandViewModel(CloseSearchBox);
                return _closeSearchCommand;
            }
            set
            {
                _closeSearchCommand = value;
            }
        }

        /// <summary>
        /// Search Command
        /// </summary>
        protected ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new CommandViewModel(Search);
                return _searchCommand;
            }
            set
            {
                _searchCommand = value;
            }
        }
        #endregion
        #endregion

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
        public ObservableCollection<ChatListData> mChats;
        public ObservableCollection<ChatListData> mPinnedChats;
        public ObservableCollection<ChatListData> Chats
        {
            get => mChats;
            set
            {
                //To Change the list
                if (mChats == value)
                    return;

                //To Update the list
                mChats = value;

                //Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatListData>(mChats);
                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
            }
        }
        public ObservableCollection<ChatListData> PinnedChats
        {
            get => mPinnedChats;
            set
            {
                //To Change the list
                if (mPinnedChats == value)
                    return;

                //To Update the list
                mPinnedChats = value;

                //Updating filtered chats to match
                FilteredPinnedChats = new ObservableCollection<ChatListData>(mPinnedChats);
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredPinnedChats");
            }
        }

        protected ObservableCollection<ChatListData> _archivedChats;
        public ObservableCollection<ChatListData> ArchivedChats
        {
            get => _archivedChats; set
            {
                _archivedChats = value;
                OnPropertyChanged();
            }
        }

        //Filtering Chats & Pinned Chats
        public ObservableCollection<ChatListData> FilteredChats { get; set; }
        public ObservableCollection<ChatListData> FilteredPinnedChats { get; set; }

        protected int ChatPosition { get; set; }
        #endregion

        #region Logics
        void LoadChats()
        {
            //Loading data from Database
            if (Chats == null)
                Chats = new ObservableCollection<ChatListData>();

            //Opening Sql Connection
            connection.Open();

            //Temporary Collection
            ObservableCollection<ChatListData> temp = new ObservableCollection<ChatListData>();

            using (SqlCommand command = new SqlCommand("select * from contacts p left join (select a.*, row_number() over(partition by a.contactname order by a.id desc) as seqnum from conversations a ) a on a.ContactName = p.contactname and a.seqnum = 1 order by a.Id desc", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //To avoid duplication
                    string lastItem = string.Empty;
                    string newItem = string.Empty;

                    while (reader.Read())
                    {
                        string time = string.Empty;
                        string lastmessage = string.Empty;

                        //if the last message is received from sender than update time & lastmessage variables..
                        if (!string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()))
                        {
                            time = Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("ddd hh:mm tt");
                            lastmessage = reader["ReceivedMsgs"].ToString();
                        }

                        //else if we have sent last message then update accordingly...
                        if (!string.IsNullOrEmpty(reader["MsgSentOn"].ToString()))
                        {
                            time = Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("ddd hh:mm tt");
                            lastmessage = reader["SentMsgs"].ToString();
                        }

                        //if the chat is new or we are starting new conversation which means there will be no previous sent or received msgs in that case..
                        //show 'Start new conversation' message...
                        if (string.IsNullOrEmpty(lastmessage))
                            lastmessage = "Start new conversation";

                        //Update data in model...
                        ChatListData chat = new ChatListData()
                        {
                            ContactPhoto = (byte[])reader["photo"],
                            ContactName = reader["contactname"].ToString(),
                            Message = lastmessage,
                            LastMessageTime = time
                        };

                        //Update 
                        newItem = reader["contactname"].ToString();

                        //if last added chat contact is not same as new one then only add..
                        if (lastItem != newItem)
                            temp.Add(chat);

                        lastItem = newItem;
                    }
                }
            }
            //Transfer data
            Chats = temp;

            //Update
            OnPropertyChanged("Chats");
                //Chats = new ObservableCollection<ChatListData>()
                //{
                //    new ChatListData{
                //    ContactName = "Billy",
                //    ContactPhoto = new Uri("/assets/6.jpg", UriKind.RelativeOrAbsolute),
                //    Message="Hey, What's Up?",
                //    LastMessageTime="Tue, 12:58 PM",
                //    ChatIsSelected=true
                //    },
                //    new ChatListData{
                //    ContactName = "Mike",
                //    ContactPhoto = new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                //    Message="Check the mail.",
                //    LastMessageTime="Mon, 10:07 AM"
                //    },
                //    new ChatListData{
                //    ContactName = "Steve",
                //    ContactPhoto = new Uri("/assets/7.png", UriKind.RelativeOrAbsolute),
                //    Message="Yes, we had fun.",
                //    LastMessageTime="Tue, 08:10 AM"
                //    },
                //    new ChatListData{
                //    ContactName = "John",
                //    ContactPhoto = new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                //    Message="What about you?",
                //    LastMessageTime="Tue, 01:00 PM"
                //    }
                //};
                OnPropertyChanged();
        }
        #endregion

        #region Commands
        //To get the ContactName of selected chat so that we can open corresponding conversation
        protected ICommand _getSelectedChatCommand;
        public ICommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                //getting contactname from selected chat
                ContactName = v.ContactName;
                OnPropertyChanged("ContactName");

                //getting contactphoto from selected chat
                ContactPhoto = v.ContactPhoto;
                OnPropertyChanged("ContactPhoto");

                LoadChatConversation(v);
            }
        });

        //To Pin Chat on Pin Button Click
        protected ICommand _pinChatCommand;
        public ICommand PinChatCommand => _pinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredPinnedChats.Contains(v))
                {
                    //Add selected chat to pin chat
                    PinnedChats.Add(v);
                    FilteredPinnedChats.Add(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = true;


                    //Remove selected chat from all chats / unpinned chats
                    //Store position of chat before pinning so that when we unpin or unarchive we get it on same original position...
                    ChatPosition = Chats.IndexOf(v);
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");


                    //Remember, Chat will be removed from Pinned List when Archived.. and Vice Versa...
                    //Fixed
                    if (ArchivedChats != null)
                    {
                        if (ArchivedChats.Contains(v))
                        {
                            ArchivedChats.Remove(v);
                            v.ChatIsArchived = false;
                        }
                    }
                }
            }
        });

        //To Pin Chat on Pin Button Click
        protected ICommand _unPinChatCommand;
        public ICommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v))
                {
                    //Add selected chat to Normal Unpinned chat list
                    Chats.Add(v);
                    FilteredChats.Add(v);

                    //Restore position of chat before pinning so that when we unpin or unarchive we get it on same original position...
                    Chats.Move(Chats.Count-1, ChatPosition);
                    FilteredChats.Move(Chats.Count-1, ChatPosition);

                    //Update
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");

                    //Remove selected pinned chats list
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = false;
                }
            }
        });

        /// <summary>
        /// Archive Chat Command
        /// </summary>
        protected ICommand _archiveChatCommand;
        public ICommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!ArchivedChats.Contains(v))
                {
                    //Remember, Chat will be removed from Pinned List when Archived.. and Vice Versa...                    

                    //Add Chat in Archive List
                    ArchivedChats.Add(v);
                    v.ChatIsArchived = true;
                    v.ChatIsPinned = false;

                    //Remove Chat from Pinned & Unpinned Chat List
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);

                    //Update Lists
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    OnPropertyChanged("ArchivedChats");
                }
            }
        });
        /// <summary>
        /// UnArchive Chat Command
        /// </summary>
        protected ICommand _UnArchiveChatCommand;
        public ICommand UnArchiveChatCommand => _UnArchiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v) && !Chats.Contains(v))
                {
                    Chats.Add(v);
                    FilteredChats.Add(v);
                }
                ArchivedChats.Remove(v);
                v.ChatIsArchived = false;
                v.ChatIsPinned = false;


                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
                OnPropertyChanged("ArchivedChats");
            }
        });

        #endregion

        #endregion

        #region Conversations

        #region Properties
        protected bool _isConversationSearchBoxOpen;
        public bool IsConversationSearchBoxOpen
        {
            get => _isConversationSearchBoxOpen;
            set
            {
                if (_isConversationSearchBoxOpen == value)
                    return;

                _isConversationSearchBoxOpen = value;


                if (_isConversationSearchBoxOpen == false)
                    //Clear Search Box
                    SearchConversationText = string.Empty;
                OnPropertyChanged("IsConversationSearchBoxOpen");
                OnPropertyChanged("SearchConversationText");
            }
        }

        protected ObservableCollection<ChatConversation> mConversations;
        public ObservableCollection<ChatConversation> Conversations
        {
            get => mConversations;
            set
            {
                //To Change the list
                if (mConversations == value)
                    return;

                //To Update the list
                mConversations = value;

                //Updating filtered chats to match
                FilteredConversations = new ObservableCollection<ChatConversation>(mConversations);
                OnPropertyChanged("Conversations");
                OnPropertyChanged("FilteredConversations");
            }
        }

        /// <summary>
        /// Filter Conversation
        /// </summary>
        public ObservableCollection<ChatConversation> FilteredConversations { get; set; }

        //We will use this message text to transfer the send message value to our conversation body
        protected string messageText;
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        protected string LastSearchConversationText;
        protected string mSearchConversationText;
        public string SearchConversationText
        {
            get => mSearchConversationText;
            set
            {

                //checked if value is different
                if (mSearchConversationText == value)
                    return;

                //Update Value
                mSearchConversationText = value;

                //if search text is empty restore messages
                if (string.IsNullOrEmpty(SearchConversationText))
                    SearchInConversation();
            }
        }

        public bool FocusMessageBox { get; set; }
        public bool IsThisAReplyMessage { get; set; }
        public string MessageToReplyText { get; set; }
        #endregion

        #region Logics
        protected bool _isSearchConversationBoxOpen;
        public bool IsSearchConversationBoxOpen
        {
            get => _isSearchConversationBoxOpen;
            set
            {
                if (_isSearchConversationBoxOpen == value)
                    return;

                _isSearchConversationBoxOpen = value;


                if (_isSearchConversationBoxOpen == false)
                    //Clear Search Box
                    SearchConversationText = string.Empty;
                OnPropertyChanged("IsSearchConversationBoxOpen");
                OnPropertyChanged("SearchConversationText");
            }
        }
        public void OpenConversationSearchBox()
        {
            IsSearchConversationBoxOpen = true;
        }
        public void ClearConversationSearchBox()
        {
            if (!string.IsNullOrEmpty(SearchConversationText))
                SearchConversationText = string.Empty;
            else
                CloseConversationSearchBox();
        }
        public void CloseConversationSearchBox() => IsSearchConversationBoxOpen = false;

        void LoadChatConversation(ChatListData chat)
        {
            //Since the conversation data is big i will be using database containing contact details & conversations instead...
            //        i will provide you with database file link in Description
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();
            Conversations.Clear();
            FilteredConversations.Clear();
            using (SqlCommand com = new SqlCommand("select * from conversations where ContactName=@ContactName", connection))
            {
                com.Parameters.AddWithValue("@ContactName", chat.ContactName);
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // to set date format 
                        // Like this Jun 15, 01:15 PM = MMM dd, hh:mm tt
                        // We used Ternary Operator ('?:') for condition here
                        //this is how it works:
                        //Condition ? True : False

                        //Like if(condition)
                        //        true;
                        //     else
                        //        false;

                        string MsgReceivedOn = !string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()) ?
                            Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        string MsgSentOn = !string.IsNullOrEmpty(reader["MsgSentOn"].ToString()) ?
    Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        var conversation = new ChatConversation()
                        {
                            ContactName = reader["ContactName"].ToString(),
                            ReceivedMessage = reader["ReceivedMsgs"].ToString(),
                            MsgReceivedOn = MsgReceivedOn,
                            SentMessage = reader["SentMsgs"].ToString(),
                            MsgSentOn = MsgSentOn,
                            IsMessageReceived = string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? false : true
                        };
                        Conversations.Add(conversation);
                        OnPropertyChanged("Conversations");
                        FilteredConversations.Add(conversation);
                        OnPropertyChanged("FilteredConversations");

                        chat.Message = !string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? reader["ReceivedMsgs"].ToString() : reader["SentMsgs"].ToString();
                    }
                }
            }
            //Reset reply message text when the new chat is fetched
            MessageToReplyText = string.Empty;
            OnPropertyChanged("MessageToReplyText");
        }

        void SearchInConversation()
        {
            //To avoid re searching same text again
            if ((string.IsNullOrEmpty(LastSearchConversationText) && string.IsNullOrEmpty(SearchConversationText)) || string.Equals(LastSearchConversationText, SearchConversationText))
                return;

            //If searchbox is empty or Conversations is null pr chat cound less than 0
            if (string.IsNullOrEmpty(SearchConversationText) || Conversations == null || Conversations.Count <= 0)
            {
                FilteredConversations = new ObservableCollection<ChatConversation>(Conversations ?? Enumerable.Empty<ChatConversation>());
                OnPropertyChanged("FilteredConversations");

                //Update Last search Text
                LastSearchConversationText = SearchConversationText;

                return;
            }

            //Now, to find all Conversations that contain the text in our search box

            FilteredConversations = new ObservableCollection<ChatConversation>(
                Conversations.Where(chat => chat.ReceivedMessage.ToLower().Contains(SearchConversationText) || chat.SentMessage.ToLower().Contains(SearchConversationText)));
            OnPropertyChanged("FilteredConversations");

            //Update Last search Text
            LastSearchConversationText = SearchConversationText;
        }

        public void CancelReply()
        {
            IsThisAReplyMessage = false;
            //Reset Reply Message Text
            MessageToReplyText = string.Empty;
            OnPropertyChanged("MessageToReplyText");
        }

        public void SendMessage()
        {
            //Send message only when the textbox is not empty..
            if (!string.IsNullOrEmpty(MessageText))
            {
                var conversation = new ChatConversation()
                {
                    ReceivedMessage = MessageToReplyText,
                    SentMessage = MessageText,
                    MsgSentOn = DateTime.Now.ToString("MMM dd, hh:mm tt"),
                    MessageContainsReply = IsThisAReplyMessage
                };

                //My badd...
                //Add message to converstion list
                FilteredConversations.Add(conversation);
                Conversations.Add(conversation);

                UpdateChatAndMoveUp(Chats, conversation);
                UpdateChatAndMoveUp(PinnedChats, conversation);
                UpdateChatAndMoveUp(FilteredChats, conversation);
                UpdateChatAndMoveUp(FilteredPinnedChats, conversation);
                UpdateChatAndMoveUp(ArchivedChats, conversation);

                //Clear Message properties and textbox when message is sent
                MessageText = string.Empty;
                IsThisAReplyMessage = false;
                MessageToReplyText = string.Empty;               

                //Update
                OnPropertyChanged("FilteredConversations");
                OnPropertyChanged("Conversations");
                OnPropertyChanged("MessageText");
                OnPropertyChanged("IsThisAReplyMessage");
                OnPropertyChanged("MessageToReplyText");
            }
        }

        //Move the chat contact on top when new message is sent or received
        protected void UpdateChatAndMoveUp(ObservableCollection<ChatListData> chatList, ChatConversation conversation)
        {
            //Check if the message sent is to the selected contact or not...
            var chat = chatList.FirstOrDefault(x => x.ContactName == ContactName);

            //if found.. then..
            if (chat != null)
            {
                //Update Contact Chat Last Message and Message Time..
                chat.Message = MessageText;
                chat.LastMessageTime = conversation.MsgSentOn;

                //Move Chat on top when new message is received/sent...
                chatList.Move(chatList.IndexOf(chat), 0);

                //Update Collections
                OnPropertyChanged("Chats");
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredChats");
                OnPropertyChanged("FilteredPinnedChats");
                OnPropertyChanged("ArchivedChats");
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Search Command
        /// </summary>
        protected ICommand _openConversationSearchCommand;
        public ICommand OpenConversationSearchCommand
        {
            get
            {
                if (_openConversationSearchCommand == null)
                    _openConversationSearchCommand = new CommandViewModel(OpenConversationSearchBox);
                return _openConversationSearchCommand;
            }
            set
            {
                _openConversationSearchCommand = value;
            }
        }

        /// <summary>
        /// Clear Search Command
        /// </summary>
        protected ICommand _clearConversationSearchCommand;
        public ICommand ClearConversationSearchCommand
        {
            get
            {
                if (_clearConversationSearchCommand == null)
                    _clearConversationSearchCommand = new CommandViewModel(ClearConversationSearchBox);
                return _clearConversationSearchCommand;
            }
            set
            {
                _clearConversationSearchCommand = value;
            }
        }

        /// <summary>
        /// Close Search Command
        /// </summary>
        protected ICommand _closeConversationSearchCommand;
        public ICommand CloseConversationSearchCommand
        {
            get
            {
                if (_closeConversationSearchCommand == null)
                    _closeConversationSearchCommand = new CommandViewModel(CloseConversationSearchBox);
                return _closeConversationSearchCommand;
            }
            set
            {
                _closeConversationSearchCommand = value;
            }
        }

        protected ICommand _searchConversationCommand;
        public ICommand SearchConversationCommand
        {
            get
            {
                if (_searchConversationCommand == null)
                    _searchConversationCommand = new CommandViewModel(SearchInConversation);
                return _searchConversationCommand;
            }
            set
            {
                _searchConversationCommand = value;
            }
        }

        protected ICommand _replyCommand;
        public ICommand ReplyCommand => _replyCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatConversation v)
            {
                //if replying sender's message
                if (v.IsMessageReceived)
                    MessageToReplyText = v.ReceivedMessage;
                //if replying own message
                else
                    MessageToReplyText = v.SentMessage;

                //update
                OnPropertyChanged("MessageToReplyText");

                //Set focus on Message Box when user clicks reply button
                FocusMessageBox = true;
                OnPropertyChanged("FocusMessageBox");

                //Flag this message as reply message
                IsThisAReplyMessage = true;
                OnPropertyChanged("IsThisAReplyMessage");
            }
        });

        protected ICommand _cancelReplyCommand;
        public ICommand CancelReplyCommand
        {
            get
            {
                if (_cancelReplyCommand == null)
                    _cancelReplyCommand = new CommandViewModel(CancelReply);
                return _cancelReplyCommand;
            }
            set
            {
                _cancelReplyCommand = value;
            }
        }

        protected ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (_sendMessageCommand == null)
                    _sendMessageCommand = new CommandViewModel(SendMessage);
                return _sendMessageCommand;
            }
            set
            {
                _sendMessageCommand = value;
            }
        }
        #endregion
        #endregion

        #region ContactInfo
        #region Properties
        protected bool _IsContactInfoOpen;
        public bool IsContactInfoOpen
        {
            get => _IsContactInfoOpen;
            set
            {
                _IsContactInfoOpen = value;
                OnPropertyChanged("IsContactInfoOpen");
            }
        }
        #endregion

        #region Logics
        public void OpenContactInfo() => IsContactInfoOpen = true;
        public void CloseContactInfo() => IsContactInfoOpen = false;
        #endregion

        #region Commands
        /// <summary>
        /// Open ContactInfo Command
        /// </summary>
        protected ICommand _openContactInfoCommand;
        public ICommand OpenContactinfoCommand
        {
            get
            {
                if (_openContactInfoCommand == null)
                    _openContactInfoCommand = new CommandViewModel(OpenContactInfo);
                return _openContactInfoCommand;
            }
            set
            {
                _openContactInfoCommand = value;
            }
        }

        /// <summary>
        /// Open ContactInfo Command
        /// </summary>
        protected ICommand _closeontactInfoCommand;
        public ICommand CloseContactinfoCommand
        {
            get
            {
                if (_closeontactInfoCommand == null)
                    _closeontactInfoCommand = new CommandViewModel(CloseContactInfo);
                return _closeontactInfoCommand;
            }
            set
            {
                _closeontactInfoCommand = value;
            }
        }
        #endregion
        #endregion

        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Projects\ChatApp\Database\Database1.mdf;Integrated Security=True");
        public ViewModel()
        {
            LoadStatusThumbs();
            LoadChats();
            PinnedChats = new ObservableCollection<ChatListData>();
            ArchivedChats = new ObservableCollection<ChatListData>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}