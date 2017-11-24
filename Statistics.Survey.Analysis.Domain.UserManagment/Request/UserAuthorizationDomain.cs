using Statistics.Survey.Analysis.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Statistics.Survey.Analysis.Domain.UserManagment.Request
{
    [DataContract]
    public class UserAuthorizationDomain : RequestBaseDomain
    {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string userName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string userPassword { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string token { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string imei { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string iccid { get; set; }
    }
}