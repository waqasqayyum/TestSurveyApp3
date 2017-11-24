using Statistics.Survey.Analysis.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Statistics.Survey.Analysis.Domain.UserManagment.Response
{
    [DataContract]
    public class UserAuthorizationResponse : ResponseBaseDomain
    {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public bool isUserAuthenticated { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string token { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string message { get; set; }
    }
}