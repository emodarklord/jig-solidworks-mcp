using System.ComponentModel;
using Jig.Core;
using ModelContextProtocol.Server;

namespace Jig.Mcp;

[McpServerResourceType]
public static class RecipeResources
{
    public const string ListUri = "jig://recipes";

    [McpServerResource(UriTemplate = ListUri, Name = "recipes", MimeType = "text/plain")]
    [Description("Saved modelling sessions (*.intent.json in the Jig folder) that recipe_run can replay, with their steps' tools.")]
    public static string List(Workspace workspace)
    {
        var lines = Recipes.Find(workspace.AllowedFolders[0]).Select(file =>
        {
            try
            {
                return Recipes.Summary(file, Recipes.Read(file));
            }
            catch (JigException e)
            {
                return $"{Recipes.NameOf(file)}: {e}";
            }
        }).ToList();
        return lines.Count == 0 ? "No recipes yet. Save a session with intent_export as <name>.intent.json\n" : string.Concat(lines.Select(l => $"{l}\n"));
    }

    [McpServerResource(UriTemplate = ListUri + "/{name}", Name = "recipe", MimeType = "text/plain")]
    [Description("One recipe's steps in order, with the arguments each tool was called with.")]
    public static string Recipe(Workspace workspace, string name)
    {
        var file = Recipes.PathOf(workspace.AllowedFolders[0], name);
        return Recipes.Describe(file, Recipes.Read(file));
    }
}
