using System;
using System.Collections.Generic;
using System.Linq;
//   ReturnStatus Model for Error Handling
//  The purpose of this class is to make passing data around safer and more reliable.
//  This class contains methods used to extract Habitat for Humanity model objects out
//  of a data object. 
//
using System.Web;
using HabitatForHumanity.Models;
using HabitatForHumanity.ViewModels;

namespace HabitatForHumanity.Models
{
    public class ReturnStatus
    {
        public int errorCode { get; set; }
        public object data { get; set; }
        public string errorMessage { get; set; }

        public static int ALL_CLEAR { get; } = 0;
        public static int COULD_NOT_CONNECT_TO_DATABASE = 1;
        public static int COULD_NOT_UPDATE_DATABASE = 2;
        public static int FAIL_ON_INSERT = 3;
 


   }
}