using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace WebWardrobe;

internal class HudMod : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/HUD/playerhud.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        var topOfFile = new MultiTokenWaiter([
            t => t.Type == TokenType.Newline,
            t => t.Type == TokenType.Newline,
        ], allowPartialMatch: true);

        // 	if popups == [] and not OptionsMenu.open:
        var check = new MultiTokenWaiter([
            t => t.Type == TokenType.CfIf,
            t => t is IdentifierToken { Name: "popups" },
            t => t.Type == TokenType.OpEqual,
            t => t.Type == TokenType.OpAnd,
            t => t.Type == TokenType.OpNot,
            t => t is IdentifierToken { Name: "open" }
        ], allowPartialMatch: true);

        foreach (var token in tokens)
        {
            if (topOfFile.Check(token))
            {
                yield return token;

                // var typing = false
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("typing");
                yield return new Token(TokenType.OpAssign);
                yield return new ConstantToken(new BoolVariant(false));
                yield return new Token(TokenType.Newline);
            } else if (check.Check(token))
            {
                yield return token;

                // and not typing
                yield return new Token(TokenType.OpAnd);
                yield return new Token(TokenType.OpNot);
                yield return new IdentifierToken("typing");
            }
            else
            {
                // return the original token
                yield return token;
            }
        }
    }
}
