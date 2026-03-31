using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DnDWorldMapEditor.Tests.TestHelperFunctions;

public static class MockingFunctions
{
    public static ControllerContext CreateMockUser()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
       ControllerContext newContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext{User =  claimsPrincipal}
        };
       
       return  newContext;
    }

    public static IFormFile CreateMockFile(string fileContent, string contentType, string fileName )
    {
        var content = Encoding.UTF8.GetBytes(fileContent);
        IFormFile file = new FormFile(new MemoryStream(content), 0, content.Length, "test", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }
}