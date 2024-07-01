using KardiaApi.Logic.BusinessLogic;
using KardiaApi.Logic.Model;
using KardiaAPI;
using KardiaAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

var builder = WebApplication.CreateBuilder(args);

string apiUrl = builder.Configuration.GetConnectionString("WebBhokeUrl");
var connectionString = builder.Configuration.GetConnectionString("SQLConnectionString");
var ApiConnectionString = builder.Configuration.GetConnectionString("APIURL");
BllForKardiaApi bllObjForPatients = new BllForKardiaApi(connectionString);
string kardia_apikey = builder.Configuration.GetSection("Kardia")["APIKEY"].ToString();
string Username = builder.Configuration.GetSection("Kardia")["Username"].ToString();
string Password = builder.Configuration.GetSection("Kardia")["Password"].ToString();
string isValidUrl = builder.Configuration.GetSection("Kardia")["ValidUrl"].ToString();
Common logs = new Common( );
HttpClient client = new HttpClient();
string patientID = string.Empty;
string authToken = string.Empty;
string specificURL = string.Empty;
//int heartRate = 0;
//string mrnNo = string.Empty;


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Kardia API", Version = "v1" });
    //option.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    //{
    //    Description = "ApiKey must appear in Header",
    //    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    //    Name = "XApiKey",i changer
    //    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    //    Scheme = "ApiKeyScheme"
    //});
    //var key = new OpenApiSecurityScheme()
    //{
    //    Reference = new OpenApiReference
    //    {
    //        Type = ReferenceType.SecurityScheme,
    //        Id = "ApiKey"
    //    },
    //    In = ParameterLocation.Header
    //};
    //var requirement = new OpenApiSecurityRequirement
    //                {
    //                         { key, new List<string>() }
    //                };
    //option.AddSecurityRequirement(requirement);
    //Authorize API Key
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ValidAudience = builder.Configuration["JWT:ValidAudiance"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
    };
});
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "My API V1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/KardiaApi/Login" , [AllowAnonymous] async(string username, string password) =>
{
    AuthenticationHelper authenticationHelper = new AuthenticationHelper(builder.Configuration);
    var token = "";
    password = password.Replace("|DSG|", "+");
    string pass = BllSecurity.DecryptPassword(password);
    //string Passwrd = BllSecurity.DecryptPassword(Password);
    var checkLogin = new userValidate()
    {
        UserName = username,
        UserPassKey = pass,
    };
    var content = new StringContent(JsonConvert.SerializeObject(checkLogin), System.Text.Encoding.UTF8, "application/json");
    string url = isValidUrl;
    var response = await client.PostAsync(url, content);
    string rResp = await response.Content.ReadAsStringAsync();
    bool isValid = JsonConvert.DeserializeObject<bool>(rResp);

    
    if (isValid )
    {

        token = authenticationHelper.Login();
        logs.logsData(token);
    }
    authToken = token;
      
    if (!string.IsNullOrEmpty(token))
        return token;
    else
        return "Unauthorized";

}).WithTags("Generate Token");
app.MapGet("/GetPatients",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{
    string rResp = string.Empty;

    try
    {
        var byteArray = Encoding.ASCII.GetBytes($"{kardia_apikey}:");


        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        string _apiUrl = ApiConnectionString + "patients";
        var responce = await client.GetAsync(_apiUrl);
        logs.logsData(responce.ToString());
        if (responce.IsSuccessStatusCode)
        {
            rResp = await responce.Content.ReadAsStringAsync();
            logs.logsData(rResp);

        }
        PatientDetailsModel patientDetails = JsonConvert.DeserializeObject<PatientDetailsModel>(rResp);
        return patientDetails;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});

app.MapGet("/Kardia/GetIndividualPatientData",
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string id) =>
{
    string rResp = string.Empty;
    string specificURL = string.Empty;
    try
    {
        string url = ApiConnectionString + "patients/" + id;
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        { 
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        Patients data = JsonConvert.DeserializeObject<Patients>(rResp);
        

        //mrnNo = data.mrn;
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});




app.MapPost("/Kardia/CreatePatient",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string mrn, string? dob, string? email, string? firstname, string? lastname, string? gender) =>
{
    string jsonReturn = string.Empty;
    int sex = 0;
    //if (gender == "Male")
    //{
    //    sex = 1;
    //}
    //else
    //{
    //    sex = 2;
    //}
    try
    {
        string data = "?mrn=" + mrn + "&dob=" + dob + "&email=" + email + "&firstname=" + firstname + "&lastname=" + lastname + "&sex=" + sex;
        string url = ApiConnectionString + "patients" + data;
        var poststring = new StringContent("");
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.PostAsync(url, poststring);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            jsonReturn = await response.Content.ReadAsStringAsync();
            logs.logsData(jsonReturn);
        }
        Patients Patientsdata = JsonConvert.DeserializeObject<Patients>(jsonReturn);
        return Patientsdata;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});


app.MapGet("/Kardia/GetPatientRecordings",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string id) =>
{
    string rResp = string.Empty;
    try
    {
        string url = ApiConnectionString + "patients/" + id + "/recordings";
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        PatientRecordingsModel data = JsonConvert.DeserializeObject<PatientRecordingsModel>(rResp);
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});

app.MapGet("/Kardia/GetConnectionCode",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string id) =>
{
    string rResp = string.Empty;
    try
    {
        string url = ApiConnectionString + "patients/" + id + "/code";
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        ConnectionCodeModel data = JsonConvert.DeserializeObject<ConnectionCodeModel>(rResp);
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});

app.MapGet("/Kardia/GetAllRecordings",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{
    string rResp = string.Empty;

    try
    {
        string url = ApiConnectionString + "recordings";
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        RecordingsModel data = JsonConvert.DeserializeObject<RecordingsModel>(rResp);
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});

app.MapGet("/Kardia/SingleECG/Recordings",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string id) =>
{
    string rResp = string.Empty;

    try
    {
        string url = ApiConnectionString + "recordings/" + id;
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);

        }
        SingleECGModel data = JsonConvert.DeserializeObject<SingleECGModel>(rResp);
        //heartRate = data.heartRate;
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});



app.MapGet("/PostBack",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string id) =>
{
    patientID = id;
    var getURLResponse = string.Empty;
    var rResp = string.Empty;
    try
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        string geturl_url = apiUrl + "Webhooks/GetCallbackURL";
        var getlResponse = await client.GetAsync(geturl_url);
        if (getlResponse.IsSuccessStatusCode)
        {
            getURLResponse = await getlResponse.Content.ReadAsStringAsync();
        }
        NewURL seturl = JsonConvert.DeserializeObject<NewURL>(getURLResponse);
        string Url = apiUrl + "Kardia/GetIndividualPatientData?id=" + id;
        var response = await client.GetAsync(Url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        Patients data = JsonConvert.DeserializeObject<Patients>(rResp);
        return data;
    }
    catch (Exception ex) { 
        throw ex;
        logs.logsData(ex.ToString());
    }
});

app.MapPost("/Webhooks/SetCallback",
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async (string callback_url) =>
{
    string jsonReturn = string.Empty;
    try
    {
        string url = ApiConnectionString + "callback";
        var dict = new Dictionary<string, string>();
        dict.Add("url", callback_url);
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var req = new HttpRequestMessage(HttpMethod.Put, url) { Content = new FormUrlEncodedContent(dict) };
        var response = await client.SendAsync(req);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            jsonReturn = response.Content.ReadAsStringAsync().Result;
            logs.logsData(jsonReturn);
        }
        NewURL data = JsonConvert.DeserializeObject<NewURL>(jsonReturn);
        return data;
    }
    catch (Exception ex)
    {
        throw ex;
        logs.logsData(ex.ToString());
    }
});
app.MapGet("/Webhooks/GetCallbackURL",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
async () =>
{
    string jsonReturn = string.Empty;
    try
    {
        string url = ApiConnectionString + "callback";
        string loginDetails = kardia_apikey + ":";
        var byteArray = Encoding.ASCII.GetBytes(loginDetails);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        var response = await client.GetAsync(url);
        logs.logsData(response.ToString());
        if (response.IsSuccessStatusCode)
        {
            jsonReturn = await response.Content.ReadAsStringAsync();
            logs.logsData(jsonReturn);
        }
        NewURL data = JsonConvert.DeserializeObject<NewURL>(jsonReturn);
        return data;
    }
    catch (Exception ex)
    {
        logs.logsData(ex.ToString());
        throw ex;        
    }
});
app.MapPost("/Webhooks/Receive",
async ([FromBody] CallBackModel value) =>
{
    string rResp = string.Empty;
    string token = string.Empty;
    try
    {
        string jsonValue = JsonConvert.SerializeObject(value);
        JsonNode json = JsonNode.Parse(jsonValue);
        logs.logsData(jsonValue);
        string recordingID = value.recordingId;
        string patientID = value.patientId;
        string passwrd = Password.Replace("+", "|DSG|");
        string url = apiUrl + $"KardiaApi/Login?username={Username}&password={passwrd}";
        StringContent content = new StringContent("");
        var resp = client.PostAsync(url, content);
        if (resp.Result.IsSuccessStatusCode)
        {
            token = resp.Result.Content.ReadAsStringAsync().Result;
            //logs.log(token, "");
        }
        string uri = apiUrl + $"Kardia/SingleECG/Recordings?id={recordingID}";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        SingleECGModel data = JsonConvert.DeserializeObject<SingleECGModel>(rResp);
        int heartRate = data.heartRate;
        uri = apiUrl + $"Kardia/GetIndividualPatientData?id={patientID}";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        response = await client.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
            logs.logsData(rResp);
        }
        Patients patientDetails = JsonConvert.DeserializeObject<Patients>(rResp);
        string mrnNo = patientDetails.mrn;
        string servicesURL = bllObjForPatients.GetURL(mrnNo);
        char separator = '-';
        string[] authorsList = mrnNo.Split(separator);
        string enviromentId = authorsList[1];
        int protocolID = Convert.ToInt32(authorsList[0]);
        int usubjID = Convert.ToInt32(authorsList[2]);
        var myObject = new KardiaRequest
        {
            protocolID = protocolID,
            usubJID = usubjID,
            heartRate = heartRate.ToString()
        };
        string jsonDetails = JsonConvert.SerializeObject(myObject);
        content = new StringContent(jsonDetails,System.Text.Encoding.UTF8, "application/json");
        //content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
        string urlforSaveData = servicesURL + $"/Services/IntegrationService/DSG.Integration.ProtocolServices/Webhooks/SaveKardiaData";
        logs.logsData(urlforSaveData);
        response = await client.PostAsync(urlforSaveData, content);
        if (response.IsSuccessStatusCode)
        {
            rResp = await response.Content.ReadAsStringAsync();
        }
        return "True";
    }
    catch(Exception ex)
    {
        logs.logsData("Exception Occured on Recieve Method");
        throw ex;
    }
    
});

await app.RunAsync();