using System;
using System.Collections.Generic;
using System.Linq;
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


        public enum ErrorCodes
        {
            All_CLEAR,
            COULD_NOT_CONNECT_TO_DATABASE,
            COULD_NOT_AUTHENTICATE_USER,
            COULD_NOT_FIND_SINGLE_USER,
            COULD_NOT_FIND_EMAIL,
            USER_PASSWORD_CANNOT_BE_NULL,
            COULD_NOT_DELETE,
            COULD_NOT_FIND_SINGLE_TIMESHEET,
            ID_CANNOT_BE_NULL,
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

        public static bool tryParseUserList(ReturnStatus st, out List<User> result)
        {

            result = new List<User>();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (List<User>)st.data;
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

        /// <summary>
        /// Tries to safely parse a PunchInVM out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object </param>
        /// <param name="punchIn">Uninitialized PunchInVM object</param>
        /// <returns></returns>
        public static bool tryParsePunchInVM(ReturnStatus st, out PunchInVM punchIn)
        {
            punchIn = new PunchInVM();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    punchIn = (PunchInVM)st.data;
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

        /// <summary>
        /// Tries to safely parse a timesheet out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a timesheet in it's data property.</param>
        /// <param name="result">The object reference in which to store the parsed timesheet.</param>
        /// <returns>True if it was capable of parsing a TimeSheet, false otherwise</returns>
        public static bool tryParseTimeSheet(ReturnStatus st, out TimeSheet result)
        {

            result = new TimeSheet();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (TimeSheet)st.data;
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