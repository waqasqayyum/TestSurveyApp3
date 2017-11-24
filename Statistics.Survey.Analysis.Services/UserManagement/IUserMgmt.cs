using Statistics.Survey.Analysis.Domain.UserManagment.Domain;
using Statistics.Survey.Analysis.Domain.UserManagment.Request;
using Statistics.Survey.Analysis.Domain.UserManagment.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Statistics.Survey.Analysis.Services.UserManagement
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserMgmt" in both code and config file together.
    [ServiceContract]
    public interface IUserMgmt
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "GetUserDetails")]
        UserDetailsDomain GetUserDetails();

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "AuthenticateUser")]
        UserAuthorizationResponse AuthenticateUser(UserAuthorizationDomain oUserLoginRequestDomain);

        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "SaveSurvey")]
        UserAuthorizationResponse SaveSurvey(SurveyOne oSurveyOne);
    }
}
