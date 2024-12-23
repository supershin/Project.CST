using Newtonsoft.Json;
using Project.ConstructionTracking.Web.Models.WebAPIRest;

namespace Project.ConstructionTracking.Web.Infras.Repositories
{
    public class WebAPIRestRepo
    {
        public interface IGRVenderrportalRepo
        {
            Task<RequestPostModel.GRVenderrportal.Responds> UploadFileAsync(RequestPostModel.GRVenderrportal.Sends request);
        }
        public class GRVenderrportalRepo : IGRVenderrportalRepo
        {
            private const string ApiUrl = "https://aswinno.assetwise.co.th/OBLUAT/api/APIQC/upload";
            private const string AswApiKey = "YXN3b2JsYXBpOmFzd29ibGFwaUAyMDI0";  // Provide the real key

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
                        client.DefaultRequestHeaders.Add("ASWApiKey", AswApiKey);

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

                            // 3) Post the form to the external API
                            var response = await client.PostAsync(ApiUrl, form);
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
        }
    }
}
