using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace informaticsge.Modules;

public class PlainTextInputFormatter : TextInputFormatter
{
    public PlainTextInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var request = context.HttpContext.Request;

        using var reader = new System.IO.StreamReader(request.Body, encoding);
        var text = await reader.ReadToEndAsync();
        
        return await InputFormatterResult.SuccessAsync(text);
    }
 
    protected override bool CanReadType(Type type)
    {
        return type == typeof(string);
    }
}
//as C# cannot handle plain/text context I implemented costume handler 