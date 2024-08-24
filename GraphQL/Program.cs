using OfficeOpenXml;

// EPPlus モジュールの使用ライセンスを非商用に設定
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// GraphQLモジュールの設定
var builder = WebApplication.CreateSlimBuilder(args);
builder.Services
    .AddSingleton<Repository>()
    .AddGraphQLServer()
    .AddQueryType<Query>();


// JSONシリアライザの設定
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});


// CORSポリシーの設定
string CorsPolicyName = "AllowAllOrigin";
builder.Services.AddCors(option =>
{
    option.AddPolicy(CorsPolicyName,
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin",
//        builder => builder.WithOrigins("http://example.com")); }); app.UseCors("AllowSpecificOrigin");

//"AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
//rvice.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
//{
//    builder.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader();
//}));

var app = builder.Build();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.UseCors(CorsPolicyName);
app.MapGraphQL();
app.RunWithGraphQLCommands(args);
