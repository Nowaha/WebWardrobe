using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace WebWardrobe;

internal class CosmeticMenuMod : IScriptMod
{
    public bool ShouldRun(string path) => path == "res://Scenes/HUD/CosmeticMenu/cosmetic_menu.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        // $HBoxContainer / misc.modulate = Color(0.7, 0.7, 0.7) if category != "misc" else Color(1.0, 1.0, 1.0)
        var test = new MultiTokenWaiter([
            t => t is IdentifierToken { Name: "misc" },
            t => t is IdentifierToken { Name: "modulate" },
            t => t.Type == TokenType.Newline,
        ], allowPartialMatch: true);

        foreach (var token in tokens)
        {
            if (test.Check(token))
            {
                yield return token;

                // $HBoxContainer / outfits.modulate = Color(0.7, 0.7, 0.7) if category != "outfits" else Color(1.0, 1.0, 1.0)
                yield return new Token(TokenType.Dollar);
                yield return new IdentifierToken("HBoxContainer");
                yield return new Token(TokenType.OpDiv);
                yield return new IdentifierToken("outfits");
                yield return new Token(TokenType.Period);
                yield return new IdentifierToken("modulate");
                yield return new Token(TokenType.OpAssign);
                yield return new Token(TokenType.BuiltInType, 14); // Color
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new RealVariant(0.7));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new RealVariant(0.7));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new RealVariant(0.7));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.CfIf);
                yield return new IdentifierToken("category");
                yield return new Token(TokenType.OpNotEqual);
                yield return new ConstantToken(new StringVariant("outfits"));
                yield return new Token(TokenType.CfElse);
                yield return new Token(TokenType.BuiltInType, 14); // Color
                yield return new Token(TokenType.ParenthesisOpen);
                yield return new ConstantToken(new RealVariant(1.0));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new RealVariant(1.0));
                yield return new Token(TokenType.Comma);
                yield return new ConstantToken(new RealVariant(1.0));
                yield return new Token(TokenType.ParenthesisClose);
                yield return new Token(TokenType.Newline, 1);
            }
            else
            {
                // return the original token
                yield return token;
            }
        }
    }
}
