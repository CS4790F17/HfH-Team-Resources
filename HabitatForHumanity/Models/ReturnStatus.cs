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
        /// <summary>
        /// String to be logged
        /// </summary>
        public string errorMessage { get; set; }
        /// <summary>
        /// String to show the user
        /// </summary>
        public string userErrorMsg { get; set; }

        /// <summary>
        /// An enumerator of error codes used by the ReturnStatus class object.
        /// </summary>
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
        /// Gets a name associated with the supplied error code.
        /// </summary>
        /// <param name="errorCode">0 based index</param>
        /// <returns>Name of the error code</returns>
        public static string getErrorCodeName(int errorCode)
        {
            return Enum.GetName(typeof(ErrorCodes), errorCode);
        }

        /// <summary>
        /// Tries to safely parse a user out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a user in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed user.</param>
        /// <returns>true if it was capable of parsing a User, false otherwise</returns>
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

        /// <summary>
        /// Tries to safely parse a user list out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a user list in its data property.</param>
        /// <param name="result">The unitialized object reference in which to store the parsed List of User.</param>
        /// <returns>true if it was capable of parsing a User, false otherwise</returns>
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
        /// <returns>true if it was capable of parsing a User, false otherwise</returns>
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
        /// <param name="st">ReturnStatus object that potentially contains a timesheet in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed timesheet.</param>
        /// <returns>true if it was capable of parsing a TimeSheet, false otherwise</returns>
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

        /// <summary>
        /// Tries to safely parse a timesheet list out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a timesheet list in its data property.</param>
        /// <param name="result">The uninitialized object reference in which to store the parsed timesheet list.</param>
        /// <returns>true if it was capable of parsing a List of TimeSheet, false otherwise</returns>
        public static bool tryParseTimeSheetList(ReturnStatus st, out List<TimeSheet> result)
        {

            result = new List<TimeSheet>();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (List<TimeSheet>)st.data;
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
        /// Tries to safely parse an Organization out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains an Organization in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed Organization.</param>
        /// <returns>true if it was capable of parsing an Organization, false otherwise</returns>
        public static bool tryParseOrganization(ReturnStatus st, out Organization result)
        {

            result = new Organization();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (Organization)st.data;
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
        /// Tries to safely parse a List of Organizations out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a List of Organization in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed List of Organizations.</param>
        /// <returns>true if it was capable of parsing a List of Organization, false otherwise</returns>
        public static bool tryParseOrganizationList(ReturnStatus st, out List<Organization> result)
        {

            result = new List<Organization>();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (List<Organization>)st.data;
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
        /// Tries to safely parse a Project out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a Project in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed Project.</param>
        /// <returns>true if it was capable of parsing a Project, false otherwise</returns>
        public static bool tryParseProject(ReturnStatus st, out Project result)
        {

            result = new Project();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (Project)st.data;
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
        /// Tries to safely parse a List of Projects out of a ReturnStatus object
        /// </summary>
        /// <param name="st">ReturnStatus object that potentially contains a List of Projects in its data property.</param>
        /// <param name="result">The object reference in which to store the parsed List of Projects.</param>
        /// <returns>true if it was capable of parsing a List of Projects, false otherwise</returns>
        public static bool tryParseProjectList(ReturnStatus st, out List<Project> result)
        {

            result = new List<Project>();
            try
            {
                if (st.errorCode == 0 && st.data != null)
                {
                    result = (List<Project>)st.data;
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