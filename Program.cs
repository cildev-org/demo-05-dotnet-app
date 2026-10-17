// Lab app of the NuGet scenarios: extracts an uploaded tar into
// /data/uploads with SharpZipLib's TarArchive (CVE-2021-32841 before 1.3.3).
using System.Reflection;
using ICSharpCode.SharpZipLib.Tar;

var app = WebApplication.CreateBuilder(args).Build();
const string Root = "/data";
const string Dest = "/data/uploads";

app.MapPost("/extract", async (HttpRequest req) =>
{
    Directory.CreateDirectory(Dest);
    using var ms = new MemoryStream();
    await req.Body.CopyToAsync(ms);
    ms.Position = 0;
    try
    {
        using var tar = TarArchive.CreateInputTarArchive(ms, System.Text.Encoding.UTF8);
        tar.ExtractContents(Dest);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.GetType().Name);
    }
    return Results.Ok("extracted");
});

// Whether a file exists under /data: in uploads/, or next to it.
app.MapGet("/file", (string name) =>
    name.Contains("..") ? Results.BadRequest("name")
    : File.Exists(Path.Combine(Root, name)) ? Results.Ok("present") : Results.NotFound("absent"));

app.MapGet("/version", () =>
    typeof(TarArchive).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "?");

app.Run("http://0.0.0.0:8080");
