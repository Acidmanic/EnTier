using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace EnTier.Utility;

public class ObjectUtilities
{


    public static string ExtractAllTexts(object value,bool fullTree = true)
    {
        if (value == null)
        {
            return "";
        }

        var evaluator = new ObjectEvaluator(value);

        var stringType = typeof(string);

        IEnumerable<AccessNode> textNodes =
            fullTree ? evaluator.Map.Nodes.Where(n => n.IsLeaf) : evaluator.RootNode.GetDirectLeaves();

        textNodes = textNodes.Where(n => n.Type == stringType);

        var sb = new StringBuilder();

        foreach (var textNode in textNodes)
        {
            var key = evaluator.Map.FieldKeyByNode(textNode);

            var text = evaluator.Read(key, true) as string ?? "";

            sb.Append(text).Append(" ");
        }

        return sb.ToString();
    }
}