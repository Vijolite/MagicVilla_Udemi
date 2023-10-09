using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Helpers;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


/*builder.Services.AddControllers(option => //did not work on swagger
{
    option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters(); */// support xml format for response too
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ILogging, Logging>(); //new service registration AddSingleton - 1 ex; AddScoped - 1ex for each request; AddTransient
//builder.Services.AddSingleton<ILogging, Logging_v2>();

//add AWS
AWSOptions awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

//Get a secret's value from SecretManager - just an example no need for now
//Deleted to secrets from Seret Manager
//Console.WriteLine(await Secrets.GetOneSecretPairValue("key1"));

//Get a secret's value from SSM parameters - just an example no need for now
//Console.WriteLine(SecretsSSM.GetSecretUrl());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); // http://localhost:7056/swagger/index.html
