using Statistics.Survey.Analysis.BLL.Base;
//using Statistics.Survey.Analysis.Domain.Survey.Request;
using Statistics.Survey.Analysis.Domain.UserManagment.Request;
using Statistics.Survey.Analysis.Domain.UserManagment.Response;
using Statistics.Survey.Analysis.Repository.UserManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statistics.Survey.Analysis.BLL.UserManagement
{
    public class UserManagementApplication : BaseApplication
    {
        #region user authentication
        public UserAuthorizationResponse AuthenticateUser(UserAuthorizationDomain oUserAuthorizationDomain)
        {
            UserAuthorizationResponse oUserAuthorizationResponse = UserManagementRepository.AuthenticateUser(oUserAuthorizationDomain);

            try
            {
                // Generate an Encrypted Token and Pass in Message, if OTP Successfully Validated 
                // This new Token is passed for Security Purposes for APP, so that OTP Screen cannot be ByPassed
                if (oUserAuthorizationResponse.isUserAuthenticated)
                    oUserAuthorizationResponse.message = "User authenticated successfully.";
                else
                    oUserAuthorizationResponse.message = "User name or password is incorrect.";
            }
            catch (Exception ex)
            {
                oUserAuthorizationResponse.isUserAuthenticated = false;
                oUserAuthorizationResponse.message = "Error authenticating user : " + ex.Message;
            }

            return oUserAuthorizationResponse;
        }
        #endregion


    }
}
