using MySql.Data.MySqlClient;
using Statistics.Survey.Analysis.Domain.UserManagment.Request;
using Statistics.Survey.Analysis.Domain.UserManagment.Response;
using Statistics.Survey.Analysis.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Statistics.Survey.Analysis.Repository.UserManagment
{
    public class UserManagementRepository : BaseRepository
    {
        #region "Validate OTP"

        public static UserAuthorizationResponse AuthenticateUser(UserAuthorizationDomain oUserAuthorizationDomain)
        {
            MySqlCommand command = null;
            UserAuthorizationResponse oUserAuthorizationResponse = new UserAuthorizationResponse();

            try
            {
                #region "Validate User"

                using (command = SurveyDB.GetStoredProcCommand("PRC_Authenticate_User"))
                {
                    //command.CommandType = CommandType.StoredProcedure;

                    MySqlParameter userNameParameter = new MySqlParameter();
                    userNameParameter.ParameterName = "pi_username";
                    userNameParameter.Value = oUserAuthorizationDomain.userName;
                    userNameParameter.MySqlDbType = MySqlDbType.VarChar;
                    userNameParameter.Size = 100;
                    userNameParameter.Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add(userNameParameter);

                    MySqlParameter passwordParameter = new MySqlParameter();
                    passwordParameter.ParameterName = "pi_password";
                    passwordParameter.Value = oUserAuthorizationDomain.userPassword;
                    passwordParameter.MySqlDbType = MySqlDbType.VarChar;
                    passwordParameter.Size = 100;
                    passwordParameter.Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add(passwordParameter);

                    MySqlParameter tokenParameter = new MySqlParameter();
                    tokenParameter.ParameterName = "pi_token";
                    tokenParameter.Value = oUserAuthorizationDomain.token;
                    tokenParameter.MySqlDbType = MySqlDbType.VarChar;
                    tokenParameter.Size = 100;
                    tokenParameter.Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add(tokenParameter);

                    MySqlParameter imeiParameter = new MySqlParameter();
                    imeiParameter.ParameterName = "pi_imei";
                    imeiParameter.Value = oUserAuthorizationDomain.imei;
                    imeiParameter.MySqlDbType = MySqlDbType.VarChar;
                    imeiParameter.Size = 30;
                    imeiParameter.Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add(imeiParameter);

                    MySqlParameter iccidParameter = new MySqlParameter();
                    iccidParameter.ParameterName = "pi_iccid";
                    iccidParameter.Value = oUserAuthorizationDomain.iccid;
                    iccidParameter.MySqlDbType = MySqlDbType.VarChar;
                    iccidParameter.Size = 30;
                    iccidParameter.Direction = System.Data.ParameterDirection.Input;

                    command.Parameters.Add(iccidParameter);

                    MySqlParameter poMsgParameter = new MySqlParameter();
                    poMsgParameter.ParameterName = "po_msg";
                    poMsgParameter.MySqlDbType = MySqlDbType.VarChar;
                    iccidParameter.Size = 10;
                    poMsgParameter.Direction = System.Data.ParameterDirection.Output;

                    command.Parameters.Add(poMsgParameter);


                    SurveyDB.ExecuteNonQuery(command);

                    if (command.Parameters["po_msg"].Value.ToString().ToLower() == "true")
                    {
                        oUserAuthorizationResponse.isUserAuthenticated = true;
                        oUserAuthorizationResponse.token = Guid.NewGuid().ToString();
                    }
                    else if (command.Parameters["po_result"].Value.ToString().ToLower() == "false")
                    {
                        oUserAuthorizationResponse.isUserAuthenticated = false;
                        oUserAuthorizationResponse.token = "";
                    }
                    else
                    {
                        oUserAuthorizationResponse.isUserAuthenticated = false;
                        oUserAuthorizationResponse.token = "";
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                oUserAuthorizationResponse.isUserAuthenticated = false;
                //entity.Message = ex.Message;
            }
            finally
            {
                DisposeCommand(command);
            }

            return oUserAuthorizationResponse;
        }

        #endregion
    }
}
