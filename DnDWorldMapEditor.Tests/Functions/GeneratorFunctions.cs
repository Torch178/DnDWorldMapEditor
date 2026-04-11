using System.Security.Claims;
using System.Text;
using DnDWorldMapEditor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DnDWorldMapEditor.Tests.Functions;

public static class GeneratorFunctions
{
    public static IFormFile CreateMockFile(string fileContent, string contentType, string fileName)
    {
        var content = Encoding.UTF8.GetBytes(fileContent);
        IFormFile file = new FormFile(new MemoryStream(content), 0, content.Length, "test", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }

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
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        return newContext;
    }


    public static List<GridSpace> GenerateGridSpaces(int num, int worldMapId)
    {
        List<GridSpace> gridSpaces = new List<GridSpace>();
        for (int i = 0; i < num; i++)
        {
            GridSpace gridSpace = new GridSpace(worldMapId, i, i);
            gridSpaces.Add(gridSpace);
        }

        return gridSpaces;
    }

    public static List<GridEncounter> GenerateGridEncounters(int num, int encounterId, int gridSpaceId)
    {
        List<GridEncounter> gridEncounters = new List<GridEncounter>();
        for (int i = 0; i < num; i++)
        {
            GridEncounter gridEncounter = new GridEncounter(gridSpaceId, encounterId, false);
            gridEncounters.Add(gridEncounter);
        }

        return gridEncounters;
    }

    public static List<GridCharacter> GenerateGridCharacters(int num, int characterId, int gridSpaceId)
    {
        List<GridCharacter> gridCharacters = new List<GridCharacter>();
        for (int i = 0; i < num; i++)
        {
            GridCharacter gridCharacter = new GridCharacter(gridSpaceId, characterId);
            gridCharacters.Add(gridCharacter);
        }

        return gridCharacters;
    }

    public static List<Character> GenerateCharacters(int num, string userId)
    {
        List<Character> characters = new List<Character>();
        for (int i = 0; i < num; i++)
        {
            string name = "Character" + i;
            string description = "Description for Character" + i;
            Character character = new Character()
            {
                Name = name,
                Description = description,
                UserId = userId
            };
            
            characters.Add(character);
        }
        
        return characters;
    }
    
    public static List<Encounter> GenerateEncounters(int num, string userId)
    {
        List<Encounter> encounters = new List<Encounter>();
        for (int i = 0; i < num; i++)
        {
            string name = "Encounter" + i;
            string description = "Description for Encounter" + i;
            Encounter encounter = new Encounter()
            {
                Name = name,
                Description = description,
                UserId = userId
            };
            
            encounters.Add(encounter);
        }
        
        return encounters;
    }
    
}