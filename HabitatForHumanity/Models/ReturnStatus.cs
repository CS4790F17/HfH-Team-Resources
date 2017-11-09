using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    public class ReturnStatus
    {
        public int errorCode { get; set; }
        public object data { get; set; }
        public string errorMessage { get; set; }


        public enum ErrorCodes
        {
            All_CLEAR,
            COULD_NOT_CONNECT_TO_DATABASE,
        };


        /// <summary>
        /// Tries to safely parse a user out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a user in it's data property.</param>
        /// <param name="result">The object reference in which to store the parsed user.</param>
        /// <returns>True if it was capable of parsing a User, false otherwise</returns>
        public static bool tryParseUser(ReturnStatus st, out User result)
        {

            result = new User();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (User)st.data;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //TODO: log e
                //could not parse user
                return false;

            }
        }
    }
}