using Statistics.Survey.Analysis.BLL.UserManagement;
using Statistics.Survey.Analysis.Domain.UserManagment.Domain;
using Statistics.Survey.Analysis.Domain.UserManagment.Request;
using Statistics.Survey.Analysis.Domain.UserManagment.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;
using Utilities.Logger;


namespace Statistics.Survey.Analysis.Services.UserManagement
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserMgmt" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UserMgmt.svc or UserMgmt.svc.cs at the Solution Explorer and start debugging.
    public class UserMgmt : IUserMgmt
    {
        public UserDetailsDomain GetUserDetails()
        {
            LogEngine.Default.Log("[GetUserDetails Start =" + DateTime.Now + "]", null);
            string jsonResponse = string.Empty;
            string userName = "";

            UserDetailsDomain oUserDetailsDomain = new UserDetailsDomain();
            oUserDetailsDomain.isUserAuthenticated = false;

            string exception = "";

            try
            {
                if (userName.ToUpper() == "WAQAS")
                    oUserDetailsDomain.isUserAuthenticated = true;
                else
                    oUserDetailsDomain.isUserAuthenticated = false;
            }
            catch (Exception ex)
            {
                oUserDetailsDomain.isUserAuthenticated = false;

                exception = ex.Message;
                LogEngine.Default.Log("[User_Number=" + userName + "][Exception=" + exception + "], Exception", null);
            }
            return oUserDetailsDomain;
        }

        public UserAuthorizationResponse AuthenticateUser(UserAuthorizationDomain oUserAuthorizationDomain)
        {
            LogEngine.Default.Log("[AuthenticateUser Start =" + DateTime.Now + "]", null);
            string jsonResponse = string.Empty;

            UserAuthorizationResponse oUserAuthorizationResponse = new UserAuthorizationResponse();
            oUserAuthorizationResponse.isUserAuthenticated = false;

            string exception = "";

            try
            {
                /*
                if (oUserAuthorizationDomain.userName.ToUpper() == "WAQAS")
                    oUserAuthorizationResponse.isUserAuthenticated = true;
                else
                    oUserAuthorizationResponse.isUserAuthenticated = false;
                */

                UserManagementApplication oUserManagementApplication = new UserManagementApplication();
                oUserAuthorizationResponse = oUserManagementApplication.AuthenticateUser(oUserAuthorizationDomain);

                var deserializer = new JavaScriptSerializer();
                oUserAuthorizationResponse = deserializer.Deserialize<UserAuthorizationResponse>(oUserAuthorizationResponse.isUserAuthenticated.ToString());
            }
            catch (Exception ex)
            {
                oUserAuthorizationResponse.isUserAuthenticated = false;

                exception = ex.Message;
                LogEngine.Default.Log("[User_Number=" + oUserAuthorizationDomain.userName + "][Exception=" + exception + "], Exception", null);
            }
            return oUserAuthorizationResponse;
        }

        public UserAuthorizationResponse SaveSurvey(SurveyOne oSurveyOne)
        {
            LogEngine.Default.Log("[SaveSurvey Start =" + DateTime.Now + "]", null);
            string jsonResponse = string.Empty;

            UserAuthorizationResponse oUserLoginResponseDomain = new UserAuthorizationResponse();
            oUserLoginResponseDomain.isUserAuthenticated = false;

            string exception = "";

            try
            {
                LogEngine.Default.Log("[SaveSurvey Object =" + DateTime.Now + "]", oSurveyOne);

                oUserLoginResponseDomain.isUserAuthenticated = true;

                var deserializer = new JavaScriptSerializer();
                oUserLoginResponseDomain = deserializer.Deserialize<UserAuthorizationResponse>(oUserLoginResponseDomain.isUserAuthenticated.ToString());
            }
            catch (Exception ex)
            {
                oUserLoginResponseDomain.isUserAuthenticated = false;

                exception = ex.Message;
                LogEngine.Default.Log("[User_Number=" + "oUserLoginRequestDomain.userName" + "][Exception=" + exception + "], Exception", null);
            }
            return oUserLoginResponseDomain;
        }
    }
}
