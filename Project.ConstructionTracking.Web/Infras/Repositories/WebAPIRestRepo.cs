using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Models.WebAPIRest;
using System.Text;
using static Project.ConstructionTracking.Web.Models.WebAPIRest.RequestPostModel.Get_User_CRM;

namespace Project.ConstructionTracking.Web.Infras.Repositories
{
    public class WebAPIRestRepo
    {
        public interface IWebAPIRestRepo
        {
            Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request);
            Task<RequestPostModel.Get_User_CRM.Responds> CentralizeGetUserCRM(RequestPostModel.Get_User_CRM.Sends request);
            Task<RequestPostModel.QC_Status_Update_QC5.Responds> QcStatusUpdateQc5(RequestPostModel.QC_Status_Update_QC5.Sends request);
        }
        public class _WebAPIRestRepo : IWebAPIRestRepo
        {
            private readonly string _apiUrl;           
            private readonly string _aswApiKey;

            private readonly string _apiCentralizeGet_User_CRMUrl;
            private readonly string _apiCentralizeGet_AuthorizationApiKey;

            private readonly string _api_QC_CRM_QC_Status_Update_QC5_Url;
            private readonly string _rem_api_username;
            private readonly string _rem_api_password;
            private readonly string _rem_api_secretkey;

            public _WebAPIRestRepo(IConfiguration configuration)
            {
                _apiUrl = configuration["ThirdPartyApis:AswinnoAPI:ApiUrl"];
                _aswApiKey = configuration["ThirdPartyApis:AswinnoAPI:AswApiKey"];

                _apiCentralizeGet_User_CRMUrl = configuration["ThirdPartyApis:Centralize_API:Get_User_CRMApiUrl"];
                _apiCentralizeGet_AuthorizationApiKey = configuration["ThirdPartyApis:Centralize_API:AuthorizationApiKey"];

                _api_QC_CRM_QC_Status_Update_QC5_Url = configuration["ThirdPartyApis:QC_API_CRM:QC_Status_Update_QC5_PRD_ApiUrl"];
                _rem_api_username = configuration["ThirdPartyApis:QC_API_CRM:rem-api-username"];
                _rem_api_password = configuration["ThirdPartyApis:QC_API_CRM:rem-api-password"];
                _rem_api_secretkey = configuration["ThirdPartyApis:QC_API_CRM:rem-api-secretkey"];
            }

            public async Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request)
            {

                // Prepare the response model
                var responds = new RequestPostModel.GRVenderrportal.Responds
                {
                    status = 0,
                    message = "Unknown error"
                };

                try
                {
                    using (var client = new HttpClient())
                    {
                        // Add the ASWApiKey header
                        client.DefaultRequestHeaders.Add("ASWApiKey", _aswApiKey);

                        // Use multipart/form-data
                        using (var form = new MultipartFormDataContent())
                        {
                            // 1) Add files to the form
                            if (request.fileData != null && request.fileData.Count > 0)
                            {
                                foreach (var file in request.fileData)
                                {
                                    // Each file must be added as a StreamContent with file name
                                    var streamContent = new StreamContent(file.OpenReadStream());
                                    form.Add(streamContent, "fileData", file.FileName);
                                }
                            }
                            else
                            {
                                // All required, if file is missing => handle an error or skip
                                responds.status = 0;
                                responds.message = "No file data provided";
                                return responds;
                            }

                            // 2) Add other required form fields
                            if (string.IsNullOrEmpty(request.grno) || string.IsNullOrEmpty(request.pono))
                            {
                                responds.status = 0;
                                responds.message = "grno or pono is missing";
                                return responds;
                            }
                            form.Add(new StringContent(request.grno ?? ""), "grno");
                            form.Add(new StringContent(request.pono ?? ""), "pono");
                            form.Add(new StringContent(request.remark ?? ""), "remark");


                            // 3) Post the form to the external API
                            var response = await client.PostAsync(_apiUrl, form);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                // If the API returns a known JSON, parse it:
                                // Example response: { status: "success", message: "file uploaded is successfully" }
                                dynamic? resultObj = null;
                                try
                                {
                                    resultObj = JsonConvert.DeserializeObject(responseContent);
                                }
                                catch { /* handle parse errors if needed */ }

                                // Based on your example, let's say "status=success" => success
                                if (resultObj != null && resultObj.status == "success")
                                {
                                    responds.status = 200;
                                    responds.message = (string)resultObj.message;
                                }
                                else
                                {
                                    // Possibly an unknown format, handle gracefully
                                    responds.status = 200;
                                    responds.message = "Upload success, but unexpected response: " + responseContent;
                                }
                            }
                            else
                            {
                                // The request failed
                                responds.status = (int)response.StatusCode;
                                responds.message = "Failed to upload. Response: " + responseContent;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    responds.status = 0;
                    responds.message = "Exception: " + ex.Message;
                }

                return responds;
            }

            public async Task<RequestPostModel.Get_User_CRM.Responds> CentralizeGetUserCRM(RequestPostModel.Get_User_CRM.Sends request) 
            {
                var responds = new RequestPostModel.Get_User_CRM.Responds
                {
                    status = 0,
                    message = "Unknown error"
                };

                try
                {
                    var url = _apiCentralizeGet_User_CRMUrl + request.email;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Authorization", _apiCentralizeGet_AuthorizationApiKey);

                        var json = JsonConvert.SerializeObject(request);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(url, content);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            responds.message = $"HTTP Error: {response.StatusCode}";
                            return responds;
                        }

                        var result = JsonConvert.DeserializeObject<CentralizeUserResponse>(responseContent);

                        if (result != null && result.Success && result.Data != null && result.Data.Count > 0)
                        {
                            responds.status = 1;
                            responds.message = result.Message;
                            responds.UserID = result.Data[0].UserID;
                        }
                        else
                        {
                            responds.message = "No user found or invalid data.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    responds.message = "Exception: " + ex.Message;
                }

                return responds;
            }

            public async Task<RequestPostModel.QC_Status_Update_QC5.Responds> QcStatusUpdateQc5(RequestPostModel.QC_Status_Update_QC5.Sends request)
            {
                var responds = new RequestPostModel.QC_Status_Update_QC5.Responds
                {
                    Status = 0,
                    message = "Unknown error"
                };

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("rem-api-username", _rem_api_username);
                        client.DefaultRequestHeaders.Add("rem-api-password", _rem_api_password);
                        client.DefaultRequestHeaders.Add("rem-api-secretkey", _rem_api_secretkey);

                        // Validate required fields
                        if (string.IsNullOrEmpty(request.project_code))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบรหัสโครงการ (Project Code)";
                            return responds;
                        }
                        if (string.IsNullOrEmpty(request.unit_number))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบรหัสยูนิต (Unit Number)";
                            return responds;
                        }
                        if (string.IsNullOrEmpty(request.contractor_appointment_date))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบวันที่นัดตรวจ";
                            return responds;
                        }
                        if (string.IsNullOrEmpty(request.contractor_appointment_timeStart))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบเวลาที่นัดตรวจ";
                            return responds;
                        }
                        if (string.IsNullOrEmpty(request.qc_response_user_id))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบผู้ตรวจ QC5";
                            return responds;
                        }
                        if (string.IsNullOrEmpty(request.qc_response_date))
                        {
                            responds.Status = 0;
                            responds.message = "ไม่พบวันที่ QC5";
                            return responds;
                        }


                        // Prepare JSON content
                        var jsonBody = JsonConvert.SerializeObject(new
                        {
                            project_id = request.project_code,
                            unit_number = request.unit_number,
                            contractor_appointment_date = request.contractor_appointment_date,
                            contractor_appointment_timeStart = request.contractor_appointment_timeStart,
                            contractor_appointment_timeEnd = request.contractor_appointment_timeEnd,
                            qc_response_user_id = request.qc_response_user_id,
                            qc_response_date = request.qc_response_date,
                            qc_remark = request.qc_remark,
                            qc_type = request.qc_type
                        });

                        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(_api_QC_CRM_QC_Status_Update_QC5_Url, content);
                        var responseContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            responds.Status = 1;
                            responds.message = "Send data success";
                        }
                        else
                        {
                            responds.Status = (int)response.StatusCode;
                            responds.message = "API Error: " + responseContent;
                        }
                    }
                }
                catch (Exception ex)
                {
                    responds.Status = 0;
                    responds.message = "Exception: " + ex.Message;
                }

                return responds;
            }


        }
    }
}
