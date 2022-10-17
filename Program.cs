using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApplicationLocalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //��ȡxml�ļ�����
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);

    typeof(WebApplicationLocalAPI.APIVersion).GetEnumNames().ToList().ForEach(version =>
    {
        options.SwaggerDoc(version, new OpenApiInfo
        {
            Title = "LablePrintTest",
            Version = version.ToString(),
            Description = $"LablePrintTest:{version}�汾"
        });
    });
}
    );

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddCors(options =>
{

    options.AddPolicy(MyAllowSpecificOrigins, builder =>
    {

        builder.AllowAnyMethod()
                     .SetIsOriginAllowed(_ => true)
                     .AllowAnyHeader()
                     .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);//���ÿ�������
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options=>
    {
        typeof(APIVersion).GetEnumNames().ToList().ForEach(version =>
        {
            options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"�汾ѡ��{version}");
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
