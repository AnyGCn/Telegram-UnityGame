using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.StaticFiles;

namespace HttpsServer;

public class HttpServerInstance
{
    private readonly WebApplication _app;
    private readonly FileExtensionContentTypeProvider _provider;
    private readonly CancellationTokenSource _cts;
    
    public HttpServerInstance(WebApplicationBuilder builder)
    {
        _app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!_app.Environment.IsDevelopment())
        {
            _app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _app.UseHsts();
        }

        _provider = new FileExtensionContentTypeProvider
        {
            Mappings =
            {
                [".data"] = "application/octet-stream",
                [".br"] = "application/octet-stream"
            }
        };

        _app.UseHttpsRedirection();
        _app.UseStaticFiles(new StaticFileOptions()
        {
            ContentTypeProvider = _provider,
            OnPrepareResponse = StaticFileCompressProcess,
        });

        _app.UseRouting();

        _app.UseAuthorization();

        _app.MapRazorPages();
        
        _cts = new CancellationTokenSource();
    }
    
    private void StaticFileCompressProcess(StaticFileResponseContext context)
    {
        var path = context.Context.Request.Path.Value;
        var extension = Path.GetExtension(path);
        
        if (extension == ".gz" || extension == ".br")
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path) ?? "";
            if (_provider.TryGetContentType(fileNameWithoutExtension, out string? contentType))
            {
                context.Context.Response.ContentType = contentType;
                context.Context.Response.Headers.Append("Content-Encoding", extension == ".gz" ? "gzip" : "br");
            }
        }
    }
    
    public void Run()
    {
        _app.Run();
    }
    
    public void RunAsync()
    {
        _app.RunAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
    }
}