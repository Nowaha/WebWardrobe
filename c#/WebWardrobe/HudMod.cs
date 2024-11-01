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

        // func _ready():
        var onready = new MultiTokenWaiter([
            t => t.Type == TokenType.PrFunction,
            t => t is IdentifierToken { Name: "_ready" },
            t => t.Type == TokenType.Colon,
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

                // onready var webwardrobe_mod = get_node("/root/WebWardrobe")
                yield return new Token(TokenType.PrOnready);
                yield return new Token(TokenType.PrVar);
                yield return new IdentifierToken("webwardrobe_mod");
                yield return new Token(TokenType.OpAssign);
                yield return new IdentifierToken("get_node");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new StringVariant("/root/WebWardrobe"));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline);
            }
            else if (onready.Check(token))
            {
                yield return token;

                // webwardrobe_mod._inject(self)
                yield return new IdentifierToken("webwardrobe_mod");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("_inject");
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new Token(TokenType.Self);
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else if (check.Check(token))
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
