using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class StatusDataModel
    {
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public Uri StatusImage { get; set; }

        //If we want to add our status
        public bool IsMeAddStatus { get; set; }

        /// <summary>
        /// We will be covering in one of our upcoming videos
        /// To-Do: Status Message 
        /// </summary>
        //public string StatusMessage { get; set; }
    }
}
