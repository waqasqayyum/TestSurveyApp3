using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Statistics.Survey.Analysis.Domain.UserManagment.Request
{
    [DataContract]
    public class SurveyOne
    {
         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string surveyName { get; set; }

         //[DataMember(IsRequired = true, EmitDefaultValue = true)]
         //public string dateTime { get; set; }

         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string province { get; set; }

         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string city { get; set; }

         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string personName { get; set; }

         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string gender { get; set; }

         [DataMember(IsRequired = true, EmitDefaultValue = true)]
         public string age { get; set; }
    }
}