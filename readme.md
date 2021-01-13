# Razor Pages Web App
## How to create from Visual Studio
1. 'Create a new project'
2. 'ASP.NET Core Web Application'
3. Configure project name and location
4. '.NET Core' - 'ASP.NET Core 5.0', 'Web Application''

## How to create from the command-line
Change to the directory where you want your project folder to be stored:
```
dotnet new webapp -o RazorPagesMovie
```

To run from the command line, first run:
```
dotnet dev-certs https --trust
```

Then:
```
dotnet run
```

## Folders & Files
### Pages
Contains Razor pages and supporting files. Each Razor page is a pair of files:
- A .cshtml file that has HTML markup with C# code using Razor syntax.
- A .cshtml.cs file that has C# code that handles page events.

Supporting files have names that begin with an underscore. For example, the _Layout.cshtml file configures UI elements common to all pages. This file sets up the navigation menu at the top of the page and the copyright notice at the bottom of the page.

### wwwroot
Contains static assets like HTML, JS and CSS files.

### appsettings.json
Contains configuration data.

### Program.cs
The apps entry point.

### Startup.cs
Code to configure app behaviour.

## Adding a Model
### Adding a Model from Visual Studio
Model classes use the `Entity Framework Core (EF Core)` to work with a database, EF Core is an object-relational mapper (O/RM) that simplifies data access. The model
classes are written and EF Core creates the database.

Model classes are called *POCO* classes (Plain-Old CLR Objects) as they do not have dependencies on EF Core.

1. Add a folder called 'Models'.
2. Add a model class.
3. Add properties to the model class:
```
public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
    }
```

### Adding a Model from Visual Studio Code
1. Add a folder called 'Models'.
2. Add a model class.
3. Add properties to the model class as above.

#### Add NuGet packages and EF tools
```
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet add package Microsoft.EntityFrameworkCore.SQLite
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.Extensions.Logging.Debug
```

The ID field is the primary key for the database items. The `[DataType]` attribute specifies the type of the data (`Date`).

#### Add the database context class
1. Create a folder named 'Data'
2. Add file named 'RazorPagesMovieContext.cs' to the 'Data' folder:
```
using Microsoft.EntityFrameworkCore;

namespace RazorPagesMovie.Data
{
    public class RazorPagesMovieContext : DbContext
    {
        public RazorPagesMovieContext (
            DbContextOptions<RazorPagesMovieContext> options)
            : base(options)
        {
        }

        public DbSet<RazorPagesMovie.Models.Movie> Movie { get; set; }
    }
}
```

The above creates a `DbSet` property for the entity set. An *entity set* typically corresponds to a database table, an *entity* corresponds to a row in the table.

#### Add the database connection string
Add the connection string to *appsettings.json*:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "RazorPagesMovieContext": "Data Source=MvcMovie.db"
  }
}
```

#### Register the database context
1. Add these `using` statements to the top of *Startup.cs*:
```
using RazorPagesMovie.Data;
using Microsoft.EntityFrameworkCore;
```

2. Register the DB context with dependency injection container in `Startup.ConfigureServices`:
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();

    services.AddDbContext<RazorPagesMovieContext>(options =>
        options.UseSqlite(Configuration.GetConnectionString("RazorPagesMovieContext")));
}
```

## Scaffold the movie model
The scaffolding process allows pages for CRUD operations to be generated for the movie model.

### Scaffold from Visual Studio
1. Create 'Pages/Movies' folder:
2. Right-click Movies folder > **Add** > **New Scaffolded Item**
3. In the **Add Scaffold** dialog, select **Razor Pages using Entity Framework (CRUD)** > **Add**
4. In the **Razor Pages using Entity Framework (CRUD)** dialog:
4a. In the **Model class** drop down, select **Movie**
4b. In the **Data context class** row, select the **+** sign:
4bi. In the **Add Data Context** dialog, the class name is generated
4bc. Select **Add**

The scaffolding process creates and updates the following files:
- Pages/Movies: Create, Delete, Details, Edit, and Index
- Data/RazorPagesMovieContext.cs
- Startup.cs

### Scaffold from the Command-Line
1. Open a command shell in the root project directory where `Program.cs` is contained
2. Run the following command:
2a: WINDOWS:
```
dotnet-aspnet-codegenerator razorpage -m Movie -dc RazorPagesMovieContext -udl -outDir Pages\Movies --referenceScriptLibraries
```
2b: MACOS/LINUX
```
dotnet-aspnet-codegenerator razorpage -m Movie -dc RazorPagesMovieContext -udl -outDir Pages/Movies --referenceScriptLibraries
```

#### Generator commands
- -m	                        The name of the model.
- -dc	                        The DbContext class to use.
- -udl	                        Use the default layout.
- -outDir	                    The relative output folder path to create the views.
- --referenceScriptLibraries	Adds _ValidationScriptsPartial to Edit and Create pages

```
// To get help
dotnet-aspnet-codegenerator razorpage -h
```

#### Use SQLite for development, SQL Server for production
When SQLite is selected, template generated code is development-ready. The following shows the injection of `IWebHostEnvironment` into `Startup.cs`,
this is so SQLite is used in development and SQL Server in production:
```
public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Environment = env;
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        if (Environment.IsDevelopment())
        {
            services.AddDbContext<RazorPagesMovieContext>(options =>
            options.UseSqlite(
                Configuration.GetConnectionString("RazorPagesMovieContext")));
        }
        else
        {
            services.AddDbContext<RazorPagesMovieContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("MovieContext")));
        }

        services.AddRazorPages();
    }

    public void Configure(IApplicationBuilder app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}
```

The scaffolding process creates and updates the following files:
- Pages/Movies: Create, Delete, Details, Edit, and Index

## Create the initial database schema using EF's migration feature
The migrations feature provided by the EF Core provides a way to:
- Generate the initial DB schema,
- Incrementally update the DB schema to keep it in sync with the app's data model. Existing data is preserved.

### Visual Studio
Open the **Package Manager Console**:
- Add an initial migration
- Update the DB with the initial migration

1. **Tools** > **NuGet Package Manager** > **Package Manager Console**
2. Run the commands:
```
Add-Migration InitialCreate
Update-Database
```

The `migrations` command initialises a DB schema based on the model specified in the `DbContext`. The `InitialCreate` argument
is the name of the migration.

The `update` command runs the `Up` method in migrations that have not been applied.

### Visual Studio Code
If `dotnet ef` is not installed, install as a global tool:
```
dotnet tool install --global dotnet-ef
```

Run the following .NET CLI commands:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Scaffolding Explained
Razor Pages are derived from `PageModel`, the naming convention for a `PageModel`-derived class is `<PageName>Model`. Razer pages use dependency
injection in the constructor to add the DB context to the page:
```
public class IndexModel : PageModel
{
    private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

    public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
    {
        _context = context;
    }
```

When a request is made for the Index page, the `OnGetAsync` method returns a list of movies to the Razor Page. On Razor Page's, `OnGetAsync` or
`OnGet` are called to initialise the state of the page. 
```
public async Task OnGetAsync()
        {
            Movie = await _context.Movie.ToListAsync();
        }
```

When `OnGet` returns `void` or `OnGetAsync` returns `Task`, no return statement is used:
```
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
```

If the return type is `IActionResult` or `Task<IActionResult>`, a return statement must be provided:
```
public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Movie.Add(Movie);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
```

Razor can transition from HTML into C# or Razor-specific markup. If an `@` symbol is followed by a Razor reserved keyword,
the transition is to Razor markup otherwise it is to C#.

#### The @page directive
The `@page` directive found at the top of `.cshtml` files makes the file an MVC action, which means the page can handle request. `@page` must
be the first Razor directive on a page.

#### The @model directive
The `@model` directive specifies the type of a model being passed to the Razor Page. This also makes the `PageModel`-derived class available to 
Razor page. The model can be used in varying ways, including `@Html' helpers like `@Html.DisplayFor` and `@Html.DisplayNameFor`.

#### The layout page
The layout page, specified in *Pages/Shared/_Layout.cshtml* is a template that allows the HTML container layout to be:
- specified in a single place,
- applied in multiple pages

The `@RenderBody()` line is a placeholder where all page-specific views are rendered, wrapped in the layout page.

#### ViewData and layout
```
@page
@model RazorPagesMovie.Pages.Movies.IndexModel

@{
    ViewData["Title"] = "Index";
}
```

The `@{ ... }` is an example of Razor transitioning into C#. Enclosed in the braces is the C# code.

The `PageModel` base class contains a `ViewData` dictionary property, this can pass data to a View. Add objects to the `ViewData` dictionary using a 
**key value** pattern.

The `Title` property shown above is used in the *Pages/Shared/_Layout.cshtml* file:
```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - RazorPagesMovie</title>

    @*Markup removed for brevity.*@
```

The last line is a Razor comment, which never gets sent to the client.

The `Layout` property is set in the *Pages/_ViewStart.cshtml* file:
```
@{
    Layout = "_Layout";
}
```

#### Anchor Tag Helper
Tag Helpers are used when making MVC template views. The Anchor Tag Helper has two attributes applied by default:
```
<a class="navbar-brand" asp-area="" asp-page="/Index">RazorPagesMovie</a>
```

The `asp-page` attribute creates a link to '/Index'. The `asp-area` tag is used indicate the group/area that the page belongs to, such as a controller.

#### The Create Page Model
Create.cshtml.cs
```
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Models;
using System;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
```

The `OnGet` mdthod initialises the state, `Page` is returned as there is no state to initialise. The `Page` method creates a `PageResult` object that 
renders the *Create.cshtml* page.

The `Movie` property uses the `[BindProperty]` attribute to opt-in to model binding. This means when the create form posts form values, the ASP.NET Core
runtime will attempt to bind the posted values to the `Movie` model.

The `OnPostAsync` method runs when the page posts form data. If a model error occurs, the form is redisplayed with any form data posted.

#### The Create Razor Page
Create.cshtml
```
@page
@model RazorPagesMovie.Pages.Movies.CreateModel

@{
    ViewData["Title"] = "Create";
}

<h1>Create</h1>

<h4>Movie</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form method="post">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Movie.Title" class="control-label"></label>
                <input asp-for="Movie.Title" class="form-control" />
                <span asp-validation-for="Movie.Title" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Movie.ReleaseDate" class="control-label"></label>
                <input asp-for="Movie.ReleaseDate" class="form-control" />
                <span asp-validation-for="Movie.ReleaseDate" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Movie.Genre" class="control-label"></label>
                <input asp-for="Movie.Genre" class="form-control" />
                <span asp-validation-for="Movie.Genre" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Movie.Price" class="control-label"></label>
                <input asp-for="Movie.Price" class="form-control" />
                <span asp-validation-for="Movie.Price" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-page="Index">Back to List</a>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

The `<form method="post">` element is a Form Tag Helper, which automatically includes an antiforgery token. The scaffolding engine
creates Razor markup for each field in the model, except the ID.

##### Validation Tag Helpers
`<div asp-validation-summary="">` and `<span asp-validation-for="">` display validation errors.

##### Label Tag Helper
`<label asp-for="Movie.Title" class="control-label"></label>` generates the label caption and [for] attribute for the Title property.

##### Input Tag Helper
`<input asp-for="Movie.Title" class="form-control">` uses the DataAnnotations attributes and produces HTML attributes needed 
for jQuery Validation on the client-side.

## Working with a Database
The `RazorPagesMovieContext` object handles the task of connecting to the DB and mapping `Movie` objects to DB records. The DB context is registered with
the dependency injection container in the `ConfigureServices` method in *Startup.cs*.

The ASP.NET Core configuration system reads the `ConnectionString` key. For local development, config gets the connection string from *appsettings.json*. When 
the app is deployed to a test or production server, an environment variable can be used to set the connection string.

### Seed the database
Create a class for seeding data in the models folder:
```
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesMovie.Data;
using System;
using System.Linq;

namespace RazorPagesMovie.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RazorPagesMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<RazorPagesMovieContext>>()))
            {
                // Look for any movies.
                if (context.Movie.Any())
                {
                    return;   // DB has been seeded
                }

                context.Movie.AddRange(
                    new Movie
                    {
                        Title = "When Harry Met Sally",
                        ReleaseDate = DateTime.Parse("1989-2-12"),
                        Genre = "Romantic Comedy",
                        Price = 7.99M
                    },

                    new Movie
                    {
                        Title = "Ghostbusters ",
                        ReleaseDate = DateTime.Parse("1984-3-13"),
                        Genre = "Comedy",
                        Price = 8.99M
                    },

                    new Movie
                    {
                        Title = "Ghostbusters 2",
                        ReleaseDate = DateTime.Parse("1986-2-23"),
                        Genre = "Comedy",
                        Price = 9.99M
                    },

                    new Movie
                    {
                        Title = "Rio Bravo",
                        ReleaseDate = DateTime.Parse("1959-4-15"),
                        Genre = "Western",
                        Price = 3.99M
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
```

### Add the Seed Initialiser
Program.cs
```
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RazorPagesMovie.Models;
using System;

namespace RazorPagesMovie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

## Updating Generated Pages
1. Update the model

- Add the `[Display]` attribute and change the `Name`
- Add the `[Column]` attribute and specify the `TypeName` as a `decimal(18, 2)`
2. Update the generated Edit, Details, and Delete Razor page to use the `{id:int}` route template.

- Modify the page directive for the pages to `@page "{id:int?}"`
- The question mark makes the id optional in the url: `https://localhost:5000/Movies/Details`

## Adding search functionality
### Add search parameters as bounded properties to the Index model
To add search functionality, some bounded properties for the search parameters needs to be added to the `IndexModel` in *Pages/Movies/Index.cshtml.cs*:
```
[BindProperty(SupportsGet = true)]
public string SearchString { get; set; }

public SelectList Genres { get; set; }

[BindProperty(SupportsGet = true)]
public string MovieGenre { get; set; }
```

- Above, `SearchString` has the `[BindProperty]` attribute which will bind form values and query strings with the same name. The `SupportsGet` argument
is required to enable binding for HTTP GET requests.
- `Genres` will contain a list of genres for the user to select from, note that this property is not bound. `SelectList` will require `Microsoft.AspNetCore.Mvc.Rendering`.
- The bound property `MovieGenre` will be given data for searching from the `SelectList Genres` property.

### Update the Index models 'OnGetAsync' method
*Pages/Movies/Index.cshtml.cs*
#### Basic
1. Create a LINQ
2. Logic
3. Execution

```
public async Task OnGetAsync()
{
    // Creates a LINQ for selecting all movies, the query has not yet run
    var movies = from m in _context.Movie
                 select m;

    // Checks the SearchString property is not null or containing whitespace
    if (!string.IsNullOrEmpty(SearchString))
    {
        // Modifies the query to filter out movies where the Title property does not contain values from the search string
        //   - uses a programmatic LINQ statement with a lambda expression
        movies = movies.Where(s => s.Title.Contains(SearchString));
    }

    // Defered execution ends as the ToListAsync method has been called
    Movie = await movies.ToListAsync();
}
```

#### Basic + Genre
```
public async Task OnGetAsync()
{
    // Use LINQ to get list of genres.
    IQueryable<string> genreQuery = from m in _context.Movie
                                    orderby m.Genre
                                    select m.Genre;

    // Movie LINQ
    var movies = from m in _context.Movie
                 select m;

    // Basic search
    if (!string.IsNullOrEmpty(SearchString))
    {
        movies = movies.Where(s => s.Title.Contains(SearchString));
    }

    // Genre search
    if (!string.IsNullOrEmpty(MovieGenre))
    {
        movies = movies.Where(x => x.Genre == MovieGenre);
    }

    // Generate the Genres SelectList by executing the genreQuery
    Genres = new SelectList(await genreQuery.Distinct().ToListAsync());

    // Executes the movies query
    Movie = await movies.ToListAsync();
}
```

### Update the Index pages
*Pages/Movies/Index.cshtml*
#### Basic
Add the form to submit the search string.
```
<form>
    <p>
        Title: <input type="text" asp-for="SearchString" />
        <input type="submit" value="Filter" />
    </p>
</form>
```

The `<form>` is using the following tag helpers:
- Form Tag Helper - On submission, the filter string is sent to the 'Pages/Movies/Index' page via a query string.
- Input Tag Helper

#### Basic + Genre
Update the basic `<form>` tag to include in the `<p>` tag:
```
<select asp-for="MovieGenre" asp-items="Model.Genres">
    <option value="">All</option>
</select>
```

The `asp-for` attribute indicates this will be sent as a query string like `?MovieGenre=<xxx>`. The `asp-items` attribute specifies the items for the
`<select>` tag will be generated from the `Model.Genres` property.

## Adding a new field to a Razor Page
EF *Code First Migrations* can be used to:
- Add new fields to a model,
- Migrate the new field schema change to the database.

EF Code First when creating a database will:
- Add an `_EFMigrationsHistory` table to the database to track whether the schema of the DB is in sync with the model classes it was generated from,
- If the model classes are not in sync with the DB, EF will throw an exception.

### Steps
#### Add a new property to the model
Open the *Models/Movie.cs* file and add a `Rating` property:
```
public string Rating { get; set; }
```

#### Build the app

#### Add the `Rating` field to the Index page
```
<th>
    @Html.DisplayNameFor(model => model.Movie[0].Rating)
</th>

<td>
    @Html.DisplayFor(modelItem => item.Rating)
</td>
```

#### Add the `Rating` field to the Delete, Details, Edit and Create pages

#### Update the `SeedData.cs` data to include the `Rating` field

#### Update the database
1. Have EF auto-drop and re-create the DB with the new model schema. Convenient for development but not production.
2. Explicit schema modification of an existing DB.
3. Code First Migrations to update the DB schema.

##### Code First Migrations
1. **Tools** > **NuGet Package Manager** > **Package Manager Console**
2. **PMC**
```
Add-Migration Rating
Update-Database
```

The `Add-Migration` command instructs the framework to:
- Compare the `Movie` DB schema with the `Movie` model
- Create migration code

The name `Rating` for the migration merely indicates the name of the migration file.

The `Update-Database` command instructs EF to apply schema changes to the existing DB whilst preserving data.

## Adding Validation
Validation logic sets enforcable rules for clients modifying or creating models.

The validation support by Razor Pages and EF is DRY:
- Validation rules declaratively specified in one location - the model class,
- Rules are enforced everywhere in the app.

### Some Annotations
*DataAnnotations* namespace
- [DataType]
- [Required]
- [StringLength]
- [RegularExpression]
- [Range]

Annotations also take arguments, such as `[StringLength(60, MinimumLength = 3)]`.
- The string can contain no more than 60 characters and no less than 3.

### Adding Validation Rules
The `DataAnnotations` namespace provides:
- Built-in validation attributes to declaratively apply to classes or properties,
- Formatting attributes like that don't provide validation but help with the format, such as the `[DataType]` attribute.

The string can contain no more than 60 characters and no less than 3.
```
[StringLength(60, MinimumLength = 3)]
public string Title { get; set; }
```

The string must only use letters, the first letter must be uppercased. White space, numbers, and special characters are forbidden.
```
[RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
```

The strings first character must be a uppercase letter. Special characters and numbers in subsequent spaces are allowed, such as *PG-18*.
```
[RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
```

### Migrate and update the database
```
Add-Migration Validation
Update-Database
```