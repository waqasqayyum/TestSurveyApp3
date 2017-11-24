using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Statistics.Survey.Analysis.Domain.UserManagment.Domain
{
    [DataContract]
    public class UserDetailsDomain
    {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string userName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public bool isUserAuthenticated { get; set; }
    }
}